using System.Net;
using System.Text;
using HtmlAgilityPack;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.Models;

namespace ServerProxy.Classes;

public class ServerProxy
{
    private static async Task OnBeforeRequest(object sender, SessionEventArgs e)
    {
        var hostname = e.HttpClient.Request.RequestUri.Host.ToLower();
        // Console.WriteLine(GetHostname(hostname));
        
        if (IsSiteWithHPKP(hostname))
        {
            // Skip interception for sites with HPKP
            return;
        }

        // Continue with your ad-blocking logic for other sites
        if (AdBlock.IsAdRequest(e.HttpClient.Request.RequestUri.ToString()))
        {
            // Block the request by returning a custom response
            e.Ok("tesat");
        }
    }

    private static async Task OnBeforeResponse(object sender, SessionEventArgs e)
    {
        if (!e.HttpClient.Response.HasBody || !e.HttpClient.Response.ContentType?.Contains("text/html") == true)
        {
            return;
        }

        var bodyBytes = await e.GetResponseBody();
        var bodyString = Encoding.UTF8.GetString(bodyBytes);

        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(bodyString);
        
        RemoveNodes(doc, "//script[contains(@src, 'known-ad-domain')]");
        RemoveNodes(doc, "//*[contains(@class, 'ad-banner')]");
        RemoveNodes(doc, "//*[contains(@class, 'ad-container')]");
        RemoveNodes(doc, "//*[contains(@class, 'ad-wrapper')]");
        RemoveNodes(doc, "//*[contains(@class, 'ad-slot')]");
        RemoveNodes(doc, "//*[contains(@class, 'adsbox')]");
        RemoveNodes(doc, "//iframe");
        RemoveNodes(doc, "//iframe[contains(@src, 'doubleclick.net')]");
        RemoveNodes(doc, "//iframe[contains(@src, 'adnxs.com')]");
        RemoveNodes(doc, "//script[contains(@src, 'doubleclick.net')]");
        RemoveNodes(doc, "//script[contains(@src, 'adnxs.com')]");
        RemoveNodes(doc, "//script[contains(@src, 'adservice.google.com')]");
        RemoveNodes(doc, "//div[contains(@id, 'google_ads')]");
        RemoveNodes(doc, "//div[contains(@id, 'adtech_banner')]");
        RemoveNodes(doc, "//div[contains(@data-ad, 'true')]");
        RemoveNodes(doc, "//*[contains(@class, 'popup-ad')]");
        RemoveNodes(doc, "//*[contains(@class, 'modal-ad')]");
        RemoveNodes(doc, "//*[contains(@class, 'adw-mascot--container')]");
        

        var modifiedBodyString = doc.DocumentNode.OuterHtml;
        var modifiedBodyBytes = Encoding.UTF8.GetBytes(modifiedBodyString);
    
        e.SetResponseBody(modifiedBodyBytes);
    }

    private static void RemoveNodes(HtmlDocument doc, string xpath)
    {
        var nodesToRemove = doc.DocumentNode.SelectNodes(xpath);
        if (nodesToRemove != null)
        {
            foreach (var node in nodesToRemove)
            {
                node.Remove();
            }
        }
    }
    
    private static bool IsSiteWithHPKP(string hostname)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://" + hostname);
            request.Method = "HEAD";
        
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                return response.Headers["Public-Key-Pins"] != null;
            }
        }
        catch (WebException ex)
        {
            if (ex.Response is HttpWebResponse response && response.StatusCode == HttpStatusCode.BadRequest)
            {
                // Console.WriteLine("HTTP 400 Bad Request: The site may not support HPKP.");
                return false;
            }
            else
            {
                throw;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public void StartProxyServer(int port)
    {
        var proxyServer = new ProxyServer();

        proxyServer.BeforeRequest += OnBeforeRequest;
        proxyServer.BeforeResponse += OnBeforeResponse;
        if (port > 1 && port < 100000)
        {
            var explicitEndPoint = new ExplicitProxyEndPoint(System.Net.IPAddress.Any, port, true);

            proxyServer.AddEndPoint(explicitEndPoint);
            proxyServer.Start();

            // Set up the root certificate for SSL interception
            proxyServer.CertificateManager.CreateRootCertificate();

            // Install the root certificate in your system's trusted root authorities
            proxyServer.CertificateManager.TrustRootCertificate(true);

            Console.WriteLine($"Proxy server listening on port {port}. Press any key to exit...");
            Console.ReadKey();

            proxyServer.Stop();
            return;
        }

        Console.WriteLine($"Your proxy port {port} doesnt exist.");
    }

    public static string GetHostname(string hostname)
    {
        switch (hostname.Split(".").Length)
        {
            case 2:
                return $"You are on: {hostname.Split(".")[0]}";
            
            case 3:
                return $"You are on: {hostname.Split(".")[1]}";
            
            case 4:
                return $"You are on: {hostname.Split(".")[2]}";
            
            default:
                return $"You are on: {hostname}";
                
        }
    }
}