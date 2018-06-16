using ErpAlgerie.Modules.Core.Module;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErpAlgerie.Modules.Core.Data
{



    public class GenericData<T> where T : ExtendedDocument
    {

        public List<T> Find(Expression<Func<T, bool>> filter)
        {
            return DS.db.GetAll<T>(filter);
        }


        public List<T> Find(string field, object value, bool strict)
        {

            try
            {

                FilterDefinition<T> filt;

                if (value.GetType() == typeof(ObjectId))
                {

                    filt = Builders<T>.Filter.Eq(field, ObjectId.Parse(value.ToString()));
                }
                else
                {
                    if (strict)
                    {
                        filt = Builders<T>.Filter.Eq(field, value);
                    }
                    else
                    {
                        filt = Builders<T>.Filter.Regex(field, new BsonRegularExpression(value?.ToString(), "i"));
                    }
                }

                //FindOptions<BsonDocument> options = new FindOptions<BsonDocument>
                //{
                //    BatchSize = 500,
                //    NoCursorTimeout = false
                //};

                return DS.db.HandlePartitioned<T>(null).FindSync<T>(filt).ToList();
                //  return  Find((a => a.GetType().GetProperty(field).GetValue(a).ToString() == value));
                // return DS.db.GetAll<T>(filter);
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message + $"FIELD: {field} VALUE: {value}");
                return null;
            }
        }


        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> filter)
        {
            return await DS.db.GetAllAsync<T>(filter);
        }


        public T GetById(ObjectId id)
        {
            return DS.db.GetOne<T>(a => a.Id.Equals(id));
        }

        public T GetByIdNullable(ObjectId? id)
        {
            if (id.HasValue)
                return GetById(id.Value);
            return null;
        }


        public IEnumerable<ExtendedDocument> FindAll()
        {
            return Find(a => true) as IEnumerable<ExtendedDocument>;
        }

        public async Task<IEnumerable<ExtendedDocument>> FindAllAsync()
        {
            return await FindAsync(a => true) as IEnumerable<ExtendedDocument>;
        }


        public dynamic GetExpressionForSearch()
        {
            return typeof(Expression<Func<T, bool>>);
        }

        public GenericData()
        {

        }
    }
}
