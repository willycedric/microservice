using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoesOnContainers.Services.ProductCatalogApi.ViewModels
{
    public class PaginatedItemsViewModel<TEntity> where TEntity:class
    {
        public int PageSize { get; private set; }
        public int PageIndex { get; private set; }
        public long Count { get; set; }
        public IEnumerable<TEntity> Data { get; set; }

        public PaginatedItemsViewModel(int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.Count = count;
            this.Data = data;
        }
    }
}
