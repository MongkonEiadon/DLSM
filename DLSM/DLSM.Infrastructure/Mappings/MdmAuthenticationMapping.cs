using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using DLSM.Infrastructure.API.MdmServices.Models;
using DLSM.Infrastructure.MdmUserServices;
using DLSM.Infrastructure.Models;

namespace DLSM.Infrastructure.Mappings
{

    /// <summary>
    /// Create Mapping Profile from WebServiceReference to MdmModels
    /// </summary>
    public class MdmAuthenticationMapping : Profile
    {
        public MdmAuthenticationMapping()
        {
            CreateMap<authenUserResponse, AuthenticationResult>()
                .ForMember(x => x.LoginToken,
                    c => c.MapFrom(x => x.AuthenUserOutput.authenUserResponse.@return.authenUserToken))
                .ForMember(x => x.IsSuccess,
                    c => c.MapFrom(x => x.AuthenUserOutput.authenUserResponse.@return.authenUserResult));
            
            CreateMap<MdmAuthenticationInput, AuthenticationInput>()
                .ForMember(x => x.userId, c => c.MapFrom(x => x.Uid))
                .ForMember(x => x.password, c => c.MapFrom(x => x.Password));

            CreateMap<MdmServiceConfiguration, authenUserBean>()
                .ForMember(x => x.password, c => c.MapFrom(x => x.Password))
                .ForMember(x => x.ipAddress, c => c.MapFrom(x => x.Ip))
                .ForMember(x => x.userId, c => c.MapFrom(x => x.Uid));
            
        }
    }
}
