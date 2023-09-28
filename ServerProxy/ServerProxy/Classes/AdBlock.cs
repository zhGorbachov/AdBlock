using System.Text.RegularExpressions;

namespace ServerProxy.Classes;

public static class AdBlock
{
    public static bool IsAdRequest(string url)
    {
        // Implement your ad-blocking logic here.
        // You can use regular expressions or other methods to match ad-related URLs.
        // For demonstration, we block requests containing "example.com/ad" in the URL.
        return Regex.IsMatch(url, @"example\.com/ad", RegexOptions.IgnoreCase);
    }
    
    public static bool ContainsAds(string responseBody)
    {
        // Add your ad-detection logic here
        // You can use regular expressions, keywords, or other methods to detect ads

        // Example: Block ads based on a keyword
        if (responseBody.Contains("advertising"))
        {
            return true;
        }

        // Example: Block ads using a regular expression
        // Replace this with a regex pattern that matches ad-related content
        // if (Regex.IsMatch(responseBody, "your-regex-pattern"))
        // {
        //     return true;
        // }

        return false;
    }
}