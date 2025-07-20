namespace NZWalks.API.Repositories.V2
{
    public interface IUnitOfWork
    {
        IRegionRepository RegionRepository { get; }
        IWalkRepository WalkRepository { get; }

        Task SaveAsync(CancellationToken cancellationToken = default);
    }
}
