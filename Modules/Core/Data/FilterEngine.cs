//using ErpAlgerie.Modules.Core.Module;
//using ErpAlgerie.Modules.CRM;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;


//namespace ErpAlgerie.Modules.Core.Data
//{
//    public static class FilterEngine
//    {
//        internal static async Task<IEnumerable<ExtendedDocument>> GetMongoData(string concrete)
//        {

//            try
//            {
//                switch (concrete)
//                {

//                    //case "Tier":
//                    //    return await DS.db.GetAllAsync<Tier>(a => true);
//                    ////////////// AUTO INSERT
//                    // 128 






//                    case "AccesRule":
//                        return await DS.db.GetAllAsync<AccesRule>(a => true);



//                    case "Adresse":
//                        return await DS.db.GetAllAsync<Adresse>(a => true);



//                    case "Article":
//                        return await DS.db.GetAllAsync<Article>(a => true);



//                    case "BonLaivraion":
//                        return await DS.db.GetAllAsync<BonLaivraion>(a => true);



//                    case "BonLivraionArticle":
//                        return await DS.db.GetAllAsync<BonLivraionArticle>(a => true);



//                    case "Client":
//                        return await DS.db.GetAllAsync<Client>(a => true);



//                    case "CommandeVente":
//                        return await DS.db.GetAllAsync<CommandeVente>(a => true);



//                    case "CompteCompta":
//                        return await DS.db.GetAllAsync<CompteCompta>(a => true);



//                    case "Contact":
//                        return await DS.db.GetAllAsync<Contact>(a => true);



//                    case "DbSourceLink":
//                        return await DS.db.GetAllAsync<DbSourceLink>(a => true);



//                    case "DemandeMateriel":
//                        return await DS.db.GetAllAsync<DemandeMateriel>(a => true);



//                    case "Devis":
//                        return await DS.db.GetAllAsync<Devis>(a => true);



//                    case "DomaineActivite":
//                        return await DS.db.GetAllAsync<DomaineActivite>(a => true);



//                    case "EcritureStock":
//                        return await DS.db.GetAllAsync<EcritureStock>(a => true);



//                    case "ElvaSettings":
//                        return await DS.db.GetAllAsync<ElvaSettings>(a => true);



//                    case "Facture":
//                        return await DS.db.GetAllAsync<Facture>(a => true);



//                    case "FraisSupplaimentaire":
//                        return await DS.db.GetAllAsync<FraisSupplaimentaire>(a => true);



//                    case "GroupeArticle":
//                        return await DS.db.GetAllAsync<GroupeArticle>(a => true);



//                    case "GroupeClient":
//                        return await DS.db.GetAllAsync<GroupeClient>(a => true);



//                    case "GroupeEntete":
//                        return await DS.db.GetAllAsync<GroupeEntete>(a => true);



//                    case "LigneEcritureStock":
//                        return await DS.db.GetAllAsync<LigneEcritureStock>(a => true);



//                    case "LigneFacture":
//                        return await DS.db.GetAllAsync<LigneFacture>(a => true);



//                    case "LigneRecuAchat":
//                        return await DS.db.GetAllAsync<LigneRecuAchat>(a => true);



//                    case "ListePrix":
//                        return await DS.db.GetAllAsync<ListePrix>(a => true);



//                    case "LotArticle":
//                        return await DS.db.GetAllAsync<LotArticle>(a => true);



//                    case "Marque":
//                        return await DS.db.GetAllAsync<Marque>(a => true);



//                    case "ModeleEntete":
//                        return await DS.db.GetAllAsync<ModeleEntete>(a => true);



//                    case "ModuleBuilder":
//                        return await DS.db.GetAllAsync<ModuleBuilder>(a => true);



//                    case "ModuleErp":
//                        return await DS.db.GetAllAsync<ModuleErp>(a => true);



//                    case "ModuleProperty":
//                        return await DS.db.GetAllAsync<ModuleProperty>(a => true);



//                    case "NumSerieStock":
//                        return await DS.db.GetAllAsync<NumSerieStock>(a => true);



//                    case "PosSettings":
//                        return await DS.db.GetAllAsync<PosSettings>(a => true);



//                    case "PrixArticle":
//                        return await DS.db.GetAllAsync<PrixArticle>(a => true);



//                    case "RecuAchat":
//                        return await DS.db.GetAllAsync<RecuAchat>(a => true);



//                    case "Role":
//                        return await DS.db.GetAllAsync<Role>(a => true);



//                    case "SeriesName":
//                        return await DS.db.GetAllAsync<SeriesName>(a => true);



//                    case "Stock":
//                        return await DS.db.GetAllAsync<Stock>(a => true);



