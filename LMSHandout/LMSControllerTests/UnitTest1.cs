using LMS.Controllers;
using LMS.Models.LMSModels;
using LMS_CustomIdentity.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Text.Json;
using Xunit.Abstractions;

namespace LMSControllerTests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Test1()
        {
            // An example of a simple unit test on the CommonController
            CommonController ctrl = new CommonController(MakeTinyDB());

            // TEST: GetDepartments
            var allDepts = ctrl.GetDepartments() as JsonResult;

            var jsonResult = JsonSerializer.Serialize(allDepts.Value);
            output.WriteLine(jsonResult);

            var jsonArray = JArray.Parse(jsonResult);
            var firstItem = jsonArray[0] as JObject;

            Assert.Equal(1, jsonArray.Count);

            var subject = (string)firstItem["subject"];
            var name = (string)firstItem["name"];

            Assert.Equal("CS", subject);
            Assert.Equal("KSoC", name);

            // TODO: test other controllers
        }


        /// <summary>
        /// Make a very tiny in-memory database, containing just one department
        /// and nothing else.
        /// </summary>
        /// <returns></returns>
        LMSContext MakeTinyDB()
        {
            var contextOptions = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase("LMSControllerTest")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .UseApplicationServiceProvider(NewServiceProvider())
            .Options;

            var db = new LMSContext(contextOptions);

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Departments.Add(new Department { Name = "KSoC", Subject = "CS" });

            // TODO: add more objects to the test database

            db.SaveChanges();

            return db;
        }

        private static ServiceProvider NewServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
          .AddEntityFrameworkInMemoryDatabase()
          .BuildServiceProvider();

            return serviceProvider;
        }

    }
}