using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Operations;
using Newtonsoft.Json;
using Persistence.Context;
using Persistence.Mappers;

namespace Persistence.Tests
{
    public abstract class TestBase
    {
        protected FeedContext Context;

        public virtual void Setup()
        {
            var dbContextOptions = new DbContextOptionsBuilder<FeedContext>()
               .UseInMemoryDatabase(databaseName: "FeedInMemory")
               .Options;

            Context = new FeedContext(dbContextOptions);

            MapConfig.Configure();
        }

        protected void AssertAreObjectEquals(object expected, object actual)
        {
            var setting = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            Assert.AreEqual(JsonConvert.SerializeObject(expected, setting), JsonConvert.SerializeObject(actual, setting));
        }

        protected void TestCleanup()
        {
            var posts = Context.Products.ToList();
            Context.Products.RemoveRange(posts);
            
            Context.SaveChangesAsync();

            Context.Dispose();
        }
    }
}
