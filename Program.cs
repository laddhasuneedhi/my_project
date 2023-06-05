using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
//store the links in a hash-set
using System.Globalization;
using System.Collections.Generic;
using System;
using System.IO;

HashSet<string> hashSet = new HashSet<string>();
Queue<string> queue = new Queue<string>();

string base_url = "https://529ia.voya.com/";

hashSet.Add(base_url);
queue.Enqueue(base_url);

 bool IsRelativePath(string websiteUrl, string path)
    {
        //re
        string baseUrl = websiteUrl.TrimEnd('/');  // Remove trailing slashes from the website URL
        if (path.StartsWith("/"))
        {
            return true;  // relative
        }
        else
        {
            return false;  //absolute 
        }
    }

 bool DoesNotHavePdfExtension(string url)
{
    return !url.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);
}

bool IsSameDomain(string baseUrl, string absolutePath)
{
    try{
    //if absolute path check if same domain. If same domain return true
    Uri compare_uri = new Uri(absolutePath);
    Uri base_uri = new Uri(baseUrl);
    if(compare_uri.Host == base_uri.Host){
        return true;
    }else{
        return false;
    }
    }catch(UriFormatException ex){
            return false;
        }
}

  string GetAbsolutePath(string baseUrl, string relativePath)
    {
        try{
        Uri baseUri = new Uri(baseUrl);
        Uri absoluteUri = new Uri(baseUri, relativePath);
        
        string absolutePath = absoluteUri.AbsoluteUri;
        return absolutePath;
        }catch(UriFormatException ex){
            return null;
        }

    }




 async Task ScrapeWebsiteAsync(String url)
{
   
try{
    var playwright = await Playwright.CreateAsync();
    await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
    {
        Headless = false,
        SlowMo = 50
    });
    var context = await browser.NewContextAsync();
    var page = await context.NewPageAsync();
    await page.GotoAsync(url);
    var links = await page.QuerySelectorAllAsync("a");


    var linkUrls = new List<string>();
    foreach (var link in links)
    {
      
        var href = await link.GetAttributeAsync("href");
        if (!string.IsNullOrEmpty(href))
        {
            linkUrls.Add(href);
        }
    }

    foreach (var link in linkUrls)
    { 
        if (IsRelativePath(base_url, link))
        {
            if( DoesNotHavePdfExtension(link)){
            string fullpath = GetAbsolutePath(base_url, link);
            if (fullpath == null){
                continue;
            }
            if (!hashSet.Contains(fullpath) )
            {
                hashSet.Add(fullpath);
                queue.Enqueue(fullpath);
            }
            }
        }
        else
        {
            
            if (link[0] != '#' && IsSameDomain(base_url, link))

            {
                if(!hashSet.Contains(link)){
                hashSet.Add(link);
                queue.Enqueue(link);
                }
          
          }
        }
      }

     
    

    var cookies = await page.Context.CookiesAsync();

    var cookieInfoBuilder = new StringBuilder();

     
    foreach (var cookie in cookies)
    {
        cookieInfoBuilder.AppendLine($"Website name: {url}");
        cookieInfoBuilder.AppendLine($"Name: {cookie.Name}");
        cookieInfoBuilder.AppendLine($"Value: {cookie.Value}");
        cookieInfoBuilder.AppendLine($"Domain: {cookie.Domain}");
        cookieInfoBuilder.AppendLine($"Path: {cookie.Path}");
        cookieInfoBuilder.AppendLine($"Expires: {cookie.Expires}");
        cookieInfoBuilder.AppendLine($"Is Secure: {cookie.Secure}");
        cookieInfoBuilder.AppendLine($"Is Http Only: {cookie.HttpOnly}");
        cookieInfoBuilder.AppendLine("---------------------------");
    }

    // Display the cookie information
    Console.WriteLine(cookieInfoBuilder.ToString());

    // Get the DNT status
    var dntHeader = await page.EvaluateAsync<string>("navigator.doNotTrack");
    
    // Display the DNT status
    Console.WriteLine($"Do Not Track Enabled: {dntHeader}");

}

catch (PlaywrightException ex)
        {
            Console.WriteLine("An error occurred during web scraping: " + ex.Message);
        }

    // Build the cookie information
    
}

int count = 0;

while (queue.Count > 0)
{
    // Perform actions using the front element of the queue
    // ...

    // Remove the front element from the queue
    Console.WriteLine(queue.Count);
    string url = queue.Dequeue();
    Console.WriteLine("wrong----------");
    Console.WriteLine("https://529ia.voya.com/page/start-investing-college");

    await ScrapeWebsiteAsync(url);
    Console.WriteLine("-----------------------------------------------");
    int queueLength = queue.Count;
     Console.WriteLine("Queue length: " + queueLength);

     if(count == 0){
        break;
     }
    

    count = count + 1;
    

}














   

  
