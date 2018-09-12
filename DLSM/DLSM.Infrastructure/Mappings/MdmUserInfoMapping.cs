using AutoMapper;
using DLSM.Infrastructure.MdmUserServices;
using DLSM.Infrastructure.Models;

namespace DLSM.Infrastructure.Mappings
{
    public class MdmUserInfoMapping : Profile
    {
        public MdmUserInfoMapping()
        {
            CreateMap<getUserInfoResultBean, MdmUserInfo>()
                .ForMember(x => x.OffLocCode, c => c.MapFrom(x => x.offLocCode))
                .ForMember(x => x.Name, c => c.MapFrom(x => x.name))
                .ForMember(x => x.OffLocDes, c => c.MapFrom(x => x.offLocDesc))
                .ForMember(x => x.OrgFullNameDes, c => c.MapFrom(x => x.orgFullNameDes))
                .ForMember(x => x.Surname, c => c.MapFrom(x => x.surname))
                .ForMember(x => x.Title, c => c.MapFrom(x => x.title));


            CreateMap<MdmGetUserInfoInput, getUserInfoBean>()
                .ForMember(x => x.authenUserToken, c => c.MapFrom(x => x.Token));
        }
    }
}