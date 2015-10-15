using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Shouldly;
using Xunit;

namespace Drey.Server.Tests.Directors
{
    public class OpenLogFileDirectorTests
    {
        IEventBus _eventBus;
        DomainModel.IRuntimeClient _runtimeClient;
        IHubContext<DomainModel.IRuntimeClient> _runtimeClientContext;
        IHubConnectionContext<DomainModel.IRuntimeClient> _runtimeClientConnectionContext;

        Server.Directors.IOpenLogFileDirector _sut;

        public OpenLogFileDirectorTests()
        {
            _eventBus = A.Fake<IEventBus>();
            _runtimeClient = A.Fake<DomainModel.IRuntimeClient>();
            _runtimeClientConnectionContext = A.Fake<IHubConnectionContext<DomainModel.IRuntimeClient>>();
            _runtimeClientContext = A.Fake<IHubContext<DomainModel.IRuntimeClient>>();

            A.CallTo(() => _runtimeClientContext.Clients).Returns(_runtimeClientConnectionContext);
            A.CallTo(() => _runtimeClientConnectionContext.Group(A<string>.Ignored, A<string[]>.Ignored)).Returns(_runtimeClient);

            _sut = new Server.Directors.OpenLogFileDirector(_eventBus, _runtimeClientContext);
        }

        [Fact]
        public void Test()
        {
            var fileContent = new byte[10];
            var successfulMessage = DomainModel.Response<byte[]>.Success("SOMETOKEN", fileContent);

            A.CallTo(() => _runtimeClient.BeginOpenLogFile(A<DomainModel.Request<DomainModel.FileDownloadOptions>>.Ignored))
                .Invokes(() => _sut.Handle(DomainModel.Response<byte[]>.Success("SOMETOKEN", fileContent)));

            _sut.Initiate("one", new DomainModel.Request<DomainModel.FileDownloadOptions> { Token = "SOMETOKEN", Message = new DomainModel.FileDownloadOptions { RelativeOrAbsolutePath = "some/path/to/file.log" } });
            _sut.PendingTask.Wait();

            _sut.PendingTask.Result.Message.ShouldBeSameAs(fileContent);
            A.CallTo(() => _eventBus.Subscribe(A<object>.That.IsSameAs(_sut), A<string>.That.IsEqualTo("SOMETOKEN"))).MustHaveHappened();
        }
    }
}
