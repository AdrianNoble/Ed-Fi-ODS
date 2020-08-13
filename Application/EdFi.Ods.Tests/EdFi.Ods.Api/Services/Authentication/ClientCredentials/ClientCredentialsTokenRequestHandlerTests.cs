// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net.Http;
using System.Web.Http;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.Api.Models.ClientCredentials;
using EdFi.Ods.Api.Models.Tokens;
using EdFi.Ods.Api.Providers;
using EdFi.Ods.Common.Security;
using EdFi.Ods.Sandbox.Repositories;
using EdFi.TestFixture;
using FakeItEasy;
using NUnit.Framework;
using Test.Common;

// ReSharper disable InconsistentNaming

namespace EdFi.Ods.Tests.EdFi.Ods.Api.Services.Authentication.ClientCredentials
{
    [TestFixture]
    public class ClientCredentialsTokenRequestHandlerTests
    {
        internal const string ClientId = "clientId";
        internal const string ClientSecret = "clientSecret";
        private static readonly ApiClientAuthenticatorHelper _apiClientAuthenticatorHelper = new ApiClientAuthenticatorHelper();

        public class When_handling_a_valid_token_request
            : TestFixtureBase
        {
            private TokenRequest _tokenRequest;
            private IClientAppRepo _clientAppRepo;
            private IApiClientAuthenticator _apiClientAuthenticator;
            private ClientCredentialsTokenRequestProvider _clientCredentialsTokenRequestHandler;
            private AuthenticationResponse _actionResult;

            protected override void Arrange()
            {
                var apiClient = new ApiClient
                                {
                                    ApiClientId = 0
                                };

                _clientAppRepo = A.Fake<IClientAppRepo>();

                A.CallTo(() => _clientAppRepo.GetClient(A<string>._)).Returns(apiClient);

                A.CallTo(() => _clientAppRepo.AddClientAccessToken(A<int>._, A<string>._)).Returns(new ClientAccessToken());
               
                _tokenRequest = new TokenRequest
                                {
                                    Client_id = ClientId, Client_secret = ClientSecret,Grant_type= "client_credentials"
                };

                _apiClientAuthenticator = _apiClientAuthenticatorHelper.Mock();

                _clientCredentialsTokenRequestHandler = new ClientCredentialsTokenRequestProvider(_clientAppRepo, _apiClientAuthenticator);
            }

            protected override void Act()
            {
                _actionResult = _clientCredentialsTokenRequestHandler.HandleAsync(_tokenRequest).Result;
            }

            [Assert]
            public void Should_return_a_non_null_action_result()
            {
                Assert.That(_actionResult, Is.Not.Null);
            }

            [Assert]
            public void Should_have_action_result_type_of_authentication_success()
            {
                Assert.That(_actionResult, Is.InstanceOf<AuthenticationSuccess>());
            }

            [Assert]
            public void Should_call_get_client_from_the_database_once()
            {

               A.CallTo(
                    () => _clientAppRepo.GetClient(ClientId))
                    .MustHaveHappenedOnceExactly();
            }

            [Assert]
            public void Should_call_try_authenticate_from_the_database_once()
            {
                ApiClientIdentity apiClientIdentity;

                A.CallTo(
                    () => _apiClientAuthenticator.TryAuthenticate(ClientId, ClientSecret, out apiClientIdentity)).MustHaveHappened();
            }

            [Assert]
            public void Should_add_the_client_access_token_to_the_database_once()
            {
                A.CallTo(() => _clientAppRepo.AddClientAccessToken(0, null)).MustHaveHappened();
            }
        }

        public class When_handling_an_invalid_token_request
            : TestFixtureBase
        {
            
            private IClientAppRepo _clientAppRepo;
            private IApiClientAuthenticator _apiClientAuthenticator;
            private TokenRequest _tokenRequest;
            private ClientCredentialsTokenRequestProvider _clientCredentialsTokenRequestHandler;
            private AuthenticationResponse _actionResult;
            protected override void Arrange()
            {
                _clientAppRepo = A.Fake<IClientAppRepo>();

                _tokenRequest = new TokenRequest
                                {
                                    Client_id = ClientId, Client_secret = ClientSecret
                                };

                _apiClientAuthenticator = A.Fake<IApiClientAuthenticator>();
                ApiClientIdentity apiClientIdentity;

                var apiclientauthenticatedelegate = new ApiClientAuthenticatorDelegates.TryAuthenticateDelegate(
                            (string key, string password, out ApiClientIdentity identity) =>
                            {
                                identity = null;
                                return false;
                            });

                A.CallTo(() => _apiClientAuthenticator.TryAuthenticate(null, null, out apiClientIdentity)).Returns(false);
                
                _clientCredentialsTokenRequestHandler = new ClientCredentialsTokenRequestProvider(_clientAppRepo, _apiClientAuthenticator);
            }

            protected override void Act()
            {
                _actionResult =  _clientCredentialsTokenRequestHandler.HandleAsync(_tokenRequest).Result;
            }

            [Assert]
            public void Should_return_a_non_null_action_result()
            {
                Assert.That(_actionResult, Is.Not.Null);
            }

            [Assert]
            public void Should_have_action_result_type_of_authentication_error()
            {
                Assert.That(_actionResult, Is.InstanceOf<AuthenticationError>());
            }

            [Assert]
            public void Should_call_try_authenticate_from_database_once()
            {
                ApiClientIdentity apiClientIdentity;

                A.CallTo(
                    () => _apiClientAuthenticator.TryAuthenticate(ClientId, ClientSecret, out apiClientIdentity)).MustHaveHappened();
            }

            [Assert]
            public void Should_not_add_the_client_access_token_to_the_database_once()
            {
                A.CallTo(
                   () => _clientAppRepo.AddClientAccessToken(A<int>._, A<string>._)).MustNotHaveHappened();
            }
        }
    }
}
