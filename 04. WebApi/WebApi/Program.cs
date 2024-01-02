using Application.Services.Jwt.FiltersResult;
using Application.Services.Jwt.ServiceExtensions;
using Application.Services.MediatR;
using FluentValidation;
using Persistence.Repository.Personel;
using Shared.Resources;
using WebApi.Controllers.V1;
using WebApi.Services.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
 
builder.Services.AddJwtAuthenticationService(builder);
builder.Services.AddSwaggerService();
builder.Services.AddMediatR_FluentService("WebApi");

// Fluent Validation
builder.Services.AddScoped<IPersonelRepository, PersonelRepository>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>();
//End Fluent Validation------------------------- 

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ValidationExceptionFilterAttribute));
});

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