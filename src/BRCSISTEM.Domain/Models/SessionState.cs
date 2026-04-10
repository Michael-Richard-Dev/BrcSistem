using System;
using System.Collections.Generic;

namespace BRCSISTEM.Domain.Models
{
    public sealed class SessionState
    {
        public SessionState()
        {
            OpenModules = new List<OpenModuleState>();
            SavedAt = DateTime.UtcNow;
        }

        public string UserName { get; set; }

        public DateTime SavedAt { get; set; }

        public IList<OpenModuleState> OpenModules { get; private set; }
    }
}
