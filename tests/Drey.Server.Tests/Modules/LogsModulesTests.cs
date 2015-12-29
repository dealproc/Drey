using Drey.Server.Exceptions;
using FakeItEasy;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Drey.Server.Tests.Modules
{
    public class LogsModulesTests
    {
        public class ListLogsTests : IDisposable
        {
            bool _disposed = false;
            ApiTestFixture _testFixture;

            public ListLogsTests()
            {
                _testFixture = new ApiTestFixture();
            }
            ~ListLogsTests()
            {
                Dispose(false);
            }

            [Fact]
            public Task Should_Succeed_Async()
            {
                return _testFixture.WithOwinServer(async (client) =>
                {
                    var result = await client.PostAsync("/Runtime/Logs/ListFiles/1?token=" + Guid.NewGuid().ToString().ToLower(), new StringContent(string.Empty));

                    result.StatusCode.ShouldBe(HttpStatusCode.OK);
                    A.CallTo(() => _testFixture.ListLogsDirector.Initiate(A<string>.Ignored, A<DomainModel.Request<DomainModel.Empty>>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
                });
            }

            [Fact]
            public Task Should_Return_Bad_Request_When_Missing_Token_Async()
            {
                return _testFixture.WithOwinServer(async (client) =>
                {
                    var result = await client.PostAsync("/Runtime/Logs/ListFiles/1", new StringContent(string.Empty));
                    result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
                });
            }

            [Fact]
            public Task Should_Return_Bad_Request_When_Provided_Empty_Token_Async()
            {
                return _testFixture.WithOwinServer(async (client) =>
                {
                    var result = await client.PostAsync("/Runtime/Logs/ListFiles/1?token=", new StringContent(string.Empty));
                    result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
                });
            }

            [Fact]
            public Task Should_Return_Timeout_StatusCode_Async()
            {
                TaskCompletionSource<DomainModel.Response<IEnumerable<string>>> tsc = new TaskCompletionSource<DomainModel.Response<IEnumerable<string>>>();
                tsc.SetException(new TimeoutException());

                A.CallTo(() => _testFixture.ListLogsDirector.PendingTask)
                    .Returns(tsc.Task);

                return _testFixture.WithOwinServer(async (client) =>
                {
                    var result = await client.PostAsync("/Runtime/Logs/ListFiles/1?token=" + Guid.NewGuid().ToString().ToLower(), new StringContent(string.Empty));
                    result.StatusCode.ShouldBe(HttpStatusCode.RequestTimeout);
                });
            }

            [Fact]
            public Task Should_Return_ServiceUnavailable_StatusCode_Async()
            {
                TaskCompletionSource<DomainModel.Response<IEnumerable<string>>> tsc = new TaskCompletionSource<DomainModel.Response<IEnumerable<string>>>();
                tsc.SetException(new RuntimeHasNotConnectedException());

                A.CallTo(() => _testFixture.ListLogsDirector.PendingTask)
                    .Returns(tsc.Task);

                return _testFixture.WithOwinServer(async (client) =>
                {
                    var result = await client.PostAsync("/Runtime/Logs/ListFiles/1?token=" + Guid.NewGuid().ToString().ToLower(), new StringContent(string.Empty));
                    result.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
                });
            }

            public void Dispose()
            {
                Dispose(true);
                _disposed = true;
            }
            protected virtual void Dispose(bool disposing)
            {
                if (_testFixture != null)
                {
                    _testFixture.Dispose();
                    _testFixture = null;
                }

                if (!disposing || _disposed) { return; }
            }
        }

        public class OpenLogFileTests : IDisposable
        {
            ApiTestFixture _testFixture;
            bool _disposed = false;

            public OpenLogFileTests()
            {
                _testFixture = new ApiTestFixture();
            }
            ~OpenLogFileTests()
            {
                Dispose(false);
            }

            [Fact]
            public Task Should_Succeed_Async()
            {
                return _testFixture.WithOwinServer(async (client) =>
                {
                    var result = await client.PostAsync("/runtime/Logs/OpenLog/1?token=" + Guid.NewGuid().ToString().ToLower(), new StringContent(string.Empty));

                    result.StatusCode.ShouldBe(HttpStatusCode.OK);
                    A.CallTo(() => _testFixture.OpenLogFileDirector.Initiate(A<string>.Ignored, A<DomainModel.Request<DomainModel.FileDownloadOptions>>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
                });
            }

            [Fact]
            public Task Should_Return_Bad_Request_When_Missing_Token_Async()
            {
                return _testFixture.WithOwinServer(async (client) =>
                {
                    var result = await client.PostAsync("/runtime/Logs/OpenLog/1", new StringContent(string.Empty));
                    result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
                });
            }

            [Fact]
            public Task Should_Return_Bad_Request_When_Provided_Empty_Token_Async()
            {
                A.CallTo(() => _testFixture.OpenLogFileDirector.PendingTask)
                    .Returns(new DomainModel.Response<byte[]> { Message = new byte[10] });

                return _testFixture.WithOwinServer(async (client) =>
                {
                    var result = await client.PostAsync("/runtime/Logs/OpenLog/1?token=", new StringContent(string.Empty));
                    result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
                });
            }

            [Fact]
            public Task Should_Return_Timeout_StatusCode_Async()
            {
                TaskCompletionSource<DomainModel.Response<byte[]>> tsc = new TaskCompletionSource<DomainModel.Response<byte[]>>();
                tsc.SetException(new TimeoutException());

                A.CallTo(() => _testFixture.OpenLogFileDirector.PendingTask)
                    .Returns(tsc.Task);

                return _testFixture.WithOwinServer(async (client) =>
                {
                    var result = await client.PostAsync("/runtime/Logs/OpenLog/1?token=" + Guid.NewGuid().ToString().ToLower(), new StringContent(string.Empty));
                    result.StatusCode.ShouldBe(HttpStatusCode.RequestTimeout);
                });
            }

            [Fact]
            public Task Should_Return_ServiceUnavailable_StatusCode_Async()
            {
                TaskCompletionSource<DomainModel.Response<byte[]>> tsc = new TaskCompletionSource<DomainModel.Response<byte[]>>();
                tsc.SetException(new RuntimeHasNotConnectedException());

                A.CallTo(() => _testFixture.OpenLogFileDirector.PendingTask)
                    .Returns(tsc.Task);

                return _testFixture.WithOwinServer(async (client) =>
                {
                    var result = await client.PostAsync("/runtime/Logs/OpenLog/1?token=" + Guid.NewGuid().ToString().ToLower(), new StringContent(string.Empty));
                    result.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
                });
            }

            public void Dispose()
            {
                Dispose(true);
                _disposed = true;
            }
            protected virtual void Dispose(bool disposing)
            {
                if (_testFixture != null)
                {
                    _testFixture.Dispose();
                    _testFixture = null;
                }

                if (!disposing || _disposed) { return; }
            }
        }
    }
}
