using ContentServiceManagementAPI.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContentServiceManagementAPI.Infrastructure.Helpers
{
    public static class Pagination
    {
        // method to return a pagged data content loaded from the server using the details of the pararmeter given

        public static PaginatedItemsViewModel  <T> PaggedResult<T>(this IList<T> listcontent, int pagesize, int pagenumber) where T : class
        {
            var result = new PaginatedItemsViewModel<T>();
            result.Data = listcontent.Skip(pagesize * (pagenumber - 1)).Take(pagesize).ToList();
            result.TotalPages = Convert.ToInt32(Math.Ceiling((double)listcontent.Count() / pagesize));
            result.Pagenumber = pagenumber;
            result.PageSize = pagesize;
            return result;
        }

    }
}
