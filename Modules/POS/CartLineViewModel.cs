using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ErpAlgerie.Modules.POS
{

    class CartLineViewModel : Screen, IDisposable
    {
        public event EventHandler clickEvent;
        public CartLineViewModel(CartLine cartLine)
        {
            this.cartLine = cartLine;
            NotifyOfPropertyChange("cartLine");

             
        }
        public event EventHandler DoubleClick;
        public void doubleClick(object sender, EventArgs e)
        {
            EventHandler handler = DoubleClick;
            if (handler != null)
            {
                handler(cartLine, EventArgs.Empty);
            }
        }
        public async void DeleteClicked()
        {
            EventHandler handler = clickEvent;
            if (handler != null)
            {
                handler(cartLine, EventArgs.Empty);
            }
        }

        public CartLine cartLine { get; set; }

        public void Dispose()
        {
           

        }
    }
}
