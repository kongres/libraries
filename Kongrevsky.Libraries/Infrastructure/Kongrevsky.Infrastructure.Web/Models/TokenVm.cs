namespace Kongrevsky.Infrastructure.Web.Models
{
    #region << Using >>

    using FluentValidation;

    #endregion

    public class TokenVm
    {
        public string Token { get; set; }
    }

    public class TokenVmValidator : AbstractValidator<TokenVm>
    {
        public TokenVmValidator()
        {
            RuleFor(x => x.Token).NotEmpty();
        }
    }
}