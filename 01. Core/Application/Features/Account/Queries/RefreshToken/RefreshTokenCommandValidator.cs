using FluentValidation;
using Localization.Resources;


namespace Application.Features.Account.Queries.RefreshToken
{
    public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        private readonly ISharedResource localizer;

        public RefreshTokenCommandValidator(ISharedResource viewLocalizer)
        {
            localizer = viewLocalizer;

            RuleFor(p => p.Token).NotNull().NotEmpty().WithMessage(string.Format(localizer.GetTranslation("Do_Not_Enter_a_PropertyName_Value"), localizer.GetTranslation("UserName")));
            RuleFor(p => p.RefreshToken).NotNull().NotEmpty().WithMessage(string.Format(localizer.GetTranslation("Do_Not_Enter_a_PropertyName_Value"), localizer.GetTranslation("Password")));
        }
    }
}