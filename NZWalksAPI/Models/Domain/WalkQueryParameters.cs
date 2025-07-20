using NZWalks.API.Models.DTOs;

namespace NZWalks.API.Models.Domain
{
    public class WalkQueryParameters
    {
        public string? FilterOn { get; set; }
        public string? FilterQuery { get; set; }

        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = false;

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 1000;
    }
}
