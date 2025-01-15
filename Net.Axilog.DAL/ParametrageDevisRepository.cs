using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Axilog.Model.Devis;
using Net.Axilog.Model.Base;
using IBM.Data.DB2.iSeries;
using System.Diagnostics;

namespace Net.Axilog.DAL
{
    public static class ParametrageDevisRepository
    {
        public static List<RegleCalcul> ReglesCalcul { get; set; }
        public static List<Rubrique> Rubriques { get; set; }
        public static List<RubriqueImpression> RubriquesImpression { get; set; }
        public static List<TableauCalcul> TableauxCalcul { get; set; }
        public static List<TableauRubrique> TableauxRubriques { get; set; }
        public static ParametresGeneraux Parametre { get; set; }
        public static List<UniteVente> UnitesVente { get; set; }
        public static List<TypeProduit> TypesProduit { get; set; }

        public static void LoadParametresGeneraux(String _societe)
        {
            iDB2Connection cn1 = DBConnection.GetDBConnection();

            string _sql = "SELECT A.SNOMC, D.CFTYPE, D.CKFG1, D.CKFG2, D.CKFG3, D.CKFG4, " +
                        " D.CKFG5, D.CKMA1, D.CKMA2, D.CKMA3, D.CKMA4, D.CKMA5, D.CFMA, " +
                        " D.CFAD, D.CFRSTO, D.CFRFIN, D.TRP, D.TCM FROM aa0007 a join aa000y " +
                        " b on a.rsoc=b.rsoc join aa000z d on a.rsoc=d.rsoc WHERE a.rsoc=" + _societe;

            iDB2Command _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();

            iDB2DataReader read = _command.ExecuteReader();

            Parametre = new ParametresGeneraux();

            read.Read();

            Parametre.MethodeCoefficient = read.GetString(1);
            Parametre.TauxFGPapierOuMethodeB = read.GetDecimal(2);
            Parametre.TauxFGMainOeuvre = read.GetDecimal(3);
            Parametre.TauxFGMachine = read.GetDecimal(4);
            Parametre.TauxFGSousTraitance= read.GetDecimal(5);
            Parametre.TauxFGMatiere = read.GetDecimal(6);
            Parametre.TauxPVMini = read.GetDecimal(7);
            Parametre.TauxPVConseille = read.GetDecimal(8);

            Parametre.MajorationCR60 = read.GetDecimal(9);
            Parametre.MajorationCR90 = read.GetDecimal(10);
            Parametre.MajorationCR120 = read.GetDecimal(11);

            Parametre.TauxManutention = read.GetDecimal(12);
            Parametre.MontantFraisDossier = read.GetDecimal(13);
            Parametre.TauxFraisStockageParMois = read.GetDecimal(14);
            Parametre.TauxFraisFinanciersParMois = read.GetDecimal(15);

            String rat = read.GetString(16);
            String com = read.GetString(17);
            


            try
            {
                for (int i = 0; i < 10; i++)
                {
                    String tmpres = rat.Substring(i * 3, 1) + ',' + rat.Substring((i * 3) + 1, 2);
                    Parametre.ratiosPV100[i] = Decimal.Parse(tmpres);

                }
                for (int i = 0; i < 10; i++)
                {
                    String tmpres = com.Substring(i * 4, 2) + ',' + com.Substring((i * 4) + 2, 2);
                    Parametre.tauxCommission[i] = Decimal.Parse(tmpres);

                }
                
            }
            catch (Exception)
            {

                //throw;
            }
           


        }

        public static void LoadUnitesVente(String _societe)
        {
            iDB2Connection cn1 = DBConnection.GetDBConnection();

            string _sql = "SELECT trim(RARG), trim(TLIB1), TNUM FROM aa00083 " +
                       " WHERE rsoc=" + _societe + " and RTABLE='UV' and TTAG =' ' " +
                        "ORDER BY RARG ";



            iDB2Command _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();

            iDB2DataReader read = _command.ExecuteReader();

            UnitesVente = new List<UniteVente>();

            while (read.Read())
            {
                UniteVente unit = new UniteVente();
                unit.Id = read.GetString(0);
                unit.Designation = read.GetString(1);
                unit.DiviseurQuantite = read.GetInt32(2);
                unit.Autout = read.GetInt32(2) == 999999999;

                UnitesVente.Add(unit);

            }

            read.Close();
            _command.Dispose();

        }

