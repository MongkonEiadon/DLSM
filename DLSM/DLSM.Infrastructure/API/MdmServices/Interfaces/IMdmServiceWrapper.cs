using System.Threading.Tasks;
using DLSM.Infrastructure.API.MdmServices.Models;

namespace DLSM.Infrastructure.API.MdmServices.Interfaces {
    public interface IMdmServiceWrapper {
        Task<AuthenticationResult> AuthenticationUserAsync(MdmAuthenticationInput input);
        Task<MdmUserInfo> GetUserInfoAsync(MdmAuthenticationInput input);
    }
}