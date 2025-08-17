using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace webapi.Authorization
{
    public static class AuthorizationPolicies
    {
        public const string AdminOnly = "AdminOnly";
        public const string AdminOrReceptionist = "AdminOrReceptionist";
        public const string DoctorOnly = "DoctorOnly";

        public static void AddPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AdminOnly, policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy(AdminOrReceptionist, policy =>
                    policy.RequireRole("Admin", "Receptionist"));

                options.AddPolicy(DoctorOnly, policy =>
                    policy.RequireRole("Doctor"));
            });
        }
    }
}