        public static void LoadTypesProduit(String _societe)
        {
            iDB2Connection cn1 = DBConnection.GetDBConnection();

            string _sql = "SELECT RTYPD, TPLIB, trim(RCAT), TPEPAI, TPGACA, " +
                        "substr(tpopt1, 4, 1) , substr(tpopt1, 5, 1), " +
                        " VDFFXM, TPMILL, " +
                        " TPMACU , TPLBFF, TPLBNC, substr(tpopt1, 2, 1) , substr(tpopt1, 1, 1), ONCODH, VDCODH, ONNCAH, ONNCAR  FROM aa0002 " +
                       " WHERE rsoc=" + _societe + 
                        " ORDER BY Rtypd, RCAT ";



            iDB2Command _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();

            iDB2DataReader read = _command.ExecuteReader();

            TypesProduit = new List<TypeProduit>();

            while (read.Read())
            {
                TypeProduit tp = new TypeProduit();
                tp.Id = read.GetString(0);
                tp.categorie = read.GetString(2);
                tp.Designation = read.GetString(1);
                tp.SaisieEpaisseur = read.GetString(3)=="O";
                tp.CalculGacheAutomatique= read.GetString(4)=="O";
                tp.MethodeImposition = read.GetString(5);
                tp.PresentationElement = read.GetString(6);
                tp.FraisFixesMultiplies = read.GetString(7) == "O";
                tp.GacheMillePlus = read.GetString(8) == "O";
                tp.PourcentageMacule = read.GetInt32(9);
                tp.libelleFormatFini = read.GetString(10);
                tp.libelleElements = read.GetString(11);
                tp.AfficheSensEnroulement = read.GetString(12) == "O";
                tp.AfficheFormatOuvert = read.GetString(13) == "O";
                tp.AfficheCodeHauteur = read.GetString(14) == "O";
                tp.UniteHauteurParDefaut = read.GetString(15);
                tp.AfficheNombreCahier = read.GetString(16) == "O";
                tp.AfficheNombreCarbone = read.GetString(17) == "O";
                

                TypesProduit.Add(tp);

            }

            read.Close();
            _command.Dispose();

        }



        public static void LoadReglesCalcul(String _societe)
        {
            iDB2Connection cn1 = DBConnection.GetDBConnection();

            string _sql = "SELECT RREGLE, trim(CCLIB), case   " +          
                       " when CCQTEX='O' then 1         " +       
                       " when CCMLEX='O' then 2         " +
                       " when ccNTRT='O' then 4        " +         
                       " when CCNTRE='O' then 3  " +               
                       " when CCMLNT='O' then 5  " +               
                       " when CCNFP='O' then 6   " +                
                       " when cccond='O' then 7  " +                
                       " else 0 end as base,  " +                   
                       " case when ccsp='S' and ccsfmt='1' then 1 " +
                       " when ccsp='S' and ccsfmt='2' then 2    " + 
                       " when ccsp='S' and ccsfmt='3' then 3    " + 
                       " when ccsp='S' and ccsfmt='4' then 4    " + 
                       " when ccsp='S' and ccsfmt='5' then 5    " + 
                       " when ccsp='S' and ccsfmt='6' then 6    " + 
                       " else 0 end as baseSurface,              " +
                       " case when ccsp='P' and ccsfmt='1' then 1 " +
                       " when ccsp='P' and ccsfmt='2' then 2 " +
                       " when ccsp='P' and ccsfmt='3' then 3 " +
                       " when ccsp='P' and ccsfmt='4' then 4 " +
                       " when ccsp='P' and ccsfmt='5' then 5 " +
                       " when ccsp='P' and ccsfmt='6' then 6 " +
                       " else 0 end as basePoids, " +
                       "   CCFFM, CCNBR, trim(CCNBRP), CCTPU, trim(CCTPUP), " +                
                       "  CCCAD, trim(CCCADP), CCQTE, trim(CCQTEP), CCDIV, trim(CCDIVP), CCTAB , CCOMS FROM aa000e " +
                       " WHERE rsoc=" + _societe + " and ccTAG =' ' " +
                        "ORDER BY RREGLE ";



            iDB2Command _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();

            iDB2DataReader read = _command.ExecuteReader();

            ReglesCalcul = new List<RegleCalcul>();

            while (read.Read())
            {
                RegleCalcul regle = new RegleCalcul();
                regle.Id = read.GetString(0);
                regle.Nom = read.GetString(1);
                regle.typeBase = (TypeBase)read.GetInt32(2);
                regle.typeBaseSurface = (TypeBaseSurfacePoids)read.GetInt32(3);
                regle.typeBasePoids = (TypeBaseSurfacePoids)read.GetInt32(4);
                regle.ProportionnelElementsIdentiques = read.GetString(5) == "O";
                regle.UtilisationNombre = read.GetString(6) == "O";
                regle.PrecalculNombre = read.GetString(7);
                regle.UtilisationTemps = read.GetString(8) == "O";
                regle.PrecalculTemps = read.GetString(9);
                regle.UtilisationCadence = read.GetString(10) == "O";
                regle.PrecalculCadence = read.GetString(11);
                regle.UtilisationQuantite = read.GetString(12) == "O";
                regle.PrecalculQuantite = read.GetString(13);
                regle.UtilisationDiviseur = read.GetString(14) == "O";
                regle.PrecalculDiviseur = read.GetString(15);
                regle.UtilisationTableau = read.GetString(16)=="O";
                regle.TypeLigne = read.GetString(17);
                ReglesCalcul.Add(regle);

            }

            read.Close();
            _command.Dispose();

        }

