using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace DataSys.App.Presentation.Security
{
    public class HeldClaimsPrincipal
    {
        public HeldClaimsPrincipal(ClaimsPrincipal p)
        {
            Identities = p?.Identities.Select(i => new HeldClaimsIdentity(i)) ?? new List<HeldClaimsIdentity>();
        }

        public IEnumerable<HeldClaimsIdentity> Identities { get; set; }
    }
}