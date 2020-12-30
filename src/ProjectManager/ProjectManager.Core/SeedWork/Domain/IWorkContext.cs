namespace ProjectManager.Core.SeedWork.Domain
{
    public interface IWorkContext
    {
        string GetUserId();
        string GetAccessToken();
        string TenantId { get; }
        string FullName { get; }
        string Email { get; }
    }
}