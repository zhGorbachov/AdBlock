namespace ServerProxy.Classes;

public static class Logger
{
    
    private static Dictionary<string, DateTime> lastLoggedDomains = new Dictionary<string, DateTime>();
    private static TimeSpan logInterval = TimeSpan.FromMinutes(5);
    
    public static void LogBlockedAd(string domain)
    {
        if (!lastLoggedDomains.ContainsKey(domain) || DateTime.UtcNow - lastLoggedDomains[domain] > logInterval)
        {
            Console.WriteLine($"[{DateTime.Now}] Blocked ad on the {domain}");
            lastLoggedDomains[domain] = DateTime.UtcNow;
        }
    }
}