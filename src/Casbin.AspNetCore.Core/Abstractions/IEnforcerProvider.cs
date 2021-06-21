using Casbin;

// ReSharper disable once CheckNamespace
namespace Casbin.AspNetCore.Authorization
{
    public interface IEnforcerProvider
    {
        public Enforcer? GetEnforcer();
    }
}
