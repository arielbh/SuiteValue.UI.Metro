using System;
using System.Collections.Generic;
using System.Composition;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CodeValue.SuiteValue.UI.Metro.Authentications;
using Newtonsoft.Json;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace CodeValue.SuiteValue.UI.Metro.TwitterAuthentication
{
    [Export(typeof(IAuthProvider))]
    public class TwitterAuthProvider : IAuthProvider
    {
        const string RequsetUrl = "https://api.twitter.com/oauth/request_token";
        const string AuthorizeUrl = "https://api.twitter.com/oauth/authorize";
        const string AccessTokenUrl = "https://api.twitter.com/oauth/access_token";
        const string UserShowUrl = "https://api.twitter.com/1/users/show.json";

        

        private string _clientId;
        private string _clientSecret;
        private string _redirectUrl;
        private string _oAuthToken;
        private string _oAuthTokenSecret;

        public string OAuthTokenSecret
        {
            get
            {
                return this._oAuthTokenSecret;
            }
        }

        public string OAuthToken
        {
            get
            {
                return this._oAuthToken;
            }
        }

        public void Configure(dynamic configuration)
        {
            _clientId = configuration.TwitterClientId;
            _clientSecret = configuration.TwitterClientSecret;
            _redirectUrl = configuration.TwitterRedirectUrl;
            _redirectUrl = _redirectUrl.TrimEnd('/');
        }

        private void ExtractOAuthToken(string content)
        {
            _oAuthToken = null;
            _oAuthTokenSecret = null;
            String[] keyValPairs = content.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                String[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        _oAuthToken = splits[1];
                        break;
                    case "oauth_token_secret":
                        _oAuthTokenSecret = splits[1];
                        break;
                }
            }
        }

        private async Task AcquireRequestToken()
        {
            var authHeader = CreateAuthenticationSignature(RequsetUrl);

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", authHeader);
            //var postData = "oauth_verifier=" + authVerifier;
            StringContent c = new StringContent("", Encoding.UTF8, "application/x-www-form-urlencoded");
            httpClient.MaxResponseContentBufferSize = 100000;

            var result = await httpClient.PostAsync(RequsetUrl,c);
            result.EnsureSuccessStatusCode();
            var data = await result.Content.ReadAsStringAsync();
            ExtractOAuthToken(data);
        }

        public async Task<string> RequestToken(string token, string authVerifier, string tokenSecret)
        {
            var authHeader = CreateAuthenticationSignature(AccessTokenUrl, token);

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", authHeader);
            var postData = "oauth_verifier=" + authVerifier;
            StringContent c = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
            httpClient.MaxResponseContentBufferSize = 100000;

            var result = await httpClient.PostAsync(AccessTokenUrl, c);
            result.EnsureSuccessStatusCode();
            var data = await result.Content.ReadAsStringAsync();
            Dictionary<string, string> accessToken = new Dictionary<string, string>();
            foreach (var pair in data.Split('&'))
            {
                var p = pair.Split('=');
                accessToken.Add(p[0], p[1]);
            }
            _oAuthToken = accessToken["oauth_token"];
            _oAuthTokenSecret = accessToken["oauth_token_secret"];
            return accessToken["user_id"];
        }

        private async Task<User> RequestUser(string userId)
        {
            HttpClient httpClient = new HttpClient();

            var userData = await httpClient.GetAsync(UserShowUrl + "?user_id=" + userId);
            userData.EnsureSuccessStatusCode();
            var jsonresult = await userData.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(jsonresult);
            return user;
        }

        private string CreateAuthenticationSignature(string url, string token = null)
        {
            var sinceEpoch = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
            var rand = new Random();
            var nonce = rand.Next(1000000000);

            //
            // Compute base signature string and sign it.
            //    This is a common operation that is required for all requests even after the token is obtained.
            //    Parameters need to be sorted in alphabetical order
            //    Keys and values should be URL Encoded.
            //
            var signatureMethod = "oauth_callback=" + Uri.EscapeDataString(_redirectUrl);
            signatureMethod += "&" + "oauth_consumer_key=" + _clientId;
            signatureMethod += "&" + "oauth_nonce=" + nonce.ToString();
            signatureMethod += "&" + "oauth_signature_method=HMAC-SHA1";
            signatureMethod += "&" + "oauth_timestamp=" + Math.Round(sinceEpoch.TotalSeconds);
            signatureMethod += "&" + "oauth_version=1.0";
            if (!string.IsNullOrEmpty(token))
            {
                signatureMethod += "&" + "oauth_token=" + token;
            }

            var signatureString = "POST&";
            signatureString += Uri.EscapeDataString(url) + "&" + Uri.EscapeDataString(signatureMethod);

            var keyMaterial = CryptographicBuffer.ConvertStringToBinary(_clientSecret + "&", BinaryStringEncoding.Utf8);
            var hmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            var macKey = hmacSha1Provider.CreateKey(keyMaterial);
            var dataToBeSigned = CryptographicBuffer.ConvertStringToBinary(signatureString, BinaryStringEncoding.Utf8);
            var signatureBuffer = CryptographicEngine.Sign(macKey, dataToBeSigned);
            var signature = CryptographicBuffer.EncodeToBase64String(signatureBuffer);
            var dataToPost = "OAuth oauth_callback=\"" + Uri.EscapeDataString(_redirectUrl) + "\", oauth_consumer_key=\"" +
                _clientId + "\", oauth_nonce=\"" + nonce.ToString() + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"" + Math.Round(sinceEpoch.TotalSeconds) + "\", oauth_version=\"1.0\", oauth_signature=\"" + 
                Uri.EscapeDataString(signature) + "\"";
            if (!string.IsNullOrEmpty(token))
            {
                dataToPost += ", oauth_token=\"" + token + "\"";
            }


            return dataToPost;
        }

        public async Task<UserInfo> Authenticate()
        {
            await AcquireRequestToken();
            var startUri = new Uri(AuthorizeUrl + "?oauth_token=" + _oAuthToken);
            var endUri = new Uri(_redirectUrl);


            var webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                               WebAuthenticationOptions.None,
                                                               startUri, endUri);
            if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                var parameters =
                    webAuthenticationResult.ResponseData.Replace(_redirectUrl + "/?oauth_token=", "").Replace("oauth_verifier=", "").Split('&');
                var result = await RequestToken(parameters[0], parameters[1], _oAuthTokenSecret);
                var user = await RequestUser(result);

                return new UserInfo {Id = user.id, Name = user.name, UserName = user.screen_name};
            }
            return null;
        }

        public string Name
        {
            get { return "Twitter"; }
        }
    }

    [DataContract]
    class User
    {
        [DataMember]
        public string id { get;set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string screen_name { get; set; }
    }

    [DataContract]
    class Token
    {
        [DataMember]
        public string oauth_token { get; set; }
        [DataMember]
        public string oauth_token_secret { get; set; }
        [DataMember]
        public string user_id { get; set; }
        [DataMember]
        public string screen_name { get; set; }
    }
}
