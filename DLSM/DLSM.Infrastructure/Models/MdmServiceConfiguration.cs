namespace DLSM.Infrastructure.API.MdmServices.Models {
    public class MdmServiceConfiguration {
        public string Uid { get; }
        public string Password { get; }
        public string Ip { get; }
        public string Thumprint { get; }
        public string Endpoints { get; }

        public MdmServiceConfiguration(string uid, string password, string ip, string thumprint, string endPoints) {
            Uid = uid;
            Password = password;
            Ip = ip;
            Thumprint = thumprint;
            Endpoints = endPoints;
        }
    }
}