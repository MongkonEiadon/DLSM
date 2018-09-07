using System;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using System.Web.Mvc.Html;
using AutoMapper;
using DLSM.Infrastructure.API.MdmServices.Interfaces;
using DLSM.Infrastructure.API.MdmServices.Models;
using DLSM.Infrastructure.CertificateServices;
using DLSM.MdmServiceTest;

namespace DLSM.Infrastructure.API.MdmServices {

    /// <summary>
    /// this class should move to DLSM.Infrastructure, for modularity purpose
    /// </summary>
    public class MdmServiceWrapper : IMdmServiceWrapper
    {
        private readonly IMapper _mapper;
        private readonly IClientCertificateService _clientCertificateService;
        private readonly MdmServiceConfiguration _mdmConfiguration;
        
        private MdmServiceTest.MdmUserServiceClient Client { get; }

        public MdmServiceWrapper(
            IMapper mapper,
            IClientCertificateService clientCertificateService, 
            MdmServiceConfiguration mdmConfiguration)
        {
            _mapper = mapper;
            _clientCertificateService = clientCertificateService;
            _mdmConfiguration = mdmConfiguration;

            Client = new MdmUserServiceClient();

            var cert = _clientCertificateService.GetCertificateByThumbprint(mdmConfiguration.Thumprint);

            if (cert != null) {
                if (Client.ClientCredentials != null)
                    Client.ClientCredentials.ClientCertificate.Certificate = cert;
            }
        }

        public async Task<AuthenticationResult> AuthenticationUserAsync(MdmAuthenticationInput input)
        {
            var auth = new authenUser() {
                AuthenUserInput = new AuthenUserInput() {
                    authenticationInput = new AuthenticationInput() {
                        userId = input.Uid,
                        password = input.Password
                    },

                    authenUserBeanInput = new authenUserBean() {
                        userId = _mdmConfiguration.Uid,
                        password = _mdmConfiguration.Password,
                        ipAddress = _mdmConfiguration.Ip
                    }
                },
                
            };

            var result = Client.authenUser(auth);
            
            return new AuthenticationResult(
                result.AuthenUserOutput.authenUserResponse.@return.authenUserResult,
                result.AuthenUserOutput.authenUserResponse.@return.authenUserResultSpecified.ToString(),
                result.AuthenUserOutput.authenUserResponse.@return.authenUserToken,
                input);
        }

        public async Task<MdmUserInfo> GetUserInfoAsync(MdmAuthenticationInput input) {

            //do authen first to get token
            var result = await AuthenticationUserAsync(input);

            var userInfo = getUserInfoHandle(result);

            return  userInfo != null ? _mapper.Map<MdmUserInfo>(userInfo) : null;
        }

        private getUserInfoResultBean getUserInfoHandle(AuthenticationResult authResult) {

            if (!authResult.IsSuccess) return null;

            var getUserInfo = new getUserInfo() {
                GetUserInfoInput = new GetUserInfoInput() {
                    getUserInfoBeanInput = new getUserInfoBean() {
                        authenUserToken = authResult.LoginToken
                    },
                    authenticationInput = new AuthenticationInput() {
                        userId = authResult.UserInput.Uid,
                        password = authResult.UserInput.Password
                    }
                }
            };

            var result = Client.getUserInfo(getUserInfo);

            return result.GetUserInfoOutput.getUserInfoResponse.@return;
        }
    }
}
