using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Axilog.DAL;
using Net.Axilog.Model.Devis;
using Net.Axilog.Model.Base;
using System.Diagnostics;

namespace Net.Axilog.BLL
{
    public class DevisBLL
    {
        /// <summary>Récupère le prochain numéro de devis 
        /// </summary>
        public static int GetProchainNumeroDevis()
        {
            String _societe;
            AppService.GlobalVariables.TryGetValue("societe", out _societe);
            return DevisDB.GetProchainNumeroDevis(_societe);
        }

        /// <summary>Récupère un devis selon son numéro, sa version, son hypothèse
        /// </summary>
        public static Devis GetDevisById(int _devis, int _version, int _hypothese)
        {
            String _societe;
            AppService.GlobalVariables.TryGetValue("societe", out _societe);
            return DevisDB.GetDevisById(_societe, _devis, _version, _hypothese);
        }

        /// <summary>permet d'ajouter une nouvelle étape à l'élément
        /// </summary>
        public static EtapeProcess AddEtape(ElementDevis _elem, int _ord, String _codeEtape)
        {
            EtapeProcess etape = new EtapeProcess(_elem);
            etape.Etape = _codeEtape;
            etape.Ordre = _ord;
            _elem.EtapesProcess.Add(etape);
            return etape;

        }

        /// <summary>permet d'inserer une nouvelle étape à l'élément
        /// </summary>
        public static EtapeProcess InsereEtapeAvant(ElementDevis _elem, EtapeProcess _etape, int _ord, String _codeEtape)
        {
            EtapeProcess etape = new EtapeProcess(_elem);
            etape.Etape = _codeEtape;
            etape.Ordre = _ord;
            _elem.EtapesProcess.Insert(_elem.EtapesProcess.IndexOf(_etape), etape);
            return etape;

        }

        
        /// <summary>permet de supprimer une étape complète
        /// </summary>
        public static void DeleteEtape(EtapeProcess _etape)
        {
            foreach (var rubc in _etape.RubriquesChoisies)
            {
                rubc.LignesFabrication.Clear();
                              
            }
            _etape.element.CadenceARecalculer = true;
            _etape.element.EtapesProcess.Remove(_etape);
            _etape = null;

        }


        /// <summary>permet d'ajouter une nouvelle rubrique à l'etape process
        /// </summary>
        public static void AddRubriqueToEtape(EtapeProcess _etape, int _id)
        {
            if (_etape.RubriquesChoisies.Any(rubch => rubch.rubrique.Id == _id)) return;

            RubriqueChoisie rubc = new RubriqueChoisie(_etape);
            if (rubc != null)
            {
                rubc.rubrique = ParametrageDevisBLL.GetRubrique(_id);
                rubc.AValoriser = true;
                rubc.DetailAConstituer = true;
                rubc.ReponsesAdonner = false;


                //if ((rubc.rubrique.Question1 == null) && (rubc.rubrique.Question2 == null) && (rubc.rubrique.Question3 == null) && (rubc.rubrique.Question4 == null) && (rubc.rubrique.Question5 == null)) rubc.ReponsesAdonner = false;
                if (rubc.rubrique != null)
                {

                    if (((rubc.rubrique.Question1) != null) && (rubc.rubrique.Question1.ReponseParDefaut > 0)) rubc.Reponses[0] = rubc.rubrique.Question1.ReponseParDefaut;
                    if (((rubc.rubrique.Question2) != null) && (rubc.rubrique.Question2.ReponseParDefaut > 0)) rubc.Reponses[1] = rubc.rubrique.Question2.ReponseParDefaut;
                    if (((rubc.rubrique.Question3) != null) && (rubc.rubrique.Question3.ReponseParDefaut > 0)) rubc.Reponses[2] = rubc.rubrique.Question3.ReponseParDefaut;
                    if (((rubc.rubrique.Question4) != null) && (rubc.rubrique.Question4.ReponseParDefaut > 0)) rubc.Reponses[3] = rubc.rubrique.Question4.ReponseParDefaut;
                    if (((rubc.rubrique.Question5) != null) && (rubc.rubrique.Question5.ReponseParDefaut > 0)) rubc.Reponses[4] = rubc.rubrique.Question5.ReponseParDefaut;

                    if (((rubc.rubrique.Question1) != null) && (rubc.rubrique.Question1.reponseObligatoire) && (rubc.Reponses[0] == 0)) rubc.ReponsesAdonner = true;
                    if (((rubc.rubrique.Question2) != null) && (rubc.rubrique.Question2.reponseObligatoire) && (rubc.Reponses[1] == 0)) rubc.ReponsesAdonner = true;
                    if (((rubc.rubrique.Question3) != null) && (rubc.rubrique.Question3.reponseObligatoire) && (rubc.Reponses[2] == 0)) rubc.ReponsesAdonner = true;
                    if (((rubc.rubrique.Question4) != null) && (rubc.rubrique.Question4.reponseObligatoire) && (rubc.Reponses[3] == 0)) rubc.ReponsesAdonner = true;
                    if (((rubc.rubrique.Question5) != null) && (rubc.rubrique.Question5.reponseObligatoire) && (rubc.Reponses[4] == 0)) rubc.ReponsesAdonner = true;


                    // Recherche du lien machine
                    try
                    {
                        rubc.lienMachineUtilise = rubc.rubrique.LiensMachine.Single(x => (x.LieMachine == false)
                                       || (x.LieMachine == true && x.machine == _etape.element.machineImpression));
                    }
                    catch (Exception)
                    {

                        Trace.TraceError(String.Concat("Lien Machine pour la Rubrique ", _id.ToString(), " non trouvé"));
                    }
                    if (rubc.lienMachineUtilise != null)
                    {
                        rubc.ReductionCadence = rubc.lienMachineUtilise.ReductionCadence;
                        rubc.typeReductionCadence = rubc.lienMachineUtilise.TypeReductionCadence;
                        rubc.LieMachine = rubc.lienMachineUtilise.LieMachine;
                        if ((rubc.typeReductionCadence != String.Empty) && (rubc.ReductionCadence > 0)) _etape.element.CadenceARecalculer = true;
                    }

                    _etape.RubriquesChoisies.Add(rubc);
                }
            }
        }

