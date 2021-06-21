namespace Casbin.AspNetCore.Authorization
{
    public interface ICasbinModelProvider
    {
        public IModel? GetModel();
    }
}
