// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ConsumerAuthorizationService.Manage.Tests.Acceptance.Models.Accesses;
using FluentAssertions;

namespace ConsumerAuthorizationService.Manage.Tests.Acceptance.Apis.Accesses
{
    public partial class AccessesApiTests
    {
        [Fact]
        public async Task ShouldPostAccessValidationAsync()
        {
            // given
            ValidateAccessRequest randomRequest = CreateRandomValidateAccessRequest();
            ValidateAccessRequest inputRequest = randomRequest;

            // when
            Access actualAccess =
                await this.apiBroker.PostAccessValidationAsync(inputRequest);

            // then
            actualAccess.Should().NotBeNull();
            actualAccess.CorrelationId.Should().Be(inputRequest.CorrelationId);
            actualAccess.ConsumerId.Should().Be(inputRequest.ConsumerUserId);
            actualAccess.NhsNumber.Should().Be(inputRequest.NhsNumber);
            actualAccess.IsAccessAllowed.Should().BeTrue();
        }
    }
}
