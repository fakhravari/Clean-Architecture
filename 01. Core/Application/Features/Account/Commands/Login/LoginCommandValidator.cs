using FluentValidation;
using Localization.Resources;

namespace Application.Features.Account.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    private readonly ISharedResource localizer;

    public LoginCommandValidator(ISharedResource viewLocalizer)
    {
        localizer = viewLocalizer;

        When(x => x.UserName.Length == 2,
            () =>
            {
                RuleFor(p => p.UserName).Must(b => false)
                    .WithMessage(localizer.GetTranslation("Dont_Enter_Two_Characters"));
            });

        RuleFor(p => p.UserName).NotNull().NotEmpty().WithMessage(string.Format(
            localizer.GetTranslation("Do_Not_Enter_a_PropertyName_Value"), localizer.GetTranslation("UserName")));
        RuleFor(p => p.Password).NotNull().NotEmpty().WithMessage(string.Format(
            localizer.GetTranslation("Do_Not_Enter_a_PropertyName_Value"), localizer.GetTranslation("Password")));

        // RuleFor(p => p.Input.UserName).Must(v => false).WithMessage(localizer.Locale("Do_Not_Enter_a_PropertyName_Value"));
        // RuleFor(p => p.Input.UserName).MustAsync(IsValidName2).WithMessage(localizer.Locale("Do_Not_Enter_a_PropertyName_Value"));
    }


    private async Task<bool> IsValidName2(string Name, CancellationToken cancellationToken)
    {
        if (Name == "test2")
            return false;
        return true;
    }
}