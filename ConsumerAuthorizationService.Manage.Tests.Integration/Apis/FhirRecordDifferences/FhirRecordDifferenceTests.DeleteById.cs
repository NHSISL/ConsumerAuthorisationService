// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ConsumerAuthorizationService.Manage.Tests.Integration.Models.FhirRecordDifferences;
using RESTFulSense.Exceptions;

namespace ConsumerAuthorizationService.Manage.Tests.Integration.Apis.FhirRecordDifferences
{
    public partial class FhirRecordDifferenceApiTests
    {
        [Fact]
        public async Task ShouldDeleteFhirRecordDifferenceAsync()
        {
            // given
            FhirRecordDifference randomFhirRecordDifference = await PostRandomFhirRecordDifferenceAsync();

            // when
            await this.apiBroker.DeleteFhirRecordDifferenceByIdAsync(randomFhirRecordDifference.Id);

            // then
            ValueTask<FhirRecordDifference> getFhirRecordDifferenceByIdTask =
                this.apiBroker.GetFhirRecordDifferenceByIdAsync(randomFhirRecordDifference.Id);

            await Assert.ThrowsAsync<HttpResponseNotFoundException>(getFhirRecordDifferenceByIdTask.AsTask);
        }
    }
}
