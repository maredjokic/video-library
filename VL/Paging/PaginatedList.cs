using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Video_Library_Api.Paging
{
    public class PaginatedList<T> 
    {
        /// <summary>
        /// for response for list of tags or videos
        /// </summary>
        public IEnumerable<T> Data { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }//number of all elements 
        public int TotalPages { get; set; }

        public PaginatedList(IEnumerable<T> data, int totalCount, PagingParams pagingParams)
        {
            CurrentPage = pagingParams.Page;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pagingParams.PageSize);
            TotalCount = totalCount;
            PageSize = pagingParams.PageSize;

            Data = data;
        }

        public bool HasPreviousPage
        {
            get
            {
                return (CurrentPage > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (CurrentPage < TotalPages);
            }
        }
    }
}
