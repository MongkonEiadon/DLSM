using Autofac;
using DLSM.Infrastructure.Modules;

namespace DLSM.Tests
{
    public class BaseIntegrationTest
    {
        public IContainer Container { get; }
        public BaseIntegrationTest()
        {
            var container = new ContainerBuilder();

            container.RegisterModule<ApiModules>();


            var build = container.Build();
            Container = build;

        }
    }
}