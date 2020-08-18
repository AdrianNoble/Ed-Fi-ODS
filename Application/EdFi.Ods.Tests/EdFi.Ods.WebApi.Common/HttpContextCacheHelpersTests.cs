// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Ods.Api.Providers;
using FakeItEasy;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.Tests.EdFi.Ods.WebApi.Common
{
    [TestFixture]
    public class HttpContextCacheHelpersTests
    {
        [Test]
        public void Should_Cache_item_in_HttpContext_Items()
        {
            var key = "abc";
            var expected = "XYZ";
            var memorycacheoprion = A.Fake<IOptions<MemoryCacheOptions>>();
            MemoryCache memoryCache = new MemoryCache(memorycacheoprion);

            var memorycacheprovider = new MemoryCacheProvider(memoryCache);
            memorycacheprovider.Insert(key, expected, DateTime.MaxValue, TimeSpan.FromMinutes(5));
            Assert.AreEqual(memoryCache.Count, 1);
            object output;
            memorycacheprovider.TryGetCachedObject(key, out output);
            output.ShouldBe(expected);
        }

        [Test]
        public void Should_Not_Try_To_Add_Item_If_Already_In_Cache()
        {
            var key = "abc";
            var expected = "XYZ";
            var memorycacheoprion = A.Fake<IOptions<MemoryCacheOptions>>();
            MemoryCache memoryCache = new MemoryCache(memorycacheoprion);

            var memorycacheprovider = new MemoryCacheProvider(memoryCache);
            memorycacheprovider.Insert(key, expected, DateTime.MaxValue, TimeSpan.FromMinutes(5));
            memorycacheprovider.Insert(key, expected, DateTime.MaxValue, TimeSpan.FromMinutes(5));
            Assert.AreEqual(memoryCache.Count,1);
            object output;
            var actual = memorycacheprovider.TryGetCachedObject(key, out output);
            output.ShouldBe(expected);
        }

        // TODO: GKM - review these tests

        //[Test]
        //public void Should_Return_True_If_Item_is_In_Cache()
        //{
        //    using (var simulatedContext = new HttpSimulator())
        //    {
        //        var key = "abc";
        //        var expected = "XYZ";
        //        simulatedContext.SimulateRequest(new Uri("http://abc.com"));
        //        HttpContext.Current.CacheInRequest(key, expected);
        //        HttpContext.Current.RequestCacheContains(key, expected).ShouldBeTrue();
        //    }

        //}

        [Test]
        public void Should_Retrieve_Item_from_Cache()
        {
            var key = "abc";
            var expected = "XYZ";
            var memorycacheoprion = A.Fake<IOptions<MemoryCacheOptions>>();
            MemoryCache memoryCache = new MemoryCache(memorycacheoprion);

            var memorycacheprovider = new MemoryCacheProvider(memoryCache);
            memorycacheprovider.Insert(key, expected, DateTime.MaxValue, TimeSpan.FromMinutes(5));
            Assert.AreEqual(memoryCache.Count, 1);
            object output;
            memorycacheprovider.TryGetCachedObject(key, out output);
            output.ShouldBe(expected);
        }

        [Test]
        public void Should_Throw____Exception_When_Key_Is_Not_In_Cache()
        {
            var key = "abc";
            var expected = "XYZ";
            var memorycacheoprion = A.Fake<IOptions<MemoryCacheOptions>>();
            MemoryCache memoryCache = new MemoryCache(memorycacheoprion);

            var memorycacheprovider = new MemoryCacheProvider(memoryCache);
            memorycacheprovider.Insert(key, expected, DateTime.MaxValue, TimeSpan.FromMinutes(5));
            Assert.AreEqual(memoryCache.Count, 1);
            object output;
            memorycacheprovider.TryGetCachedObject("XYZ", out output);
            output.ShouldBe(null);
        }
    }
}
