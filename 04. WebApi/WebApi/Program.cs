using Application.ExceptionsHandler;
using Application.Services.Jwt;
using Application.Services.MediatR;
using Application.Services.NewtonSoft;
using Localization;
using Persistence;
using WebApi.Services.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();




builder.Services.AddService_Localizer();
builder.Services.AddService_Swagger();
builder.Services.AddService_JwtIdentity(builder);
builder.Services.AddServices_Persistence();
builder.Services.AddService_MediatR_Fluent();
builder.Services.Add_NewtonsoftJsonSettings();




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

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseStaticFiles();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStatusCodePages();
app.UseRequestLocalization();

app.Run();