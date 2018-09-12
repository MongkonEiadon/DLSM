using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using DLSM.Infrastructure.Models;
using DLSM.Models;

namespace DLSM.Mapping
{
    public class GetUserProfileMapping : Profile
    {
        public GetUserProfileMapping()
        {
            CreateMap<MdmUserInfo, GetUserInfo>()
                .ReverseMap();
        }
    }
}