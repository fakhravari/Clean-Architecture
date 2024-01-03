using Application;
using Application.MediatR;
using Application.Services.Jwt;
using Newtonsoft.Json;
using Persistence;
using Shared.Resources;
using WebApi.Services.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();



builder.Services.AddControllers().AddNewtonsoftJson(options => { options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; });

builder.Services.AddJwtAuthenticationService(builder);
builder.Services.AddSwaggerService();


builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddFluentValidationRegistration();
builder.Services.AddMediatR_FluentService("WebApi");


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

app.ResourcesBuilder();

app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseStaticFiles();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();