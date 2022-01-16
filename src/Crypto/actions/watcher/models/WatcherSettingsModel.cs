namespace Crypto.Actions.Watcher.Models
{
    public class WatcherSettingsModel
    {
        public string Name { get; set; } = "bitcoin";
        public string Currency { get; set; } = "usd";
        public int RefreshDelay { get; set; } = 10;
    }
}
