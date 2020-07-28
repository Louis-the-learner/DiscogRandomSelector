using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using discogSelector.Models;
using discogSelector.Services;

namespace discogSelector.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DiscogSelectorController : ControllerBase
    {
        private const int MinimumAllowedSelection = 1;
        private const int MaximumAllowedSelection = 5;

        private static readonly String InvalidArgument = "Invalid argument value: {0} \r\nMust be between {1} and {2}";
    
        public DiscogSelectorController(DiscogSelectorService selectorService)
        {
            this.SelectorService = selectorService;
        }

        private DiscogSelectorService SelectorService { get; }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Release>>> Get([FromQuery] int nbOfSelections)
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

            for ( int randomReleaseTurn = 0; randomReleaseTurn < nbOfSelections; randomReleaseTurn++ )
            {
                int releaseItemPosition = r.Next(0, nbTotalOfItems);

                randomReleases.Add(await SelectorService.GetItem(releaseItemPosition));

            }
            return randomReleases;
        }

    }
}
