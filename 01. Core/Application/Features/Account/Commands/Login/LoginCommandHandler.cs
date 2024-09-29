﻿using Application.Contracts.Persistence.IRepository;
using Application.Features.Account.Commands.Login.Dto;
using Application.Services.JWTAuthetication;
using Domain.Common;
using MediatR;

namespace Application.Features.Account.Commands.Login
{
    public sealed class GetListProductsQuerieHandler : IRequestHandler<LoginCommand, BaseResponse<LoginResponseDto>>
    {
        private readonly IPersonelRepository personelRepository;
        private readonly IJwtService jwt;

        public GetListProductsQuerieHandler(IPersonelRepository _personelRepository, IJwtService _jwt)
        {
            personelRepository = _personelRepository;
            jwt = _jwt;
        }

        public async Task<BaseResponse<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await personelRepository.Login(request.Input.UserName, request.Input.Password);

            if (user.IsLogin)
            {
                string GetToken = jwt.GenerateJwtToken(user.Id);

                return new BaseResponse<LoginResponseDto>()
                {
                    Data = new LoginResponseDto()
                    {
                        IsLogin = string.IsNullOrWhiteSpace(GetToken) == false,
                        Token = GetToken
                    }
                };
            }
            else
            {
                return new BaseResponse<LoginResponseDto>()
                {
                    Data = new LoginResponseDto()
                    {
                        IsLogin = false
                    }
                };
            }
        }
    }
}