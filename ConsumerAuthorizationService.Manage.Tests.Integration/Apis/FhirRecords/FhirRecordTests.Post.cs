// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ConsumerAuthorizationService.Manage.Tests.Integration.Models.FhirRecords;

namespace ConsumerAuthorizationService.Manage.Tests.Integration.Apis.FhirRecords
{
    public partial class FhirRecordApiTests
    {
        [Fact]
        public async Task ShouldPostFhirRecordAsync()
        {
            // given
            FhirRecord randomFhirRecord = CreateRandomFhirRecord();
            FhirRecord expectedFhirRecord = randomFhirRecord;

            // when 
            await this.apiBroker.PostFhirRecordAsync(randomFhirRecord);

            FhirRecord actualFhirRecord =
                await this.apiBroker.GetFhirRecordByIdAsync(randomFhirRecord.Id);

            // then
            actualFhirRecord.Should().BeEquivalentTo(
                expectedFhirRecord,
                options => options
                    .Excluding(fhirRecord => fhirRecord.CreatedBy)
                    .Excluding(fhirRecord => fhirRecord.CreatedWhen)
                    .Excluding(fhirRecord => fhirRecord.UpdatedBy)
                    .Excluding(fhirRecord => fhirRecord.UpdatedWhen));

            await this.apiBroker.DeleteFhirRecordByIdAsync(actualFhirRecord.Id);
        }
    }
}
