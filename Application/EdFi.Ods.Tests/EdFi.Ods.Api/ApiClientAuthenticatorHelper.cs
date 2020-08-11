// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
using EdFi.Ods.Common.Models.Domain;
using EdFi.Ods.Common.Security;
using FakeItEasy;
using Shouldly;

namespace EdFi.Ods.Tests.EdFi.Ods.Api
{
    internal class ApiClientAuthenticatorHelper
    {
        public IApiClientAuthenticator Mock()
        {
            var apiClientAuthenticator = A.Fake<IApiClientAuthenticator>();
            
            ApiClientIdentity apiClientIdentity;
            var fakedata = new ApiClientAuthenticatorDelegates.TryAuthenticateDelegate(
                        (string key, string password, out ApiClientIdentity identity) =>
                        {
                            identity = new ApiClientIdentity
                            {
                                Key = key
                            };

                            return true;
                        });


            A.CallTo(() => apiClientAuthenticator.TryAuthenticate("good_clientId", "good_clientSecret", out apiClientIdentity))
               .Returns(true);
                
            return apiClientAuthenticator;
        }

        public IApiClientAuthenticator MockFalse()
        {
            var apiClientAuthenticator = A.Fake<IApiClientAuthenticator>();
            ApiClientIdentity apiClientIdentity;

            var fakedata = new ApiClientAuthenticatorDelegates.TryAuthenticateDelegate(
                        (string key, string password, out ApiClientIdentity identity) =>
                        {
                            identity = null;
                            return false;
                        });

            A.CallTo(() => apiClientAuthenticator.TryAuthenticate("badClientId", "badClientSecret", out apiClientIdentity)).Returns(false);

            return apiClientAuthenticator;

        }
    }
}
