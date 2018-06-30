using ErpAlgerie.Modules.CRM;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErpAlgerie.Modules.POS
{
    class PosSelectViewModel : Screen,IDisposable
    {
        public PosTicket currentTicket;
        private BindableCollection<PosTicket> tickets;

        public bool Edit { get; set; } = false;
        public PosSelectViewModel(PosTicket currentTicket)
        {
            Edit = true;
            this.currentTicket = currentTicket;
            ticketType = currentTicket.ticketType;
            Numero = currentTicket.Numero;
        }

        public PosSelectViewModel()
        {

        }

        public PosSelectViewModel(PosTicket currentTicket, BindableCollection<PosTicket> tickets) : this(currentTicket)
        {
            this.tickets = tickets;
        }

        public TicketType ticketType { get; set; }
        public int Numero { get; set; }
        public void Dispose()
        {
            
        }

        public void SetPrePaye()
        {
            if (!Edit)
            {
                ticketType = TicketType.PREPAYE;
                this.RequestClose();
            }
            else
            {
                currentTicket.ticketType = TicketType.PREPAYE;
                this.RequestClose();
            }
        }

        public void SetTable()
        {
            int value = 0;
            int.TryParse(PAD_TEXT, out value);
            Numero = value;
            if (Numero < 1)
            {
                MessageBox.Show("Saisir numero table");
                return;
            }

            if (!Edit)
            {
                ticketType = TicketType.TABLE;
                this.RequestClose();
            }
            else
            {
                bool DoJumle = false;
                if (tickets.Select(a => a.Numero).Contains(Numero))
                {
                    var item = tickets.First(a => a.Numero == Numero);
                    // JUMLAGE
                    var response = MessageBox.Show("Table existe déja, jumeler les commandes?", "Jumler", MessageBoxButton.YesNo);
                    if (response == MessageBoxResult.Yes)
                    {
                        DoJumle = true;
                        var items = currentTicket.CarteLines;
                        item.CarteLines.AddRange(items);

                        tickets.Remove(currentTicket);
                        currentTicket.ticketType = TicketType.TABLE;
                        currentTicket.Numero = Numero;
                    }
                    else
                    {
                        RequestClose();
                    }
                }
                else
                {
                    currentTicket.ticketType = TicketType.TABLE;
                    currentTicket.Numero = Numero;
                }
                this.RequestClose();
            }
        }

        public string PAD_TEXT { get; set; }
        public void PAD_CLICK(string value)
        {
            PAD_TEXT += value;
            NotifyOfPropertyChange("PAD_TEXT");
        }


        public void PAD_DELETE()
        {
            if (PAD_TEXT.Length > 0)
                PAD_TEXT = PAD_TEXT.Remove(PAD_TEXT.Length - 1);
            NotifyOfPropertyChange("PAD_TEXT");
        }

    }
}
