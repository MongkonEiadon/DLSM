using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using DLSM.Infrastructure.API.MdmServices.Models;
using DLSM.Infrastructure.Modules;
using DLSM.Models;

namespace DLSM {
    public abstract class WebContainerConfig {


        public static void ConfigureService() {

            var builder = new ContainerBuilder();

            //register the configuration
            builder.Register<MdmServiceConfiguration>(c => {
                    var config = ConfigurationManager.AppSettings;

                    var ip = config["ip"];
                    var uid = config["uid"];
                    var token = config["mdmtoken"];
                    var upw = config["upw"];

                    return new MdmServiceConfiguration(uid, upw, ip, token);

                }
            );

            builder.RegisterControllers(typeof(WebContainerConfig).Assembly);
            builder.RegisterSource(new ViewRegistrationSource());
            builder.RegisterType<DLSMEntities>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterModule<ApiModules>();
            builder.RegisterModule<MappingProfileModule>();

            //build the container
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }

    
}