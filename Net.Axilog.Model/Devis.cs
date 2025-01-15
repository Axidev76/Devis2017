using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Axilog.Model.Base;
using System.ComponentModel;


namespace Net.Axilog.Model.Devis
{
    public enum TypeAdresseDevis
    {Facturation=1,Livraison=2, Manuelle=3}

    public class ligneRecap
    {
        public String CodeTri { get; set; }
        public Decimal TotalValeur { get; set; }
        public Decimal TotalValeurMP { get; set; }
        public Decimal TotalHeures { get; set; }
        public Decimal TotalFraisFixes { get; set; }
        public Decimal TotalPoids { get; set; }
    }
 
    public class Devis
    {
        public int Id { get; set; }
        public int Version { get; set; }
        public int Hypothese { get; set; }
        public String Designation { get; set; }
        public String Designation2 { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime DateDemande { get; set; }
        public int CollectifClient { get; set; }
        public int ClientId { get; set; }
        public int ProspectId { get; set; }
        public Tiers tiers { get; set; }
        public Contact contact { get; set; }
        public Representant representant { get; set; }
        public String fabricant { get; set; }
        public CategorieProduit Categorie { get; set; }
        public SousCategorieProduit souscategorie { get; set; }
        public TypeProduit typeProduit { get; set; }
        public int Quantite { get; set; }
        public FormatFini formatFini { get; set; }
        public FormatBase formatOuvert { get; set; }
        public int GacheVariable { get; set; }
        public String sensEnroulement { get; set; }
        public int EmballagePar { get; set; }
        public String refImprime { get; set; }

        public Norme norme { get; set; }

        public Boolean productionStockee { get; set; }
        public Boolean quantiteJuste { get; set; }

        public String enseigne { get; set; }

        public String texteTechnique { get; set; }

        public String enlevementAlivrer { get; set; }
        public String typePort { get; set; }
        public int nombreLieuxLivraison { get; set; }


        public int FractionLivraisons { get; set; }
        public int EspacementMois { get; set; }
        public String ModeFacturation { get; set; }
        public Transporteur transporteur { get; set; }

        public int nombreElements { get; set; }

        public int etude { get; set; }

        public decimal TotalValeurSupport { get; set; }
        public decimal TotalPoidsSupport { get; set; }

        public decimal TotalValeurMatiere { get; set; }
        public decimal TotalValeurSousTraitance { get; set; }
        public decimal TotalValeurMachine { get; set; }
        public decimal TotalValeurMainOeuvre { get; set; }
        public decimal TotalValeurTransport { get; set; }

        public decimal MontantFraisManutention { get; set; }
        public decimal MontantFraisDossier { get; set; }

        public Decimal TauxFraisStockage { get; set; }
        public Decimal TauxFraisFinanciers { get; set; }
        public decimal MontantFraisStockage { get; set; }
        public decimal MontantFraisFinanciers { get; set; }

        public decimal MontantFraisRistourne { get; set; }
        public decimal MontantFraisEscompte { get; set; }
        public decimal TauxFraisCR { get; set; }
        public decimal MontantFraisCR { get; set; }

        public decimal MontantFraisGeneraux { get; set; }
        public decimal MontantFraisCommerciaux { get; set; }
        public decimal MontantHonoraires { get; set; }
        public decimal TauxHonoraires { get; set; }

        public decimal MontantBase100
        {
            get
            {
                return Decimal.Round(TotalValeurMachine + TotalValeurMainOeuvre + TotalValeurMatiere + TotalValeurSousTraitance 
                    + TotalValeurSupport + TotalValeurTransport + MontantHonoraires + MontantFraisManutention, 2);
            }
        }

        public decimal MontantMajorationsMethodeB
        {
            get
            {
                return MontantFraisManutention + MontantFraisEscompte + MontantFraisRistourne + MontantFraisCR + MontantFraisFinanciers + MontantFraisStockage;
            }
        }

        public decimal MontantTotalPRMethodeB
        {
            get
            {
                return MontantBase100 + MontantMajorationsMethodeB + MontantFraisGeneraux + MontantFraisCommerciaux;
            }
        }


        public decimal PVMini { get; set; }
        public decimal PVConseille { get; set; }
        public decimal PVRemisEnUV { get; set; }
        public decimal PVRemisQuantite
        {
            get
            {
                if (uniteVente ==null) return 0.0M;
                if (PVRemisEnUV == 0) return 0.0M;
                if (uniteVente.Autout) return PVRemisEnUV;
                if (uniteVente.DiviseurQuantite > 0) return PVRemisEnUV*Quantite / uniteVente.DiviseurQuantite;

                return PVRemisEnUV;
            }
        }
        public UniteVente uniteVente { get; set; }




        public bool IsNew { get; set; }

        public List<ElementDevis> Elements { get; set; }

        public ElementDevis Prepresse { get; set; }
        public ElementDevis Faconnage { get; set; }

        public Devis()
        {
            this.Elements = new List<ElementDevis>();
        }

        public ElementDevis[] GetElementsAsArray()
        {
            return Elements.ToArray();
        }

        public List<ligneRecap> GetTableauRecap()
        {
            List<LigneFabrication> detail = new List<LigneFabrication>();
            //TotalValeurSupport = 0.0M;
            //TotalPoidsSupport = 0.0M;

            foreach (var elm in this.Elements)
            {
                                
                foreach (var etape in elm.EtapesProcess)
                {
                    foreach (var rubc in etape.RubriquesChoisies)
                    {
                        detail.AddRange(rubc.LignesFabrication.Where(e => e.Resultat > 0));
                    }
                }
                detail.AddRange(elm.LignesImpression.Where(e => e.Resultat > 0));

                TotalValeurSupport += elm.ValeurSupport;
                TotalPoidsSupport += elm.PoidsAchete;

            }

            foreach (var etape in Prepresse.EtapesProcess)
            {
                foreach (var rubc in etape.RubriquesChoisies)
                {
                    detail.AddRange(rubc.LignesFabrication.Where(e => e.Resultat > 0));
                }
            }

            foreach (var etape in Faconnage.EtapesProcess)
            {
                foreach (var rubc in etape.RubriquesChoisies)
                {
                    detail.AddRange(rubc.LignesFabrication.Where(e => e.Resultat > 0));
                }
            }

            var recap = detail.GroupBy(l => l.CodeTri)
                          .Select(lg =>
                                new ligneRecap
                                {
                                    CodeTri = lg.Key,
                                    TotalValeur = lg.Sum(w => w.Valeur),
                                    TotalValeurMP = lg.Sum(w => w.ValeurMP),
                                    TotalHeures=lg.Where(j=>j is LigneFabricationOperation).Sum(w=>w.Resultat),
                                    TotalFraisFixes = lg.Where(j => j.ResultatMP==0).Sum(w => w.Valeur),
                                    TotalPoids=0.0M
                                }).OrderBy(e => e.CodeTri);
            var result=recap.ToList();
            result.Insert(0, new ligneRecap  { CodeTri = "Supports", 
                TotalValeur = TotalValeurSupport, TotalValeurMP = 0.0M, TotalHeures = 0.0M, TotalFraisFixes = 0.0M, TotalPoids=TotalPoidsSupport });
            result.Add(new ligneRecap { CodeTri = "BASE 100", TotalValeur = this.MontantBase100, TotalValeurMP = 0.0M, TotalHeures = 0.0M, TotalFraisFixes = 0.0M, TotalPoids = 0.0M });
            result.Add(new ligneRecap { CodeTri = "Majorations", TotalValeur = this.MontantMajorationsMethodeB, TotalValeurMP = 0.0M, TotalHeures = 0.0M, TotalFraisFixes = 0.0M, TotalPoids = 0.0M });
            result.Add(new ligneRecap { CodeTri = "Frais Généraux", TotalValeur = this.MontantFraisGeneraux, TotalValeurMP = 0.0M, TotalHeures = 0.0M, TotalFraisFixes = 0.0M, TotalPoids = 0.0M });
            result.Add(new ligneRecap { CodeTri = "Frais Commerciaux", TotalValeur = this.MontantFraisCommerciaux, TotalValeurMP = 0.0M, TotalHeures = 0.0M, TotalFraisFixes = 0.0M, TotalPoids = 0.0M });
            result.Add(new ligneRecap { CodeTri = "PR Total", TotalValeur = this.MontantTotalPRMethodeB, TotalValeurMP = 0.0M, TotalHeures = 0.0M, TotalFraisFixes = 0.0M, TotalPoids = 0.0M });
            return result; ;
        }

        public static Devis Clone(Devis _dev)
        {
            Devis cpy = new Devis();
            cpy.Id = _dev.Id;
            cpy.Version = _dev.Version;
            cpy.Hypothese = _dev.Hypothese;
            cpy.Designation = _dev.Designation;
            cpy.DateCreation = _dev.DateCreation;
            cpy.CollectifClient = _dev.CollectifClient;
            cpy.ClientId = _dev.ClientId;
            cpy.ProspectId = _dev.ProspectId;
            cpy.Quantite = _dev.Quantite;
            cpy.GacheVariable = _dev.GacheVariable;
            cpy.formatFini = new FormatFini(_dev.formatFini.Largeur, _dev.formatFini.Hauteur, _dev.formatFini.Cote3);
            cpy.Categorie = _dev.Categorie;
            cpy.EmballagePar = _dev.EmballagePar;
            cpy.EspacementMois = _dev.EspacementMois;
            cpy.FractionLivraisons = _dev.FractionLivraisons;
            cpy.ModeFacturation = _dev.ModeFacturation;
            cpy.PVRemisEnUV = _dev.PVRemisEnUV;
            cpy.uniteVente = _dev.uniteVente;
            
            cpy.Prepresse=ElementDevis.Clone(_dev.Prepresse);
            
            cpy.Faconnage=ElementDevis.Clone(_dev.Faconnage);


            foreach (ElementDevis elt in _dev.Elements)
            {
                cpy.Elements.Add(ElementDevis.Clone(elt));
            }


            return cpy;
        }

    }