        /// <summary>permet de completer les réponses aux questions de cette rubrique. Les réponses sont passées en décimal
        /// </summary>
        public static void AddReponsesToRubrique(EtapeProcess _etape, int _idrub, decimal[] _reponses)

        {
            try
            {
                RubriqueChoisie rubc = _etape.RubriquesChoisies.Single(rubch => rubch.rubrique.Id == _idrub);

                // on compare les anciennes réponses avec les nouvelles
                // si elles ont changé , on supprime le détail de la rubrique et on le recrée
                for (int i = 0; i < 5; i++)
                {
                    if (_reponses[i] != rubc.Reponses[i]) rubc.DetailAConstituer = true;
                }

                rubc.Reponses = _reponses;
                rubc.ReponsesAdonner = false;

                //if (((rubc.rubrique.Question1) != null) && (rubc.rubrique.Question1.reponseObligatoire) && (rubc.Reponses[0] == 0)) rubc.ReponsesAdonner = true;
                //if (((rubc.rubrique.Question2) != null) && (rubc.rubrique.Question2.reponseObligatoire) && (rubc.Reponses[1] == 0)) rubc.ReponsesAdonner = true;
                //if (((rubc.rubrique.Question3) != null) && (rubc.rubrique.Question3.reponseObligatoire) && (rubc.Reponses[2] == 0)) rubc.ReponsesAdonner = true;
                //if (((rubc.rubrique.Question4) != null) && (rubc.rubrique.Question4.reponseObligatoire) && (rubc.Reponses[3] == 0)) rubc.ReponsesAdonner = true;
                //if (((rubc.rubrique.Question5) != null) && (rubc.rubrique.Question5.reponseObligatoire) && (rubc.Reponses[4] == 0)) rubc.ReponsesAdonner = true;



                if (rubc.DetailAConstituer) rubc.LignesFabrication.Clear();
            }
            catch (Exception)
            {
                Trace.TraceError(String.Concat("Rubrique ", _idrub.ToString(), " non trouvée"));
            }

        }

        /// <summary>permet de completer les réponses aux questions de cette rubrique. Les réponses sont passées en chaine de caractère
        /// </summary>
        public static void AddReponsesToRubrique(EtapeProcess _etape, int _idrub, String[] _reponses)
        {
            //RubriqueChoisie rubc = _elmt.RubriquesChoisies.Single(rubch => rubch.rubrique.Id == _idrub);
            decimal[] dreponses = new decimal[5];
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    if (_reponses[i] != String.Empty) dreponses[i] = decimal.Parse(_reponses[i]);
                }
                catch (Exception)
                {
                    Trace.TraceError(String.Concat("Certaines réponses à la rubrique ", _idrub.ToString(), " sont incorrectes"));
                    return;
                }

            }

