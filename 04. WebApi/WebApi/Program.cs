using WebApi.Config.Jwt;
using WebApi.Config.Jwt.Common;
using WebApi.Config.Jwt.Extensions;
using WebApi.Config.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();





var siteSetting = builder.Configuration.GetSection(nameof(SiteSettingsJwt)).Get<SiteSettingsJwt>();
builder.Services.Configure<SiteSettingsJwt>(builder.Configuration.GetSection(nameof(SiteSettingsJwt)));
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddJwtAuthentication(siteSetting.JwtSettings);




builder.Services.AddSwaggerGen();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Project v2");
    });
}
else
{
    app.UseHsts();
}

app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseStaticFiles();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();