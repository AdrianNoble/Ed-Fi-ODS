// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
using EdFi.Ods.Common.Models.Domain;
using EdFi.Ods.Common.Security;
using FakeItEasy;
using Shouldly;
using System.Threading.Tasks;

namespace EdFi.Ods.Tests.EdFi.Ods.Api
{
    internal class ApiClientAuthenticatorHelper
    {
       
        public IApiClientAuthenticator Mock(string key, ApiClientSecret value)
        {
            var _apiClientIdentity = new ApiClientIdentity
            {
                Key = key
            };

          var  _apiClientAuthenticator = A.Fake<IApiClientAuthenticator>();
            var apiClientIdentityProvider = A.Fake<IApiClientIdentityProvider>();

            A.CallTo(() => apiClientIdentityProvider.GetApiClientIdentity(key))
                                     .Returns(_apiClientIdentity);



            var apiClientSecretProvider = A.Fake<IApiClientSecretProvider>();

            A.CallTo(() => apiClientSecretProvider.GetSecret(key))
                                    .Returns(value);

            var secretVerifier = A.Fake<ISecretVerifier>();

            A.CallTo(() => secretVerifier.VerifySecret(key, value.Secret, value))
                                    .Returns(true);

            _apiClientAuthenticator = new ApiClientAuthenticator(apiClientIdentityProvider, apiClientSecretProvider, secretVerifier);

            return _apiClientAuthenticator;
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
