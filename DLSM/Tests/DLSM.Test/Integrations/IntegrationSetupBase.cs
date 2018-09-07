using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace DLSM.Test.Integrations
{
    public class IntegrationSetupBase 
    {
        protected IContainer Container { get; }

        public IntegrationSetupBase() {

            Container = WebContainerConfig.ConfigureService();
        }

        public T Resolve<T>() {
            return Container.Resolve<T>();
        }
    }
}
