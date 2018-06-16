using ErpAlgerie.Modules.Core.Module;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpAlgerie.Modules.Core.Helpers
{
   public interface IShell
    {
        void OpenScreen(IScreen screen, string title);
        void CloseScreen(IScreen screen);
        Task OpenScreenDetach(ExtendedDocument doc, string s);
    }
}
