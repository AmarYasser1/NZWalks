using System.ComponentModel.DataAnnotations;

namespace NZWalks.UI.Models.VMs
{
    public class AddRegionVM
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        public string? RegionImageUrl { get; set; }
    }
}
