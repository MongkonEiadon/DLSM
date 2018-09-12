using System;
using System.Threading.Tasks;
using System.Web.Services.Protocols;
using AutoMapper;
using DLSM.Infrastructure.API.MdmServices.Models;
using DLSM.Infrastructure.API.MdmUserServices.Interfaces;
using DLSM.Infrastructure.CertificateServices;
using DLSM.Infrastructure.MdmUserServices;
using DLSM.Infrastructure.Models;

namespace DLSM.Infrastructure.API.MdmUserServices {

    public sealed class MdmUserServiceWrapper : IMdmServiceWrapper
    {
        private readonly IMapper _mapper;
        private readonly IMdmServiceConfiguration _configuration;
        private readonly IClientCertificateService _clientCertificateService;
        private readonly IMdmServiceConfiguration _mdmServiceConfiguration;
        private MdmUserServiceExport1_MdmUserServiceHttpService Client { get; }

        public MdmUserServiceWrapper(
            IMapper mapper, 
            IMdmServiceConfiguration configuration,
            IClientCertificateService clientCertificateService)
        {
            _mapper = mapper;
            _configuration = configuration;
            _clientCertificateService = clientCertificateService;


            Client = new MdmUserServiceExport1_MdmUserServiceHttpService();
            Client.Url = _configuration.Endpoints;

            var cert = _clientCertificateService.GetCertificateByThumbprint(_configuration.Thumprint);
            if (cert != null)
            {
                Client.ClientCertificates.Add(cert);
            }

        }

        public Task<AuthenticationResult> AuthenticationUserAsync(MdmAuthenticationInput input)
        {
            var tcs = new TaskCompletionSource<AuthenticationResult>();
            try { 

                var auth = CreateAuthenticateUserInput(input);
                var response = Client.authenUser(auth);
                var result = _mapper.Map<AuthenticationResult>(response);
                result.IsSuccess = true;

                tcs.TrySetResult(result);
            }
            catch (SoapException soapex)
            {
                tcs.TrySetResult(new AuthenticationResult()
                {
                    IsSuccess = false,
                    LoginToken = null,
                    Message = soapex.Message
                });
            }

            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }

        public Task<MdmUserInfo> GetUserInfoAsync(MdmGetUserInfoInput input)
        {
            var tcs = new TaskCompletionSource<MdmUserInfo>();

            try
            {
                var request = new getUserInfo()
                {
                    GetUserInfoInput = new GetUserInfoInput()
                    {
                        authenticationInput = _mapper.Map<AuthenticationInput>(input.AuthenticationUser),
                        getUserInfoBeanInput = _mapper.Map<getUserInfoBean>(input)
                    }
                };

                var response = Client.getUserInfo(request);

                if (response.GetUserInfoOutput?.getUserInfoResponse?.@return != null)
                    tcs.TrySetResult(_mapper.Map<MdmUserInfo>(response.GetUserInfoOutput.getUserInfoResponse.@return));

                tcs.TrySetResult(null);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }


            return tcs.Task;
        }

        public Task<MdmUserInfo> GetUserInfoAsync(MdmAuthenticationInput input)
        {
            return AuthenticationUserAsync(input).ContinueWith(t =>
            {
                if (t.Result != null)
                {
                    return GetUserInfoAsync(new MdmGetUserInfoInput()
                    {
                        Token = t.Result.LoginToken,
                        AuthenticationUser = input
                    });
                }

                return null;
            }).Unwrap();
        }

        private authenUser CreateAuthenticateUserInput(MdmAuthenticationInput input)
        {
            var auth = new authenUser();
            auth.AuthenUserInput = new AuthenUserInput()
            {
                authenUserBeanInput = _mapper.Map<authenUserBean>(_mdmServiceConfiguration),
                authenticationInput = _mapper.Map<AuthenticationInput>(input)
            };

            return auth;
        }
        
    }
}
