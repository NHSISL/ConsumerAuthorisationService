// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ConsumerAuthorizationService.Core.Models.Coordinations.Patients.Exceptions
{
    public class PatientCoordinationServiceException : Xeption
    {
        public PatientCoordinationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
