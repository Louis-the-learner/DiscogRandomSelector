using System.Net.Http;
using System.Net.Http.Headers;

namespace discogRandomSelector.Services
{
    public class DiscogApiHelper
    {
        public static HttpClient ApiClient { get; set; }

        public static void InitializeClient()
        {
            ApiClient = new HttpClient();
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            ApiClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter"); 
       }
    }
}