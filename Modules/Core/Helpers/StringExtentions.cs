using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpAlgerie.Modules.Core.Helpers
{
    public static class StringExtentions
    {

        public static bool ContainsIgniorCase(this string value, string tocompareagainst)
        {
            return (value.ToLower().Contains(tocompareagainst.ToLower()));
        }
       
    }
}
