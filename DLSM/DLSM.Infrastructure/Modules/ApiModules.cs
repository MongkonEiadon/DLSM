using System.Collections.Generic;
using System.Text;
using Autofac;
using DLSM.Infrastructure.API.MdmServices.Models;
using DLSM.Infrastructure.API.MdmUserServices;
using DLSM.Infrastructure.CertificateServices;

namespace DLSM.Infrastructure.Modules
{
    public class ApiModules : Autofac.Module {
        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterType<MdmUserServiceWrapper>().AsImplementedInterfaces();
            builder.RegisterType<ClientCertificateService>().AsImplementedInterfaces();

            builder.RegisterModule<MappingProfileModule>();

            base.Load(builder);
        }
    }
}