        public static void LoadRubriquesImpression(String _societe)
        {
            iDB2Connection cn1 = DBConnection.GetDBConnection();

            string _sql = "SELECT a.RSECT, a.LICRO, a.LICVO, a.LICBA, LIGACH, LIGACG, LINSEQ , " +
                        " B.LITYPL, B.LIREF, B.LIREGL, B.LINBR, B.LITPS, B.LICAD, B.LIQTE, " +   
                        " B.LIDIV, B.LICOM, B.LINBRR, B.LITPSR, B.LICADR, B.LIQTER, B.LIDIVR, LIPGAV, trim(LICTABV), trim(LICTABF) " + 
                        " FROM c50002 a join c50021 b on a.rsoc=b.rsoc and a.rsect=b.rsect " +   
                        " and a.licro=b.licro and a.licvo=b.licvo and a.licba=b.licba WHERE " +
                   " a.rsoc=" + _societe + " order by a.rsect, a.licro, a.licvo, a.licba, b.linlig ";


            iDB2Command _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();

            iDB2DataReader read = _command.ExecuteReader();

            RubriquesImpression = new List<RubriqueImpression>();
            RubriqueImpression rubI = new RubriqueImpression();

            String savCRO = String.Empty;
            String savCVO = String.Empty;
            String savCBA = String.Empty;
            String savMach = String.Empty;
            bool EOFIndicator;
            
            EOFIndicator = read.Read();

            while (EOFIndicator)
            {
                rubI = new RubriqueImpression();
                rubI.machine = MachineRepository.GetMachineImpression(read.GetString(0));
                rubI.CasGeneral = read.GetString(1) == "**";
                if (!rubI.CasGeneral)
                {
                    rubI.CouleursRecto = int.Parse(read.GetString(1));
                    rubI.CouleursVerso = int.Parse(read.GetString(2));
                    rubI.CouleursBasculees = int.Parse(read.GetString(3));

                }
                rubI.GacheFixeInitiale = read.GetInt32(4);
                rubI.GacheFixeParGroupe = read.GetInt32(5);
                rubI.SequenceGache = read.GetInt32(6);

                rubI.GacheVariableEtape = read.GetDecimal(21);
                rubI.CodeRechercheTableauGacheVariable = read.GetString(22);
                rubI.CodeRechercheTableauGacheFixe = read.GetString(23);

                
                savCRO = read.GetString(1);
                savCVO = read.GetString(2);
                savCBA = read.GetString(3);
                savMach = read.GetString(0);

                while ((EOFIndicator) && (read.GetString(0) == savMach) && (read.GetString(1) == savCRO) && (read.GetString(2) == savCVO) && (read.GetString(3) == savCBA))
                {

                    ModeleLigneRubrique ligne = ModeleFactory.CreateModele(read.GetString(7));
                    if (ligne != null)
                    {
                        ligne.regle = ParametrageDevisRepository.GetRegle(read.GetString(9));
                        ligne.Nombre = read.GetDecimal(10);
                        ligne.TempsUnitaire = read.GetDecimal(11);
                        ligne.Cadence = read.GetDecimal(12);
                        ligne.Quantite = read.GetDecimal(13);
                        ligne.Diviseur = read.GetDecimal(14);
                        ligne.Commentaire = read.GetString(15);
                        
                        if (read.GetString(7) == "O")
                        {
                            try
                            {
                                (ligne as ModeleLigneRubriqueOperation).operationMachine = MachineRepository.GetOperationMachine(read.GetString(8));

                            }
                            catch (Exception)
                            {

                                Trace.TraceError(String.Concat("Opération ", read.GetString(8), " Non Trouvée"));
                            }

                        }
                        if (read.GetString(7) == "M")
                        {
                            try
                            {
                                
                                    int codeMatiere = Int32.Parse(read.GetString(8));
                                    //TODO tester le retour null pour mettre en non codé
                                    (ligne as ModeleLigneRubriqueMatiere).matiere = MatiereRepository.GetMatiere(codeMatiere);
                               

                            }
                            catch (Exception)
                            {

                                Trace.TraceError(String.Concat("Matière ", read.GetString(8), " Non Trouvée"));
                            }

                        }
                        if (read.GetString(7) == "S")
                        {
                            try
                            {
                                (ligne as ModeleLigneRubriqueSousTraitance).operationSousTraitance = SousTraitanceRepository.GetOperationSousTraitance(read.GetString(8));

                            }
                            catch (Exception)
                            {

                                Trace.TraceError(String.Concat("Sous-Traitance ", read.GetString(8), " Non Trouvée"));
                            }
                        }

                        rubI.LignesDetail.Add(ligne);
                    }



                    EOFIndicator = read.Read();
                }

                RubriquesImpression.Add(rubI);

            }
            read.Close();
            _command.Dispose();
        }

