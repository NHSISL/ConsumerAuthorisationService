// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ConsumerAuthorizationService.Manage.Tests.Acceptance.Models.Accesses;

namespace ConsumerAuthorizationService.Manage.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string AccessRelativeUrl = "api/access";

        public async ValueTask<Access> PostAccessValidationAsync(ValidateAccessRequest request)
        {
            return await this.apiFactoryClient
                .PostContentAsync<ValidateAccessRequest, Access>(AccessRelativeUrl, request);
        }
    }
}
