using ErpAlgerie.Modules.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpAlgerie.Modules.POS
{
    class CartLine
    {
        public CartLine(Article article)
        {
            this.article = article;
            PricUnitaire = article.PrixVente;
        }

        public CartLine(Article article, Variante variante)
        {
            this.article = article;
            this.variante = variante;
            PricUnitaire = article.PrixVente;
        }

        public Article  article { get; set; }
        public Variante variante { get; set; }

        public decimal Qts { get; set; } = 1;
        public decimal PricUnitaire { get; set; }

        public string Name { get
            {
                var propInSetting = PosSettings.getInstance().NameProperty;
                if (string.IsNullOrWhiteSpace(propInSetting))
                {
                    return article.Name;
                }
                else
                {
                    var nameProp = article.GetType().GetProperty(propInSetting);
                    if(nameProp != null)
                    {
                        return article.GetType().GetProperty(propInSetting)?.GetValue(article)?.ToString();
                    }
                    else
                    {
                        return article.Name;
                    }
                   
                }
            } }
        public decimal Total
        {
            get
            {
                return PricUnitaire * Qts;
            }
        }

        public string Message { get; set; }

    }
}
