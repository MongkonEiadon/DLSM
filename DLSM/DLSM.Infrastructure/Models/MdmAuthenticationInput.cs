namespace DLSM.Infrastructure.Models
{
    public class MdmAuthenticationInput
    {
        public string Uid { get; }
        public string Password { get; }
        

        public MdmAuthenticationInput(string uid, string password) {
            Uid = uid;
            Password = password;
        }
    }
}
