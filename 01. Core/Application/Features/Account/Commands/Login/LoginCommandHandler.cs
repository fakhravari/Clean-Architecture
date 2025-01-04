using Application.Contracts.Persistence.IRepository;
using Application.Features.Account.Commands.Login.Dto;
using Domain.Common;
using MediatR;

namespace Application.Features.Account.Commands.Login;

public sealed class GetListProductsQuerieHandler : IRequestHandler<LoginCommand, BaseResponse<LoginResponseDto>>
{
    private readonly IPersonelRepository personelRepository;

    public GetListProductsQuerieHandler(IPersonelRepository _personelRepository)
    {
        personelRepository = _personelRepository;
    }

    public async Task<BaseResponse<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await personelRepository.Login(request.UserName, request.Password);

        if (user.IsLogin)
            return new BaseResponse<LoginResponseDto>
            {
                Data = new LoginResponseDto
                {
                    IsLogin = user.IsLogin,
                    Token = user.Token,
                    RefreshToken = user.RefreshToken
                }
            };
        return new BaseResponse<LoginResponseDto>
        {
            Data = new LoginResponseDto
            {
                IsLogin = false
            }
        };
    }
}