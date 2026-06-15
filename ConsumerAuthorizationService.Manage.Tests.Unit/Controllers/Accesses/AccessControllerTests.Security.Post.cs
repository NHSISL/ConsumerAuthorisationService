// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using ConsumerAuthorizationService.Manage.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;

namespace ConsumerAuthorizationService.Manage.Tests.Unit.Controllers.Accesses
{
    public partial class AccessControllerTests
    {
        [Fact]
        public void PostShouldHaveRoleAttributeWithRoles()
        {
            // Given
            var controllerType = typeof(AccessController);
            var methodInfo = controllerType.GetMethod("PostAccessValidationAsync");
            Type attributeType = typeof(AuthorizeAttribute);
            string attributeProperty = "Roles";

            List<string> expectedAttributeValues = new List<string>
            {
                "Administrators",
                "Users"
            };

            // When
            var methodAttribute = methodInfo?
                .GetCustomAttributes(attributeType, inherit: true)
                .FirstOrDefault();

            var controllerAttribute = controllerType
                .GetCustomAttributes(attributeType, inherit: true)
                .FirstOrDefault();

            var attribute = methodAttribute ?? controllerAttribute;

            // Then
            attribute.Should().NotBeNull();

            var actualAttributeValue = attributeType
                .GetProperty(attributeProperty)?
                .GetValue(attribute) as string ?? string.Empty;

            var actualAttributeValues = actualAttributeValue?
                .Split(',')
                .Select(role => role.Trim())
                .Where(role => !string.IsNullOrEmpty(role))
                .ToList();

            actualAttributeValues.Should().BeEquivalentTo(expectedAttributeValues);
        }
    }
}
