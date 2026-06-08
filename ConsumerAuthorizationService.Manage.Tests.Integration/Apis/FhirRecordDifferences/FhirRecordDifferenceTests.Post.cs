// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ConsumerAuthorizationService.Manage.Tests.Integration.Models.FhirRecordDifferences;

namespace ConsumerAuthorizationService.Manage.Tests.Integration.Apis.FhirRecordDifferences
{
    public partial class FhirRecordDifferenceApiTests
    {
        [Fact]
        public async Task ShouldPostFhirRecordDifferenceAsync()
        {
            // given
            FhirRecordDifference randomFhirRecordDifference = CreateRandomFhirRecordDifference();
            FhirRecordDifference expectedFhirRecordDifference = randomFhirRecordDifference;

            // when 
            await this.apiBroker.PostFhirRecordDifferenceAsync(randomFhirRecordDifference);

            FhirRecordDifference actualFhirRecordDifference =
                await this.apiBroker.GetFhirRecordDifferenceByIdAsync(randomFhirRecordDifference.Id);

            // then
            actualFhirRecordDifference.Should().BeEquivalentTo(
                expectedFhirRecordDifference,
                options => options
                    .Excluding(fhirRecordDifference => fhirRecordDifference.CreatedBy)
                    .Excluding(fhirRecordDifference => fhirRecordDifference.CreatedWhen)
                    .Excluding(fhirRecordDifference => fhirRecordDifference.UpdatedBy)
                    .Excluding(fhirRecordDifference => fhirRecordDifference.UpdatedWhen));

            await this.apiBroker.DeleteFhirRecordDifferenceByIdAsync(actualFhirRecordDifference.Id);
        }
    }
}
