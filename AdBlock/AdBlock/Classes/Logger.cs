namespace AdBlock.Classes;

public class Logger
{
    private readonly ILogger<Logger> _logger;

    public Logger(ILogger<Logger> logger)
    {
        _logger = logger;
    }

    public void LogBlockedAdResource(string url)
    {
        _logger.LogInformation($"Blocked ad resource: {url}");
    }
}