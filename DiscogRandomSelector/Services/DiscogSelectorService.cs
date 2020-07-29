using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

using discogRandomSelector.Models;

namespace discogRandomSelector.Services
{

    /// <summary>
    /// Service class responsible for the interaction with the Discog existing web API, including the HTTP calls to it.
    /// The client of this service will not have to deal with the details of the API, like number 
    /// of pages and items per page.
    /// Implements the <see cref="ISelectorService"/> interface, so it can be mocked easily for client unit testing.
    /// </summary>
    public class DiscogSelectorService: ISelectorService
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

        /// <summary>
        /// Retrieve and return the total number of items stored and available on the Discog Web API
        /// </summary>
        /// <returns>the number of items from the Discog Web API</returns>
        public async Task<int> GetTotalItems()
        {
            if (nbItemsPerPage == 0 || nbTotalOfItems == 0 || nbTotalOfPages == 0)
            {
               await InitPageAndItemNumbers();
            }

            return nbTotalOfItems;

        }


        /// <summary>
        /// Retrieve a specific item, based on its position related to the total number of items from 
        /// the Discog Web API. This method will determine on which page the requested item 
        /// is located, to query the right page.
        /// </summary>
        /// <param name="itemPosition">the position of the requested item</param>
        /// <returns>the item corresponding to the specified position</returns>
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


        /// <summary>
        /// Retrieve and return the release items from a specific page, and store them in memory.
        /// If the page has been retrieved previously, return the results from memory, 
        /// without fetching the page.
        /// </summary>
        /// <param name="pageNumber">the page number from which the release items must 
        /// be retrieved.</param>
        /// <returns>the release items from the specified page.</returns>
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

        /// <summary>
        /// Build the complete URL to retrieve a specific page
        /// </summary>
        /// <param name="pageNumber">the page number of interest</param>
        /// <returns>the complete URL that will permit to navigate to the desired page</returns>
        private String GetUrlFromPageNumber(int pageNumber)
        {
            
            var uriBuilder = new UriBuilder(DiscogApiUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[PageQueryParam] = pageNumber.ToString();
            query[PerPageQueryParam] = nbItemsPerPage.ToString();
            uriBuilder.Query = query.ToString();

            return uriBuilder.ToString();

        }

        /// <summary>
        /// Fetch a page from the Discog Web API based on a supplied URL
        /// </summary>
        /// <param name="pageUrl">The page URL of interest</param>
        /// <returns>The page matching the supplied URL</returns>
        private async Task<PageResult> RetrievePageFromHttpCall(String pageUrl)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var streamTask = client.GetStreamAsync(pageUrl);
            return await JsonSerializer.DeserializeAsync<PageResult>(await streamTask);
        }


        /// <summary>
        /// Retrieve and store pagination useful values from Discog Web API initial page.
        /// Those values are:
        /// - number of items per page
        /// - total number of items
        /// - total number of pages
        /// </summary>
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
