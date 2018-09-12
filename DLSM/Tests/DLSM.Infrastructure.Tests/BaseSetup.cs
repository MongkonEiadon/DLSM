using Autofac;
using DLSM.Infrastructure.API.MdmServices.Models;
using DLSM.Infrastructure.Modules;
using Microsoft.Extensions.Configuration;

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

            //var config = new ConfigurationBuilder()
            //    .AddXmlFile("Web.config")
            //    .Build();
            //var appSettings = config.GetSection("appSettings");

            //container.Register(c => new MdmServiceConfiguration(
            //    appSettings["uid"],
            //    appSettings["password"],
            //    appSettings["ip"],
            //    appSettings["thumbprint"],
            //    appSettings["key"])).AsImplementedInterfaces();

            var build = container.Build();
            Container = build;
        }
        
    }
}