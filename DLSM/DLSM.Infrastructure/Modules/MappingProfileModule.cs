using System;
using System.Linq;
using Autofac;
using AutoMapper;

namespace DLSM.Infrastructure.Modules {
    public class MappingProfileModule : Autofac.Module {
        protected override void Load(ContainerBuilder builder) {

            var profiles =  from t in typeof(MappingProfileModule).Assembly.GetTypes()
                where typeof(Profile).IsAssignableFrom(t)
                select (Profile) Activator.CreateInstance(t);

            builder.Register(ctx => new MapperConfiguration(cfg => {
                foreach (var profile in profiles) {
                    cfg.AddProfile(profile);
                }
            }));

            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>();

        }
    }
}