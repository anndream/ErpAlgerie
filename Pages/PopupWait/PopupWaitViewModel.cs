using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ErpAlgerie.Pages.PopupWait
{
    class PopupWaitViewModel : Screen, IDisposable
    {
        public int Periode { get; set; } = 500;

        public PopupWaitViewModel(string message)
        {
            Message = message;
        }

        public PopupWaitViewModel(string message,int periode)
        {
            Message = message;
            Periode = periode;
        }

        public string Message { get; set; }
        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            DispatcherTimer time = new DispatcherTimer();
            time.Interval = TimeSpan.FromMilliseconds(Periode);
            time.Start();
            time.Tick += delegate
            {
                this.RequestClose();
                time.Stop();
            };
        }

        public void closePop()
        {
            this.RequestClose();
        }
        public void Dispose()
        {
            
         }
    }
}
