using Microsoft.Extensions.Logging;

namespace Application.Services.Serilog
{
    public interface ISerilogService
    {
        void LogSystem(Exception ex);
    }

    public class SerilogService : ISerilogService
    {
        private readonly ILogger<SerilogService> _logger;

        public SerilogService(ILogger<SerilogService> logger)
        {
            _logger = logger;
        }

        public void LogSystem(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}
