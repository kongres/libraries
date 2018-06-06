namespace Kongrevsky.Infrastructure.Web.Validators
{
    using FluentValidation;
    using Kongrevsky.Utilities.Enumerable.Models;

    public class PagingVmValidator<T> : AbstractValidator<T> where T : PagingModel
    {
        public PagingVmValidator()
        {
            RuleFor(x => x.PageSize).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(0);
        }
    }
}