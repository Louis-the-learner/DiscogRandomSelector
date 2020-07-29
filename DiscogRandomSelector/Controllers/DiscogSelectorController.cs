using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using discogRandomSelector.Models;
using discogRandomSelector.Services;

namespace discogRandomSelector.Controllers
{
    /// <summary>
    /// This is the API controller that will expose a Get method to retrieve random items from
    /// the Discog collection.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class DiscogSelectorController : ControllerBase
    {
        private const int MinimumAllowedSelection = 1;
        private const int MaximumAllowedSelection = 5;

        private static readonly String InvalidArgument = "Invalid argument value: {0} \r\nMust be between {1} and {2}";

        public DiscogSelectorController(ISelectorService selectorService)
        {
            this.SelectorService = selectorService;
        }

        private ISelectorService SelectorService { get; }

        /// <summary>
        /// Retrieve a random selection of items from the Discog collection. 
        /// A supplied parameter determines how many to select, from 1 up to 5.
        /// </summary>
        /// <param name="nbOfSelections">The desired number of selections. 
        /// Must be between 1 and 5, will return error otherwise</param>
        /// <returns>A desired number of random selection of item from Discog collection</returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int nbOfSelections)
        {
            if (nbOfSelections < MinimumAllowedSelection || nbOfSelections > MaximumAllowedSelection)
            {
                return BadRequest(String.Format(InvalidArgument, nbOfSelections, MinimumAllowedSelection, MaximumAllowedSelection));
            }
            return Ok(await GetRandomReleases(nbOfSelections));

        }

        private async Task<List<Release>> GetRandomReleases(int nbOfSelections)
        {
            Random r = new Random();
            List<Release> randomReleases = new List<Release>();

            var nbTotalOfItems = await SelectorService.GetTotalItems();

            for (int randomReleaseTurn = 0; randomReleaseTurn < nbOfSelections; randomReleaseTurn++)
            {
                int releaseItemPosition = r.Next(0, nbTotalOfItems);

                randomReleases.Add(await SelectorService.GetItem(releaseItemPosition));
            }

            return randomReleases;
        }

    }
}