            AddReponsesToRubrique(_etape, _idrub, dreponses);

        }

        /// <summary>permet de marquer le code rubrique à l'état "A supprimer"
        /// </summary>
        public static void DeleteRubriqueFromEtape(EtapeProcess _etape, int _rub)
        {
            try
            {
                RubriqueChoisie rubc = _etape.RubriquesChoisies.Single(rubch => rubch.rubrique.Id == _rub);
                rubc.ASupprimer = true;
                if ((rubc.typeReductionCadence != String.Empty) && (rubc.ReductionCadence > 0)) _etape.element.CadenceARecalculer = true;
            }
            catch (Exception)
            {

                //throw;
            }
        }


        /// <summary>permet de constituer et valoriser le detail de fabrication de l'élément passé en parametre
        /// </summary>
        public static void TraiteElement(ElementDevis _elmt)
        {
            

            // constitution de la rubrique d'impression si besoin
            if (_elmt.ImpressionAConstituer)
            {

                for (int i = _elmt.LignesImpression.Count() - 1; i >= 0; i--)
                {

                    _elmt.LignesImpression[i] = null;
                    _elmt.LignesImpression.RemoveAt(i);
                }


                if (_elmt.SansImpression == false)
                {

                    _elmt.rubriqueImpression = ParametrageDevisBLL.GetRubriqueImpression(_elmt.machineImpression.Id, _elmt.CouleursRecto, _elmt.CouleursVerso, _elmt.CouleursBascule);
                    if (_elmt.rubriqueImpression != null)
                    {
                        foreach (ModeleLigneRubrique modele in _elmt.rubriqueImpression.LignesDetail)
                        {
                            LigneFabrication ligne = LigneFabricationFactory.CreateLigneFabrication(_elmt, modele);
                            _elmt.LignesImpression.Add(ligne);
                        }
                        _elmt.ImpressionAValoriser = true;
                    }
                }
            }


            // suppression des rubriques marquées
            foreach (var etape in _elmt.EtapesProcess)
            {
                etape.RubriquesChoisies.RemoveAll(x => x.ASupprimer == true);
            }


            if (_elmt.CadenceARecalculer)
            {
                int wPerte = 0;
                decimal wPourcentCumul = 100.0M;
                decimal wPourcentLimite = 0.0M;
                int wCadenceMaxi = 999999;

                List<RubriqueChoisie> tmpRubC = new List<RubriqueChoisie>();

                foreach (var etape in _elmt.EtapesProcess)
                {
                    tmpRubC.Concat(etape.RubriquesChoisies.Where(l => l.ReductionCadence > 0));
                }

                foreach (var item in tmpRubC
                          .Select(lg =>
                                new
                                {
                                    TypeReductionCadence = lg.typeReductionCadence,
                                    ReductionCadence = lg.ReductionCadence
                                }))
                {
                    switch (item.TypeReductionCadence)
                    {
                        case "P":
                            wPerte += item.ReductionCadence;
                            break;
                        case "R":
                            wPourcentCumul = wPourcentCumul * item.ReductionCadence / 100.0M;
                            break;
                        case "S":
                            if (wPourcentLimite < item.ReductionCadence) wPourcentLimite = item.ReductionCadence;
                            break;
                        case "L":
                            if (item.ReductionCadence < wCadenceMaxi) wCadenceMaxi = item.ReductionCadence;
                            break;
                        default:
                            break;

                    }

                }

                foreach (var item in _elmt.LignesImpression.Where(l => (Array.IndexOf(RegleCalcul.ReglesDeRoulage, l.regle.Id) >= 0) && (l is LigneFabricationOperation)))
                {
                    int cadenceDeBase = (item as LigneFabricationOperation).operationMachine.CadenceStandard;
                    int abattement = _elmt.machineImpression.GetAbattementCadence(_elmt.support.grammage, _elmt.ToursUtiles);

                    decimal cad0 = cadenceDeBase * abattement / 100.0M;
                    decimal cad1 = (cad0 * wPourcentCumul / 100.0M) - wPerte;
                    decimal cad2 = cad0 * (100 - wPourcentLimite) / 100.0M;

                    int cadResult = (int)Math.Min(Math.Min(cad1, cad2), wCadenceMaxi);

                    item.Cadence = cadResult;

                    string codeSection = (item as LigneFabricationOperation).operationMachine.section.Id;

                    // Traitement du tableau CAD
                    List<TableauCalcul> tableaux = ParametrageDevisBLL.GetTableauCalcul(item.regle.Id, String.Concat(codeSection, "CAD"));
                    if (tableaux != null)
                    {
                        foreach (TableauCalcul tabc in tableaux)
                        {
                            decimal abs = 0.0M;
                            if (!tabc.Special) abs = (decimal)typeof(RechercheTableau).GetMethod(tabc.critereX).Invoke(null, new object[] { _elmt, null });
                            decimal ord = (decimal)typeof(RechercheTableau).GetMethod(tabc.critereY).Invoke(null, new object[] { _elmt, null });

                            decimal rest = tabc.GetResult(abs, ord);

                            // TODO Si tableau CAD, LIM, R01, ... il ne faut pas remettre les boites à 0 
                            if (rest > 0.0M)
                            {
                                item.Cadence = rest;
                            }
                        }
                    }
                }


            }

            //TODO Traiter les gaches automatiques avant de valoriser l'élément
            if (_elmt.devis.typeProduit.CalculGacheAutomatique)
            {
                // constitution des rubriques choisies dans l'ordre des étapes et des séquence gache
                
                List<RubriqueChoisie> rubs = new List<RubriqueChoisie>();
                foreach(var etape in _elmt.EtapesProcess.OrderBy(e=>e.Ordre))
                {
                    rubs.AddRange(etape.RubriquesChoisies);
                }

                // TODO Inserer la ruprique d'impression avec ses parametres
                // on lit les rubriques choisies en partant de la fin
                decimal dTauxProgressif = 100.0M;
                int iPerteFixeProgressive = 0;

                for (int i = rubs.Count-1; i >= 0; i--)
                {

                    int iGacheFixe = rubs[i].lienMachineUtilise.GacheFixeEtape;
                    decimal dTauxGache = rubs[i].lienMachineUtilise.GacheVariableEtape;

                    // recherche en tableau pour la gache fixe
                    if (rubs[i].lienMachineUtilise.CodeRechercheTableauGacheFixe != String.Empty)
                    {
                        iGacheFixe= (int)TraitementTableauxGache(rubs[i], rubs[i].lienMachineUtilise.CodeRechercheTableauGacheFixe);
                    }
                    // recherche en tableau pour la gache variable
                    if (rubs[i].lienMachineUtilise.CodeRechercheTableauGacheVariable != String.Empty)
                    {
                        dTauxGache = TraitementTableauxGache(rubs[i], rubs[i].lienMachineUtilise.CodeRechercheTableauGacheVariable);
                    }

                    // calcul coeff gache
                    int iCoeffGacheFixe = 1;
                    if (rubs[i].lienMachineUtilise.ReponseMultiplieLaGacheFixe == "1") iCoeffGacheFixe = (int) rubs[i].Reponses[0];
                    if (rubs[i].lienMachineUtilise.ReponseMultiplieLaGacheFixe == "2") iCoeffGacheFixe = (int)rubs[i].Reponses[1];
                    if (rubs[i].lienMachineUtilise.ReponseMultiplieLaGacheFixe == "3") iCoeffGacheFixe = (int)rubs[i].Reponses[2];
                    if (rubs[i].lienMachineUtilise.ReponseMultiplieLaGacheFixe == "4") iCoeffGacheFixe = (int)rubs[i].Reponses[3];
                    if (rubs[i].lienMachineUtilise.ReponseMultiplieLaGacheFixe == "5") iCoeffGacheFixe = (int)rubs[i].Reponses[4];

                    
                    dTauxProgressif = dTauxProgressif * (100.0M + dTauxGache) / 100.0M;
                    rubs[i].GacheFixe = iPerteFixeProgressive;
                    iPerteFixeProgressive = iPerteFixeProgressive + (iGacheFixe*iCoeffGacheFixe);
                    rubs[i].TauxGacheVariable = dTauxProgressif;
                    
                }

                _elmt.GacheVariable = dTauxProgressif;
                _elmt.ToursGacheFixe = iPerteFixeProgressive;
            }

            //valorisation de la rubrique d'impression
            if (_elmt.ImpressionAValoriser)
            {
                for (int i = 0; i < _elmt.LignesImpression.Count(); i++)
                {
                    LigneFabrication ligne = _elmt.LignesImpression[i];

                    //Recherche en tableau
                    if (ligne.regle.UtilisationTableau)
                    {
                        TraitementTableaux(_elmt, ref ligne, null);
                    }
                    // Traitement des précalculs
                    TraitementPrecalculs(_elmt, ref ligne, null);

                    //Cas particulier des lignes avec règle RO RM : nombre = nombre de passages
                    //TODO et cadence=cadence d'impression abattue
                    if (Array.IndexOf(RegleCalcul.ReglesDeRoulage, ligne.regle.Id) >= 0)
                    {
                        ligne.Nombre = _elmt.NombreDePassagesImpression;
                    }

                    ligne.Calcule();

                }
            }

            foreach (EtapeProcess etape in _elmt.EtapesProcess)
            {
                foreach (RubriqueChoisie rubc in etape.RubriquesChoisies)
                {
                    if (rubc.DetailAConstituer)
                    {
                        rubc.LignesFabrication.Clear();
                        try
                        {
                            rubc.lienMachineUtilise = rubc.rubrique.LiensMachine.Single(x => (x.LieMachine == false)
                                            || (x.LieMachine == true && x.machine == _elmt.machineImpression));
                            
                            foreach (var modele in rubc.lienMachineUtilise.LignesDetail)
                            {
                                LigneFabrication ligne = LigneFabricationFactory.CreateLigneFabrication(_elmt, modele);
                                rubc.LignesFabrication.Add(ligne);
                            }

                        }
                        catch (Exception)
                        {

                            Trace.TraceError(String.Concat("Aucun lien trouvé pour la rubrique ", rubc.rubrique.Id.ToString()));
                        }

                        rubc.DetailAConstituer = false;
                        rubc.AValoriser = true;

                    }
                    if ((rubc.AValoriser) && (!rubc.ReponsesAdonner))
                    {

                        for (int i = 0; i < rubc.LignesFabrication.Count(); i++)
                        {
                            LigneFabrication ligne = rubc.LignesFabrication[i];
                            TraitementReponses(ref ligne, rubc);

                            //Recherche en tableau
                            if (ligne.regle.UtilisationTableau)
                            {
                                TraitementTableaux(_elmt, ref ligne, rubc);
                            }
                            // Traitement des précalculs
                            TraitementPrecalculs(_elmt, ref ligne, rubc);

                            ligne.Calcule();


                        }

                    }
                }
            }
        }





        // TODO Gérer la sous-catégorie
        // TODO _type produit et _cat ne sont plus necessaires

        /// <summary>Récupere le tableau de rubriques de l'élément passé en parametre, selon type de produit, catégorie et le type d'élément
        /// </summary>
        public static TableauRubrique GetTableauRubriqueElement(TypeProduit _typeProduit, String _cat, ElementDevis _elem)
        {
            String typTab = "";
            if (_elem.Id < 16) typTab = "C";
            if (_elem.Id == 16) typTab = "P";
            if (_elem.Id == 17) typTab = "F";

            return ParametrageDevisRepository.GetTableauRubrique(_elem.devis.Categorie.produit, _elem.devis.Categorie.Id, _elem.machineImpression.Id, typTab);

        }

        public static void TraitementTableaux(ElementDevis _elmt, ref LigneFabrication _ligne, RubriqueChoisie _rubc)
        {
            List<TableauCalcul> tableaux = ParametrageDevisBLL.GetTableauCalcul(_ligne.regle.Id, _ligne.GetReference());

            if (tableaux != null)
            {
                foreach (TableauCalcul tabc in tableaux)
                {
                    decimal abs = 0.0M;
                    if (!tabc.Special) abs = (decimal)typeof(RechercheTableau).GetMethod(tabc.critereX).Invoke(null, new object[] { _elmt, _rubc });
                    decimal ord = (decimal)typeof(RechercheTableau).GetMethod(tabc.critereY).Invoke(null, new object[] { _elmt, _rubc });

                    decimal rest = tabc.GetResult(abs, ord);

                    // TODO Si tableau CAD, LIM, R01, ... il ne faut pas remettre les boites à 0 
                    if (rest != -1.0M)
                    {
                        switch (tabc.TypeResultat)
                        {
                            case "N":
                                _ligne.Nombre = rest;
                                break;
                            case "T":
                                _ligne.TempsUnitaire = rest;
                                break;
                            case "C":
                                _ligne.Cadence = rest;
                                break;
                            case "Q":
                                _ligne.Quantite = rest;
                                break;
                            case "D":
                                _ligne.Diviseur = rest;
                                break;
                        }
                    }
                }
            }
        }

        public static decimal TraitementTableauxGache(RubriqueChoisie _rubc, String _code)
        {
            List<TableauCalcul> tableaux = ParametrageDevisBLL.GetTableauxCalculPourGache(_code);

            if (tableaux != null)
            {
                decimal dres = 1.0M;
                
                foreach (TableauCalcul tabc in tableaux)
                {
                    decimal abs = 0.0M;
                    if (!tabc.Special) abs = (decimal)typeof(RechercheTableau).GetMethod(tabc.critereX).Invoke(null, new object[] { _rubc.etape.element, _rubc });
                    decimal ord = (decimal)typeof(RechercheTableau).GetMethod(tabc.critereY).Invoke(null, new object[] { _rubc.etape.element, _rubc });

                    decimal rest = tabc.GetResult(abs, ord);

                    if (rest >0.0M)
                    {
                        dres=dres*rest;
                    }
                }

                return dres;
            }

            return 0.0M;
        }

        public static void TraitementPrecalculs(ElementDevis _elmt, ref LigneFabrication _ligne, RubriqueChoisie _rubc)
        {
            //TODO n'alimenter les boites que si elles sont à 0 ou si un critere a changé dans le précalcul
            if (_ligne.regle.PrecalculNombre != String.Empty)
            {
                _ligne.Nombre = (Decimal)typeof(Precalculs).GetMethod(_ligne.regle.PrecalculNombre).Invoke(null, new object[] { _elmt, _rubc });
            }
            if (_ligne.regle.PrecalculTemps != String.Empty)
            {
                _ligne.TempsUnitaire = (Decimal)typeof(Precalculs).GetMethod(_ligne.regle.PrecalculTemps).Invoke(null, new object[] { _elmt, _rubc });
            }
            if (_ligne.regle.PrecalculCadence != String.Empty)
            {
                _ligne.Cadence = (Decimal)typeof(Precalculs).GetMethod(_ligne.regle.PrecalculCadence).Invoke(null, new object[] { _elmt, _rubc });
            }
            if (_ligne.regle.PrecalculQuantite != String.Empty)
            {
                _ligne.Quantite = (Decimal)typeof(Precalculs).GetMethod(_ligne.regle.PrecalculQuantite).Invoke(null, new object[] { _elmt, _rubc });
            }
            if (_ligne.regle.PrecalculDiviseur != String.Empty)
            {
                _ligne.Diviseur = (Decimal)typeof(Precalculs).GetMethod(_ligne.regle.PrecalculDiviseur).Invoke(null, new object[] { _elmt, _rubc });
            }

        }

        public static void TraitementReponses(ref LigneFabrication _ligne, RubriqueChoisie _rubc)
        {
            // Alimentation du code matiere eventuel
            if (_ligne is LigneFabricationMatiere)
            {
                if ((_ligne as LigneFabricationMatiere).ReponseMatiere > 0)
                {
                    int codeMat = (int)_rubc.Reponses[(_ligne as LigneFabricationMatiere).ReponseMatiere - 1];
                    (_ligne as LigneFabricationMatiere).matiere = MatiereBLL.GetMatiereById(codeMat);
                    if ((_ligne as LigneFabricationMatiere).matiere != null)
                    {
                        (_ligne as LigneFabricationMatiere).PrixAchat = (_ligne as LigneFabricationMatiere).matiere.PrixAchat;
                        (_ligne as LigneFabricationMatiere).Coefficient = (_ligne as LigneFabricationMatiere).matiere.Coefficient;

                    }
                }
            }
            // Alimentation des réponses aux questions
            // Alimentation du Nombre avec la réponse à la question
            if (_ligne.regle.UtilisationNombre)
            {

                if (_ligne.ReponseNombre > 0) _ligne.Nombre = _rubc.Reponses[_ligne.ReponseNombre - 1];
            }
            // Alimentation du Temps unitaire
            if (_ligne.regle.UtilisationTemps)
            {

                if (_ligne.ReponseTemps > 0) _ligne.TempsUnitaire = _rubc.Reponses[_ligne.ReponseTemps - 1];
            }
            // Alimentation de la cadence
            if (_ligne.regle.UtilisationCadence)
            {

                if (_ligne.ReponseCadence > 0) _ligne.Cadence = _rubc.Reponses[_ligne.ReponseCadence - 1];
            }
            // Alimentation de la quantite
            if (_ligne.regle.UtilisationQuantite)
            {

                if (_ligne.ReponseQuantite > 0) _ligne.Quantite = _rubc.Reponses[_ligne.ReponseQuantite - 1];
            }
            // Alimentation du diviseur
            if (_ligne.regle.UtilisationDiviseur)
            {

                if (_ligne.ReponseDiviseur > 0) _ligne.Diviseur = _rubc.Reponses[_ligne.ReponseDiviseur - 1];
            }

        }


        /// <summary>permet de constituer et valoriser le pied du devis passé en parametre
        /// </summary>
        public static void ValoriseDevis(Devis _dev)
        {
            _dev.TotalValeurMachine = 0.0M;
            _dev.TotalValeurMainOeuvre = 0.0M;
            _dev.TotalValeurMatiere = 0.0M;
            _dev.TotalValeurSousTraitance = 0.0M;
            _dev.TotalValeurSupport = 0.0M;
            _dev.TotalValeurTransport=0.0M;
            _dev.TotalPoidsSupport = 0.0M;

            // calcul du poids total , valeur support, valeur matiere, ...
            foreach (var elm in _dev.Elements)
            {
                foreach (var etape in elm.EtapesProcess)
                {
                    foreach (var rubc in etape.RubriquesChoisies)
                    {
                        
                        foreach (var ligf in rubc.LignesFabrication)
                        {
                            if (ligf is LigneFabricationMatiere)
                            {
                                _dev.TotalValeurMatiere += ligf.Valeur;
                            }
                            if (ligf is LigneFabricationSousTraitance)
                            {
                                _dev.TotalValeurSousTraitance += ligf.Valeur;
                            }
                            if (ligf is LigneFabricationOperation)
                            {
                                _dev.TotalValeurMachine += (ligf as LigneFabricationOperation).ValeurMachine;
                                _dev.TotalValeurMainOeuvre += (ligf as LigneFabricationOperation).ValeurMainOeuvre;

                            }
                        }
                    }
                }
                //detail.AddRange(elm.LignesImpression.Where(e => e.Resultat > 0));

                foreach (var ligf in elm.LignesImpression)
                {
                    if (ligf is LigneFabricationMatiere)
                    {
                        _dev.TotalValeurMatiere += ligf.Valeur;
                    }
                    if (ligf is LigneFabricationSousTraitance)
                    {
                        _dev.TotalValeurSousTraitance += ligf.Valeur;
                    }
                    if (ligf is LigneFabricationOperation)
                    {
                        _dev.TotalValeurMachine += (ligf as LigneFabricationOperation).ValeurMachine;
                        _dev.TotalValeurMainOeuvre += (ligf as LigneFabricationOperation).ValeurMainOeuvre;

                    }
                }

                _dev.TotalValeurSupport += elm.ValeurSupport;
                _dev.TotalPoidsSupport += elm.PoidsAchete;

            }

            // Calcul des frais de manutention
            _dev.MontantFraisManutention = Decimal.Round(_dev.TotalPoidsSupport * ParametrageDevisBLL.GetParametresGeneraux().TauxManutention , 2);

            // Determination des frais de stockage et financier
            _dev.TauxFraisFinanciers = 0.0M;
            _dev.TauxFraisStockage = 0.0M;
            if (_dev.FractionLivraisons > 1)
            {
                _dev.TauxFraisStockage = (((_dev.FractionLivraisons - 1) * _dev.EspacementMois) / 2.0M) * ParametrageDevisBLL.GetParametresGeneraux().TauxFraisStockageParMois;

                if (_dev.ModeFacturation == "1") _dev.TauxFraisFinanciers = (((_dev.FractionLivraisons - 1) * _dev.EspacementMois) / 2.0M) * ParametrageDevisBLL.GetParametresGeneraux().TauxFraisFinanciersParMois;
            }

            // Determination majoration selon conditions de reglement
            _dev.TauxFraisCR = 0.0M;
            switch (_dev.tiers.Reglement.NombreDeJours)
            {
                case 120:
                    _dev.TauxFraisCR=ParametrageDevisBLL.GetParametresGeneraux().MajorationCR120;
                    break;
                case 90:
                    _dev.TauxFraisCR=ParametrageDevisBLL.GetParametresGeneraux().MajorationCR90;
                    break;
                case 60:
                    _dev.TauxFraisCR=ParametrageDevisBLL.GetParametresGeneraux().MajorationCR60;
                    break;
                case 45:
                    _dev.TauxFraisCR = ParametrageDevisBLL.GetParametresGeneraux().MajorationCR60;
                    break;
                case 30:
                    if ((_dev.tiers.Reglement.JourEcheance==31) || (_dev.tiers.Reglement.JourEcheance==15))
                            _dev.TauxFraisCR = ParametrageDevisBLL.GetParametresGeneraux().MajorationCR60;
                    break;
                default:
                    break;
            }

            if (ParametrageDevisBLL.GetParametresGeneraux().MethodeCoefficient == "B")
            {
                ValoriseMethodeB(_dev);
            }
            else ValoriseMethodeA(_dev);


        }

        private static void ValoriseMethodeB(Devis _dev)
        {
            _dev.PVMini = 0.0M;
            _dev.PVConseille = 0.0M;
            Decimal dPV = 0.0M;
            Decimal dRatio = 0.0M;
            Decimal dTauxCommission=0.0M;

            // Somme des taux de majorations
            Decimal dSommeTaux = _dev.TauxFraisCR + _dev.TauxFraisFinanciers + _dev.TauxFraisStockage + _dev.TauxHonoraires + _dev.tiers.TauxEscompte + _dev.tiers.TauxRistourne;

            Decimal dTauxPVMini = (1.0M / (ParametrageDevisBLL.GetParametresGeneraux().TauxPVMini / 100.0M + 1)) - dSommeTaux / 100.0M;
            Decimal dTauxPVConseille = (1.0M / (ParametrageDevisBLL.GetParametresGeneraux().TauxPVConseille / 100.0M + 1)) - dSommeTaux / 100.0M;

            
            // Calcul du Prix de vente mini
            
            if (dTauxPVMini > 0) _dev.PVMini = _dev.MontantBase100 / dTauxPVMini;
            // Calcul du Prix de vente conseillé
            
            if (dTauxPVConseille > 0) _dev.PVConseille = _dev.MontantBase100 / dTauxPVConseille;

            // Calcul du ratio Prix de vente/Base100
            
            if (_dev.PVRemisQuantite > 0) dPV = _dev.PVRemisQuantite;
            else
                dPV = _dev.PVConseille;

            dRatio=dPV/(_dev.MontantBase100 + dPV*(dSommeTaux/100.0M));

            // recherche du taux de commission thérorique selon le ratio
            if (dRatio >= ParametrageDevisBLL.GetParametresGeneraux().ratiosPV100[9]) dTauxCommission = ParametrageDevisBLL.GetParametresGeneraux().tauxCommission[9];
            for (int i = 0; i < 9; i++)
            {
                if (dRatio <= ParametrageDevisBLL.GetParametresGeneraux().ratiosPV100[i])
                {
                    dTauxCommission = ParametrageDevisBLL.GetParametresGeneraux().tauxCommission[i];
                    break;
                }
            }

            //TODO multiplier le taux de com par ICOEF du représentant

            // Calcul des frais généraux
            _dev.MontantFraisGeneraux = Decimal.Round(dPV * ParametrageDevisBLL.GetParametresGeneraux().TauxFGPapierOuMethodeB/100.0M, 2);
            // Calcul Majoration escompte
            _dev.MontantFraisEscompte = Decimal.Round(dPV * _dev.tiers.TauxEscompte / 100.0M, 2);
            // Calcul Majoration ristourne
            _dev.MontantFraisRistourne = Decimal.Round(dPV * _dev.tiers.TauxRistourne / 100.0M, 2);
            // Calcul Majoration frais financiers
            _dev.MontantFraisFinanciers = Decimal.Round(dPV * _dev.TauxFraisFinanciers / 100.0M, 2);
            // Calcul Majoration frais stockage
            _dev.MontantFraisStockage = Decimal.Round(dPV * _dev.TauxFraisStockage / 100.0M, 2);
            // Calcul Majoration Condition de reglement
            _dev.MontantFraisCR = Decimal.Round(dPV * _dev.TauxFraisCR / 100.0M, 2);

            // Calcul des frais commerciaux
            _dev.MontantFraisCommerciaux = Decimal.Round(dPV * (_dev.TauxHonoraires+ dTauxCommission) / 100.0M, 2) + _dev.MontantHonoraires;


        }

        private static void ValoriseMethodeA(Devis _dev)
        {
            _dev.PVMini = 0.0M;
            _dev.PVConseille = 0.0M;
            Decimal dPV = 0.0M;

            // calcul des frais généraux
            _dev.MontantFraisGeneraux = Decimal.Round(_dev.TotalValeurSupport * ParametrageDevisBLL.GetParametresGeneraux().TauxFGPapierOuMethodeB / 100.0M, 2)
                                      + Decimal.Round(_dev.TotalValeurMachine * ParametrageDevisBLL.GetParametresGeneraux().TauxFGMachine / 100.0M, 2)
                                      + Decimal.Round(_dev.TotalValeurMainOeuvre * ParametrageDevisBLL.GetParametresGeneraux().TauxFGMainOeuvre / 100.0M, 2)
                                      + Decimal.Round(_dev.TotalValeurMatiere * ParametrageDevisBLL.GetParametresGeneraux().TauxFGMatiere / 100.0M, 2)
                                      + Decimal.Round(_dev.TotalValeurSousTraitance * ParametrageDevisBLL.GetParametresGeneraux().TauxFGSousTraitance / 100.0M, 2);

        }

        public static void SaveDevis(Devis _dev)
        {
            String _societe;
            AppService.GlobalVariables.TryGetValue("societe", out _societe);
            DevisDB.SaveDevis(_societe, _dev);
        }


    }
}
