﻿using Microsoft.Extensions.Logging;

namespace Casbin.AspNetCore.Extensions
{
    internal static class LoggerExtension
    {
        internal static void CasbinAuthorizationSucceeded(this ILogger logger)
        {
            // not implement
        }

        internal static void CasbinAuthorizationFailed(this ILogger logger)
        {
            // not implement
        }
    }
}
