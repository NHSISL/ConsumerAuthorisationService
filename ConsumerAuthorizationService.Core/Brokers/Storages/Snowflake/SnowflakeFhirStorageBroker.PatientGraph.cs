// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ConsumerAuthorizationService.Core.Models.Foundations.Accesses;

namespace ConsumerAuthorizationService.Core.Brokers.Storages.Snowflake
{
    public partial class SnowflakeFhirStorageBroker
    {
        public async ValueTask<List<Access>> ValidateConsumerAccessToPatientAsync(
            string nhsNumber,
            string consumerUserId,
            List<string> subscriberAgreementIds,
            Guid correlationId,
            CancellationToken cancellationToken)
        {
            //const string sql =
            //    """
            //    SELECT *
            //    FROM FHIR_DEV.FHIRL_OLIDS.FHIR_DATASET
            //    WHERE NHS_NUMBER = :nhsNumber
            //    """;

            //return SelectByNhsNumber(
            //    nhsNumber: nhsNumber,
            //    sql: sql,
            //    mapRow: MapPatientGraph);

            List<Access> accesses = new()             {
                new Access
                {
                    NhsNumber = nhsNumber,
                    ConsumerId = consumerUserId,
                }
            };

            return accesses;
        }

        //private PatientGraph MapPatientGraph(IDataReader reader)
        //{
        //    return new PatientGraph
        //    {
        //        Id =
        //            ReadString(reader, "PATIENT_ID") ?? string.Empty,

        //        NhsNumber =
        //            ReadString(reader, "NHS_NUMBER") ?? string.Empty,

        //        Year =
        //            ReadInt32(reader, "YEAR"),

        //        FamilyMemberHistoryRecordCount =
        //            ReadInt64(reader, "FAMILY_HISTORY_RECORD_COUNT"),

        //        FamilyMemberHistoryVariant =
        //            ReadVariantOrArrayAsString(reader, "FAMILY_HISTORY_VARIANT"),

        //        ImmunizationRecordCount =
        //            ReadInt64(reader, "IMMUNISATIONS_RECORD_COUNT"),

        //        ImmunizationVariant =
        //            ReadVariantOrArrayAsString(reader, "IMMUNISATIONS_VARIANT"),

        //        ObservationRecordCount =
        //            ReadInt64(reader, "VITAL_SIGNS_RECORD_COUNT"),

        //        ObservationVariant =
        //            ReadVariantOrArrayAsString(reader, "VITAL_SIGNS_VARIANT"),

        //        EncounterRecordCount =
        //            ReadInt64(reader, "ENCOUNTER_VISITS_RECORD_COUNT"),

        //        EncounterVariant =
        //            ReadVariantOrArrayAsString(reader, "ENCOUNTER_VISITS_VARIANT"),

        //        AllergyIntoleranceRecordCount =
        //            ReadInt64(reader, "ALLERGIES_RECORD_COUNT"),

        //        AllergyIntoleranceVariant =
        //            ReadVariantOrArrayAsString(reader, "ALLERGIES_VARIANT"),

        //        ReferralRequestRecordCount =
        //            ReadInt64(reader, "REFERRALS_RECORD_COUNT"),

        //        ReferralRequestVariant =
        //            ReadVariantOrArrayAsString(reader, "REFERRALS_VARIANT"),

        //        LastUpdatedDate =
        //            ReadDateTime(reader, "LAST_UPDATED_DATE")
        //    };
        //}
    }
}
