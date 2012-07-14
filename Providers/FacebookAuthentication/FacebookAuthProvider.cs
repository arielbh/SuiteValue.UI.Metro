using System;
using System.Composition;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CodeValue.SuiteValue.UI.Metro.Authentications;
using Newtonsoft.Json;
using Windows.Security.Authentication.Web;

namespace CodeValue.SuiteValue.UI.Metro.FacebookAuthentication
{
    [Export(typeof(IAuthProvider))]
    public class FacebookAuthProvider : IAuthProvider
    {
        private const string AuthUrl = "https://www.facebook.com/dialog/oauth";
        private const string UserUrl = "https://graph.facebook.com/me";

        private string _clientId;
        private string _redirectUrl;

        public void Configure(dynamic configuration)
        {
            _clientId = configuration.FacebookClientId;
            _redirectUrl = configuration.FacebookRedirectUrl;
        }

        public async Task<UserInfo> Authenticate()
        {
            var facebookUrl = AuthUrl+"?client_id=" + Uri.EscapeDataString(_clientId) + "&redirect_uri=" + Uri.EscapeDataString(_redirectUrl) + "&scope=read_stream&display=popup&response_type=token";

            var startUri = new Uri(facebookUrl);
            var endUri = new Uri(_redirectUrl);


            var webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                    WebAuthenticationOptions.None,
                                                    startUri,
                                                    endUri);
            if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {                                             
                var target = "access_token=";
                var expires = "&expires";
                var expiresIndex = webAuthenticationResult.ResponseData.IndexOf(expires);
                var index = webAuthenticationResult.ResponseData.IndexOf(target) + target.Length;
                var token = webAuthenticationResult.ResponseData.Substring(index, expiresIndex - index);

                var client = new HttpClient();
                var result = await client.GetAsync(UserUrl+"?access_token=" + token);
                result.EnsureSuccessStatusCode();
                var json = await result.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(json);
                return new UserInfo { Id = user.id, Name = user.name, UserName = user.username };
            }
            return null;
        }

        public string Name { get { return "Facebook"; } }
    }

    [DataContract]
    class User
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string username { get; set; }
    }

}
