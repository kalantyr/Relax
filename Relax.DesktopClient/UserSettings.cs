using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Relax.DesktopClient
{
    public class UserSettings
    {
        public LoginPassword[] Logins { get; set; } = Array.Empty<LoginPassword>();

        public static UserSettings Instance = new();

        private static readonly string FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Relax.settings.json");

        static UserSettings()
        {
            if (File.Exists(FileName))
            {
                using var file = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                Instance = JsonSerializer.Deserialize<UserSettings>(file);
            }
        }

        public async Task SaveAsync()
        {
            await using var file = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(file, this);
        }

        public class LoginPassword
        {
            public string Login { get; set; }

            public string Password { get; set; }
        }
    }
}