    public class LigneLivraison
    {
        public Devis devis { get; private set; }
        public TypeAdresseDevis TypeAdresse { get; set; }

        public String RaisonSociale { get; set; }

        public Adresse adresse { get; set; }

        public int QuantiteALivrer { get; set; }

        public Transporteur transporteur { get; set; }

        public decimal montantTransport { get; set; }

        public decimal getPoidsALivrer()
        {
            return 0.0M; //TODO regle de trois (poids total/quantité devisée)*QuantiteAlivrer 
        }


    }


    




    public static class LigneFabricationFactory
    {
        public static LigneFabrication CreateLigneFabrication(ElementDevis _elem, String _type)
        {

            switch (_type)
            {
                case "O":
                    return new LigneFabricationOperation(_elem);
                case "M":
                    return new LigneFabricationMatiere(_elem);
                case "S":
                    return new LigneFabricationSousTraitance(_elem);
                default:
                    return null;
            }

            
        }

        public static LigneFabrication CreateLigneFabrication(ElementDevis _elem, ModeleLigneRubrique _modele)
        {
            
            
            if (_modele is ModeleLigneRubriqueOperation)
            {
                ModeleLigneRubriqueOperation modeleO = (ModeleLigneRubriqueOperation)_modele;
                LigneFabricationOperation ligne = new LigneFabricationOperation(_elem);
                ligne.operationMachine = modeleO.operationMachine;

                ligne.modele = _modele;
                ligne.regle = _modele.regle;

                ligne.Sequence = ligne.operationMachine.Sequence;

                AffecteBoites(ligne);

                return ligne;
            
                
            }

            if (_modele is ModeleLigneRubriqueMatiere)
            {
                ModeleLigneRubriqueMatiere modeleM = (ModeleLigneRubriqueMatiere)_modele;
                LigneFabricationMatiere ligne = new LigneFabricationMatiere(_elem);
                ligne.matiere = modeleM.matiere;

                ligne.ReponseMatiere = modeleM.ReponseMatiere;

                if (ligne.matiere != null)
                {
                    ligne.PrixAchat = ligne.matiere.PrixAchat;
                    ligne.Coefficient = ligne.matiere.Coefficient;
                }

                ligne.modele = _modele;
                ligne.regle = _modele.regle;

                AffecteBoites(ligne);

                return ligne;
            
                
            }

            if (_modele is ModeleLigneRubriqueSousTraitance)
            {
                ModeleLigneRubriqueSousTraitance modeleS = (ModeleLigneRubriqueSousTraitance)_modele;
                LigneFabricationSousTraitance ligne = new LigneFabricationSousTraitance(_elem);
                ligne.operationSousTraitance = modeleS.operationSousTraitance;

                ligne.modele = _modele;
                ligne.regle = _modele.regle;

                AffecteBoites(ligne);

                return ligne;
            
            }


            return null;            
            
        }

