namespace Kongrevsky.Utilities.Common
{
    #region << Using >>

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using Kongrevsky.Utilities.DateTime;
    using Kongrevsky.Utilities.Json.JsonConverters;
    using Kongrevsky.Utilities.Object;
    using LinqKit;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    #endregion

    public class ObjectObserver<TObj> where TObj : new()
    {
        #region Properties

        private TObj _object { get; }

        public bool IsLogChangedPropertiesEnabled { get; set; }

        public bool IsObfuscatorEnabled { get; set; }

        public TObj Object => _object;

        public bool IsChanged { get; set; }

        public List<ChangedPropertyItem> ChangedProperties { get; } = new List<ChangedPropertyItem>();

        private Action _actionIfChanged { get; }

        private static ConcurrentDictionary<string, object> _setExprDict { get; } = new ConcurrentDictionary<string, object>();
        private static ConcurrentDictionary<string, object> _getExprDict { get; } = new ConcurrentDictionary<string, object>();

        #endregion

        #region Constructors

        public ObjectObserver(TObj o)
        {
            _object = o;
            IsChanged = false;
        }

        public ObjectObserver(TObj o, Action actionIfChanged)
        {
            _object = o;
            _actionIfChanged = actionIfChanged;
            IsChanged = false;
        }

        #endregion

        #region Nested Classes

        public class ChangedPropertyItem
        {
            #region Properties

            public string PropertyName { get; set; }

            public Type PropertyType { get; set; }

            public object OldValue { get; set; }

            public object NewValue { get; set; }

            #endregion
        }

        #endregion

        /// <summary>
        /// Changes property of the object
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="propertyExpr"></param>
        /// <param name="value"></param>
        public void Set<TTarget>(Expression<Func<TObj, TTarget>> propertyExpr, TTarget value)
        {
            if (!(propertyExpr.Body is MemberExpression memberSelectorExpression))
                return;

            var property = memberSelectorExpression.Member as PropertyInfo;
            if (property == null)
                return;

            var getter = CreateGetter(propertyExpr);
            var oldValue = getter(_object);

            var handledValue = value;

            if (typeof(TTarget) == typeof(DateTime))
            {
                if (((DateTime)(object)oldValue).Difference((DateTime)(object)handledValue) > TimeSpan.FromSeconds(1))
                    ActionIfDiff();

                return;
            }
            if (typeof(TTarget) == typeof(TimeSpan))
            {
                if (((TimeSpan)(object)oldValue).Subtract((TimeSpan)(object)handledValue).Duration() > TimeSpan.FromSeconds(1))
                    ActionIfDiff();

                return;
            }

            if (!typeof(TTarget).IsSimple())
            {
                var jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.Converters.Add(new DecimalConverter() { Precision = 4 });
                var jsonSerializer = JsonSerializer.Create(jsonSerializerSettings);
                if (typeof(IEnumerable).IsAssignableFrom(typeof(TTarget)))
                {
                    if (!JToken.DeepEquals(JArray.FromObject(oldValue, jsonSerializer), JArray.FromObject(handledValue, jsonSerializer)))
                        ActionIfDiff();
                }
                else
                {
                    if (!JToken.DeepEquals(JObject.FromObject(oldValue, jsonSerializer), JObject.FromObject(handledValue, jsonSerializer)))
                        ActionIfDiff();
                }

                return;
            }

            if (!Equals(oldValue, handledValue))
                ActionIfDiff();

            void ActionIfDiff()
            {
                var setter = CreateSetter(propertyExpr);
                setter(_object, handledValue);
                _actionIfChanged?.Invoke();
                IsChanged = true;

                if (IsLogChangedPropertiesEnabled)
                    ChangedProperties.Add(new ChangedPropertyItem()
                    {
                        PropertyName = property.Name,
                        PropertyType = property.PropertyType,
                        OldValue = oldValue,
                        NewValue = handledValue
                    });
            }
        }

        /// <summary>
        /// Changes property of the object
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="propertyExpr"></param>
        /// <param name="value"></param>
        public void Set<TTarget>(Expression<Func<TObj, TTarget>> propertyExpr, Func<TTarget> value)
        {
            Set(propertyExpr, value());
        }

        /// <summary>
        /// Changes property of the object
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="propertyExpr"></param>
        /// <param name="value"></param>
        /// <param name="handlerValue"></param>
        public void Set<TTarget>(Expression<Func<TObj, TTarget>> propertyExpr, TTarget value, Func<TTarget, TTarget> handlerValue)
        {
            var handledValue = handlerValue != null && IsObfuscatorEnabled ? handlerValue.Invoke(value) : value;
            Set(propertyExpr, handledValue);
        }

        /// <summary>
        /// Changes property of the object
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="propertyExpr"></param>
        /// <param name="value"></param>
        /// <param name="handlerValue"></param>
        public void Set<TTarget>(Expression<Func<TObj, TTarget>> propertyExpr, TTarget value, Func<TObj, TTarget> handlerValue)
        {
            var handledValue = handlerValue != null && IsObfuscatorEnabled ? handlerValue.Invoke(_object) : value;
            Set(propertyExpr, handledValue);
        }

        public override string ToString()
        {
            var str = new StringBuilder();

            str.AppendLine($"Object: {_object.ToString()}");
            if (!IsLogChangedPropertiesEnabled)
                return str.ToString();
            str.AppendLine($"Changed Properties:");
            str.AppendLine("Property Name | Old Value | New Value");

            foreach (var changedPropertyItem in ChangedProperties)
                str.AppendLine($"{changedPropertyItem.PropertyName} | {JsonConvert.SerializeObject(changedPropertyItem.OldValue)} | {JsonConvert.SerializeObject(changedPropertyItem.NewValue)}");

            return str.ToString();
        }

        private Action<TEntity, TProperty> CreateSetter<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> selector)
        {
            var key = GetValidationKey(selector);
            if (_setExprDict.ContainsKey(key))
                return (Action<TEntity, TProperty>)_setExprDict[key];

            var valueParam = Expression.Parameter(typeof(TProperty));
            var expression = Expression.Lambda<Action<TEntity, TProperty>>(Expression.Assign(selector.Body, valueParam), selector.Parameters.Single(), valueParam).Compile();
            _setExprDict[key] = expression;
            return expression;
        }

        private Func<TObj, TTarget> CreateGetter<TTarget>(Expression<Func<TObj, TTarget>> propertyExpr)
        {
            var key = GetValidationKey(propertyExpr);
            if (_getExprDict.ContainsKey(key))
                return (Func<TObj, TTarget>)_getExprDict[key];

            var compile = propertyExpr.Compile();
            _getExprDict.TryAdd(key, compile);
            return compile;
        }

        private string GetValidationKey<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertyLambda)
        {
            if (!(propertyLambda.Body is MemberExpression member))
                throw new ArgumentException($"Expression '{propertyLambda}' can't be cast to a {nameof(MemberExpression)}.");

            var key = $"{member.Expression.Type.FullName}_{member}";
            return key;
        }
    }
}