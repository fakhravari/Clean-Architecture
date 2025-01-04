namespace WebApi.Infrastructure;

public static class CorsExtensions
{
    public static IServiceCollection Add_AnyCors(this IServiceCollection services)
    {
        return services.AddCors(x =>
        {
            x.AddPolicy("Any", b =>
            {
                b.AllowAnyOrigin();
                b.AllowAnyHeader();
                b.AllowAnyMethod();
            });
        });
    }

    public static IApplicationBuilder Use_AnyCors(this IApplicationBuilder app)
    {
        return app.UseCors("Any");
    }
}