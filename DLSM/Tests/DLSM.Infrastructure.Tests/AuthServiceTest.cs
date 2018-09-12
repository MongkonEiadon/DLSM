using System;
using Autofac;
using DLSM.Infrastructure.API.MdmServices.Models;
using DLSM.Infrastructure.API.MdmUserServices.Interfaces;
using Xunit;

namespace DLSM.Infrastructure.Tests
{
    public class AuthServiceTest : IClassFixture<BaseSetup>
    {
        private readonly BaseSetup _baseSetup;

        public AuthServiceTest(BaseSetup baseSetup)
        {
            _baseSetup = baseSetup;
        }

        [Fact]
        public void Test1()
        {
            var services = _baseSetup.Container.Resolve<IMdmServiceWrapper>();
            var result = services.AuthenticationUserAsync(new MdmAuthenticationInput("", ""));


           

        }
    }
}
