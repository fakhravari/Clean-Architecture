using Application.Contracts.Persistence;
using FluentValidation;
using Shared.Extentions;

namespace Application.Features.Account.Commands.Login
{
    public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        private readonly IPersonelRepository _productRepository;

        public LoginCommandValidator(IPersonelRepository productRepository)
        {
            _productRepository = productRepository;

            When(x => x.Input.UserName.Length == 2, () =>
            {
                RuleFor(p => p.Input.UserName)
                    .Must(b => false)
                    .WithMessage("{PropertyName} is required. testing")
                    .WithMessage("دو کاراکتر وارد نکنید");
            });

            RuleFor(p => p.Input.UserName)
                .NotNull().NotEmpty().WithMessage("نام کاربردی را وارد کنید.")
                .Must(p => p.IsValidName()).WithMessage("مقدار test را وارد نکنید")
                .MustAsync(IsValidName2).WithMessage("مقدار test2 را وارد نکنید");
        }



        private async Task<bool> IsValidName2(string Name, CancellationToken cancellationToken)
        {
            var tt = await _productRepository.ListAllAsync();

            if (Name == "test2")
            {
                return false ;
            }
            else
            {
                return true;
            }
        }
    }
}