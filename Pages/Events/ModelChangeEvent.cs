using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpAlgerie.Pages.Events
{
   public class ModelChangeEvent
    {
        public ModelChangeEvent(Type type)
        {
            this.type = type;
        }

        public Type type { get; set; }

    }
}
