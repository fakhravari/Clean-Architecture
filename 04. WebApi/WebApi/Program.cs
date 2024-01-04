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


builder.Services.Add_JwtIdentity_Service(builder);
builder.Services.Add_Persistence_Services(builder.Configuration);


builder.Services.Add_MediatR_Fluent_ApiResult_Service();

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