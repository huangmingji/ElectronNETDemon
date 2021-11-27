using Microsoft.AspNetCore.Authorization;

namespace ElectronNETVueDemon.Authentication
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
        
    }
}