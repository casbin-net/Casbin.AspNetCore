﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Casbin.AspNetCore.Authorization.Transformers
{
    public class BasicRequestTransformer : RequestTransformer
    {
        public override string PreferSubClaimType { get; set; } = ClaimTypes.NameIdentifier;

        public override ValueTask<IEnumerable<object>> TransformAsync(ICasbinAuthorizationContext context, ICasbinAuthorizationData data)
        {
            object[] requestValues = new object[data.ValueCount + 1];
            requestValues[0] = SubTransform(context, data);

            requestValues[1] = ObjTransform(context, data,
                (_, d) => d.Value1);
            requestValues[2] = ActTransform(context, data,
                (_, d) => d.Value2);
            return new ValueTask<IEnumerable<object>>(requestValues);
        }

        protected virtual string SubTransform(ICasbinAuthorizationContext context, ICasbinAuthorizationData data)
        {
            HttpContext httpContext = context.HttpContext;
            Claim? claim;
            if (Issuer is null)
            {
                claim = httpContext.User.FindFirst(PreferSubClaimType);
                return claim is null ? string.Empty : claim.Value;
            }

            claim = httpContext.User.FindAll(PreferSubClaimType).FirstOrDefault(
                c => string.Equals(c.Issuer, Issuer));
            return claim is null ? string.Empty : claim.Value;
        }

        protected virtual string ObjTransform(ICasbinAuthorizationContext context, ICasbinAuthorizationData data,
            Func<ICasbinAuthorizationContext, ICasbinAuthorizationData, string> valueSelector)
            => valueSelector(context, data);

        protected virtual string ActTransform(ICasbinAuthorizationContext context, ICasbinAuthorizationData data, Func<ICasbinAuthorizationContext, ICasbinAuthorizationData, string> valueSelector)
            => valueSelector(context, data);
    }
}
