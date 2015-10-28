using Drey.Server.Extensions;

using FakeItEasy;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using Shouldly;

using System;
using System.Threading.Tasks;

using Xunit;

namespace Drey.Server.Tests.Directors
{
    public class RecycleClientDirectorTests
    {
        IEventBus _eventBus;
        Server.Hubs.IRuntimeClient _runtimeClient;
        IHubContext<Server.Hubs.IRuntimeClient> _runtimeClientContext;
        IHubConnectionContext<Server.Hubs.IRuntimeClient> _runtimeClientConnectionContext;

        public RecycleClientDirectorTests()
        {
            _eventBus = A.Fake<IEventBus>();
            _runtimeClient = A.Fake<Server.Hubs.IRuntimeClient>();
            _runtimeClientConnectionContext = A.Fake<IHubConnectionContext<Server.Hubs.IRuntimeClient>>();
            _runtimeClientContext = A.Fake<IHubContext<Server.Hubs.IRuntimeClient>>();

            A.CallTo(() => _runtimeClientContext.Clients).Returns(_runtimeClientConnectionContext);
            A.CallTo(() => _runtimeClientConnectionContext.Group(A<string>.Ignored, A<string[]>.Ignored)).Returns(_runtimeClient);
        }

        [Fact]
        public void HappyPath()
        {
            var sut = new Server.Directors.RecycleClientDirector(_eventBus, _runtimeClientContext);
            A.CallTo(() => _runtimeClient.BeginRecycleClient(A<DomainModel.Request<DomainModel.Empty>>.Ignored))
                .Invokes(() => sut.Handle(DomainModel.Response<DomainModel.Empty>.Success("SOMETOKEN", new DomainModel.Empty())));


            sut.Initiate("one", new DomainModel.Request<DomainModel.Empty> { Token = "SOMETOKEN", Message = new DomainModel.Empty() });
            sut.PendingTask.Wait();

            sut.PendingTask.Result.Successful.ShouldBe(true);
            A.CallTo(() => _eventBus.Subscribe(A<object>.That.IsSameAs(sut), A<string>.That.IsEqualTo("SOMETOKEN"))).MustHaveHappened();
        }

        [Fact]
        public void Throws_A_TimeoutException_When_Client_Does_Not_Respond()
        {
            var sut = new Server.Directors.RecycleClientDirector(_eventBus, _runtimeClientContext, 1);

            Should.Throw<TimeoutException>(() =>
            {
                try
                {
                    sut.Initiate("one", new DomainModel.Request<DomainModel.Empty> { Token = "SOMETOKEN", Message = new DomainModel.Empty() });
                    sut.PendingTask.Wait();
                }
                catch (AggregateException ex)
                {
                    throw ex.HeadException();
                }
            });

            sut.PendingTask.Status.ShouldBe(TaskStatus.Faulted);
        }

        [Fact]
        public void Unsubscribes_on_Dispose()
        {
            var sut = new Server.Directors.RecycleClientDirector(_eventBus, _runtimeClientContext);
            sut.Initiate("one", new DomainModel.Request<DomainModel.Empty> { Token = "SOMETOKEN", Message = new DomainModel.Empty() });
            sut.Dispose();

            A.CallTo(() => _eventBus.Unsubscribe(A<object>.That.IsSameAs(sut))).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
