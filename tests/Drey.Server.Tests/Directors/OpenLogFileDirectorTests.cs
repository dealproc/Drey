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
    public class OpenLogFileDirectorTests
    {
        IEventBus _eventBus;
        DomainModel.IRuntimeClient _runtimeClient;
        IHubContext<DomainModel.IRuntimeClient> _runtimeClientContext;
        IHubConnectionContext<DomainModel.IRuntimeClient> _runtimeClientConnectionContext;

        public OpenLogFileDirectorTests()
        {
            _eventBus = A.Fake<IEventBus>();
            _runtimeClient = A.Fake<DomainModel.IRuntimeClient>();
            _runtimeClientConnectionContext = A.Fake<IHubConnectionContext<DomainModel.IRuntimeClient>>();
            _runtimeClientContext = A.Fake<IHubContext<DomainModel.IRuntimeClient>>();

            A.CallTo(() => _runtimeClientContext.Clients).Returns(_runtimeClientConnectionContext);
            A.CallTo(() => _runtimeClientConnectionContext.Group(A<string>.Ignored, A<string[]>.Ignored)).Returns(_runtimeClient);

        }

        [Fact]
        public void HappyPath()
        {
            var sut = new Server.Directors.OpenLogFileDirector(_eventBus, _runtimeClientContext);

            var fileContent = new byte[10];
            var successfulMessage = DomainModel.Response<byte[]>.Success("SOMETOKEN", fileContent);

            A.CallTo(() => _runtimeClient.BeginOpenLogFile(A<DomainModel.Request<DomainModel.FileDownloadOptions>>.Ignored))
                .Invokes(() => sut.Handle(DomainModel.Response<byte[]>.Success("SOMETOKEN", fileContent)));

            sut.Initiate("one", new DomainModel.Request<DomainModel.FileDownloadOptions> { Token = "SOMETOKEN", Message = new DomainModel.FileDownloadOptions { RelativeOrAbsolutePath = "some/path/to/file.log" } });
            sut.PendingTask.Wait();

            sut.PendingTask.Result.Message.ShouldBeSameAs(fileContent);
            A.CallTo(() => _eventBus.Subscribe(A<object>.That.IsSameAs(sut), A<string>.That.IsEqualTo("SOMETOKEN"))).MustHaveHappened();
        }

        [Fact]
        public void Throws_A_TimeoutException_When_Client_Does_Not_Respond()
        {
            var sut = new Server.Directors.OpenLogFileDirector(_eventBus, _runtimeClientContext, 1);

            Should.Throw<TimeoutException>(() =>
            {
                try
                {
                    sut.Initiate("one", new DomainModel.Request<DomainModel.FileDownloadOptions> { Token = "SOMETOKEN", Message = new DomainModel.FileDownloadOptions() });
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
            var sut = new Server.Directors.OpenLogFileDirector(_eventBus, _runtimeClientContext);
            sut.Initiate("one", new DomainModel.Request<DomainModel.FileDownloadOptions> { Token = "SOMETOKEN", Message = new DomainModel.FileDownloadOptions() });
            sut.Dispose();

            A.CallTo(() => _eventBus.Unsubscribe(A<object>.That.IsSameAs(sut))).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
