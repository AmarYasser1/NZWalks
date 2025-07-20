using Microsoft.AspNetCore.Identity;

namespace NZWalks.API.Repositories.V1
{
    public interface ITokenRepository
    {
        string CreateJwtToken(IdentityUser user, IEnumerable<string> roles);
    }
}
