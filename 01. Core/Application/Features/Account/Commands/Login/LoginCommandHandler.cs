using Application.Contracts.Persistence;
using Application.Features.Account.Commands.Login.Dto;
using Application.Services.Jwt;
using MediatR;

namespace Application.Features.Account.Commands.Login
{
    public sealed class GetListProductsQuerieHandler : IRequestHandler<LoginCommand , LoginResponseDto>
    {
        private readonly IPersonelRepository personelRepository;
        private readonly IJwtService jwt;

        public GetListProductsQuerieHandler(IPersonelRepository _personelRepository, IJwtService _jwt)
        {
            personelRepository = _personelRepository;
            jwt = _jwt;
        }

        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await personelRepository.Login(request.Input.UserName, request.Input.Password);

            if (user.IsLogin)
            {
                string GetToken = jwt.GetToken(user.Id);

                return new LoginResponseDto()
                {
                    IsLogin = string.IsNullOrWhiteSpace(GetToken) == false,
                    Token = GetToken
                };
            }
            else
            {
                return new LoginResponseDto()
                {
                    IsLogin = false
                };
            }

        }
    }
}