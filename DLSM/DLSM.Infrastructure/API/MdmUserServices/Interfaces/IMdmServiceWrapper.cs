using System.Threading.Tasks;
using DLSM.Infrastructure.API.MdmServices.Models;
using DLSM.Infrastructure.Models;

namespace DLSM.Infrastructure.API.MdmUserServices.Interfaces {
    public interface IMdmServiceWrapper {

        /// <summary>
        /// This service to call to get token
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<AuthenticationResult> AuthenticationUserAsync(MdmAuthenticationInput input);


        /// <summary>
        /// Get userInfo with specific input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<MdmUserInfo> GetUserInfoAsync(MdmGetUserInfoInput input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<MdmUserInfo> GetUserInfoAsync(MdmAuthenticationInput input);
    }
}