namespace Kongrevsky.Infrastructure.Web.Models
{
    #region << Using >>

    using FluentValidation;

    #endregion

    public class IdVm
    {
        public string Id { get; set; }
    }

    public class IdVmValidator : AbstractValidator<IdVm>
    {
        public IdVmValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}