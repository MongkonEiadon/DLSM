using System.Collections.Generic;
using System.Text;
using Autofac;

namespace DLSM.Infrastructure.Modules
{
    public class ApiModules : Autofac.Module {
        protected override void Load(ContainerBuilder builder) {

            builder.RegisterAssemblyTypes(ThisAssembly).AsImplementedInterfaces().InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
