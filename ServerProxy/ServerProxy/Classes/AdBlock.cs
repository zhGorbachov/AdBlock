using System.Text.RegularExpressions;

namespace ServerProxy.Classes;

public static class AdBlock
{
    public static bool IsAdRequest(string url)
    {
        // Implement your ad-blocking logic here.
        // You can use regular expressions or other methods to match ad-related URLs.
        // For demonstration, we block requests containing "example.com/ad" in the URL.
        return Regex.IsMatch(url, @"/ad(s|verts?|vertising)?[-._]?banner(s?)|pop[-._]?up(s?)|tracker/", RegexOptions.IgnoreCase);
    }
    
    public static bool ContainsAds(string responseBody)
    {
        // Add your ad-detection logic here
        // You can use regular expressions, keywords, or other methods to detect ads

        // Example: Block ads based on a keyword
        
        var filterListContentEn = File.ReadAllText("../../../Filter/list2.txt");
        var filterListContentRu = File.ReadAllText("../../../Filter/list3.txt");
        
        var filtersEn = filterListContentEn.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var filtersRu = filterListContentRu.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var filters = filtersEn.Concat(filtersRu);
        
        foreach(var filter in filters)
        {
            if (responseBody.Contains(filter))
            {
                Console.WriteLine(filter);
                Console.WriteLine("Ban ad");
                return true;
            }
        }
        
        //
        // if (responseBody.Contains("advertising"))
        // {
        //     return true;
        // }

        // Example: Block ads using a regular expression
        // Replace this with a regex pattern that matches ad-related content
        // if (Regex.IsMatch(responseBody, "/\\/(ads?|banners?|trackers?|affiliates?)\\//"))
        // {
        //     Console.WriteLine("Ban ad");
        //     return true;
        // }

        return false;
    }
}