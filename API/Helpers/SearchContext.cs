using API.Entities;
using API.Extensions;
using System;
using System.Collections.Generic;

namespace API.Helpers
{
    public class SearchContext : BaseParams
    {
        public Dictionary<string, string> QueryParams { get; }
        public string SearchText { get; }
        public string[] Keywords { get; }
        public Dictionary<string, string> Filters { get; }

        public SearchContext(Dictionary<string, string> queryParams) : base(queryParams)
        {
            QueryParams = queryParams ?? new Dictionary<string, string>();
            QueryParams.TryGetValue("q", out var searchText);
            SearchText = searchText?.Trim() ?? string.Empty;
            Keywords = SearchText.ToLower().SubWords();
            Filters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
