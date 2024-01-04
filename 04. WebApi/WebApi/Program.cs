using Application.Services.Jwt;
using Application.Services.MediatR;
using Newtonsoft.Json;
using Persistence;
using Shared.Resources;
using WebApi.Services.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddControllers().AddNewtonsoftJson(options => { options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; });
builder.Services.AddSwaggerService();


builder.Services.AddJwtAuthenticationService(builder);
builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddMediatR_FluentService();

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