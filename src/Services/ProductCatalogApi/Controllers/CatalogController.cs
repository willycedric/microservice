using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShoesOnContainers.Services.ProductCatalogApi;
using ShoesOnContainers.Services.ProductCatalogApi.Data;
using ShoesOnContainers.Services.ProductCatalogApi.Domain;
using ShoesOnContainers.Services.ProductCatalogApi.ViewModels;

namespace ProductCatalogApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Catalog")]
    public class CatalogController : Controller
    {
        private readonly CatalogContext _catalgoContext;
        private readonly IOptionsSnapshot<CatalogSettings> _settings;
        public CatalogController(CatalogContext catalogContext, IOptionsSnapshot<CatalogSettings> settings)
        {
            _catalgoContext = catalogContext;
            _settings = settings;
            string url = _settings.Value.ExternalCalatalogBaseUrl;
            ((DbContext)catalogContext).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogTypes()
        {
            var items = await _catalgoContext.CatalogTypes.ToListAsync();
            return Ok(items);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogBrand()
        {
            var items = await _catalgoContext.CatalogBrands.ToListAsync();
            return Ok(items);
        }

        [HttpGet]
        [Route("items/{id:int}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            if(id <= 0)
            {
                return BadRequest();
            }
            var item = await _catalgoContext.CatalogItems.SingleOrDefaultAsync(c => c.Id == id);
            if(item != null)
            {
                item.PictureUrl = item.PictureUrl.Replace("http://externalcatalogbaseurltobereplaced", _settings.Value.ExternalCalatalogBaseUrl);
                return Ok(item);
            }
               
            return NotFound();
        }


        //GET api/Catalog/items[?pageSize=4&pageIndex=3]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Items([FromQuery] int pageSize=6, [FromQuery] int pageIndex=0)
        {
            var totalItems = await _catalgoContext.CatalogItems.LongCountAsync();
            var itemsOnPage = await _catalgoContext.CatalogItems
                                    .OrderBy(c => c.Name)
                                    .Skip(pageSize * pageIndex)
                                    .Take(pageSize)
                                    .ToListAsync();

            itemsOnPage = ChangeUrlPlaceHolder(itemsOnPage);
            var model = new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }


        //GET api/Catalog/items[?pageSize=4&pageIndex=3]
        [HttpGet]
        [Route("[action]/Withname/{name:minlength(1)}")]
        public async Task<IActionResult> Items(string name, [FromQuery] int pageSize = 6, [FromQuery] int pageIndex = 0)
        {
            var totalItems = await _catalgoContext.CatalogItems
                .Where(c => c.Name.StartsWith(name))
                .LongCountAsync();
                
            var itemsOnPage = await _catalgoContext.CatalogItems
                                    .Where(c=>c.Name.StartsWith(name))
                                    .OrderBy(c => c.Name)
                                    .Skip(pageSize * pageIndex)
                                    .Take(pageSize)
                                    .ToListAsync();

            itemsOnPage = ChangeUrlPlaceHolder(itemsOnPage);
            var model = new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }




        private List<CatalogItem> ChangeUrlPlaceHolder(List<CatalogItem> items)
        {
            items.ForEach(x =>
               x.PictureUrl = x.PictureUrl.Replace("http://externalcatalogbaseurltobereplaced", _settings.Value.ExternalCalatalogBaseUrl)
            );
            return items;
        }
    }
}