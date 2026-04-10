using System.Collections.Generic;

namespace BRCSISTEM.Application.Models
{
    public sealed class SaveUserTypeRequest
    {
        public string OriginalName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IReadOnlyCollection<string> PermissionKeys { get; set; }

        public string ActorUserName { get; set; }
    }
}
