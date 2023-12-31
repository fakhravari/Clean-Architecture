using FluentValidation;
using Infrastructure.Config.Jwt.ServiceExtensions;
using Infrastructure.Config.Swagger;
using Shared.Resources;
using WebApi.Controllers.V1;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
 

builder.Services.AddJwtAuthentication(builder);
builder.Services.AddSwagger();


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
//builder.Services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>();

 


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