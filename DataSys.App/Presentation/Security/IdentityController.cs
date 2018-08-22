using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataSys.App.Presentation.Security
{
    [Route(RoutePrefix)]
    public class IdentityController : ControllerBase
    {
        public const string RoutePrefix = "api/identity";
        public const string AuthorizeRoute = "authorize";
        public const string NoAuthorizeRoute = "noauthorize";
        public const string RoleAAuthorizeRoute = "rolea";

        [HttpGet]
        [Route(AuthorizeRoute)]
        [Authorize]
        public virtual HeldClaimsPrincipal ClaimsWhenAuthorize() => ToProtocol(User);

        [HttpGet]
        [Route(NoAuthorizeRoute)]
        public virtual HeldClaimsPrincipal ClaimsWhenNoAuthorize() => ToProtocol(User);

        [HttpGet]
        [Route(RoleAAuthorizeRoute)]
        [Authorize(Roles = "RoleA")]
        public virtual HeldClaimsPrincipal ClaimsWhenRoleAAuthorize() => ToProtocol(User);

        protected virtual HeldClaimsPrincipal ToProtocol(ClaimsPrincipal p) => new HeldClaimsPrincipal(p);


        // Cannot directly serrialize claim, so we have a protocol :)
        public class ClaimHeld
        {
            public ClaimHeld(Claim claim)
            {
                Issuer = claim?.Issuer;
                OriginalIssuer = claim?.OriginalIssuer;
                Type = claim?.Type;
                Value = claim?.Value;
                ValueType = claim?.ValueType;
                Properties = claim?.Properties ?? new Dictionary<string, string>();
            }

            public string Issuer { get; set; }
            public string OriginalIssuer { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
            public string ValueType { get; set; }
            public IDictionary<string, string> Properties { get; set; }
        }
        public class HeldClaimsPrincipal
        {
            public HeldClaimsPrincipal(ClaimsPrincipal p)
            {
                Identities = p?.Identities.Select(i => new HeldClaimsIdentity(i)) ?? new List<HeldClaimsIdentity>();
            }

            public IEnumerable<HeldClaimsIdentity> Identities { get; set; }
        }
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
}