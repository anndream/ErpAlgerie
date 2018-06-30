using ErpAlgerie.Modules.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpAlgerie.Modules.POS
{
    class CartLineComparer : IEqualityComparer<CartLine>
    {
        public bool Equals(CartLine x, CartLine y)
        {
            //if (x.Name == y.Name
            //    && x.Qts == y.Qts)
            //    return true;

            //if (x.Name == y.Name
            //    && x.Qts > y.Qts)
            //{
            //    x.Qts = y.Qts;
            //    return true;
            //}

            //if (x.Name == y.Name
            //    && x.Qts < y.Qts)
            //{
            //    x.Qts = y.Qts;
            //    return true;
            //}

            return true;
        }

        public int GetHashCode(CartLine obj)
        {   
            return 0;
        }
    }
}
