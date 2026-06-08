// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ConsumerAuthorizationService.Manage.Tests.Acceptance.Models.FhirRecords;

namespace ConsumerAuthorizationService.Manage.Tests.Acceptance.Apis.FhirRecords
{
    public partial class FhirRecordApiTests
    {
        [Fact]
        public async Task ShouldPostFhirRecordAsync()
        {
            // given
            FhirRecord randomFhirRecord = CreateRandomFhirRecord();
            FhirRecord inputFhirRecord = randomFhirRecord;
            FhirRecord expectedFhirRecord = inputFhirRecord;

            // when 
            await this.apiBroker.PostFhirRecordAsync(inputFhirRecord);

            FhirRecord actualFhirRecord =
                await this.apiBroker.GetFhirRecordByIdAsync(inputFhirRecord.Id);

            // then
            actualFhirRecord.Should().BeEquivalentTo(expectedFhirRecord, options => options
                .Excluding(property => property.CreatedBy)
                .Excluding(property => property.CreatedWhen)
                .Excluding(property => property.UpdatedBy)
                .Excluding(property => property.UpdatedWhen));

            await this.apiBroker.DeleteFhirRecordByIdAsync(actualFhirRecord.Id);
        }
    }
}