namespace Task_1.Services
{
    public class UserCounterService
    {
        // Key: Username_Guid, Value: Username
        private readonly Dictionary<string, string> _activeSessions = new();

        public event Action? OnChange;

        // Returns the count of UNIQUE human beings
        public int ActiveUsers
        {
            get
            {
                lock (_activeSessions)
                {
                    return _activeSessions.Values.Distinct().Count();
                }
            }
        }

        // Formats names for the hover tooltip: "Admin (2 tabs), John"
        public List<string> GetUsernamesWithTabCount()
        {
            lock (_activeSessions)
            {
                return _activeSessions.Values
                    .GroupBy(name => name)
                    .Select(g => g.Count() > 1 ? $"{g.Key} ({g.Count()} tabs)" : g.Key)
                    .ToList();
            }
        }

        public void Join(string connectionKey, string username)
        {
            lock (_activeSessions)
            {
                if (_activeSessions.TryAdd(connectionKey, username))
                {
                    NotifyStateChanged();
                }
            }
        }

        public void Leave(string connectionKey)
        {
            lock (_activeSessions)
            {
                if (_activeSessions.Remove(connectionKey))
                {
                    NotifyStateChanged();
                }
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
