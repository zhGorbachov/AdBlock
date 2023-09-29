using System.Net;
using System.Text;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.Models;

namespace ServerProxy.Classes;

public class ServerProxy
{
    private static async Task OnBeforeRequest(object sender, SessionEventArgs e)
    {
        var hostname = e.HttpClient.Request.RequestUri.Host.ToLower();
        Console.WriteLine(GetHostname(hostname));
        
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
        if (e.WebSession.Response != null)
        {
            // Check the content type of the response to make sure it's a web page
            string contentType = e.WebSession.Response.ContentType?.Trim().ToLower();
            if (contentType != null && contentType.StartsWith("text/html"))
            {
                // Read the response body
                var responseBodyBytes = await e.GetResponseBody();
                if (responseBodyBytes != null)
                {
                    // Convert the response body to a string
                    string responseBody = Encoding.UTF8.GetString(responseBodyBytes);

                    // Check for known ad-related content in the response
                    if (AdBlock.ContainsAds(responseBody))
                    {
                        // Block the response by setting an empty body
                        e.SetResponseBody(new byte[0]);
                    }
                }
            }
        }
    }

    private static bool IsSiteWithHPKP(string hostname)
    {
        try
        {
            // Create a WebClient to fetch the site's headers
            using (var client = new System.Net.WebClient())
            {
                // Fetch the headers for the given hostname
                var headers = client.DownloadString("https://" + hostname);

                // Check if the headers contain the Public-Key-Pins header
                return headers.Contains("Public-Key-Pins");
            }
        }
        catch (System.Net.WebException ex)
        {
            // Handle specific HTTP 400 Bad Request error
            if (ex.Response is HttpWebResponse response && response.StatusCode == HttpStatusCode.BadRequest)
            {
                // Log or handle the bad request error as needed
                Console.WriteLine("HTTP 400 Bad Request: The site may not support HPKP.");
                return false;
            }
            else
            {
                // Handle other exceptions or rethrow them
                throw;
            }
        }
        catch (Exception)
        {
            // Handle any other exceptions here
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