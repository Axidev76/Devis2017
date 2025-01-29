using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBM.Data.DB2.iSeries;
using Net.Axilog.Model.Devis;
using Net.Axilog.Model.Base;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace Net.Axilog.DAL
{
    public class DevisDB
    {
        
        //TODO Traiter provisoire/definitif pour update de la table
        public static int GetProchainNumeroDevis(String _societe)
        {
            int result = 0;

            iDB2Connection cn1 = DBConnection.GetDBConnection();
            string _sql = "select tnum from aa00082 " +
                " WHERE a.rsoc=" + _societe +
                " and rtable='ND' ";
            iDB2Command _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();
            
            iDB2DataReader read = _command.ExecuteReader();

            if (read.Read())
            {
                result=read.GetInt32(0) + 1;
            }
            else result=1;

            read.Close();
            _command.Dispose();

            return result;

        }

        //TODO renvoyer erreur si version>9
        public static int GetProchainNumeroVersion(String _societe, int _devis)
        {
            int result = 0;

            iDB2Connection cn1 = DBConnection.GetDBConnection();
            string _sql = "select max(rvers) from D5000F " +
                " WHERE a.rsoc=" + _societe + " and rdev=" + _devis +
                " and rcout='C' ";
            iDB2Command _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();

            iDB2DataReader read = _command.ExecuteReader();

            if (read.Read())
            {
                result = read.GetInt32(0) + 1;
            }
            else result = 1;

            read.Close();
            _command.Dispose();

            return result;

        }

        //TODO renvoyer erreur si hypothèse>9
        public static int GetProchainNumeroHypothese(String _societe, int _devis, int _version)
        {
            int result = 0;

            iDB2Connection cn1 = DBConnection.GetDBConnection();
            string _sql = "select max(rhypo) from D5000F " +
                " WHERE a.rsoc=" + _societe + " and rdev=" + _devis + " and rvers=" + _version +
                " and rcout='C' ";
            iDB2Command _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();

            iDB2DataReader read = _command.ExecuteReader();

            if (read.Read())
            {
                result = read.GetInt32(0) + 1;
            }
            else result = 1;

            read.Close();
            _command.Dispose();

            return result;

        }

        public static Devis GetDevisById(String _societe, int _devis, int _version, int _hypothese)
        {

            iDB2Connection cn1 = DBConnection.GetDBConnection();
           
            string _sql = "SELECT A.RGEN, A.RCLI, A.RPROSP, " +
                " CAST(digits(DEDATJ) concat '/' concat digits(DEDATM) concat '/' concat digits(DEDATA) as char(10) ccsid 1147) as DATEDEV, " +                   
                          " A.DECAT, trim(A.DEDES1), trim(A.DEDES2), A.DELRFF, A.DEHAFF, A.DE3CFF,      " +
                          " A.DETXGA, B.CMODIR, B.CNOM, B.CTELEP, C.DMODIR, C.DRS, C.DTELEP, trim(d.tlib1), d.ttypdt , DEEMBP , DEQTED , " +
                          " CRIST, CESCOM, CNBJOU, CJOULE , DEUNIT, DEPVR , DEPVRM , defrac, demois, defatl , descat , trim(e.tlib1) , decont , " +
                          " DEREPR, Inom , deopt1, deopt2, deopt3, deopt4 , denorm , derfim , detrs , mnom , cast(deddem as char(8)) , dencar" +
                          " delrfa, dehafa , trim(detxt) , dencah, deinat , deetud , deport, delieu, declal " +
                          " FROM d5000f a left outer join aa0003 b on a.rsoc=b.rsoc and     " +    
                          " a.rgen=b.rgen and a.rcli=b.rcli left outer join ax00f c on      " +    
                          " a.rsoc=c.rsoc and a.rprosp=c.rprosp " +
                          " join aa0009 d on a.rsoc=d.rsoc and a.decat=d.rcat " +
                          " join ae0003 e on a.rsoc=e.rsoc and a.decat=e.rcat and a.descat=e.rscat " +
                          " join b000f f on a.rsoc=f.rsoc and a.derepr=f.rrepr " +
                           " join b200f g on a.rsoc=g.rsoc and a.detrs=g.rtrans " +
                           " join d50002 h on a.rsoc=h.rsoc and a.rdev=h.rdev and a.rvers=h.rvers and a.rhypo=h.rhypo " +
                          " WHERE a.rsoc=" + _societe + 
                          " and a.rdev=" + _devis + " and a.rvers=" + _version + 
                          " and a.rhypo=" + _hypothese + " and a.RCOUT='C'"  ;
            
            iDB2Command _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();

            //_command.Parameters["@param1"].Value = wSociete;
            //_command.Parameters["@param2"].Value = _devis;
            //_command.Parameters["@param3"].Value = _version;
            //_command.Parameters["@param3"].Value = _hypothese;

            iDB2DataReader read = _command.ExecuteReader();
            Devis devis=new Devis();

            if (read.Read())
            {
                
                devis.Id=_devis;
                Console.WriteLine(devis.Id);
                devis.Version=_version;
                devis.Hypothese=_hypothese;
                devis.Designation = read.GetString(5);
                Console.WriteLine(devis.Designation);
                devis.Designation2 = read.GetString(6);
                devis.CollectifClient = read.GetInt32(0);
                devis.ClientId = read.GetInt32(1);
                devis.ProspectId = read.GetInt32(2);

                devis.Categorie = new CategorieProduit { Id = read.GetString(4), Designation = read.GetString(17), produit = read.GetString(18) };
                devis.souscategorie = new SousCategorieProduit { Id = read.GetString(31), Designation = read.GetString(32), categorie=devis.Categorie };
                devis.typeProduit = ParametrageDevisRepository.GetTypeProduit(read.GetString(18), read.GetString(4));
                devis.representant=new Representant(read.GetInt32(34), read.GetString(35));
                devis.fabricant = read.GetString(49);
                devis.etude = read.GetInt32(50);

                devis.typePort = read.GetString(51);
                devis.nombreLieuxLivraison = read.GetInt32(52);
                devis.enlevementAlivrer = read.GetString(53);

                //TODO Contact
                devis.contact = new Contact(read.GetString(33));

                if (devis.ProspectId > 0)
                {
                    Prospect prospect = new Prospect();
                    prospect.Id = devis.ProspectId;
                    prospect.MotDirecteur = read.GetString(14);
                    prospect.RaisonSociale = read.GetString(15);
                    prospect.Telephone = read.GetString(16);
                    prospect.TauxEscompte = 0;
                    prospect.TauxRistourne = 0;
                    prospect.Reglement = new ConditionsReglement { NombreDeJours = 0, JourEcheance = 0, FinDeMois = false, JourArrete = 0 };
                    devis.tiers = prospect;

                }
                if (devis.ClientId > 0)
                {
                    Client client = new Client();
                    client.Id = devis.ClientId;
                    client.Collectif = devis.CollectifClient;
                    client.MotDirecteur = read.GetString(11);
                    client.RaisonSociale = read.GetString(12);
                    client.Telephone = read.GetString(13);
                    client.TauxEscompte = read.GetDecimal(22);
                    client.TauxRistourne = read.GetDecimal(21);
                    client.Reglement = new ConditionsReglement { NombreDeJours = read.GetInt32(23), JourEcheance = read.GetInt32(24), FinDeMois = false, JourArrete = 0 };
                    devis.tiers = client;

                }
                devis.DateCreation = DateTime.ParseExact(read.GetString(3), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                if (read.GetString(44) != "0")
                {
                    try
                    {
                        devis.DateDemande = DateTime.ParseExact(read.GetString(44), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                     //throw;
                    }
                    
                }
                devis.nombreElements= read.GetInt32(48);
                devis.formatFini = new FormatFini(read.GetDecimal(7), read.GetDecimal(8), read.GetDecimal(9));
                devis.formatOuvert = new FormatFini(read.GetDecimal(45), read.GetDecimal(46));
                devis.GacheVariable = read.GetInt32(10);
                devis.EmballagePar = read.GetInt32(19);
                devis.Quantite = read.GetInt32(20);
                devis.uniteVente = ParametrageDevisRepository.GetUniteVente(read.GetString(25));
                devis.PVRemisEnUV = read.GetDecimal(26);
                devis.FractionLivraisons = read.GetInt32(27);
                devis.EspacementMois = read.GetInt32(28);
                devis.ModeFacturation = read.GetString(29);
                devis.transporteur = new Transporteur { Id = read.GetInt32(42), RaisonSociale = read.GetString(43) };

                devis.quantiteJuste = read.GetString(36) == "O";
                devis.productionStockee = read.GetString(37) == "O";
                devis.sensEnroulement = read.GetString(38);
                devis.enseigne = read.GetString(39);

                devis.norme = new Norme { code = read.GetString(40), nom = read.GetString(40) };
                devis.refImprime = read.GetString(41);

                devis.texteTechnique = read.GetString(47);

                
                //TODO Adresses de livraison
                //TODO Texte Libre
                //TODO Texte Technique

                // Chargement des elements
                
                _sql = "SELECT A.RELMT, DDDESI, trim(DDSORT), trim(DDSSOR), DDFOUB, DDGRAM, trim(DDCOUL), " +  
                " DDPROV, DDPAPI, DDLRFP, DDHAFP, DDCRO, DDCVO, DDCBAS, trim(DDSECT), " +    
                " DDPOSE, DDLRFT, DDHAFT, DDIDEM, DDPAGE, DDLRFO, DDHAFO, DDLRFF,  " + 
                " DDHAFF, DD3CFF, DDEPAIS, DDUNEPAI, DDQTEE, DDNBFT, DDGACV, DDNTR, " +
                " DDNTRG, DDPA,DDCON, DDUA, DDPROV, DDSENS, DDNOUT, DDNOU1, DDNOU2  FROM d60001 a " +
                " join d6001C b on a.rsoc=b.rsoc and a.rdev=b.rdev and a.rvers=b.rvers and a.rhypo=b.rhypo and a.relmt=b.relmt " +
                " WHERE a.rsoc=" + _societe + 
                          " and a.rdev=" + _devis + " and a.rvers=" + _version + 
                          " and a.rhypo=" + _hypothese + " and a.RCOUT='C' order by a.relmt"  ;
            
            _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();

            
            iDB2DataReader read2 = _command.ExecuteReader();

            while (read2.Read())
            {
                ElementDevis element = new ElementDevis(devis);
                element.Id = read2.GetInt32(0);
                element.Designation = read2.GetString(1);
                element.CouleursRecto = read2.GetInt32(11);
                element.CouleursVerso = read2.GetInt32(12);
                element.CouleursBascule = read2.GetInt32(13);
                element.ElementsIdentiques = read2.GetInt32(18);
                element.Pages = read2.GetInt32(19);
                element.Poses = read2.GetInt32(15);

                //TODO Traiter la sorte=* --> element sans papier
                
                element.support = new SupportDevis();
                element.support.Format = new FormatBase(read2.GetDecimal(9), read2.GetDecimal(10), FormatBase.UNITEMM);
                element.SansPapier = read2.GetString(2) == "*";
                if (element.SansPapier==false) element.support.sousSorte = SupportRepository.GetSousSorte(read2.GetString(2), read2.GetString(3));
                element.support.grammage = read2.GetInt32(5);
                element.support.coloris = read2.GetString(6);
                element.support.epaisseur = new Epaisseur { Longueur = read2.GetDecimal(25), Unite=read2.GetString(26) };
                element.support.NonCode = read2.GetInt32(8) == 999999;
                
                element.support.PrixAchat = read2.GetDecimal(32);
                element.support.UniteAchat = read2.GetString(34);
                element.support.ConditionnePar = read2.GetInt32(33);
                element.support.Provenance = read2.GetString(35);

                element.typeElement = read2.GetString(36);
                element.codeOutil = read2.GetInt32(37);
                element.codeOutil1 = read2.GetInt32(38);
                element.codeOutil2 = read2.GetInt32(39);


                element.formatTirage=new FormatBase(read2.GetDecimal(16), read2.GetDecimal(17), FormatBase.UNITEMM);
                element.formatOuvert = new FormatBase(read2.GetDecimal(20), read2.GetDecimal(21), FormatBase.UNITEMM);
                element.formatFini = new FormatFini(read2.GetDecimal(22), read2.GetDecimal(23), read2.GetDecimal(24));

                element.machineImpression=MachineRepository.GetMachineImpression(read2.GetString(14));
                element.SansImpression = read2.GetString(14) == "***";

                element.Quantite = read2.GetInt32(27);
                element.Refente = read2.GetInt32(28);
                element.GacheVariable = read2.GetInt32(29);

                //element.ToursUtiles = read2.GetInt32(30);
                element.ToursGacheFixe = read2.GetInt32(31);

                element.ImpressionAConstituer = false;
                element.ImpressionAValoriser = false;

                if (!element.SansImpression) element.rubriqueImpression = ParametrageDevisRepository.GetRubriqueImpression(element.machineImpression.Id, element.CouleursRecto, element.CouleursVerso, element.CouleursBascule);

                ChargeRubriquesChoisiesElement(_societe, _devis, _version, _hypothese, element);
                
                devis.Elements.Add(element);
            }

            read2.Close();
            }

                // Chargement Prepresse
            ElementDevis prepresse = new ElementDevis(devis, 31, "Pre-presse");
                        
            ChargeRubriquesChoisiesElement(_societe, _devis, _version, _hypothese, prepresse);
            devis.Prepresse=prepresse;

            // Chargement Faconnage
            ElementDevis faconnage = new ElementDevis(devis, 32, "Façonnage");
            ChargeRubriquesChoisiesElement(_societe, _devis, _version, _hypothese, faconnage);
            devis.Faconnage=faconnage;


            read.Close();
            
            _command.Dispose();

            return devis;

        }

        private static void ChargeRubriquesChoisiesElement(String _societe, int _devis, int _version, int _hypothese, ElementDevis element)
        {
            iDB2Connection cn1 = DBConnection.GetDBConnection();

            // lecture du detail de fab élement
            String _sql = "SELECT DDLIG, DDTYPL, DDREF, DDREGL, DDNBR, DDTPS, DDCADH, DDQTED, " +
            " DDDIV, DDCOMF, DDPXFP, DDPXF, DDQP, DDPRMA, DDPRMO, DDBASE, DDRSQT, " +
            " DDRSMP, DDRUBF, DDRES1, DDRES2, DDRES3, DDRES4, DDRES5, DDPGAV, " +
            " DDNSEQ, DDPERF, DDPERV, trim(substr(DDTREP, 1, 1)), trim(substr(DDTREP, 2, 1)), trim(substr(DDTREP, 3, 1)),  " +
            //" trim(substr(DDTREP, 4, 1)), trim(substr(DDTREP, 5, 1)) , DDAP , DDSEQ , case when b.dpord is null then 0 else b.dpord end as dpord , case when dpopep is null then ' ' else dpopep end as dpopep " +
            " trim(substr(DDTREP, 4, 1)), trim(substr(DDTREP, 5, 1)) , DDAP , DDSEQ , case when b.dpord is null then 0 else b.dpord end as dpord , case when dpopep is null then '9999' else trim(dpopep) end as dpopep " +
            " FROM d60002 a left outer join d60001p b on a.rsoc=b.rsoc and a.rdev=b.rdev and a.rcout=b.rcout and a.rvers=b.rvers " +
            " and a.rhypo=b.rhypo and a.relmt=b.relmt and ddordp=dpord " +
            " WHERE a.rsoc=" + _societe +
                   " and a.rdev=" + _devis + " and a.rvers=" + _version +
                   " and a.rhypo=" + _hypothese + " and a.RCOUT='C' and a.relmt=" + element.Id + " order by a.relmt, dpord, ddrubf";

            iDB2Command _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();


            iDB2DataReader read3 = _command.ExecuteReader();
            int savRub = 0;
            int savOrd = 0;
            int iOrdre = 0;
            EtapeProcess etape = new EtapeProcess();

            RubriqueChoisie rubc = new RubriqueChoisie();

            bool EOFIndicator;

            EOFIndicator=read3.Read();
            while (EOFIndicator)
            {
                etape = new EtapeProcess(element);
                etape.Etape = read3.GetString(36);
                //etape.Ordre = read3.GetInt32(35);
                //savOrd = etape.Ordre;
                iOrdre = iOrdre + 10;
                etape.Ordre = iOrdre;
                savOrd = read3.GetInt32(35);

                while ((EOFIndicator) && (read3.GetInt32(35) == savOrd))
                {

                    int tmpRub = read3.GetInt32(18);
                    if (tmpRub != savRub)
                    {
                        rubc = new RubriqueChoisie(etape);
                        rubc.rubrique = ParametrageDevisRepository.GetRubrique(tmpRub);
                        rubc.Reponses[0] = read3.GetDecimal(19);
                        rubc.Reponses[1] = read3.GetDecimal(20);
                        rubc.Reponses[2] = read3.GetDecimal(21);
                        rubc.Reponses[3] = read3.GetDecimal(22);
                        rubc.Reponses[4] = read3.GetDecimal(23);
                        rubc.SequenceGache = read3.GetInt32(25);

                        rubc.TauxGacheVariable = read3.GetDecimal(24);

                        // recherche du lien machine pour info de cadence et gache
                        if (tmpRub != 0)
                        {
                            try
                            {

                                rubc.lienMachineUtilise = rubc.rubrique.LiensMachine.Single(x => x.LieMachine == true && x.machine == element.machineImpression);
                            }
                            catch (InvalidOperationException)
                            {
                                rubc.lienMachineUtilise = rubc.rubrique.LiensMachine.Single(x => (x.LieMachine == false));
                            }

                            
                            if (rubc.lienMachineUtilise != null)
                            {
                                rubc.ReductionCadence = rubc.lienMachineUtilise.ReductionCadence;
                                rubc.typeReductionCadence = rubc.lienMachineUtilise.TypeReductionCadence;
                                rubc.LieMachine = rubc.lienMachineUtilise.LieMachine;

                            }
                            etape.RubriquesChoisies.Add(rubc);
                        }
                        
                        savRub = tmpRub;

                    }
                    
                    while ((EOFIndicator) && (read3.GetInt32(35) == savOrd) && (read3.GetInt32(18) == savRub))
                    {

                        

                        LigneFabrication ligne = LigneFabricationFactory.CreateLigneFabrication(element, read3.GetString(1));

                        if (ligne != null)
                        {
                            ligne.BaseCalcul = read3.GetDecimal(15);
                            ligne.BaseCalculMP = 0.0M;
                            ligne.Nombre = read3.GetDecimal(4);
                            ligne.TempsUnitaire = read3.GetDecimal(5);
                            ligne.Cadence = read3.GetDecimal(6);
                            ligne.Quantite = read3.GetDecimal(7);
                            ligne.Diviseur = read3.GetDecimal(8);
                            ligne.Commentaire = read3.GetString(9);
                            ligne.Resultat = read3.GetDecimal(16);
                            ligne.ResultatMP = read3.GetDecimal(17);
                            ligne.regle = ParametrageDevisRepository.GetRegle(read3.GetString(3));

                            try
                            {
                                if (read3.GetString(28) != String.Empty) ligne.ReponseNombre = int.Parse(read3.GetString(28));
                                else ligne.ReponseNombre = 0;
                            }
                            catch
                            {
                                ligne.ReponseNombre = 0;
                            }

                            try
                            {
                                if (read3.GetString(29) != String.Empty) ligne.ReponseTemps = int.Parse(read3.GetString(29));
                                else ligne.ReponseTemps = 0;
                            }
                            catch
                            {
                                ligne.ReponseTemps = 0;
                            }

                            try
                            {
                                if (read3.GetString(30) != String.Empty) ligne.ReponseCadence = int.Parse(read3.GetString(30));
                                else ligne.ReponseCadence = 0;
                            }
                            catch
                            {
                                ligne.ReponseCadence = 0;
                            }

                            try
                            {
                                if (read3.GetString(31) != String.Empty) ligne.ReponseQuantite = int.Parse(read3.GetString(31));
                                else ligne.ReponseQuantite = 0;
                            }
                            catch
                            {
                                ligne.ReponseQuantite = 0;
                            }
                            try
                            {
                                if (read3.GetString(32) != String.Empty) ligne.ReponseDiviseur = int.Parse(read3.GetString(32));
                                else ligne.ReponseDiviseur = 0;
                            }
                            catch
                            {
                                ligne.ReponseDiviseur = 0;
                            }


                            if (read3.GetString(1) == "O")
                            {
                                (ligne as LigneFabricationOperation).operationMachine = MachineRepository.GetOperationMachine(read3.GetString(2));

                            }
                            if (read3.GetString(1) == "M")
                            {
                                (ligne as LigneFabricationMatiere).matiere = MatiereRepository.GetMatiere(read3.GetString(2));
                                (ligne as LigneFabricationMatiere).PrixAchat = read3.GetDecimal(10);
                                if ((ligne as LigneFabricationMatiere).matiere == null)
                                    (ligne as LigneFabricationMatiere).Coefficient = 1;
                                else
                                    (ligne as LigneFabricationMatiere).Coefficient = (ligne as LigneFabricationMatiere).matiere.Coefficient;
                                //TODO Traiter le libellé non codé

                            }
                            if (read3.GetString(1) == "S")
                            {
                                (ligne as LigneFabricationSousTraitance).operationSousTraitance = SousTraitanceRepository.GetOperationSousTraitance(read3.GetString(2));
                                (ligne as LigneFabricationSousTraitance).PrixFixe = read3.GetDecimal(10);
                                (ligne as LigneFabricationSousTraitance).PrixProp = read3.GetDecimal(11);
                                (ligne as LigneFabricationSousTraitance).QuantiteProp = read3.GetDecimal(12);
                                //TODO Traiter le non codé

                            }

                            ligne.Sequence = read3.GetInt32(34);

                            if ((ligne.regle.DependantQuantite) || (ligne.regle.DependantFormat)) element.PropertyChanged += ligne.ElementDevis_PropertyChanged;

                            if ((tmpRub == 0) && (read3.GetString(33) == "I"))
                                element.LignesImpression.Add(ligne);
                            else rubc.LignesFabrication.Add(ligne);
                        }
                        EOFIndicator = read3.Read();
                    }
                }
                element.EtapesProcess.Add(etape);
            }
            read3.Close();
        }

        public static void SaveDevis(String _societe, Devis _dev)
        {
            iDB2Connection cn1 = DBConnection.GetDBConnection();
            //TODO sauvegarde entete (insert ou update selon IsNew)

                       

            // suppression du detail devis si modification
            if (!_dev.IsNew)
            {
                String _sql = "Delete from d60001 " +
                " WHERE rsoc=" + _societe +
                      " and rdev=" + _dev.Id + " and rvers=" + _dev.Version +
                      " and rhypo=" + _dev.Hypothese + " and RCOUT='C'";

                iDB2Command _command = new iDB2Command(_sql, cn1);
               // _command.ExecuteNonQuery();

                _sql = "Delete from d60001P " +
                " WHERE rsoc=" + _societe +
                       " and rdev=" + _dev.Id + " and rvers=" + _dev.Version +
                       " and rhypo=" + _dev.Hypothese + " and RCOUT='C'";

                _command = new iDB2Command(_sql, cn1);
                _command.ExecuteNonQuery();

                _sql = "Delete from d60002 " +
                " WHERE rsoc=" + _societe +
                       " and rdev=" + _dev.Id + " and rvers=" + _dev.Version +
                       " and rhypo=" + _dev.Hypothese + " and RCOUT='C'";

                _command = new iDB2Command(_sql, cn1);
                _command.ExecuteNonQuery();
                                
            }
            
            //TODO boucle de sauvegarde des éléments 
            foreach (var elem in _dev.Elements)
            {
                SaveElement(_societe, elem);
            }
            SaveElement(_societe, _dev.Prepresse);
            SaveElement(_societe, _dev.Faconnage);
        }

        public static void SaveElement(String _societe, ElementDevis _elem)
        {
            iDB2Connection cn1 = DBConnection.GetDBConnection();

            string reqD60001Insert = "INSERT INTO D60001 (RSOC, RCOUT, RDEV, " +
                       " RVERS, RHYPO, RELMT, DDDESI, DDSORT, DDSSOR, DDGRAM, DDCOUL , DDBCK , " +
                       " DDPROV, DDPAPI, DDLRFP, DDHAFP, DDCRO, DDCVO, DDCBAS, " +
                       " DDSECT, DDPOSE, DDLRFT, DDHAFT, DDIDEM, DDPAGE, DDGACV, " +
                       " DDLRFO, DDHAFO, DDLRFF, DDHAFF, DDQTEE, DDFRN, DDUA, DDDA, DDPA, " +
                       " DDNBFT, DDPDSP, DDNTR, DDNTRM, DDNTRG, DDNML, DDVIR, DDLRFA,  " +
                       " DDHAFA, DDCON, DDEPAIS, DDUNEPAI) VALUES";

            //TODO BCK, FOUB, code papier, DDPLI; DDFRN, DDDA, DDNTRM, ddlrfa, ddhafa, ddvir
    /*         string reqD60001Values = String.Format("({0}, '{1}', {2}, {3], {4}, '{5}', '{6}', '{7}', {8}, '{9}', '{10}', '{11}', {12}, {13}, {14}, " +
                                   " {15}, {16}, {17}, '{18}', {19} , {20}, {21}, {22}, {23}, {24}, {25}, " +
                                    " {26}, {27}, {28}, {29}, {30}, {31}, {32}, {33}, {34}, {35}, {36}, " +
                                    " {37}, {38}, {39}, {40}, '{41}', {42}, {43}, {44}, {45}, '{46}' ) ",
                         _societe, "C", _elem.devis.Id,
                         _elem.devis.Version, _elem.devis.Hypothese, _elem.Id, _elem.Designation , _elem.support.sousSorte.sorte.code, _elem.support.sousSorte.code, 
                         _elem.support.grammage, _elem.support.coloris, _elem.support.BCK , _elem.support.Provenance , 999999 , _elem.support.Format.Largeur.ToString("F", CultureInfo.InvariantCulture) ,
                        _elem.support.Format.Hauteur.ToString("F", CultureInfo.InvariantCulture) , _elem.CouleursRecto, _elem.CouleursVerso, _elem.CouleursBascule,
                        _elem.machineImpression.Id, _elem.Poses, _elem.formatTirage.Largeur.ToString("F", CultureInfo.InvariantCulture), _elem.formatTirage.Hauteur.ToString("F", CultureInfo.InvariantCulture),
                        _elem.ElementsIdentiques, _elem.Pages, (int)_elem.GacheVariable , _elem.formatOuvert.Largeur.ToString("F", CultureInfo.InvariantCulture),
                        _elem.formatOuvert.Hauteur.ToString("F", CultureInfo.InvariantCulture), _elem.formatFini.Largeur.ToString("F", CultureInfo.InvariantCulture),
                        _elem.formatFini.Hauteur.ToString("F", CultureInfo.InvariantCulture), _elem.Quantite, 0, _elem.support.UniteAchat, 0, _elem.support.PrixAchat.ToString("F", CultureInfo.InvariantCulture),
                        _elem.Refente, _elem.PoidsAchete.ToString("F", CultureInfo.InvariantCulture), _elem.ToursUtiles, 0, _elem.ToursGacheFixe, _elem.MetrageUtile.ToString("F", CultureInfo.InvariantCulture), "N",
                        0, 0, _elem.support.ConditionnePar, _elem.support.epaisseur.Longueur, _elem.support.epaisseur.Unite);
                        
            reqD60001Insert = String.Concat(reqD60001Insert, reqD60001Values);
 
            iDB2Command _command3 = new iDB2Command(reqD60001Insert, cn1);
            _command3.ExecuteNonQuery(); */


            string reqD60001PInsert = "INSERT INTO D60001P (RSOC, RCOUT, RDEV, " +
                                    " RVERS, RHYPO, RELMT, DPORD, DPOPEP) VALUES";

            string reqD60002Insert = "INSERT INTO D60002 (RSOC, RCOUT, RDEV, " +
                    " RVERS, RHYPO, RELMT, DDLIG, DDTYPL, DDREF, DDREGL, DDNBR, DDTPS, " +
                    " DDCADH, DDQTED, DDDIV, DDCOMF, DDPXFP, DDPXF, DDQP, DDPRMA, DDPRMO, " +
                    " DDBASE, DDRSQT, DDRSMP, DDVALO, DDLBRF, DDSEQ, DDCOEF, DDATEL,  " +
                    " DDRUBF, DDRES1, DDRES2, DDRES3, DDRES4, DDRES5, DDTREP, DDPGAV, " +
                    " DDNSEQ, DDPERF, DDPERV, DDORDP, DDAP) VALUES";

            int ctrD60001P = 1;
            int ctrD60002 = 1;

            //TODO Traiter la rubrique d'impression
            foreach (var ligne in _elem.LignesImpression)
            {

                if (ctrD60002 > 1) reqD60002Insert = String.Concat(reqD60002Insert, " , ");

                String sDesignation;
                Decimal dPrixFixe = 0.0M;
                Decimal dPrixProp = 0.0M;
                Decimal dQuantiteProp = 0.0M;
                Decimal dTauxHoraireMachine = 0.0M;
                Decimal dTauxHoraireMainDOeuvre = 0.0M;
                Decimal dCoeff = 0.0M;

                if (ligne.GetDesignationReference().Length > 30)
                    sDesignation = ligne.GetDesignationReference().Substring(0, 30);
                else
                    sDesignation = ligne.GetDesignationReference();

                if (ligne is LigneFabricationOperation)
                {
                    dTauxHoraireMachine = (ligne as LigneFabricationOperation).operationMachine.TauxHoraireMachine;
                    dTauxHoraireMainDOeuvre = (ligne as LigneFabricationOperation).operationMachine.TauxHoraireMO;

                }

                if (ligne is LigneFabricationMatiere)
                {
                    dPrixFixe = (ligne as LigneFabricationMatiere).PrixAchat;
                    dCoeff = (ligne as LigneFabricationMatiere).Coefficient;

                }

                if (ligne is LigneFabricationSousTraitance)
                {
                    dPrixFixe = (ligne as LigneFabricationSousTraitance).PrixFixe;
                    dPrixProp = (ligne as LigneFabricationSousTraitance).PrixProp;
                    dQuantiteProp = (ligne as LigneFabricationSousTraitance).QuantiteProp;
                }

                string reqD60002Values = String.Format("({0}, '{1}', {2}, {3}, {4}, {5}, {6}, '{7}', '{8}', '{9}', {10}, {11}, {12}, {13}, {14}, " +
                           " '{15}', {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, '{25}', " +
                            " {26}, {27}, '{28}', {29}, {30}, {31}, {32}, {33}, {34}, {35}, {36}, " +
                            " {37}, {38}, {39}, {40}, {41}) ",
                 _societe, "C", _elem.devis.Id,
                 _elem.devis.Version, _elem.devis.Hypothese, _elem.Id, ctrD60002++, ligne.GetTypeLigne(), ligne.GetReference(), ligne.regle.Id,
                 ligne.Nombre.ToString("F", CultureInfo.InvariantCulture), ligne.TempsUnitaire.ToString("F", CultureInfo.InvariantCulture), (int)ligne.Cadence, ligne.Quantite.ToString("F", CultureInfo.InvariantCulture), ligne.Diviseur, ligne.Commentaire,
                 dPrixFixe.ToString("F", CultureInfo.InvariantCulture), dPrixProp.ToString("F", CultureInfo.InvariantCulture), dQuantiteProp.ToString("F", CultureInfo.InvariantCulture), dTauxHoraireMachine.ToString("F", CultureInfo.InvariantCulture), dTauxHoraireMainDOeuvre.ToString("F", CultureInfo.InvariantCulture),
                 ligne.BaseCalcul.ToString("F", CultureInfo.InvariantCulture), ligne.Resultat.ToString("F", CultureInfo.InvariantCulture), ligne.ResultatMP.ToString("F", CultureInfo.InvariantCulture),
                 ligne.Valeur.ToString("F", CultureInfo.InvariantCulture), sDesignation, ligne.Sequence, dCoeff.ToString("F", CultureInfo.InvariantCulture), ligne.CodeTri.Substring(0, 1),
                 0, 0, 0, 0, 0, 0,
                 0 ,
                 0, 0, 0, 0, 0, 'I');
                reqD60002Insert = String.Concat(reqD60002Insert, reqD60002Values);
            }


            foreach (var etape in _elem.EtapesProcess.OrderBy(e => e.Ordre))
            {
                //List<LigneFabrication> detail = new List<LigneFabrication>();
                if (ctrD60001P > 1) reqD60001PInsert = String.Concat(reqD60001PInsert, ", ");

                string reqD60001PValues = String.Format("({0}, '{1}', {2}, {3}, {4}, {5}, {6}, '{7}' ) ",
                            _societe, "C", _elem.devis.Id,
                         _elem.devis.Version, _elem.devis.Hypothese, _elem.Id, etape.Ordre, etape.Etape);

                reqD60001PInsert = String.Concat(reqD60001PInsert, reqD60001PValues);

                ctrD60001P++;

                               

                 
                foreach (var rubc in etape.RubriquesChoisies)
                {
                    //detail.AddRange(rubc.LignesFabrication.Where(e => e.Resultat > 0));
                    

                    foreach (var ligne in rubc.LignesFabrication)
                    {

                        if (ctrD60002 > 1) reqD60002Insert = String.Concat(reqD60002Insert, " , ");

                        String sDesignation;
                        Decimal dPrixFixe = 0.0M;
                        Decimal dPrixProp = 0.0M;
                        Decimal dQuantiteProp = 0.0M;
                        Decimal dTauxHoraireMachine=0.0M;
                        Decimal dTauxHoraireMainDOeuvre = 0.0M;
                        Decimal dCoeff = 0.0M;

                        if (ligne.GetDesignationReference().Length > 30)
                            sDesignation = ligne.GetDesignationReference().Substring(0, 30);
                        else
                            sDesignation = ligne.GetDesignationReference();

                        if (ligne is LigneFabricationOperation)
                        {
                            dTauxHoraireMachine = (ligne as LigneFabricationOperation).operationMachine.TauxHoraireMachine;
                            dTauxHoraireMainDOeuvre = (ligne as LigneFabricationOperation).operationMachine.TauxHoraireMO;

                        }

                        if (ligne is LigneFabricationMatiere)
                        {
                            dPrixFixe = (ligne as LigneFabricationMatiere).PrixAchat;
                            dCoeff = (ligne as LigneFabricationMatiere).Coefficient;

                        }

                        if (ligne is LigneFabricationSousTraitance)
                        {
                            dPrixFixe = (ligne as LigneFabricationSousTraitance).PrixFixe;
                            dPrixProp = (ligne as LigneFabricationSousTraitance).PrixProp;
                            dQuantiteProp = (ligne as LigneFabricationSousTraitance).QuantiteProp;
                        }

                        string reqD60002Values = String.Format("({0}, '{1}', {2}, {3}, {4}, {5}, {6}, '{7}', '{8}', '{9}', {10}, {11}, {12}, {13}, {14}, " +
                                   " '{15}', {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, '{25}', " +
                                    " {26}, {27}, '{28}', {29}, {30}, {31}, {32}, {33}, {34}, {35}, {36}, " +
                                    " {37}, {38}, {39}, {40}, {41}) ",
                         _societe, "C", _elem.devis.Id,
                         _elem.devis.Version, _elem.devis.Hypothese, _elem.Id, ctrD60002++, ligne.GetTypeLigne(), ligne.GetReference(), ligne.regle.Id,
                         ligne.Nombre.ToString("F", CultureInfo.InvariantCulture), ligne.TempsUnitaire.ToString("F", CultureInfo.InvariantCulture), (int)ligne.Cadence, ligne.Quantite.ToString("F", CultureInfo.InvariantCulture), ligne.Diviseur, ligne.Commentaire,
                         dPrixFixe.ToString("F", CultureInfo.InvariantCulture), dPrixProp.ToString("F", CultureInfo.InvariantCulture), dQuantiteProp.ToString("F", CultureInfo.InvariantCulture), dTauxHoraireMachine.ToString("F", CultureInfo.InvariantCulture), dTauxHoraireMainDOeuvre.ToString("F", CultureInfo.InvariantCulture), 
                         ligne.BaseCalcul.ToString("F", CultureInfo.InvariantCulture), ligne.Resultat.ToString("F", CultureInfo.InvariantCulture), ligne.ResultatMP.ToString("F", CultureInfo.InvariantCulture),
                         ligne.Valeur.ToString("F", CultureInfo.InvariantCulture), sDesignation, ligne.Sequence, dCoeff.ToString("F", CultureInfo.InvariantCulture), ligne.CodeTri.Substring(0, 1),
                         rubc.rubrique.Id, rubc.Reponses[0].ToString("F", CultureInfo.InvariantCulture), rubc.Reponses[1].ToString("F", CultureInfo.InvariantCulture), rubc.Reponses[2].ToString("F", CultureInfo.InvariantCulture), rubc.Reponses[3].ToString("F", CultureInfo.InvariantCulture), rubc.Reponses[4].ToString("F", CultureInfo.InvariantCulture),
                         ligne.ReponseNombre.ToString() + ligne.ReponseTemps.ToString() + ligne.ReponseCadence.ToString() + ligne.ReponseQuantite.ToString() + ligne.ReponseDiviseur.ToString(),
                         rubc.TauxGacheVariable.ToString("F", CultureInfo.InvariantCulture), rubc.SequenceGache, rubc.lienMachineUtilise.GacheFixeEtape, rubc.lienMachineUtilise.GacheVariableEtape.ToString("F", CultureInfo.InvariantCulture), etape.Ordre, 'P');
                        reqD60002Insert = String.Concat(reqD60002Insert, reqD60002Values);
                    }

                   

                }

                

            }

            if (ctrD60002 > 1)
            {
                iDB2Command _command2 = new iDB2Command(reqD60002Insert, cn1);
                _command2.ExecuteNonQuery();
            }

            if (ctrD60001P > 1)
            {
                iDB2Command _command = new iDB2Command(reqD60001PInsert, cn1);
                _command.ExecuteNonQuery();
            }
            

        }
    }
}
