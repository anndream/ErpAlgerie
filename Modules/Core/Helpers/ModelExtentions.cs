using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Module;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpAlgerie.Modules.Core.Helpers
{
    public static class ModelExtentions
    {

        public static dynamic GetObject(this ObjectId? id,string model)
        {
            return DataHelpers.GetById(model, id);
        }
        public static dynamic GetObject(this ObjectId id, string model)
        {
            return DataHelpers.GetById(model, id);
        }
    }
}