//                    case "Table":
//                        return await DS.db.GetAllAsync<Table>(a => true);



//                    case "Taxe":
//                        return await DS.db.GetAllAsync<Taxe>(a => true);



//                    case "TaxeLigne":
//                        return await DS.db.GetAllAsync<TaxeLigne>(a => true);



//                    case "UniteMesure":
//                        return await DS.db.GetAllAsync<UniteMesure>(a => true);



//                    case "User":
//                        return await DS.db.GetAllAsync<User>(a => true);

                         



//                    case "VenteDirecte":
//                        return await DS.db.GetAllAsync<VenteDirecte>(a => true);



//                    //ENDAUTO
//                    default:
//                        return null;
//                        break;
//                }
//            }
//            catch (Exception s)
//            {
//                throw s;
//            }
//        }

//        public static IEnumerable<ExtendedDocument> GetMongoData(string concrete, string field, string value, bool strict)
//        {
//            switch (concrete)
//            {

//                //case "Tier":
//                //    return DS.db.GetAll<Tier>(a => true).Where(a => strict == true ?
//                //    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                //    :
//                //    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                //    );
//                ////////////// AUTO INSERT
//                // 158





//                case "AccesRule":
//                    return DS.db.GetAll<AccesRule>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "Adresse":
//                    return DS.db.GetAll<Adresse>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "Article":
//                    return DS.db.GetAll<Article>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "BonLaivraion":
//                    return DS.db.GetAll<BonLaivraion>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "BonLivraionArticle":
//                    return DS.db.GetAll<BonLivraionArticle>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "Client":
//                    return DS.db.GetAll<Client>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "CommandeVente":
//                    return DS.db.GetAll<CommandeVente>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "CompteCompta":
//                    return DS.db.GetAll<CompteCompta>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "Contact":
//                    return DS.db.GetAll<Contact>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "DbSourceLink":
//                    return DS.db.GetAll<DbSourceLink>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "DemandeMateriel":
//                    return DS.db.GetAll<DemandeMateriel>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "Devis":
//                    return DS.db.GetAll<Devis>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "DomaineActivite":
//                    return DS.db.GetAll<DomaineActivite>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "EcritureStock":
//                    return DS.db.GetAll<EcritureStock>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "ElvaSettings":
//                    return DS.db.GetAll<ElvaSettings>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "Facture":
//                    return DS.db.GetAll<Facture>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "FraisSupplaimentaire":
//                    return DS.db.GetAll<FraisSupplaimentaire>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "GroupeArticle":
//                    return DS.db.GetAll<GroupeArticle>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "GroupeClient":
//                    return DS.db.GetAll<GroupeClient>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "GroupeEntete":
//                    return DS.db.GetAll<GroupeEntete>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "LigneEcritureStock":
//                    return DS.db.GetAll<LigneEcritureStock>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "LigneFacture":
//                    return DS.db.GetAll<LigneFacture>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "LigneRecuAchat":
//                    return DS.db.GetAll<LigneRecuAchat>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "ListePrix":
//                    return DS.db.GetAll<ListePrix>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "LotArticle":
//                    return DS.db.GetAll<LotArticle>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "Marque":
//                    return DS.db.GetAll<Marque>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "ModeleEntete":
//                    return DS.db.GetAll<ModeleEntete>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "ModuleBuilder":
//                    return DS.db.GetAll<ModuleBuilder>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "ModuleErp":
//                    return DS.db.GetAll<ModuleErp>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "ModuleProperty":
//                    return DS.db.GetAll<ModuleProperty>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "NumSerieStock":
//                    return DS.db.GetAll<NumSerieStock>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "PosSettings":
//                    return DS.db.GetAll<PosSettings>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "PrixArticle":
//                    return DS.db.GetAll<PrixArticle>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "RecuAchat":
//                    return DS.db.GetAll<RecuAchat>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "Role":
//                    return DS.db.GetAll<Role>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "SeriesName":
//                    return DS.db.GetAll<SeriesName>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "Stock":
//                    return DS.db.GetAll<Stock>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "Table":
//                    return DS.db.GetAll<Table>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "Taxe":
//                    return DS.db.GetAll<Taxe>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "TaxeLigne":
//                    return DS.db.GetAll<TaxeLigne>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "UniteMesure":
//                    return DS.db.GetAll<UniteMesure>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );



//                case "User":
//                    return DS.db.GetAll<User>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );


                     


//                case "VenteDirecte":
//                    return DS.db.GetAll<VenteDirecte>(a => true).Where(a => strict == true ?
//                    a.GetType().GetProperty(field).GetValue(a, null)?.ToString() == value
//                    :
//                    a.GetType().GetProperty(field).GetValue(a, null).ToString().Contains(value)
//                     );




