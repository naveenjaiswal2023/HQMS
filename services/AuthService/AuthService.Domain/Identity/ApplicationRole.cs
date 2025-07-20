using AuthService.Domain.Common;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Domain.Identity
{
    public class ApplicationRole : IdentityRole
    {
        [NotMapped]
        private readonly List<BaseDomainEvent> _domainEvents = new();
        [NotMapped]
        public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        // Optional navigation
        // public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName)
        {
            Id = Guid.NewGuid().ToString(); // override default string ID
        }

        public static ApplicationRole Create(string roleName)
        {
            return new ApplicationRole(roleName)
            {
                NormalizedName = roleName.ToUpperInvariant()
            };
        }

        public void Update(string newName)
        {
            if (Name != newName)
            {
                Name = newName;
                NormalizedName = newName.ToUpperInvariant();
            }
        }

        public void MarkAsDeleted()
        {
            // Add domain event here if needed
        }

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}