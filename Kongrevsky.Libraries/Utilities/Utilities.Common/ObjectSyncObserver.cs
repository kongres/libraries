namespace Kongrevsky.Utilities.Common
{
    using Kongrevsky.Utilities.DateTime;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using LinqKit;
    using Kongrevsky.Utilities.Object;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class ObjectSyncObserver<TObj> where TObj : new()
    {
        private TObj _object { get; }

        public ObjectSyncObserver(TObj o)
        {
            this._object = o;
            IsChanged = false;
        }

        public ObjectSyncObserver(TObj o, Action actionIfChanged)
        {
            this._object = o;
            _actionIfChanged = actionIfChanged;
            IsChanged = false;
        }

        public bool IsLogChangedPropertiesEnabled { get; set; }
        public bool IsObfuscatorEnabled { get; set; }

        public TObj Object => this._object;

        public bool IsChanged { get; set; }

        public List<ChangedPropertyItem> ChangedProperties { get; } = new List<ChangedPropertyItem>();

        private Action _actionIfChanged { get; }



        public void Set<TTarget>(Expression<Func<TObj, TTarget>> propertyExpr, TTarget value)
        {
            if (propertyExpr.Body is MemberExpression memberSelectorExpression)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    var oldValue = propertyExpr.Invoke(_object);

                    var handledValue = value;

                    if (typeof(TTarget) == typeof(DateTime))
                    {
                        if (((DateTime)(object)oldValue).Difference((DateTime)(object)handledValue) > TimeSpan.FromSeconds(1))
                            ActionIfDiff();
                        return;
                    }

                    if (!typeof(TTarget).IsSimple())
                    {
                        if (typeof(IEnumerable).IsAssignableFrom(typeof(TTarget)))
                        {
                            if (!JToken.DeepEquals(JArray.FromObject(oldValue), JArray.FromObject(handledValue)))
                                ActionIfDiff();
                        }
                        else
                        {
                            if (!JToken.DeepEquals(JObject.FromObject(oldValue), JObject.FromObject(handledValue)))
                                ActionIfDiff();
                        }
                        return;
                    }

                    if (!Equals(oldValue, handledValue))
                    {
                        ActionIfDiff();
                        return;
                    }

                    void ActionIfDiff()
                    {
                        var setterExpr = CreateSetter(propertyExpr);
                        setterExpr.Compile()(_object, handledValue);
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
            }
        }

        public void Set<TTarget>(Expression<Func<TObj, TTarget>> propertyExpr, Func<TTarget> value)
        {
            Set(propertyExpr, value());
        }
        public void Set<TTarget>(Expression<Func<TObj, TTarget>> propertyExpr, TTarget value, Func<TTarget, TTarget> handlerValue)
        {
            var handledValue = handlerValue != null && IsObfuscatorEnabled ? handlerValue.Invoke(value) : value;
            Set(propertyExpr, handledValue);
        }

        public void Set<TTarget>(Expression<Func<TObj, TTarget>> propertyExpr, TTarget value, Func<TObj, TTarget> handlerValue)
        {
            var handledValue = handlerValue != null && IsObfuscatorEnabled ? handlerValue.Invoke(_object) : value;
            Set(propertyExpr, handledValue);
        }

        private static Expression<Action<TEntity, TProperty>> CreateSetter<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> selector)
        {
            var valueParam = Expression.Parameter(typeof(TProperty));
            var body = Expression.Assign(selector.Body, valueParam);
            return Expression.Lambda<Action<TEntity, TProperty>>(body,
                                                                 selector.Parameters.Single(),
                                                                 valueParam);
        }

        public override string ToString()
        {
            var str = new StringBuilder();

            str.AppendLine($"Object: {_object.ToString()}");
            if (IsLogChangedPropertiesEnabled)
            {
                str.AppendLine($"Changed Properties:");
                str.AppendLine("Property Name | Old Value | New Value");

                foreach (var changedPropertyItem in ChangedProperties)
                {
                    str.AppendLine($"{changedPropertyItem.PropertyName} | {JsonConvert.SerializeObject(changedPropertyItem.OldValue)} | {JsonConvert.SerializeObject(changedPropertyItem.NewValue)}");
                }
            }



            return str.ToString();
        }

        public class ChangedPropertyItem
        {
            public string PropertyName { get; set; }
            public Type PropertyType { get; set; }
            public object OldValue { get; set; }
            public object NewValue { get; set; }
        }

    }

}