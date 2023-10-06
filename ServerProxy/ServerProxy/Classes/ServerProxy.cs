using System.Net;
using System.Reflection.Metadata;
using System.Text;
using HtmlAgilityPack;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.Models;

namespace ServerProxy.Classes;

public class ServerProxy
{
    private ProxyServer _proxyServer = new ProxyServer();
    
    private static async Task OnBeforeRequest(object sender, SessionEventArgs e)
    {
        var hostname = e.HttpClient.Request.RequestUri.Host.ToLower();
        
        if (IsSiteWithHPKP(hostname))
        {
            return;
        }

        if (AdBlock.IsAdRequest(e.HttpClient.Request.RequestUri.ToString()))
        {
            e.Ok("Ad blocked");
        }
        
        if (AdBlock.RemoveAdByUrlpath(e))
        {
            e.Ok("Ad blocked by proxy");
            return;
        }
        
        if (e.HttpClient.Request.RequestUri.ToString().Contains("blocked_path"))
        {
            Logger.LogBlockedAd(e.HttpClient.Request.Host);
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

        AdBlock.RemoveAdByXpath(doc);

        var modifiedBodyString = doc.DocumentNode.OuterHtml;
        var modifiedBodyBytes = Encoding.UTF8.GetBytes(modifiedBodyString);

        e.SetResponseBody(modifiedBodyBytes);
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
        _proxyServer.BeforeRequest += OnBeforeRequest;
        _proxyServer.BeforeResponse += OnBeforeResponse;
        
        if (port > 1 && port < 100000)
        {
            var explicitEndPoint = new ExplicitProxyEndPoint(System.Net.IPAddress.Any, port, true);

            _proxyServer.AddEndPoint(explicitEndPoint);
            _proxyServer.Start();

            _proxyServer.CertificateManager.CreateRootCertificate();

            _proxyServer.CertificateManager.TrustRootCertificate(true);

            Console.WriteLine($"\n[INFO]Proxy server listening on port {port}. Press any key to exit...");
            
            Console.ReadKey();

            StopProxyServer(this, 8888);
            
            return;
        }

        Console.WriteLine($"\n\n[INFO]Your proxy port {port} doesnt exist.");
    }

    public void StopProxyServer(ServerProxy serverProxy, int port)
    {
        serverProxy._proxyServer.Stop();
        Console.WriteLine($"\n\n[INFO]Your proxy-server on {port} has stopped");
    }
    
    private static string GetHostname(string hostname)
    {
        return hostname.Split(".").Length switch
        {
            2 => $"You are on: {hostname.Split(".")[0]}",
            3 => $"You are on: {hostname.Split(".")[1]}",
            4 => $"You are on: {hostname.Split(".")[2]}",
            _ => $"You are on: {hostname}"
        };
    }
}