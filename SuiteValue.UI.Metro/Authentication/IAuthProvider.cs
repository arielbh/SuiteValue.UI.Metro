using System.Threading.Tasks;

namespace CodeValue.SuiteValue.UI.Metro.Authentication
{
    public interface IAuthProvider
    {
        void Configure(dynamic configuration);
        Task<UserInfo> Authenticate();
        string Name { get; }
    }
}