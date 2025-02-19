﻿namespace Application.Features.Account.Commands.Login.Dto;

public class LoginResponseDto
{
    public bool IsLogin { get; set; }

    //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Token { get; set; }

    //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Guid RefreshToken { get; set; }
}