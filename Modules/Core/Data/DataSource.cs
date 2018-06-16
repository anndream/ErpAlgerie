using ErpAlgerie.Modules.Core.Module;  
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDbGenericRepository;
using Stylet;
using StyletIoC;
using System;

namespace ErpAlgerie.Modules.Core.Data
{

    public static class DS
    {

        public static IWindowManager windowManager { get; set; }


        private static TestRepository2 _db;
        public static TestRepository2 db
        {
            get
            { 
                var adr = Properties.Settings.Default.MongoServerSettings;
                var db = Properties.Settings.Default.dbUrl;

                TestRepository2 testRepository = new TestRepository2(adr, db);
                return testRepository;
            }
        }
    }

    


    public class DataSource : BaseMongoRepository
    {
       // private static LiteRepository _db;
        private static IContainer ioc;

        public DataSource(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        { 

        }
         
        public static IContainer Ioc
        {
            get { return ioc; }
            set { ioc = value; }
        } 
    }





    public interface ITestRepository2 : IBaseMongoRepository
    {
         
    }

    public class TestRepository2 : BaseMongoRepository, ITestRepository2
    {
        public TestRepository2(string connectionString, string databaseName) : base(connectionString, databaseName)
        {
            Context = this.MongoDbContext;
        }
        public IMongoDbContext Context { get; set; }

    }

        
}
