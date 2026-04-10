using System;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class SessionStateService
    {
        private readonly ISessionStateStore _sessionStateStore;

        public SessionStateService(ISessionStateStore sessionStateStore)
        {
            _sessionStateStore = sessionStateStore;
        }

        public SessionState Load(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return new SessionState();
            }

            return _sessionStateStore.Load(userName.Trim());
        }

        public void Save(SessionState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            _sessionStateStore.Save(state);
        }

        public void Clear(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return;
            }

            _sessionStateStore.Clear(userName.Trim());
        }
    }
}
