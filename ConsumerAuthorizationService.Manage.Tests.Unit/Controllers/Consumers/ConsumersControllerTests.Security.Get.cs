// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using ConsumerAuthorizationService.Manage.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;

namespace ConsumerAuthorizationService.Manage.Tests.Unit.Controllers.Consumers
{
    public partial class ConsumersControllerTests
    {
        [Fact]
        public void GetShouldHaveRoleAttributeWithRoles()
        {
            // Given
            var controllerType = typeof(ConsumersController);
            var methodInfo = controllerType.GetMethod("GetConsumerByIdAsync");
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
