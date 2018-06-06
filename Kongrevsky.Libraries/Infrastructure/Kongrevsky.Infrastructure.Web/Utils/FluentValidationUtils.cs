namespace Kongrevsky.Infrastructure.Web.Utils
{
    #region << Using >>

    using FluentValidation;
    using Kongrevsky.Infrastructure.Web.Validators.PropertyValidators;

    #endregion

    public static class FluentValidationUtils
    {
        public static IRuleBuilderOptions<T, string> PhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new ProviderNumberValidator());
        }
    }
}