using System.Threading.Tasks;
using Autofac;
using DLSM.Infrastructure.API.MdmServices.Models;
using DLSM.Infrastructure.API.MdmUserServices.Interfaces;
using DLSM.Infrastructure.Models;
using DLSM.Infrastructure.Tests;
using Xunit;

namespace DLSM.Infrastructure.UnitTests
{
    public class AuthServiceTest : IClassFixture<BaseSetup>
    {
        private readonly BaseSetup _baseSetup;

        public AuthServiceTest(BaseSetup baseSetup)
        {
            _baseSetup = baseSetup;
        }

        [Fact]
        public async Task Test1()
        {
            var config = _baseSetup.Container.Resolve<IMdmServiceConfiguration>();
            config.Endpoints = "https://gservice.dlt.go.th:10443/EsvProcessWeb/sca/MdmUserService";

            var services = _baseSetup.Container.Resolve<IMdmServiceWrapper>();
            var result = await services.AuthenticationUserAsync(new MdmAuthenticationInput("3120300234990", "Iam@3101"));

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.LoginToken);
        }

        [Fact]
        public async Task Test2()
        {

            var services = _baseSetup.Container.Resolve<IMdmServiceWrapper>();
            var result = await services.GetUserInfoAsync(new MdmGetUserInfoInput());

            Assert.NotNull(result);
        }
    }
}
