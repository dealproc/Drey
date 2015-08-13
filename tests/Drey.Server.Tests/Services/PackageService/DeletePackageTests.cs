using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Drey.Server.Tests.Services.PackageService
{
    public class DeletePackageTests : PackageServiceTests
    {
        [Fact]
        public void ACallToTheStoreDeleteMethodShouldBeCalled()
        {
            A.CallTo(() => _packageStore.DeletePackage(A<string>.Ignored)).DoesNothing();

            _SUT.DeletePackage(_knownSHA);

            A.CallTo(() => _packageStore.DeletePackage(A<string>.Ignored)).MustHaveHappened();
        }
    }
}
