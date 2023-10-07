namespace ServerProxy.Classes;

public static class Logger
{
    
    private static Dictionary<string, DateTime> lastLoggedDomains = new Dictionary<string, DateTime>();
    private static TimeSpan logInterval = TimeSpan.FromMinutes(5);
    
    public static void LogBlockedAd(string domain)
    {
        if (!lastLoggedDomains.ContainsKey(domain) || DateTime.UtcNow - lastLoggedDomains[domain] > logInterval)
        {
            Console.WriteLine($"[ADBLOCK][{DateTime.Now}] Blocked ad on the {domain}\n");
            lastLoggedDomains[domain] = DateTime.UtcNow;
        }
    }

    public static void LogHostnamePage(string hostname)
    {
        switch (hostname.Split(".").Length)
        {
            case 2:
                hostname = hostname.Split(".")[0];
                break;
            case 3:
                hostname = hostname.Split(".")[1];
                break;
            case 4:
                hostname = hostname.Split(".")[2];
                break;
        };
        
        Console.WriteLine($"[INFO][{DateTime.Now}] Proxy-server is sitting on: {hostname}");
    }
}