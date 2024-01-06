using Microsoft.Extensions.Logging;

namespace Application.Services.Serilog
{
    public interface ISerilogService
    {
        void LogSystem(object ex);
    }

    public class SerilogService: ISerilogService
    {
        private readonly ILogger logger;

        public SerilogService(ILogger _logger)
        {
            logger = _logger;
        }

        public void LogSystem(object ex)
        {
            var data = ex as Exception;
            logger.LogError(data, data.Message);
        }
    }
}
