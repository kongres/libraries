namespace Kongrevsky.Infrastructure.Web.Validators.PropertyValidators
{
    #region << Using >>

    using System.Text.RegularExpressions;
    using FluentValidation.Validators;

    #endregion

    public class ProviderNumberValidator : PropertyValidator
    {
        private readonly Regex regex;

        public ProviderNumberValidator()
                : base("{PropertyName} is not a valid phone number.")
        {
            this.regex = new Regex("^[\\d,\\-,\\(,\\),\\+,\\ ]+$");
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            return context.PropertyValue == null || context.PropertyValue as string == string.Empty || this.regex.IsMatch((string)context.PropertyValue);
        }
    }
}