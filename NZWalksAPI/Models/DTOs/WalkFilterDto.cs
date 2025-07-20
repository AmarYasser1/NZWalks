namespace NZWalks.API.Models.DTOs
{
    public class WalkFilterDto
    {
        public string? Name { get; set; }
        public double? LengthInKm { get; set; }
        public Guid? DifficultyId { get; set; }
        public Guid? RegionId { get; set; }
    }
}
