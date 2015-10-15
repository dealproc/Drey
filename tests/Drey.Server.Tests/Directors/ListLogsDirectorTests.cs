using FakeItEasy;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Microsoft.AspNet.SignalR.Hubs;

namespace Drey.Server.Tests.Directors
{
    public class ListLogsDirectorTests
    {
        IEventBus _eventBus;
        DomainModel.IRuntimeClient _runtimeClient;
        IHubContext<DomainModel.IRuntimeClient> _runtimeClientContext;
        IHubConnectionContext<DomainModel.IRuntimeClient> _runtimeClientConnectionContext;

        Server.Directors.IListLogsDirector _sut;

        public ListLogsDirectorTests()
        {
            _eventBus = A.Fake<IEventBus>();
            _runtimeClient = A.Fake<DomainModel.IRuntimeClient>();
            _runtimeClientConnectionContext = A.Fake<IHubConnectionContext<DomainModel.IRuntimeClient>>();
            _runtimeClientContext = A.Fake<IHubContext<DomainModel.IRuntimeClient>>();

            A.CallTo(() => _runtimeClientContext.Clients).Returns(_runtimeClientConnectionContext);
            A.CallTo(() => _runtimeClientConnectionContext.Group(A<string>.Ignored, A<string[]>.Ignored)).Returns(_runtimeClient);

            _sut = new Server.Directors.ListLogsDirector(_eventBus, _runtimeClientContext);
        }

        [Fact]
        public void Test()
        {
            var handleValues = new string[] { "one", "two" };
            A.CallTo(() => _runtimeClient.BeginListLogFiles(A<DomainModel.Request<DomainModel.Empty>>.Ignored))
                .Invokes(() => _sut.Handle(DomainModel.Response<IEnumerable<string>>.Success("SOMETOKEN", handleValues.AsEnumerable())));

            _sut.Initiate("one", new DomainModel.Request<DomainModel.Empty> { Token = "SOMETOKEN", Message = new DomainModel.Empty() });
            _sut.PendingTask.Wait();

            _sut.PendingTask.Result.Message.ShouldBeSameAs(handleValues);
            A.CallTo(() => _eventBus.Subscribe(A<object>.That.IsSameAs(_sut), A<string>.That.IsEqualTo("SOMETOKEN"))).MustHaveHappened();
        }
    }
}