//                default:
//                    return null;
//                    break;
//            }
//        }

//        internal static IEnumerable<ExtendedDocument> GetMongoDataSync(string concrete)
//        {
//            switch (concrete)
//            {

//                //case "DossierMedicale":
//                //    return DS.db.GetAll<DossierMedicale>(a => true);


//                ////////////// AUTO INSERT
//                // 178





//                case "AccesRule":
//                    return DS.db.GetAll<AccesRule>(a => true);



//                case "Adresse":
//                    return DS.db.GetAll<Adresse>(a => true);



//                case "Article":
//                    return DS.db.GetAll<Article>(a => true);



//                case "BonLaivraion":
//                    return DS.db.GetAll<BonLaivraion>(a => true);



//                case "BonLivraionArticle":
//                    return DS.db.GetAll<BonLivraionArticle>(a => true);



//                case "Client":
//                    return DS.db.GetAll<Client>(a => true);



//                case "CommandeVente":
//                    return DS.db.GetAll<CommandeVente>(a => true);



//                case "CompteCompta":
//                    return DS.db.GetAll<CompteCompta>(a => true);



//                case "Contact":
//                    return DS.db.GetAll<Contact>(a => true);



//                case "DbSourceLink":
//                    return DS.db.GetAll<DbSourceLink>(a => true);



//                case "DemandeMateriel":
//                    return DS.db.GetAll<DemandeMateriel>(a => true);



//                case "Devis":
//                    return DS.db.GetAll<Devis>(a => true);



//                case "DomaineActivite":
//                    return DS.db.GetAll<DomaineActivite>(a => true);



//                case "EcritureStock":
//                    return DS.db.GetAll<EcritureStock>(a => true);



//                case "ElvaSettings":
//                    return DS.db.GetAll<ElvaSettings>(a => true);



//                case "Facture":
//                    return DS.db.GetAll<Facture>(a => true);



//                case "FraisSupplaimentaire":
//                    return DS.db.GetAll<FraisSupplaimentaire>(a => true);



//                case "GroupeArticle":
//                    return DS.db.GetAll<GroupeArticle>(a => true);



//                case "GroupeClient":
//                    return DS.db.GetAll<GroupeClient>(a => true);



//                case "GroupeEntete":
//                    return DS.db.GetAll<GroupeEntete>(a => true);



//                case "LigneEcritureStock":
//                    return DS.db.GetAll<LigneEcritureStock>(a => true);



//                case "LigneFacture":
//                    return DS.db.GetAll<LigneFacture>(a => true);



//                case "LigneRecuAchat":
//                    return DS.db.GetAll<LigneRecuAchat>(a => true);



//                case "ListePrix":
//                    return DS.db.GetAll<ListePrix>(a => true);



//                case "LotArticle":
//                    return DS.db.GetAll<LotArticle>(a => true);



//                case "Marque":
//                    return DS.db.GetAll<Marque>(a => true);



//                case "ModeleEntete":
//                    return DS.db.GetAll<ModeleEntete>(a => true);



//                case "ModuleBuilder":
//                    return DS.db.GetAll<ModuleBuilder>(a => true);



//                case "ModuleErp":
//                    return DS.db.GetAll<ModuleErp>(a => true);



//                case "ModuleProperty":
//                    return DS.db.GetAll<ModuleProperty>(a => true);



//                case "NumSerieStock":
//                    return DS.db.GetAll<NumSerieStock>(a => true);



//                case "PosSettings":
//                    return DS.db.GetAll<PosSettings>(a => true);



//                case "PrixArticle":
//                    return DS.db.GetAll<PrixArticle>(a => true);



//                case "RecuAchat":
//                    return DS.db.GetAll<RecuAchat>(a => true);



//                case "Role":
//                    return DS.db.GetAll<Role>(a => true);



//                case "SeriesName":
//                    return DS.db.GetAll<SeriesName>(a => true);



//                case "Stock":
//                    return DS.db.GetAll<Stock>(a => true);



//                case "Table":
//                    return DS.db.GetAll<Table>(a => true);



//                case "Taxe":
//                    return DS.db.GetAll<Taxe>(a => true);



//                case "TaxeLigne":
//                    return DS.db.GetAll<TaxeLigne>(a => true);



//                case "UniteMesure":
//                    return DS.db.GetAll<UniteMesure>(a => true);



//                case "User":
//                    return DS.db.GetAll<User>(a => true);

                     



//                case "VenteDirecte":
//                    return DS.db.GetAll<VenteDirecte>(a => true);






//                // ENDAUTO

//                default:
//                    return null;
//                    break;
//            }
//        }
//    }

//}

