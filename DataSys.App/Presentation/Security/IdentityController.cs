using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataSys.App.Presentation.Security
{
    [Route(RoutePrefix)]
    public class IdentityController: ControllerBase
    {
        public const string RoutePrefix = "api/identity";
        public class ClaimHeld
        {
            private readonly Claim _claim;

            public ClaimHeld(Claim claim)
            {
                _claim = claim;
            }

            public string Issuer => _claim.Issuer;
            public string OriginalIssuer => _claim.OriginalIssuer;
            public string ValueType => _claim.ValueType;
            public string Value => _claim.Value;
        }
        public const string AuthorizeRoute = "authorize";
        [HttpGet]
        [Route(AuthorizeRoute)]
        [Authorize]
        public virtual IEnumerable<ClaimHeld> ClaimsWhenAuthorize() => User.Claims.Select(c => new ClaimHeld(c));
        public const string NoAuthorizeRoute = "noauthorize";
        [HttpGet]
        [Route(NoAuthorizeRoute)]
        public virtual IEnumerable<ClaimHeld> ClaimsWhenNoAuthorize() => User.Claims.Select(c => new ClaimHeld(c));
        public const string RoleAAuthorizeRoute = "rolea";
        [HttpGet]
        [Route(RoleAAuthorizeRoute)]
        [Authorize(Roles = "RoleA")]
        public virtual IEnumerable<ClaimHeld> ClaimsWhenRoleAAuthorize() => User.Claims.Select(c => new ClaimHeld(c));
    }
}
