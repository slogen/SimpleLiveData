using System.Collections.Generic;
using System.Security.Claims;

namespace DataSys.App.Presentation.Security
{
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
}