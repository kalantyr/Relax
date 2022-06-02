using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Client;
using Kalantyr.Auth.Models;
using Relax.Interfaces;
using Relax.Utils;

namespace Relax.Controllers
{
    internal class AuthController: IAuthController
    {
        private readonly UserSettings _userSettings;

        public TokenInfo TokenInfo { get; private set; }

        public AuthController(UserSettings userSettings)
        {
            _userSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
        }

        public async Task LoginByPasswordAsync(string login, string password, bool storePassword, CancellationToken cancellationToken)
        {
            IAuthClient client = new AuthClient(new HttpClientFactory(Settings.Default.AuthService));
            var dto = new LoginPasswordDto { Login = login, Password = password };
            var result = await client.LoginByPasswordAsync(dto, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
            TokenInfo = result.Result;

            if (storePassword)
            {
                var loginPassword = new UserSettings.LoginPassword{Login = login, Password = password};
                _userSettings.Logins = _userSettings.Logins.Add(loginPassword);
                await _userSettings.SaveAsync();
            }

            UserLoggedIn?.Invoke();
        }

        public async Task LogoutAsync(CancellationToken cancellationToken)
        {
            IAuthClient client = new AuthClient(new HttpClientFactory(Settings.Default.AuthService));
            var result = await client.LogoutAsync(TokenInfo.Value, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
            TokenInfo = null;
            UserLoggedOut?.Invoke();
        }

        public event Action? UserLoggedIn;
        
        public bool IsUserLogged => TokenInfo != null;

        public event Action? UserLoggedOut;

        internal class HttpClientFactory : IHttpClientFactory
        {
            private readonly string _authServiceUrl;

            public HttpClientFactory(string authServiceUrl)
            {
                _authServiceUrl = authServiceUrl ?? throw new ArgumentNullException(nameof(authServiceUrl));
            }

            public HttpClient CreateClient(string name)
            {
                return new HttpClient
                {
                    BaseAddress = new Uri(_authServiceUrl)
                };
            }
        }
    }
}
