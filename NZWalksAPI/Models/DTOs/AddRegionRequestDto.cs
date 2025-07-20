using System.ComponentModel.DataAnnotations;

namespace NZWalks.API.Models.DTOs
{
    public class AddRegionRequestDto
    {
        [Required]
        [Length(3, 3, ErrorMessage ="Code must be 3 characters.")]
        public string Code { get; set; } = null!;
        [Required]
        [MaxLength(100, ErrorMessage ="Name must be at most 100 characters.")]
        public string Name { get; set; } = null!;
        public string? RegionImageUrl { get; set; }
    }
}
