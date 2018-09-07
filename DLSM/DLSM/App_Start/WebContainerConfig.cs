using System;
using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using DLSM.Infrastructure.API.MdmServices;
using DLSM.Infrastructure.API.MdmServices.Interfaces;
using DLSM.Infrastructure.API.MdmServices.Models;
using DLSM.Infrastructure.Modules;
using DLSM.Models;

namespace DLSM {
    public abstract class WebContainerConfig {


        public static IContainer ConfigureService() {

            var builder = new ContainerBuilder();

            //register the configuration
            builder.Register<MdmServiceConfiguration>(c => {
                    var config = ConfigurationManager.AppSettings;

                    var ip = config["ip"];
                    var uid = config["uid"];
                    var token = config["mdmtoken"];
                    var upw = config["upw"];
                    var endpoints = config["endpoints"];

                    return new MdmServiceConfiguration(uid, upw, ip, token, endpoints);

                }
            );

            builder.RegisterControllers(typeof(WebContainerConfig).Assembly);
            //builder.RegisterSource(new ViewRegistrationSource());
            builder.RegisterType<DLSMEntities>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterModule<ApiModules>();
            builder.RegisterModule<MappingProfileModule>();

            builder.RegisterType<MdmServiceWrapper>().As<IMdmServiceWrapper>().InstancePerLifetimeScope();

            //build the container
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            return container;
        }
    }

    
}