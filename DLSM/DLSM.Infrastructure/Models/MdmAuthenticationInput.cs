using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLSM.Infrastructure.API.MdmServices.Models
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
