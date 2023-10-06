using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Titanium.Web.Proxy.EventArguments;

namespace ServerProxy.Classes;

public static class AdBlock
{
    private static List<string> _xpathList = File.ReadAllLines("../../../Filter/xpathList.txt").ToList();
    private static List<string> _urlpathList = File.ReadAllLines("../../../Filter/urlpathList.txt").ToList();
    
    public static bool IsAdRequest(string url)
    {
        return Regex.IsMatch(url, @"/ad(s|verts?|vertising)?[-._]?banner(s?)|pop[-._]?up(s?)|tracker/", RegexOptions.IgnoreCase);
    }
    
    public static bool ContainsAds(string responseBody)
    {
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

        return false;
    }
    
    private static void RemoveNodes(HtmlDocument doc, string xpath)
    {
        var nodesToRemove = doc.DocumentNode.SelectNodes(xpath);
        if (nodesToRemove != null)
        {
            Logger.LogBlockedAd(xpath);
            
            foreach (var node in nodesToRemove)
            {
                node.Remove();
            }
        }
    }

    public static void RemoveAdByXpath(HtmlDocument doc)
    {
        foreach (var filter in _xpathList.Select(xpath => xpath.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)))
        {
            AdBlock.RemoveNodes(doc, ConvertPatternToXPath(filter[0]));
        }
    }
    
    private static string ConvertPatternToXPath(string pattern)
    {
        
        if (pattern.StartsWith("."))
        {
            var txt = $"//*[contains(@class, '{pattern.Remove(0, 1)}')]";
            return txt;
        }
        else if (pattern.StartsWith("#"))
        {
            return $"//*[@id='{pattern.Substring(1)}']";
        }
        else
        {
            return pattern;
        }
    }

    public static bool RemoveAdByUrlpath(SessionEventArgs e)
    {
        return _urlpathList.Any(adPath => e.HttpClient.Request.RequestUri.ToString().Contains(adPath));
    }
}