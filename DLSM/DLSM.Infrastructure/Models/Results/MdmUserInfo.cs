namespace DLSM.Infrastructure.Models {


    public class MdmGetUserInfoInput
    {
        public string Token { get; set; }

        public MdmAuthenticationInput AuthenticationUser { get; set; }


    }

    public class MdmUserInfo {
        public string Title { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string OffLocCode { get; set; }
        public string OffLocDes { get; set; }
        public string OrgFullNameDes { get; set; }
    }
}