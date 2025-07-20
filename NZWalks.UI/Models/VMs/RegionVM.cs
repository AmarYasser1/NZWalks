namespace NZWalks.UI.Models.VMs
{
    public class RegionVM
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? RegionImageUrl { get; set; }
    }
}
