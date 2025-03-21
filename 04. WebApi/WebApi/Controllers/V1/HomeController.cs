﻿using Application.Contracts.Infrastructure;
using Localization.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using WebApi.Controllers.Base;
using WebApi.Model;

namespace WebApi.Controllers.V1;
// [HttpPost("{questionAnswerId}/quesitons")]   [FromRoute] Guid questionAnswerId
// [FromBody] ViewModel

[ApiVersion("1")]
public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;
    private readonly IStringLocalizer<SharedResource> localizer;
    private readonly IRedisRepository _redisRepository;
    public HomeController(IStringLocalizer<SharedResource> _localizer, IRedisRepository redisRepository, ILogger<HomeController> logger)
    {
        localizer = _localizer;
        _redisRepository = redisRepository;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> PublicAction([FromBody] PublicActionDto model)
    {
        var json = JsonConvert.SerializeObject(model);
        return Ok(json);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> PrivateAction([FromBody] PrivateActionDto model)
    {
        var json = JsonConvert.SerializeObject(model);
        return Ok(json);
    }


    [HttpPost]
    public async Task<IActionResult> Redis([FromBody] string model)
    {
        await _redisRepository.ClearAllAsync();
        await _redisRepository.SetAsync("PublicAction", model, TimeSpan.FromMinutes(10));
        await _redisRepository.SetAsync("mhf", "محمدحسین فخرآوری", null);
        return Ok();
    }
}