        public static void LoadRubriques(String _societe)
        {
            iDB2Connection cn1 = DBConnection.GetDBConnection();

            string _sql = "SELECT A.RRUB, A.TRLIBR, trim(A.TRQUE1), A.TRRSD1, A.TRRSF1, A.TRRVD1,  " +
                   " A.TRROB1, trim(A.TRQUE2), A.TRRSD2, A.TRRSF2, A.TRRVD2, A.TRROB2, " +
                   " trim(A.TRQUE3), A.TRRSD3, A.TRRSF3, A.TRRVD3, A.TRROB3, trim(A.TRQUE4), " +
                   " A.TRRSD4, A.TRRSF4,  A.TRRVD4,  A.TRROB4, " +
                   "  trim(A.TRQUE5),  A.TRRSD5,  A.TRRSF5,  " +
                   " A.TRRVD5,  A.TRROB5,  A.TRCRD1,  A.TRCRD2, A.TRCRD3, A.TRCRD4, A.TRCRD5," +
                   " B.LCLIEE, B.LCMACH, B.LCGACH, trim(B.LCCTABV), trim(B.LCCTABF), B.LCCAD, trim(substr(B.LCOPT1, 1, 1)) , B.LCNSEQ, B.LCPGAV, " +
                   " C.LCTYPL, C.LCREF, C.LCREGL, C.LCNBR, C.LCTPS, C.LCCAD, C.LCQTE, " +
                   " C.LCDIV,  trim(C.LCCOM),  trim(C.LCNBRR),  trim(C.LCTPSR), " +
                   "  trim(C.LCCADR),  trim(C.LCQTER),  trim(C.LCDIVR) , trim(LCPOUL) , trim(TROPEP) " +
                   "  FROM e300f a join c50001 b on " +
                   " a.rsoc =b.rsoc and a.rrub=b.rrub join c50011 c on b.rsoc=c.rsoc and " +
                   " b.rrub=c.rrub and b.lcliee=c.lcliee and b.lcmach=c.lcmach WHERE    " +
                   " a.rsoc=" + _societe + " order by a.rrub, b.lcliee, b.lcmach, c.lcnlig ";
                    

            iDB2Command _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();

            iDB2DataReader read = _command.ExecuteReader();

            Rubriques = new List<Rubrique>();
            Rubrique rub = new Rubrique();
            LienRubriqueMachine lien = new LienRubriqueMachine();
            int savId = 0;
            String savLiee=String.Empty;
            String savMach=String.Empty;
            bool EOFIndicator;

            EOFIndicator=read.Read();
            while (EOFIndicator)
            {
                rub = new Rubrique();
                rub.Id = read.GetInt32(0);
                rub.Nom = read.GetString(1);
                rub.OperationProcess = read.GetString(56);

                // question 1
                if (read.GetString(2) != String.Empty)
                {
                    rub.Question1 = new QuestionDefinition();
                    rub.Question1.Libelle = read.GetString(2);
                    rub.Question1.BorneMini = read.GetDecimal(3);
                    rub.Question1.BorneMaxi = read.GetDecimal(4);
                    rub.Question1.ReponseParDefaut = read.GetDecimal(5);
                    rub.Question1.reponseObligatoire = read.GetString(6) == "O";
                    rub.Question1.codeRecherche = read.GetString(27);
                }
                // question 2
                if (read.GetString(7) != String.Empty)
                {
                    rub.Question2 = new QuestionDefinition();
                    rub.Question2.Libelle = read.GetString(7);
                    rub.Question2.BorneMini = read.GetDecimal(8);
                    rub.Question2.BorneMaxi = read.GetDecimal(9);
                    rub.Question2.ReponseParDefaut = read.GetDecimal(10);
                    rub.Question2.reponseObligatoire = read.GetString(11) == "O";
                    rub.Question2.codeRecherche = read.GetString(28);
                }
                // question 3
                if (read.GetString(12) != String.Empty)
                {
                    rub.Question3 = new QuestionDefinition();
                    rub.Question3.Libelle = read.GetString(12);
                    rub.Question3.BorneMini = read.GetDecimal(13);
                    rub.Question3.BorneMaxi = read.GetDecimal(14);
                    rub.Question3.ReponseParDefaut = read.GetDecimal(15);
                    rub.Question3.reponseObligatoire = read.GetString(16) == "O";
                    rub.Question3.codeRecherche = read.GetString(29);
                }
                // question 4
                if (read.GetString(17) != String.Empty)
                {
                    rub.Question4 = new QuestionDefinition();
                    rub.Question4.Libelle = read.GetString(17);
                    rub.Question4.BorneMini = read.GetDecimal(18);
                    rub.Question4.BorneMaxi = read.GetDecimal(19);
                    rub.Question4.ReponseParDefaut = read.GetDecimal(20);
                    rub.Question4.reponseObligatoire = read.GetString(21) == "O";
                    rub.Question4.codeRecherche = read.GetString(30);
                }
                // question 5
                if (read.GetString(22) != String.Empty)
                {
                    rub.Question5 = new QuestionDefinition();
                    rub.Question5.Libelle = read.GetString(22);
                    rub.Question5.BorneMini = read.GetDecimal(23);
                    rub.Question5.BorneMaxi = read.GetDecimal(24);
                    rub.Question5.ReponseParDefaut = read.GetDecimal(25);
                    rub.Question5.reponseObligatoire = read.GetString(26) == "O";
                    rub.Question5.codeRecherche = read.GetString(31);
                }
                savId = rub.Id;

                while ((EOFIndicator) && (read.GetInt32(0) == savId))
                {
                    lien = new LienRubriqueMachine();
                    lien.LieMachine = read.GetString(32) == "O";
                    if (lien.LieMachine) lien.machine = MachineRepository.GetMachineImpression(read.GetString(33));
                    lien.GacheFixeEtape = read.GetInt32(34);
                    lien.CodeRechercheTableauGacheVariable = read.GetString(35);
                    lien.CodeRechercheTableauGacheFixe = read.GetString(36);
                    lien.ReponseMultiplieLaGacheFixe = read.GetString(38);
                    lien.SequenceGache = read.GetInt32(39);
                    lien.GacheVariableEtape = read.GetDecimal(40);
                    lien.TypeReductionCadence = read.GetString(55);
                    lien.ReductionCadence = read.GetInt32(37);

                    savLiee=read.GetString(32);
                    savMach=read.GetString(33);


                    while ((EOFIndicator) && (read.GetInt32(0) == savId) && (read.GetString(32) == savLiee) && (read.GetString(33) == savMach))
                    {

                        ModeleLigneRubrique ligne=ModeleFactory.CreateModele(read.GetString(41));
                        if (ligne != null)
                        {
                            ligne.regle = ParametrageDevisRepository.GetRegle(read.GetString(43));
                            ligne.Nombre = read.GetDecimal(44);
                            ligne.TempsUnitaire = read.GetDecimal(45);
                            ligne.Cadence = read.GetDecimal(46);
                            ligne.Quantite = read.GetDecimal(47);
                            ligne.Diviseur = read.GetDecimal(48);
                            ligne.Commentaire = read.GetString(49);
                            if (read.GetString(50) != String.Empty)
                            {
                                int reponse = Int32.Parse(read.GetString(50).Substring(1, 1));
                                ligne.ReponseNombre = reponse;
                            }
                            if (read.GetString(51) != String.Empty)
                            {
                                int reponse = Int32.Parse(read.GetString(51).Substring(1, 1));
                                ligne.ReponseTemps = reponse;
                            }
                            if (read.GetString(52) != String.Empty)
                            {
                                int reponse = Int32.Parse(read.GetString(52).Substring(1, 1));
                                ligne.ReponseCadence = reponse;
                            }
                            if (read.GetString(53) != String.Empty)
                            {
                                int reponse = Int32.Parse(read.GetString(53).Substring(1, 1));
                                ligne.ReponseQuantite = reponse;
                            }
                            if (read.GetString(54) != String.Empty)
                            {
                                int reponse = Int32.Parse(read.GetString(54).Substring(1, 1));
                                ligne.ReponseDiviseur = reponse;
                            }
                            if (read.GetString(41) == "O")
                            {
                                try
                                {
                                    (ligne as ModeleLigneRubriqueOperation).operationMachine = MachineRepository.GetOperationMachine(read.GetString(42));

                                }
                                catch (Exception)
                                {

                                    Trace.TraceError(String.Concat("Opération ", read.GetString(42), " Non Trouvée"));
                                }
                                
                            }
                            if (read.GetString(41) == "M")
                            {
                                try
                                {
                                    if (read.GetString(42).Substring(0, 1) == "R")
                                        (ligne as ModeleLigneRubriqueMatiere).ReponseMatiere = Int32.Parse(read.GetString(42).Substring(1, 1));
                                    else
                                    {
                                        int codeMatiere = Int32.Parse(read.GetString(42));
                                        //TODO tester le retour null pour mettre en non codé
                                        (ligne as ModeleLigneRubriqueMatiere).matiere = MatiereRepository.GetMatiere(codeMatiere);
                                    }
  
                                }
                                catch (Exception)
                                {

                                    Trace.TraceError(String.Concat("Matière " , read.GetString(42), " Non Trouvée"));
                                }

                            }
                            if (read.GetString(41) == "S")
                            {
                                try
                                {
                                    (ligne as ModeleLigneRubriqueSousTraitance).operationSousTraitance = SousTraitanceRepository.GetOperationSousTraitance(read.GetString(42));

                                }
                                catch (Exception)
                                {

                                    Trace.TraceError(String.Concat("Sous-Traitance " , read.GetString(42), " Non Trouvée"));
                                }
                            }

                            lien.LignesDetail.Add(ligne);
                        }

                        

                        EOFIndicator=read.Read();
                    }

                    rub.LiensMachine.Add(lien);
                }

                    Rubriques.Add(rub);
                
            
            }

            read.Close();

            // Chargement des réponses prédéfinies des rubriques
            _sql="SELECT RRUB, TLQUE, trim(TLLIB), TLVAL FROM e300b " +
                " WHERE rsoc=" + _societe +  
                " ORDER BY RRUB, TLQUE, TLLIG  ";
            _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();

            read = _command.ExecuteReader();

            EOFIndicator=read.Read();
            while (EOFIndicator)
            {
                rub = GetRubrique(read.GetInt32(0));
                savId = read.GetInt32(0);

                while ((EOFIndicator) && (read.GetInt32(0) == savId))
                {
                    int numeroQuestion = read.GetInt32(1);

                    if (rub != null)
                    {
                        switch (numeroQuestion)
                        {
                            case 1: rub.Question1.ValeursPossibles.Add(new ValeurPossible { Libelle = read.GetString(2), Valeur = read.GetDecimal(3) });
                                break;
                            case 2: rub.Question2.ValeursPossibles.Add(new ValeurPossible { Libelle = read.GetString(2), Valeur = read.GetDecimal(3) });
                                break;
                            case 3: rub.Question3.ValeursPossibles.Add(new ValeurPossible { Libelle = read.GetString(2), Valeur = read.GetDecimal(3) });
                                break;
                            case 4: rub.Question4.ValeursPossibles.Add(new ValeurPossible { Libelle = read.GetString(2), Valeur = read.GetDecimal(3) });
                                break;
                            case 5: rub.Question5.ValeursPossibles.Add(new ValeurPossible { Libelle = read.GetString(2), Valeur = read.GetDecimal(3) });
                                break;
                            default:
                                break;
                        }
                    }

                    EOFIndicator = read.Read();

                }

            }
            read.Close();         
            _command.Dispose();

        }

