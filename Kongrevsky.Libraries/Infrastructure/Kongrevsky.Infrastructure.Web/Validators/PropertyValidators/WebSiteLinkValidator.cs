namespace Kongrevsky.Infrastructure.Web.Validators.PropertyValidators
{
    #region << Using >>

    using System.Text.RegularExpressions;
    using FluentValidation.Validators;

    #endregion

    public class WebSiteLinkValidator : PropertyValidator
    {
        private readonly Regex regex;

        public WebSiteLinkValidator()
                : base("{PropertyName} is not a valid WebSite link.")
        {
            this.regex = new Regex(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$");
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            return context.PropertyValue == null || context.PropertyValue as string == string.Empty || this.regex.IsMatch((string)context.PropertyValue);
        }
    }
}