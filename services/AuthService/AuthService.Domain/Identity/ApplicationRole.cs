using AuthService.Domain.Common;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AuthService.Domain.Identity
{
    public class ApplicationRole : IdentityRole
    {
        private readonly List<BaseDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        // ✅ Add this for EF Core navigation
        //public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        //public ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();

        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(Guid.NewGuid().ToString())
        {
            Name = roleName;
            //AddDomainEvent(new RoleCreatedEvent(Id, roleName));
        }

        public static ApplicationRole Create(string roleName)
        {
            return new ApplicationRole(roleName);
        }

        public void Update(string newName)
        {
            if (Name != newName)
            {
                Name = newName;
                //AddDomainEvent(new RoleUpdatedEvent(Id, Name));
            }
        }

        public void MarkAsDeleted()
        {
            //AddDomainEvent(new RoleDeletedEvent(Id, Name));
        }

        //public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
