using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.CRM;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ErpAlgerie.Pages
{
    class LoginViewModel : Screen, IDisposable
    {
        public string pwd { get; set; }
        public User user { get; set; }
        public bool Logedin { get; set; }
        public List<User> users { get
            {
                return (DataHelpers.GetMongoDataSync("User") as List<User>);
            }
        }
       

        public void Connecter()
        {
            if (user == null)
                return;

            //if (string.IsNullOrWhiteSpace(pwd))
            //    return;

            Logedin = pwd == (user.Password);
          
            if (Logedin )
            {
                DataHelpers.ConnectedUser = user;
                RequestClose(true);
            }
            else
            {
                MessageBox.Show("Mots de pass incorrect!");
                return;
            }
        }


        public void Close()
        {
            RequestClose(false);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
