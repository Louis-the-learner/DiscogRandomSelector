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

namespace discogSelector.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscogSelectorController : ControllerBase
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly String DiscogApiUrl = "https://api.discogs.com/users/ausamerika/collection/folders/0/releases";

        private static readonly String PageQueryParam = "page";

        private static readonly String PerPageQueryParam = "per_page";


        private static int nbTotalOfItems;

        private static int nbTotalOfPages;

        private static int nbItemsPerPage;

        private static Dictionary<int, List<Release>> pageResultsDictionnary = new Dictionary<int, List<Release>>();

        public DiscogSelectorController()
        {
        }

        [HttpGet]
        public async Task<List<Release>> Get([FromQuery] int nbOfSelections)
        {
            if (nbItemsPerPage == 0 || nbTotalOfItems == 0 || nbTotalOfPages == 0)
            {
               await InitPageAndItemNumbers();
            }

            return await GetRandomReleases(nbOfSelections);

        }

        private async Task<List<Release>> GetRandomReleases(int nbOfSelections)
        {
            Random r = new Random();
            List<Release> randomReleases = new List<Release>();

            // Remove the explicit 1, it is temporary
            for ( int randomReleaseTurn = 0; randomReleaseTurn < nbOfSelections; randomReleaseTurn++ )
            {
                int releaseItemPosition = r.Next(0, nbTotalOfItems);

                // determine on which page is located the selected release, using a page offset based on the remainer
                int nextPageOffSet = (releaseItemPosition % nbItemsPerPage > 0) ? 1 : 0;
                int pageNumber = releaseItemPosition / nbItemsPerPage + nextPageOffSet;

                var releasesResults = await GetPageResults(pageNumber);

                // get the position of the selected release from the page
                var releasePositionWithinPage = releaseItemPosition % nbItemsPerPage;

                randomReleases.Add(releasesResults.ElementAt(releasePositionWithinPage));

            }

            return randomReleases;
            
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
