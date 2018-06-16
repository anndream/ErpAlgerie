using ErpAlgerie.Modules.Core.Module;
using ErpAlgerie.Modules.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ErpAlgerie.Modules.Core.Data
{
   public static class FilterEngine
    {
        internal static async Task<IEnumerable<ExtendedDocument>> GetMongoData(string concrete)
        {

            try
            {
                switch (concrete)
                {

                    //case "Tier":
                    //    return await DS.db.GetAllAsync<Tier>(a => true);
                    ////////////// AUTO INSERT
                    // 128 



                ASYNC_GetMongoData


                   //ENDAUTO
                default:
                        return null;
                break;
            }
            }
            catch (Exception s)
            {
                throw s;
            }
}

        public static IEnumerable<ExtendedDocument> GetMongoData(string concrete, string field, string value, bool strict)
        {
            switch (concrete)
            {

                //case "Tier":
                //    return DS.db.GetAll<Tier>(a => true).Where(a => strict == true ?
                //    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
                //    :
                //    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
                //    );
                ////////////// AUTO INSERT
                // 158


    FILTER_GetMongoData



                default:
                    return null;
                    break;
            }
        }

        internal static IEnumerable<ExtendedDocument> GetMongoDataSync(string concrete)
        {
            switch (concrete)
            {

                //case "DossierMedicale":
                //    return DS.db.GetAll<DossierMedicale>(a => true);


        ////////////// AUTO INSERT
        // 178


    SYNC_GetMongoData





                // ENDAUTO

    default:
                    return null;
                    break;
            }
        }
    }

}