        public static void LoadTableauxCalcul(String _societe)
        {
            iDB2Connection cn1 = DBConnection.GetDBConnection();

            string _sql = "SELECT RREF, RREGLE, TMCRIX, TMCRIY, TMRCHX, TMRCHY, TMRESU, TMABS, " +
                        " TMORD, TMRES, TMSPEC, TMRESP FROM h000f " +
                       " WHERE rsoc=" + _societe;



            iDB2Command _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();

            iDB2DataReader read = _command.ExecuteReader();

            TableauxCalcul = new List<TableauCalcul>();

            while (read.Read())
            {
                TableauCalcul tab = new TableauCalcul();
                tab.regle=GetRegle(read.GetString(1));
                tab.reference = read.GetString(0);
                tab.critereX = read.GetString(2);
                tab.critereY = read.GetString(3);
                tab.modeRechercheX = read.GetString(4);
                tab.modeRechercheY = read.GetString(5);
                tab.TypeResultat = read.GetString(6);
                tab.Special = read.GetString(10) == "O";
                tab.Proportionnel = read.GetString(11) == "O";

                String abs = read.GetString(7);
                String ord = read.GetString(8);
                String res = read.GetString(9);


                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        tab.abcisses[i] = Decimal.Parse(abs.Substring(i * 9, 9));

                    }
                    for (int i = 0; i < 20; i++)
                    {
                        tab.ordonnees[i] = Decimal.Parse(ord.Substring(i * 9, 9));

                    }
                    for (int i = 0; i < 200; i++)
                    {
                        String tmpres = res.Substring(i * 8, 6) + ',' + res.Substring((i * 8) + 6, 2);
                        tab.resultats[i] = Decimal.Parse(tmpres);

                    }

                    TableauxCalcul.Add(tab);
                }
                catch (Exception)
                {
                    
                    //throw;
                }

            }

