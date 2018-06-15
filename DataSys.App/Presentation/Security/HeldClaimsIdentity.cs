using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace DataSys.App.Presentation.Security
{
    public class HeldClaimsIdentity
    {
        public HeldClaimsIdentity(ClaimsIdentity i)
        {
            if (i?.Actor != null && i.Actor != i)
                Actor = new HeldClaimsIdentity(i.Actor);
            AuthenticationType = i?.AuthenticationType;
            Label = i?.Label;
            Claims = i?.Claims.Select(c => new ClaimHeld(c)) ?? new List<ClaimHeld>();
        }

        public HeldClaimsIdentity Actor { get; set; }
        public string AuthenticationType { get; set; }
        public IEnumerable<ClaimHeld> Claims { get; set; }
        public string Label { get; set; }
    }
}