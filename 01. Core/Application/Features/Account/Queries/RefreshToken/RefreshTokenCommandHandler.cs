﻿using Application.Contracts.Persistence.IRepository;
using Application.Features.Account.Queries.RefreshToken.Dto;
using Application.Services.JWTAuthetication;
using Domain.Common;
using MediatR;

namespace Application.Features.Account.Queries.RefreshToken;

public sealed class
    GetListProductsQuerieHandler : IRequestHandler<RefreshTokenCommand, BaseResponse<RefreshTokenResponseDto>>
{
    private readonly IJwtAuthService _jwtAuth;
    private readonly IPersonelRepository personelRepository;

    public GetListProductsQuerieHandler(IPersonelRepository _personelRepository,
        IJwtAuthService jwtAuth)
    {
        personelRepository = _personelRepository;
        _jwtAuth = jwtAuth;
    }

    public async Task<BaseResponse<RefreshTokenResponseDto>> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var user = await personelRepository.ValidateRefreshToken(request.Token, Guid.Parse(request.RefreshToken));

        if (user.IsLogin)
        {
            var GetToken = await personelRepository.Login2(user.Id);

            return new BaseResponse<RefreshTokenResponseDto>
            {
                Data = new RefreshTokenResponseDto
                {
                    IsLogin = GetToken.IsLogin,
                    Token = GetToken.Token,
                    RefreshToken = GetToken.RefreshToken
                }
            };
        }

        return new BaseResponse<RefreshTokenResponseDto>
        {
            Data = new RefreshTokenResponseDto
            {
                IsLogin = false
            }
        };
    }
}