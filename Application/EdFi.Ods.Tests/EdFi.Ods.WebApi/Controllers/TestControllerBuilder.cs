// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using EdFi.Ods.Api.ExceptionHandling;
using EdFi.Ods.Api.ExceptionHandling.Translators;
using EdFi.Ods.Api.ExceptionHandling.Translators.SqlServer;
using EdFi.Ods.Api.Helpers;
using EdFi.Ods.Api.Infrastructure.Pipelines.Factories;
using EdFi.Ods.Api.InversionOfControl;
using EdFi.Ods.Api.Providers;
using EdFi.Ods.Common;
using EdFi.Ods.Common.Configuration;
using EdFi.Ods.Common.Constants;
using EdFi.Ods.Common.Container;
using EdFi.Ods.Common.Context;
using EdFi.Ods.Common.Infrastructure.Pipelines;
using EdFi.Ods.Common.InversionOfControl;
using EdFi.Ods.Common.Providers;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.Ods.Tests.EdFi.Ods.WebApi.Controllers
{
    public class StubEtagProviderSinceWeReallyDontCareWhatTheValueIs : IETagProvider
    {
        public string GetETag(object value)
        {
            return "new" + value;
        }

        public DateTime GetDateTime(string etag)
        {
            return new DateTime(2000, 1, 1);
        }
    }

    public class StubCurrentSchoolYearContextProvider : ISchoolYearContextProvider
    {
        public int GetSchoolYear()
        {
            return DateTime.Now.Year;
        }

        public void SetSchoolYear(int schoolYear)
        {
            throw new NotImplementedException();
        }
    }

    internal class StubDatabaseMetadataProvider : IDatabaseMetadataProvider
    {
        public IndexDetails GetIndexDetails(string indexName)
        {
            return new IndexDetails
            {
                IndexName = "FK_TableName_IndexId",
                TableName = "TableName",
                ColumnNames = new List<string>
                                                                                                  {
                                                                                                      "TableNameId"
                                                                                                  }
            };
        }
    }

    public static class TestControllerBuilder
    {
        public static T GetController<T>(IPipelineFactory factory, string id = null)
            where T : ControllerBase
        {
            var translators = new IExceptionTranslator[]
                              {
                                  new TypeBasedBadRequestExceptionTranslator(),
                                  new SqlServerConstraintExceptionTranslator(),
                                  new SqlServerUniqueIndexExceptionTranslator(new StubDatabaseMetadataProvider()),
                                  new EdFiSecurityExceptionTranslator(),
                                  new NotFoundExceptionTranslator(),
                                  new NotModifiedExceptionTranslator(),
                                  new ConcurrencyExceptionTranslator(),
                                  new DuplicateNaturalKeyCreateExceptionTranslator(new StubDatabaseMetadataProvider())
                              };

            var schoolYearContextProvider = A.Fake<ISchoolYearContextProvider>();

            A.CallTo(() => schoolYearContextProvider.GetSchoolYear())
                                     .Returns(DateTime.Now.Year);

            var configvalueprovider = A.Fake<IConfigValueProvider>();
            var controller =
                (T)
                Activator.CreateInstance(
                    typeof(T),
                    factory,
                    new StubCurrentSchoolYearContextProvider(),
                    new RESTErrorProvider(translators),
                    new DefaultPageSizeLimitProvider(configvalueprovider));

            //controller.Configuration = new HttpConfiguration();
            //var uri = $@"http://localhost/api/ods/v3/ed-fi/Students/{id}";

            //controller.Request = new HttpRequestMessage
            //{
            //    RequestUri = new Uri(uri)
            //};

            return controller;
        }

      
        public static IContainer GetWindsorContainer()
        {

            ContainerBuilder builder = new ContainerBuilder();
            
            RegisterModulesDynamically();

            builder.RegisterType<StubCurrentSchoolYearContextProvider>()
              .As<ISchoolYearContextProvider>();

            builder.RegisterType<StubEtagProviderSinceWeReallyDontCareWhatTheValueIs>()
  .As<IETagProvider>();

            var dataAccess = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(dataAccess)
                .Where(s=>s.BaseType== typeof(IStep<,>))
                      .AsImplementedInterfaces();

           


            void RegisterModulesDynamically()
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .ToList();

                foreach (var type in TypeHelper.GetTypesWithModules())
                {

                    if (type.IsSubclassOf(typeof(ConditionalModule)))
                    {
                        ApiSettings settings = new ApiSettings();
                        settings.Mode = ApiConfigurationConstants.Sandbox;
                        settings.Engine = ApiConfigurationConstants.SqlServer;
                        settings.GetApiMode();
                        settings.GetDatabaseEngine();
                        builder.RegisterModule((IModule)Activator.CreateInstance(type, settings));
                    }
                    else
                    {
                        builder.RegisterModule((IModule)Activator.CreateInstance(type));
                    }
                }
            }
            return builder.Build();
          
        }
    }
}
