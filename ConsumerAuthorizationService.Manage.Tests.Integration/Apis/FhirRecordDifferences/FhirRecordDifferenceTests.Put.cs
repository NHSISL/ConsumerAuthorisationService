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
        public async Task ShouldPutFhirRecordDifferenceAsync()
        {
            // given
            FhirRecordDifference randomFhirRecordDifference = await PostRandomFhirRecordDifferenceAsync();
            FhirRecordDifference modifiedFhirRecordDifference = UpdateFhirRecordDifferenceWithRandomValues(randomFhirRecordDifference);

            // when
            await this.apiBroker.PutFhirRecordDifferenceAsync(modifiedFhirRecordDifference);
            FhirRecordDifference actualFhirRecordDifference = await this.apiBroker.GetFhirRecordDifferenceByIdAsync(randomFhirRecordDifference.Id);

            // then
            actualFhirRecordDifference.Should().BeEquivalentTo(
                modifiedFhirRecordDifference,
                options => options
                    .Excluding(fhirRecordDifference => fhirRecordDifference.CreatedBy)
                    .Excluding(fhirRecordDifference => fhirRecordDifference.CreatedWhen)
                    .Excluding(fhirRecordDifference => fhirRecordDifference.UpdatedBy)
                    .Excluding(fhirRecordDifference => fhirRecordDifference.UpdatedWhen));

            await this.apiBroker.DeleteFhirRecordDifferenceByIdAsync(actualFhirRecordDifference.Id);
        }
    }
}
