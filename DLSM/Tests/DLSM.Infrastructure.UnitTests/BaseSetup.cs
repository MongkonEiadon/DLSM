using Autofac;
using DLSM.Infrastructure.API.MdmServices.Models;
using DLSM.Infrastructure.Modules;

namespace DLSM.Infrastructure.Tests
{
    public class BaseSetup
    {
        public IContainer Container { get; } 
        public BaseSetup()
        {
            var container = new Autofac.ContainerBuilder();
            container.RegisterModule<ApiModules>();
            container.RegisterType<MdmServiceConfiguration>().AsImplementedInterfaces();

            container.Register(c => new MdmServiceConfiguration(
                "TestEffective01",
                "Dlt@3618",
                "http://192.168.6.13",
                "748681ca3646ccc7c4facb7360a0e3baa0894cb5",
                "http://localhost:8015")).AsImplementedInterfaces();

            var build = container.Build();
            Container = build;
        }
        
    }
}