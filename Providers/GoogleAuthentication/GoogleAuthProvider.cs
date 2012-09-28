using System;
using System.Composition;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CodeValue.SuiteValue.UI.Metro.Authentications;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Windows.Security.Authentication.Web;

namespace CodeValue.SuiteValue.UI.Metro.GoogleAuthentication
{
    [Export(typeof(IAuthProvider))]
    public class GoogleAuthProvider : IAuthProvider
    {
        private const string AuthorizationUrl = "https://accounts.google.com/o/oauth2/auth";
        private const string ApprovalUrl = "https://accounts.google.com/o/oauth2/approval?";
        private const string UserInfoUrl = "https://www.googleapis.com/oauth2/v1/userinfo";
        private const string TokenUrl = "https://accounts.google.com/o/oauth2/token";
        private const string DefaultScope =
            "https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile ";
        private string _clientId;
        private string _redirectId;
        private string _clientSecret;
        private string _scope;
        
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }

        public void Configure(dynamic configuration)
        {
            _clientId = configuration.GoogleClientId;
            _redirectId = configuration.GoogleRedirectUrl;
            _clientSecret = configuration.GoogleClientSecret;
            try
            {
                _scope = DefaultScope + configuration.GoogleScope;
            }
            catch (RuntimeBinderException)
            {
                // GoogleScope doesn't exist.
                _scope = DefaultScope;
            }
        }
        public async Task<UserInfo> Authenticate()
        {
            var googleUrl = AuthorizationUrl + "?client_id=" + Uri.EscapeDataString(_clientId) + "&redirect_uri="
                + Uri.EscapeDataString(_redirectId) + "&response_type=code&scope=" + Uri.EscapeDataString(_scope);

            var startUri = new Uri(googleUrl);
            // When using the desktop flow, the success code is displayed in the html title of this end uri
            var endUri = new Uri(ApprovalUrl);


            var webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                    WebAuthenticationOptions.UseTitle,
                                                    startUri,
                                                    endUri);

            if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                var code = webAuthenticationResult.ResponseData.Remove(0, 13);
                Token token = await RequestToken(_clientId, code);
                AccessToken = token.access_token;
                RefreshToken = token.refresh_token;
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.access_token);
                var result = await client.GetAsync(UserInfoUrl);
                result.EnsureSuccessStatusCode();
                var json = await result.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(json);
                string handle = null;
                if (user != null && !string.IsNullOrEmpty(user.email))
                {
                    var emailParts = user.email.Split('@');
                    if (emailParts.Length == 2)
                    {
                        handle = emailParts[0];
                    }
                }
                if (user != null) return new UserInfo {Id = user.id, Name = user.name, UserName = handle ?? user.name};
            }
            return null;

        }
        public async Task<Token> RequestToken(string clientId, string key)
        {
            HttpMessageHandler handler = new HttpClientHandler();
            
            var httpClient = new HttpClient(handler);
            string postData = string.Format("client_id={0}&client_secret={1}&code={2}&redirect_uri={3}&grant_type=authorization_code", clientId, _clientSecret, key, _redirectId);
            var c = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");

            httpClient.MaxResponseContentBufferSize = 100000;

            var result = await httpClient.PostAsync(TokenUrl, c);
            result.EnsureSuccessStatusCode();
            var json = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Token>(json);
        }

        public async Task<string> RefreshTokenAsync(string refreshToken)
        {
            HttpMessageHandler handler = new HttpClientHandler();

            var httpClient = new HttpClient(handler);
            string postData = string.Format("client_id={0}&client_secret={1}&refresh_token={2}&grant_type=refresh_token", _clientId, _clientSecret, refreshToken);
            var c = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");

            httpClient.MaxResponseContentBufferSize = 100000;

            var result = await httpClient.PostAsync(TokenUrl, c);
            result.EnsureSuccessStatusCode();
            var json = await result.Content.ReadAsStringAsync();
            AccessToken = JsonConvert.DeserializeObject<Token>(json).access_token;
            return AccessToken;
        }

        public string Name
        {
            get { return "Google"; }
        }
    }

    [DataContract]
    class User
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string email { get; set; }
    }

    [DataContract]
    public class Token
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public int expires_in { get; set; }
        [DataMember]
        public string refresh_token { get; set; }
    }
}
