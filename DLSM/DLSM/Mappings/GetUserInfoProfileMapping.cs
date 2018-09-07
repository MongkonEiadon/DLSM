using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using DLSM.Infrastructure.API.MdmServices.Models;
using DLSM.MdmServiceTest;
using DLSM.Models;

namespace DLSM.Mappings
{
    public class GetUserInfoProfileMapping : Profile
    {
        public GetUserInfoProfileMapping() {
            CreateMap<GetUserInfo, Infrastructure.API.MdmServices.Models.MdmUserInfo>();
        }
    }

    public class MdmUserInfoProfileMapping : Profile {
        public MdmUserInfoProfileMapping() {
            CreateMap<getUserInfoResultBean, MdmUserInfo>();
        }
    }
}