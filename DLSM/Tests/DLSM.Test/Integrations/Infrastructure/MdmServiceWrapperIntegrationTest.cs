using System;
using System.Collections.Generic;
using System.Text;
using DLSM.Infrastructure.API.MdmServices.Interfaces;
using DLSM.Infrastructure.API.MdmServices.Models;
using Xunit;

namespace DLSM.Test.Integrations.Infrastructure
{
    public class MdmServiceWrapperIntegrationTest : IClassFixture<IntegrationSetupBase>
    {
        private readonly IntegrationSetupBase _baseSetup;

        public MdmServiceWrapperIntegrationTest(IntegrationSetupBase baseSetup) {
            _baseSetup = baseSetup;
        }

        [Fact]
        public void Test() {


            var wrapper = _baseSetup.Resolve<IMdmServiceWrapper>();
            wrapper.GetUserInfoAsync(new MdmAuthenticationInput("anyuid", "anypassword"));
        }
    }
}
