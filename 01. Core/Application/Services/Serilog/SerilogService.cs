using Microsoft.Extensions.Logging;

namespace Application.Services.Serilog;

public interface ISerilogService
{
    void LogSystem(Exception ex, string additionalInfo = null);
}

public class SerilogService : ISerilogService
{
    private readonly ILogger<SerilogService> _logger;

    public SerilogService(ILogger<SerilogService> logger)
    {
        _logger = logger;
    }

    public void LogSystem(Exception ex, string additionalInfo = null)
    {
        //_logger.LogError(ex, ex.Message + " LogError");
        //_logger.LogInformation(ex, ex.Message + " LogInformation");
        //_logger.LogCritical(ex, ex.Message + " LogCritical");
        //_logger.LogTrace(ex, ex.Message + " LogTrace");
        //_logger.LogDebug(ex, ex.Message + " LogDebug");
        //_logger.LogWarning(ex, ex.Message + " LogWarning");

        _logger.LogError(ex, "An error occurred. Additional Info: {Info}", additionalInfo);
    }
}