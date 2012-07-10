using System.Collections.Generic;
using System.Composition;
using System.Threading.Tasks;
using CodeValue.SuiteValue.UI.Metro.Authentication;
using Microsoft.Live;

namespace CodeValue.SuiteValue.UI.Metro.LiveAuthentication
{
    [Export(typeof(IAuthProvider))]
    public class LiveAuthentication : IAuthProvider
    {
        public void Configure(dynamic configuration)
        {
            // Requires going to https://manage.dev.live.com/build?wa=wsignin1.0
            // and registering the client application.
        }

        public async Task<UserInfo> Authenticate()
        {
            var authClient = new LiveAuthClient();
            var authResult = await authClient.LoginAsync(new List<string>() { "wl.signin", "wl.basic", "wl.skydrive" });
            if (authResult.Status == LiveConnectSessionStatus.Connected)
            {
                var session = authResult.Session;
                var client = new LiveConnectClient(session);
                var liveOpResult = await client.GetAsync("me");
                dynamic dynResult = liveOpResult.Result;
                return new UserInfo {Id = dynResult.id, DisplayName  = dynResult.name, Name = dynResult.name};
            }
            return null;
        }

        public string Name { get { return "LiveId"; } }
    }
}
