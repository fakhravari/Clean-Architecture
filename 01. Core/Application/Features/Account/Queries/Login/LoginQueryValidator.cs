using Application.Contracts.Persistence;
using FluentValidation;
using Shared.Extentions;

namespace Application.Features.Account.Queries.Login
{
    public class LoginQueryValidator : AbstractValidator<LoginRequestDto>
    {
        private readonly IPersonelRepository _productRepository;

        public LoginQueryValidator(IPersonelRepository productRepository)
        {
            _productRepository = productRepository;

            When(x => x.UserName.Length == 2, () =>
            {
                RuleFor(p => p.UserName)
                    .Must(b => false)
                    .WithMessage("دو کاراکتر وارد نکنید");
            });

            RuleFor(p => p.UserName)
                .NotNull().NotEmpty().WithMessage("نام محصول الزامی است.")
                .Must(p => p.IsValidName()).WithMessage("مقدار تست را وارد نکنید")
                .MustAsync(IsValidName2).WithMessage("مقدار تست2 را وارد نکنید");
        }



        private async Task<bool> IsValidName2(string Name, CancellationToken cancellationToken)
        {
            if (Name == "test2")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}