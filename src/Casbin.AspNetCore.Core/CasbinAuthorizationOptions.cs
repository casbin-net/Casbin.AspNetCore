using System;
using System.Security.Claims;
using Casbin;
using Casbin.Model;

namespace Casbin.AspNetCore.Authorization
{
    public class CasbinAuthorizationOptions
    {
        public string? DefaultModelPath { get; set; }
        public string? DefaultPolicyPath { get; set; }
        public Func<IServiceProvider, IModel?, Enforcer>? DefaultEnforcerFactory { get; set; }
        public string? DefaultAuthenticationSchemes { get; set; }
        public IRequestTransformer? DefaultRequestTransformer { get; set; }
        public string PreferSubClaimType { get; set; } = ClaimTypes.NameIdentifier;
        public bool AllowAnyone { get; set; } = false;
    }
}
