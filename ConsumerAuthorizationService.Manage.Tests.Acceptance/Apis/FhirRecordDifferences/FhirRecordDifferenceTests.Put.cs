// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ConsumerAuthorizationService.Manage.Tests.Acceptance.Models.FhirRecordDifferences;

namespace ConsumerAuthorizationService.Manage.Tests.Acceptance.Apis.FhirRecordDifferences
{
    public partial class FhirRecordDifferenceApiTests
    {
        [Fact]
        public async Task ShouldPutFhirRecordDifferenceAsync()
        {
            // given
            FhirRecordDifference randomFhirRecordDifference =
                await PostRandomFhirRecordDifferenceAsync();

            FhirRecordDifference modifiedFhirRecordDifference =
                UpdateFhirRecordDifferenceWithRandomValues(randomFhirRecordDifference);

            // when
            await this.apiBroker.PutFhirRecordDifferenceAsync(modifiedFhirRecordDifference);

            FhirRecordDifference actualFhirRecordDifference = await this.apiBroker
                .GetFhirRecordDifferenceByIdAsync(randomFhirRecordDifference.Id);

            // then
            actualFhirRecordDifference.Should().BeEquivalentTo(modifiedFhirRecordDifference, options => options
                .Excluding(property => property.CreatedBy)
                .Excluding(property => property.CreatedWhen)
                .Excluding(property => property.UpdatedBy)
                .Excluding(property => property.UpdatedWhen));

            await this.apiBroker.DeleteFhirRecordDifferenceByIdAsync(actualFhirRecordDifference.Id);
        }
    }
}