            read.Close();
            _command.Dispose();

        }

        public static void LoadTableauxRubrique(String _societe)
        {
            iDB2Connection cn1 = DBConnection.GetDBConnection();

            string _sql = "SELECT RTYPD, RTYPR, trim(RCAT), trim(RSCAT), trim(RSECT), TBCR, TBLR, TBON FROM AA000O " +
                       " WHERE rsoc=" + _societe ;



            iDB2Command _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();

            iDB2DataReader read = _command.ExecuteReader();

            TableauxRubriques = new List<TableauRubrique>();

            while (read.Read())
            {
                TableauRubrique tab = new TableauRubrique();
                tab.typeProduit = read.GetString(0);
                tab.typeTableau=read.GetString(1);
                tab.catégorie = read.GetString(2);
                tab.machine = read.GetString(4);
                
                String rub = read.GetString(5);
                String lib = read.GetString(6);
                String ouinon = read.GetString(7);


                try
                {
                    for (int i = 0; i < 68; i++)
                    {
                        tab.rubriques[i] = Int32.Parse(rub.Substring(i * 4, 4));
                        tab.libelles[i] = lib.Substring(i * 13, 13);
                        tab.dft[i] = ouinon.Substring(i, 1) == "O";

                    }
                    

                    TableauxRubriques.Add(tab);
                }
                catch (Exception)
                {

                    //throw;
                }

            }

            read.Close();
            _command.Dispose();

        }

        public static RegleCalcul GetRegle(String _code)
        {
            return ReglesCalcul.Single(e => e.Id == _code);
        }

        public static TypeProduit GetTypeProduit(String _type, String _cat)
        {
            return TypesProduit.Single(e => (e.Id == _type && e.categorie == _cat) || (e.Id == _type && e.categorie == String.Empty));
        }

        public static Rubrique GetRubrique(int _code)
        {
            try
            {
                return Rubriques.Single(e => e.Id == _code);
            }
            catch (Exception)
            {

                Trace.TraceError("Rubrique " + _code.ToString()  + " non trouvée");
                return null;
            }
        }

        public static UniteVente GetUniteVente(string _code)
        {
            try
            {
                return UnitesVente.Single(e => e.Id == _code);
            }
            catch (Exception)
            {

                Trace.TraceError("Unité de vente " + _code.ToString() + " non trouvée");
                return null;
            }
        }


        public static List<TableauCalcul> GetTableauCalcul(String _regle, String _reference)
        {
                List<TableauCalcul> tableaux= TableauxCalcul.Where(e => e.regle.Id == _regle && e.reference == _reference).ToList();

                if (tableaux.Count > 0) return tableaux;
                
                //TODO prevoir tableaux générique *** et tableaux SEU, R01, R02, R03, R04, R04 LIM, ...
           
                // recherche en tableau générique pour les lignes "Operation"
                if (GetRegle(_regle).TypeLigne == "O")
                {
                    try
                    {
                        return TableauxCalcul.Where(e => e.regle.Id == _regle && 
                            ((e.reference == _reference.Substring(0, 3) + "QTE")
                            || (e.reference == _reference.Substring(0, 3) + "CAD")
                            ||(e.reference == _reference.Substring(0, 3) + "DIV")
                            ||(e.reference == _reference.Substring(0, 3) + "***") 
                            ||(e.reference == "***" + _reference.Substring(3, 3)))).ToList();
                    }
                    catch
                    {
                        
                    }
                }
                Trace.TraceError("Tableau de calcul " + _regle + " " + _reference + " non trouvé");
                return null;
            
        }

        public static List<TableauCalcul> GetTableauxCalculPourGache(String _reference)
        {
            try
            {
                return TableauxCalcul.Where(e => e.reference.Substring(0, 3) == _reference).OrderBy(e => e.reference).ToList();
            }
            catch (Exception)
            {

                return null;
            }
        }

        public static List<Rubrique> GetRubriquesByProcess(String _opeProcess)
        {
            return Rubriques.Where(r => r.OperationProcess == _opeProcess).ToList();
        }

        public static Rubrique[] GetRubriquesByProcessAsArray(String _opeProcess)
        {
            return Rubriques.Where(r => r.OperationProcess == _opeProcess).ToArray();
        }

        public static TableauRubrique GetTableauRubrique(String _typeProduit, String _cat, String _machine, String _typTab)
        {
            try
            {
                return TableauxRubriques.Single(e => e.typeProduit == _typeProduit && e.catégorie == _cat && e.machine == _machine && e.typeTableau==_typTab);
            }
            catch (Exception)
            {

                try
                {
                    return TableauxRubriques.Single(e => e.typeProduit == _typeProduit && e.catégorie == String.Empty && e.machine == _machine && e.typeTableau == _typTab);
                }
                catch (Exception)
                {

                    try
                    {
                        return TableauxRubriques.Single(e => e.typeProduit == _typeProduit && e.catégorie == _cat && e.machine == String.Empty && e.typeTableau == _typTab);
                    }
                    catch (Exception)
                    {

                        return TableauxRubriques.Single(e => e.typeProduit == _typeProduit && e.catégorie == String.Empty && e.machine == String.Empty && e.typeTableau == _typTab);
                    }
                }
            }
        }

        public static RubriqueImpression GetRubriqueImpression(String _codeMachine, int _recto, int _verso, int _basculee)
        {
            try
            {
                return RubriquesImpression.Single(e => e.machine.Id == _codeMachine && e.CouleursRecto == _recto && e.CouleursVerso == _verso && e.CouleursBasculees == _basculee);
            }
            catch (Exception)
            {

                try
                {
                    return RubriquesImpression.Single(e => e.machine.Id == _codeMachine && e.CasGeneral == true);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

    }


}
