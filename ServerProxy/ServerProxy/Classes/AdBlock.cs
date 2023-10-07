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
            RemoveNodes(doc, ConvertPatternToXPath(filter[0]));
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

    public static async Task RemoveAdByUrlpath(SessionEventArgs e)
    {
        foreach (var urlpath in _urlpathList)
        {
            if (e.HttpClient.Request.RequestUri.ToString().Contains(urlpath))
            {
                e.Ok(string.Empty);
            }
        }
    }
    
    public static string RemoveAdsFromYouTubeMainPage(HtmlDocument doc)
    {
        var adNodes = doc.DocumentNode.SelectNodes("//*[text()='Ad']");
        if (adNodes != null)
        {
            foreach (var node in adNodes)
            {
                node.ParentNode.Remove();
            }
        }

        return doc.DocumentNode.OuterHtml;
    }
}