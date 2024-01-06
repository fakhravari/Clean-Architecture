using Application.Resource;
using Application.Services.Jwt;
using Application.Services.MediatR;
using Newtonsoft.Json;
using Persistence;
using WebApi.Services.Culture;
using WebApi.Services.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options => { options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; });


builder.Services.Add_Swagger_Service();
builder.Services.Add_JwtIdentity_Service(builder);
builder.Services.Add_Persistence_Services(builder.Configuration);
builder.Services.Add_MediatR_Fluent_ApiResult_Service();


builder.Services.Add_Localizer_Service();

 

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

app.CultureServiceBuilder();

app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseStaticFiles();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();