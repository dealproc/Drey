using FakeItEasy;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Xunit;

namespace Drey.Server.Tests.Hubs
{
    public class RuntimeHubTests
    {
        IEventBus _eventBus;
        Server.Services.IGroupMembershipService _groupMembershipService;
        Server.Services.IClientHealthService _clientHealthService;
        IRequest _request;

        public RuntimeHubTests()
        {
            _eventBus = A.Fake<IEventBus>();
            _groupMembershipService = A.Fake<Server.Services.IGroupMembershipService>();
            _clientHealthService = A.Fake<Server.Services.IClientHealthService>();
            _request = A.Fake<IRequest>();

            A.CallTo(() => _request.User).Returns(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        [Fact]
        public async Task GroupMembershipService_Joined_Called_With_OnConnected_Async()
        {
            A.CallTo(() => _groupMembershipService.Join(A<IHub>.Ignored, A<ClaimsPrincipal>.Ignored, A<string>.Ignored, A<Task>.Ignored))
                .Returns(Task.FromResult<object>(null));
            var sut = new Server.Hubs.RuntimeHub(_eventBus, _clientHealthService, _groupMembershipService);
            sut.Context = new HubCallerContext(_request, Guid.NewGuid().ToString().ToLower());

            await sut.OnConnected();

            A.CallTo(() => _groupMembershipService.Join(A<IHub>.Ignored, A<ClaimsPrincipal>.Ignored, A<string>.Ignored, A<Task>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GroupMembershipService_Leave_Called_With_OnDisconnected_Async()
        {
            A.CallTo(() => _groupMembershipService.Join(A<IHub>.Ignored, A<ClaimsPrincipal>.Ignored, A<string>.Ignored, A<Task>.Ignored))
                .Returns(Task.FromResult<object>(null));
            var sut = new Server.Hubs.RuntimeHub(_eventBus, _clientHealthService, _groupMembershipService);
            sut.Context = new HubCallerContext(_request, Guid.NewGuid().ToString().ToLower());

            await sut.OnDisconnected(true);

            A.CallTo(() => _groupMembershipService.Leave(A<IHub>.Ignored, A<ClaimsPrincipal>.Ignored, A<string>.Ignored, A<Task>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void EndListLogFiles_Is_Published()
        {
            var sut = new Server.Hubs.RuntimeHub(_eventBus, _clientHealthService, _groupMembershipService);
            sut.Context = new HubCallerContext(_request, Guid.NewGuid().ToString().ToLower());
            var completed = DomainModel.Response<IEnumerable<string>>.Success("SOMETOKEN", Enumerable.Empty<string>());

            sut.EndListLogFiles(completed);

            A.CallTo(() => _eventBus.Publish(A<object>.That.IsSameAs(completed), A<string>.That.IsEqualTo(completed.Token))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void EndOpenLogFile_Is_Published()
        {
            var sut = new Server.Hubs.RuntimeHub(_eventBus, _clientHealthService, _groupMembershipService);
            sut.Context = new HubCallerContext(_request, Guid.NewGuid().ToString().ToLower());
            var completed = DomainModel.Response<byte[]>.Success("SOMETOKEN", new byte[10]);

            sut.EndOpenLogFile(completed);

            A.CallTo(() => _eventBus.Publish(A<object>.That.IsSameAs(completed), A<string>.That.IsEqualTo(completed.Token))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Client_Can_Report_Health_Async()
        {
            var sut = new Server.Hubs.RuntimeHub(_eventBus, _clientHealthService, _groupMembershipService);
            sut.Context = new HubCallerContext(_request, Guid.NewGuid().ToString().ToLower());
            var healthInfo = new DomainModel.EnvironmentInfo();

            sut.ReportHealth(healthInfo);

            A.CallTo(() => _clientHealthService.RecordHealthAsync(A<ClaimsPrincipal>.Ignored, A<DomainModel.EnvironmentInfo>.That.IsSameAs(healthInfo)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
