#if NETFRAMEWORK
//TODO -  Should be moved to other assembly
// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.Tests.EdFi.Ods.Admin.Services
{
    [TestFixture]
    public class AppSettingsConfigurationTests
    {
        [Test]
        public void When_Accessing_App_Settings()
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(TestContext.CurrentContext.TestDirectory)
               .AddJsonFile("appsettings.json", optional: true)
               .AddEnvironmentVariables()
               .Build();

            var smtpcredentials= config.GetSection("appsetting").GetChildren().ToList()
               .ToDictionary(k => k.Key, v => v.Value);

            if (smtpcredentials["smtpUsername"] == null)
            {
                Assert.Fail("Expected smtp:Username in app.config, but not found or could not read");
            }

            if (smtpcredentials["smtpPassword"] == null)
            {
                Assert.Fail("Expected smtp:Password in app.config, but not found or could not read");
            }

            smtpcredentials["smtpUsername"].ShouldBe("Bingo");
            smtpcredentials["smtpPassword"].ShouldBe("Tingo");
        }
    }
}
#endif