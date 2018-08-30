using System.Threading.Tasks;
using DLSM.Infrastructure.API.MdmServices.Interfaces;
using DLSM.Infrastructure.API.MdmServices.Models;
using DLSM.Infrastructure.CertificateServices;

namespace DLSM.Infrastructure.MdmServices {
    public class MdmServiceWrapper : IMdmServiceWrapper{
        private readonly IClientCertificateService _clientCertificateService;
        private readonly MdmServiceConfiguration _mdmConfiguration;

        public MdmServiceWrapper(IClientCertificateService clientCertificateService, MdmServiceConfiguration mdmConfiguration) {
            _clientCertificateService = clientCertificateService;
            _mdmConfiguration = mdmConfiguration;
        }

        public Task<AuthenticationResult> AuthenticationUserAsync(MdmAuthenticationInput input) {


            var cert = _clientCertificateService.GetCertificateByThumbprint(_mdmConfiguration.Thumprint);
            //setup fix admin user

            //setup varia from input

            //login to get token


            return Task.FromResult(default(AuthenticationResult));
        }

        public Task<MdmUserInfo> GetUserInfoAsync(MdmAuthenticationInput input){

            //do authen first to get token

            // call get services

            return Task.FromResult(default(MdmUserInfo));
        }
    }
}
