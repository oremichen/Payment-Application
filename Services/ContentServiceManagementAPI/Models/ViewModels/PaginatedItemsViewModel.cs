
using System.Collections.Generic;

namespace ContentServiceManagementAPI.Models.ViewModels
{
    public class PaginatedItemsViewModel<T> where T : class
    {
      public IEnumerable<T> Data { get; set; }

      public int TotalPages { get; set; }

      public int Pagenumber { get; set; }

      public int PageSize { get; set; }

        public PaginatedItemsViewModel(int pageSize, int pagenumber, IEnumerable<T> data)
        {
            this.Pagenumber = pagenumber;
            this.PageSize = pageSize;
            this.Data = data;
        }
        public PaginatedItemsViewModel()
        {

        }
    }
}
