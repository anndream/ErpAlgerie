using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Models;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpAlgerie.Modules.Core.Module
{
     
    public interface IModel  
    {
        bool Save();
        bool Delete(bool ConfirmFromUser = true);
        bool Submit();
        bool Cancel();
     }
}