        private static void AffecteBoites(LigneFabrication ligne)
        {
          if (ligne != null)
            {

                // Alimentation du Nombre avec la réponse à la question
                if (ligne.modele.regle.UtilisationNombre)
                {
                    ligne.Nombre = ligne.modele.Nombre;
                    ligne.ReponseNombre = ligne.modele.ReponseNombre;

                }
                // Alimentation du Temps unitaire
                if (ligne.modele.regle.UtilisationTemps)
                {
                    ligne.TempsUnitaire = ligne.modele.TempsUnitaire;
                    ligne.ReponseTemps = ligne.modele.ReponseTemps;
                }
                // Alimentation de la cadence
                if (ligne.modele.regle.UtilisationCadence)
                {
                    ligne.Cadence = ligne.modele.Cadence;
                    ligne.ReponseCadence = ligne.modele.ReponseCadence;
                }
                // Alimentation de la quantite
                if (ligne.modele.regle.UtilisationQuantite)
                {
                    ligne.Quantite = ligne.modele.Quantite;
                    ligne.ReponseQuantite = ligne.modele.ReponseQuantite;
                }
                // Alimentation du diviseur
                if (ligne.modele.regle.UtilisationDiviseur)
                {
                    ligne.Diviseur = ligne.modele.Diviseur;
                    ligne.ReponseDiviseur = ligne.modele.ReponseDiviseur;
                }

                if ((ligne.regle.DependantQuantite) || (ligne.regle.DependantFormat)) ligne.Element.PropertyChanged += ligne.ElementDevis_PropertyChanged;
            }
    }

    }

}

