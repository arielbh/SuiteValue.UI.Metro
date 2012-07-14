using System.Threading.Tasks;

namespace CodeValue.SuiteValue.UI.Metro.Authentications
{
    public interface IAuthProvider
    {
        void Configure(dynamic configuration);
        Task<UserInfo> Authenticate();
        string Name { get; }
    }
}