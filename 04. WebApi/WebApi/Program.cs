using Microsoft.AspNetCore.Mvc;
using WebApi.Config.Swagger;
using WebApi.Config.Utilities.Common;
using WebApi.Config.Utilities.Extensions;
using WebApi.Config.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();





var siteSetting = builder.Configuration.GetSection(nameof(SiteSettingsJwt)).Get<SiteSettingsJwt>();
builder.Services.Configure<SiteSettingsJwt>(builder.Configuration.GetSection(nameof(SiteSettingsJwt)));
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddJwtAuthentication(siteSetting.JwtSettings);






#region Register Swagger
builder.Services.AddSwagger();
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseStaticFiles();
app.UseRouting();
app.UseSwaggerAndUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();