using Drey.Server.Exceptions;

using FakeItEasy;

using Shouldly;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Xunit;

namespace Drey.Server.Tests.Modules
{
    public class LogsModulesTests
    {
        public class ListLogsTests
        {
            [Fact]
            public Task Should_Succeed_Async()
            {
                using (var fixture = new ApiTestFixture())
                    return fixture.WithOwinServer(async (client) =>
                    {
                        var result = await client.PostAsync("/Runtime/Logs/ListFiles/1?token=" + Guid.NewGuid().ToString().ToLower(), new StringContent(string.Empty));

                        result.StatusCode.ShouldBe(HttpStatusCode.OK);
                        A.CallTo(() => fixture.ListLogsDirector.Initiate(A<string>.Ignored, A<DomainModel.Request<DomainModel.Empty>>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
                    });
            }

            [Fact]
            public Task Should_Return_Bad_Request_When_Missing_Token_Async()
            {
                using (var fixture = new ApiTestFixture())
                    return fixture.WithOwinServer(async (client) =>
                    {
                        var result = await client.PostAsync("/Runtime/Logs/ListFiles/1", new StringContent(string.Empty));
                        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
                    });
            }

            [Fact]
            public Task Should_Return_Bad_Request_When_Provided_Empty_Token_Async()
            {
                using (var fixture = new ApiTestFixture())
                    return fixture.WithOwinServer(async (client) =>
                    {
                        var result = await client.PostAsync("/Runtime/Logs/ListFiles/1?token=", new StringContent(string.Empty));
                        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
                    });
            }

            [Fact]
            public async Task Should_Return_Timeout_StatusCode_Async()
            {
                using (var fixture = new ApiTestFixture())
                {
                    TaskCompletionSource<DomainModel.Response<IEnumerable<string>>> tsc = new TaskCompletionSource<DomainModel.Response<IEnumerable<string>>>();
                    tsc.SetException(new TimeoutException());

                    A.CallTo(() => fixture.ListLogsDirector.PendingTask)
                        .Returns(tsc.Task);

                    await fixture.WithOwinServer(async (client) =>
                    {
                        var result = await client.PostAsync("/Runtime/Logs/ListFiles/1?token=" + Guid.NewGuid().ToString().ToLower(), new StringContent(string.Empty));
                        result.StatusCode.ShouldBe(HttpStatusCode.RequestTimeout);
                    });
                }
            }

            [Fact]
            public Task Should_Return_ServiceUnavailable_StatusCode_Async()
            {
                using (var fixture = new ApiTestFixture())
                {
                    TaskCompletionSource<DomainModel.Response<IEnumerable<string>>> tsc = new TaskCompletionSource<DomainModel.Response<IEnumerable<string>>>();
                    tsc.SetException(new RuntimeHasNotConnectedException());

                    A.CallTo(() => fixture.ListLogsDirector.PendingTask)
                        .Returns(tsc.Task);

                    return fixture.WithOwinServer(async (client) =>
                    {
                        var result = await client.PostAsync("/Runtime/Logs/ListFiles/1?token=" + Guid.NewGuid().ToString().ToLower(), new StringContent(string.Empty));
                        result.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
                    });
                }
            }
        }

        public class OpenLogFileTests
        {
            [Fact]
            public Task Should_Succeed_Async()
            {
                using (var fixture = new ApiTestFixture())
                    return fixture.WithOwinServer(async (client) =>
                    {
                        var result = await client.PostAsync("/runtime/Logs/OpenLog/1?token=" + Guid.NewGuid().ToString().ToLower(), new StringContent(string.Empty));

                        result.StatusCode.ShouldBe(HttpStatusCode.OK);
                        A.CallTo(() => fixture.OpenLogFileDirector.Initiate(A<string>.Ignored, A<DomainModel.Request<DomainModel.FileDownloadOptions>>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
                    });
            }

            [Fact]
            public Task Should_Return_Bad_Request_When_Missing_Token_Async()
            {
                using (var fixture = new ApiTestFixture())
                    return fixture.WithOwinServer(async (client) =>
                    {
                        var result = await client.PostAsync("/runtime/Logs/OpenLog/1", new StringContent(string.Empty));
                        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
                    });
            }

            [Fact]
            public Task Should_Return_Bad_Request_When_Provided_Empty_Token_Async()
            {
                using (var fixture = new ApiTestFixture())
                {
                    return fixture.WithOwinServer(async (client) =>
                    {
                        var result = await client.PostAsync("/runtime/Logs/OpenLog/1?token=", new StringContent(string.Empty));
                        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
                    });
                }
            }

            [Fact]
            public Task Should_Return_Timeout_StatusCode_Async()
            {
                using (var fixture = new ApiTestFixture())
                {
                    TaskCompletionSource<DomainModel.Response<byte[]>> tsc = new TaskCompletionSource<DomainModel.Response<byte[]>>();
                    tsc.SetException(new TimeoutException());

                    A.CallTo(() => fixture.OpenLogFileDirector.PendingTask)
                        .Returns(tsc.Task);

                    return fixture.WithOwinServer(async (client) =>
                    {
                        var result = await client.PostAsync("/runtime/Logs/OpenLog/1?token=" + Guid.NewGuid().ToString().ToLower(), new StringContent(string.Empty));
                        result.StatusCode.ShouldBe(HttpStatusCode.RequestTimeout);
                    });
                }
            }

            [Fact]
            public Task Should_Return_ServiceUnavailable_StatusCode_Async()
            {
                using (var fixture = new ApiTestFixture())
                {
                    TaskCompletionSource<DomainModel.Response<byte[]>> tsc = new TaskCompletionSource<DomainModel.Response<byte[]>>();
                    tsc.SetException(new RuntimeHasNotConnectedException());

                    A.CallTo(() => fixture.OpenLogFileDirector.PendingTask)
                        .Returns(tsc.Task);

                    return fixture.WithOwinServer(async (client) =>
                    {
                        var result = await client.PostAsync("/runtime/Logs/OpenLog/1?token=" + Guid.NewGuid().ToString().ToLower(), new StringContent(string.Empty));
                        result.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
                    });
                }
            }
        }
    }
}
