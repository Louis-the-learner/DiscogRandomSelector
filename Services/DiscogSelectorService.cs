using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

using discogSelector.Models;

namespace discogSelector.Services
{
    public class DiscogSelectorService
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly String DiscogApiUrl = "https://api.discogs.com/users/ausamerika/collection/folders/0/releases";

        private static readonly String PageQueryParam = "page";

        private static readonly String PerPageQueryParam = "per_page";

        private static int nbTotalOfItems;

        private static int nbTotalOfPages;

        private static int nbItemsPerPage;

        private static Dictionary<int, List<Release>> pageResultsDictionnary = new Dictionary<int, List<Release>>();

        public DiscogSelectorService()
        {

        }

        public async Task<int> GetTotalItems()
        {
            if (nbItemsPerPage == 0 || nbTotalOfItems == 0 || nbTotalOfPages == 0)
            {
               await InitPageAndItemNumbers();
            }

            return nbTotalOfItems;

        }

        
        public async Task<Release> GetItem(int itemPosition)
        {
            if (nbItemsPerPage == 0 || nbTotalOfItems == 0 || nbTotalOfPages == 0)
            {
               await InitPageAndItemNumbers();
            }

            // determine on which page is located the item, using a page offset based on the remainer
            int nextPageOffSet = (itemPosition % nbItemsPerPage > 0) ? 1 : 0;
            int pageNumber = itemPosition / nbItemsPerPage + nextPageOffSet;

            var releasesResults = await GetPageResults(pageNumber);

            // get the position of the selected release from the page
            var releasePositionWithinPage = itemPosition % nbItemsPerPage;

            return releasesResults.ElementAt(releasePositionWithinPage);

        }



        private async Task<List<Release>> GetPageResults(int pageNumber)
        {
            if (pageResultsDictionnary.ContainsKey(pageNumber))
            {
                return pageResultsDictionnary[pageNumber];
            }

            var pageResultData = await RetrievePageFromHttpCall(GetUrlFromPageNumber(pageNumber));

            pageResultsDictionnary.Add(pageNumber, pageResultData.Releases);

            return pageResultData.Releases;
        }

        private String GetUrlFromPageNumber(int pageNumber)
        {
            
            var uriBuilder = new UriBuilder(DiscogApiUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[PageQueryParam] = pageNumber.ToString();
            query[PerPageQueryParam] = nbItemsPerPage.ToString();
            uriBuilder.Query = query.ToString();

            return uriBuilder.ToString();

        }

        private async Task<ObjectData> RetrievePageFromHttpCall(String pageUrl)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var streamTask = client.GetStreamAsync(pageUrl);
            return await JsonSerializer.DeserializeAsync<ObjectData>(await streamTask);
        }


        private async Task InitPageAndItemNumbers()
        {
            var objectData = await RetrievePageFromHttpCall(DiscogApiUrl);

            // store the first page results in the dictionary
            if (!pageResultsDictionnary.ContainsKey(1))
            {
                pageResultsDictionnary.Add(1, objectData.Releases);
            }

            var pagination = objectData.Pagination;
            nbItemsPerPage = pagination.PerPage;
            nbTotalOfItems = pagination.Items;
            nbTotalOfPages = pagination.Pages;
        }

    }
}
