using Application.ExceptionsHandler;
using Application.Resource;
using Application.Services.Jwt;
using Application.Services.MediatR;
using Application.Services.NewtonSoft;
using Persistence;
using WebApi.Services.Culture;
using WebApi.Services.Swagger;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddService_Swagger();
builder.Services.AddService_JwtIdentity(builder);
builder.Services.AddServices_Persistence(builder.Configuration);
builder.Services.AddService_MediatR_Fluent();
builder.Services.Add_NewtonsoftJsonSettings();
builder.Services.AddService_Localizer();

builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwaggerAndUI();
}
else
{
    app.UseHsts();
}

app.AddUseCultureServiceBuilder();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseStaticFiles();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();