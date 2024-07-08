using Application.Contracts.Persistence;
using Application.Resource;
using FluentValidation;


namespace Application.Features.Account.Commands.Login
{
    public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        private readonly IPersonelRepository _productRepository;
        private readonly ISharedViewLocalizer localizer;

        public LoginCommandValidator(IPersonelRepository productRepository, ISharedViewLocalizer viewLocalizer)
        {
            _productRepository = productRepository;
            localizer = viewLocalizer;

            When(x => x.Input.UserName.Length == 2, () =>
            {
                RuleFor(p => p.Input.UserName).Must(b => false).WithMessage("Dont_Enter_Two_Characters");
            });

            RuleFor(p => p.Input.UserName).NotNull().NotEmpty().WithMessage(string.Format(localizer.Locale("Do_Not_Enter_a_PropertyName_Value"), localizer.Locale("UserName")));
            RuleFor(p => p.Input.Password).NotNull().NotEmpty().WithMessage(string.Format(localizer.Locale("Do_Not_Enter_a_PropertyName_Value"), localizer.Locale("Password")));

            // RuleFor(p => p.Input.UserName).Must(v => false).WithMessage(localizer.Locale("Do_Not_Enter_a_PropertyName_Value"));
            // RuleFor(p => p.Input.UserName).MustAsync(IsValidName2).WithMessage(localizer.Locale("Do_Not_Enter_a_PropertyName_Value"));
        }



        private async Task<bool> IsValidName2(string Name, CancellationToken cancellationToken)
        {
            var tt = await _productRepository.ListAllAsync();

            if (Name == "test2")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }


}