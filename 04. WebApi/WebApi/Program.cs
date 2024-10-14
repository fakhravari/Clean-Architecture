using Application.ExceptionsHandler;
using DI.FileService;
using DI.Jwt;
using DI.Localization;
using DI.MediatR_FluentValidation;
using DI.NewtonSoft;
using DI.Persistence;
using DI.Serilog;
using WebApi.Services.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();


builder.Services.AddService_Localizer();
builder.Services.AddService_Swagger();
builder.Services.AddServices_Persistence(builder.Configuration);
builder.Services.AddService_MediatR_FluentValidation();
builder.Services.Add_NewtonsoftJsonSettings();


builder.Services.Add_JwtIdentity(builder.Configuration);
builder.Services.Add_FileService();



builder.Add_SerilogLogging(); // پیکربندی Serilog


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseSwaggerAndUI();


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