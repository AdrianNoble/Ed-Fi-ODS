// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
using System.Diagnostics.CodeAnalysis;
using EdFi.Ods.Api.Filters;
using EdFi.TestFixture;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NUnit.Framework;
using Shouldly;
using Test.Common;

namespace EdFi.Ods.Tests.EdFi.Ods.Api.Services.Filters
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class CheckModelForNullAttributeTests
    {
        [TestFixture]
        public class When_calling_the_check_model_for_null_action_filter : TestFixtureBase
        {
            [Assert]
            public void Existing_arguments_should_not_trigger_filter()
            {
                var actionExecutingContext = A.Fake<ActionExecutingContext>();
                
                var actionFilter = new CheckModelForNullAttribute();
                actionExecutingContext.ActionDescriptor.Properties.Add("request", "ParameterValue");
                actionExecutingContext.ActionDescriptor.Properties.Add("classParameter", new CheckModelForNullAttribute());
               
                actionFilter.OnActionExecuting(actionExecutingContext);
                actionExecutingContext.Result.ShouldBeNull();
            }

            [Assert]
            public void Single_null_argument_should_trigger_filter()
            {
                var actionExecutingContext = A.Fake<ActionExecutingContext>();
                actionExecutingContext.ActionDescriptor.Properties.Add("request", "ParameterValue");
                actionExecutingContext.ActionDescriptor.Properties.Add("classParameter", null);

                var actionFilter = new CheckModelForNullAttribute();
               
                actionFilter.OnActionExecuting(actionExecutingContext);
                BadRequestObjectResult response =(BadRequestObjectResult)actionExecutingContext.Result;
                response.ShouldNotBeNull();

                var responseMessage = response.ExecuteResultAsync(actionExecutingContext).Exception.Message;
                responseMessage.ShouldBe("{\"Message\":\"The request is invalid because it is missing requestParameter', 'classParameter \"}");

            }

            [Assert]
            public void Multiple_null_arguments_should_trigger_filter()
            {
           
                var actionFilter = new CheckModelForNullAttribute();
                var actionExecutingContext = A.Fake<ActionExecutingContext>();
                actionExecutingContext.ActionDescriptor.Properties.Add("request", null);
                actionExecutingContext.ActionDescriptor.Properties.Add("classParameter", null);

                actionFilter.OnActionExecuting(actionExecutingContext);
                var response = actionExecutingContext.Result;
                response.ShouldNotBeNull();
               
                var responseMessage = response.ExecuteResultAsync(actionExecutingContext).Exception.Message;
                responseMessage.ShouldBe("{\"Message\":\"The request is invalid because it is missing requestParameter', 'classParameter \"}");

            }
        }
    }
}
