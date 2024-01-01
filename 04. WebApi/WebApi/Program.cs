using FluentValidation;
using Infrastructure.Config.Jwt.FiltersResult;
using Infrastructure.Config.Jwt.ServiceExtensions;
using Infrastructure.Config.MediatR;
using Infrastructure.Config.Swagger;
using Infrastructure.Repository.Product;
using Shared.Resources;
using WebApi.Controllers.V1;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
 

builder.Services.AddJwtAuthenticationService(builder);
builder.Services.AddSwaggerService();
builder.Services.AddMediatR_FluentService("WebApi");


// Fluent Validation
builder.Services.AddScoped<IProductRepository, ProductRepository>();
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