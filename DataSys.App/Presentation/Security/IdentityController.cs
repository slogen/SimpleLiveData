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
    }

    // Cannot directly serrialize claim
}