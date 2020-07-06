﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Casbin.AspNetCore.Abstractions;
using Casbin.AspNetCore.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Casbin.AspNetCore
{
    public class DefaultEnforcerService : IEnforceService
    {
        private readonly IOptions<CasbinAuthorizationCoreOptions> _options;
        private readonly IEnumerable<IRequestTransformer> _transformers;
        private readonly IEnforcerProvider _enforcerProvider;
        private readonly ILogger<DefaultEnforcerService> _logger;

        public DefaultEnforcerService(
            IOptions<CasbinAuthorizationCoreOptions> options,
            IEnumerable<IRequestTransformer> transformers,
            IEnforcerProvider enforcerProvider,
            ILogger<DefaultEnforcerService> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(CasbinAuthorizationCoreOptions));
            _transformers = transformers ?? throw new ArgumentNullException(nameof(transformers));
            _enforcerProvider = enforcerProvider ?? throw new ArgumentNullException(nameof(enforcerProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual Task<bool> EnforceAsync(ICasbinAuthorizationContext context)
        {
            var enforcer = _enforcerProvider.GetEnforcer();
            if (enforcer is null)
            {
                throw new ArgumentException("Can not find any enforcer.");
            }

            bool noDefault = _options.Value.DefaultRequestTransformer is null;
            var transformersArray = _transformers.ToArray();
            if (transformersArray.Length == 0 && noDefault)
            {
                throw new ArgumentException("Can find any request transformer.");
            }

            // The order of decide transformer is :
            // 1. context.Data.RequestTransformerType >
            // 2. _options.Value.DefaultRequestTransformer >
            // 3. _transformers.FirstOrDefault()
            IRequestTransformer? transformer = null;
            if (!(context.Data.RequestTransformerType is null))
            {
                transformer = _transformers.FirstOrDefault( t => t.GetType() == context.Data.RequestTransformerType);
            }
            else if (!noDefault)
            {
                transformer = _options.Value.DefaultRequestTransformer;
            }
            transformer ??= _transformers.FirstOrDefault();

            // The order of deciding transformer.PreferSubClaimType is :
            // 1. context.Data.PreferSubClaimType >
            // 2. _options.Value.PreferSubClaimType
            transformer.PreferSubClaimType = context.Data.PreferSubClaimType ?? _options.Value.PreferSubClaimType;

            // The order of deciding transformer.PreferSubClaimType is :
            // 1. context.Data.PreferSubClaimType >
            // 2. null (if this issuer is null, it will be ignored)
            transformer.Issuer = context.Data.Issuer;

            string sub = transformer.SubTransform(context);
            object obj = transformer.ObjTransform(context);
            string act = transformer.ActTransform(context);

            if (enforcer.Enforce(sub, obj, act))
            {
                _logger.CasbinAuthorizationSucceeded();
                return Task.FromResult(true);
            }

            _logger.CasbinAuthorizationFailed();
            return Task.FromResult(false);
        }
    }
}
