using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Axilog.Model.Base;
using System.ComponentModel;
using System.Globalization;

namespace Net.Axilog.Model.Devis 
{
    public class ElementDevis : INotifyPropertyChanged
    {
        public Devis devis { get; private set; }
        public int Id { get; set; }
        public String Designation { get; set; }
        public String typeElement { get; set; }
        public SupportDevis support { get; set; }
        private MachineImpression miMachineImpression;
        public MachineImpression machineImpression
        {
            get { return miMachineImpression; }
            set
            {
                if (value != miMachineImpression)
                {
                    // Call OnPropertyChanged whenever the property is updated
                    miMachineImpression = value;
                    this.ImpressionAConstituer = true;
                    this.CadenceARecalculer = true;
                    foreach (var etape in this.EtapesProcess)
                    {
                        foreach (RubriqueChoisie rubc in etape.RubriquesChoisies.Where(rubc => rubc.LieMachine))
                        {
                            rubc.DetailAConstituer = true;
                        }
                    }
                    OnPropertyChanged("machineImpression");
                }
            }
        }
        private FormatFini ffFormatFini;
        public FormatFini formatFini
        {
            get { return ffFormatFini; }
            set
            {
                if (value != ffFormatFini)
                {
                    // Call OnPropertyChanged whenever the property is updated
                    ffFormatFini = value;
                    OnPropertyChanged("FormatFini");
                }
            }
        }
        public FormatBase formatOuvert { get; set; }
        public FormatBase formatTirage { get; set; }
        public int CouleursRecto { get; set; }
        public int CouleursVerso { get; set; }
        public int CouleursBascule { get; set; }
        public bool RectoVersoIdentique { get; set; }
        public int Poses { get; set; }
        public int Pages { get; set; }
        public int ElementsIdentiques { get; set; }
        private int iQuantite;
        public int Quantite {
            get { return iQuantite; }
            set
            {
                if (value != iQuantite)
                {
                    // Call OnPropertyChanged whenever the property is updated
                    iQuantite = value;
                    this.ImpressionAValoriser = true;
                    this.CadenceARecalculer = true;
                    OnPropertyChanged("Quantite");
                }
            }
        }
        public bool SansPapier { get; set; }
        private bool bSansImpression;
        public bool SansImpression
        {
            get { return bSansImpression; }
            set
            {
                if (value != bSansImpression)
                {
                    // Call OnPropertyChanged whenever the property is updated
                    bSansImpression = value;
                    OnPropertyChanged("SansImpression");

                    if (bSansImpression)
                    {
                        machineImpression = null;
                        
                    }
                }
            }
        }
        public decimal GacheVariable { get; set; }
        public int Refente { get; set; }

        public int codeOutil { get; set; }
        public int codeOutil1 { get; set; }
        public int codeOutil2 { get; set; }


        public List<EtapeProcess> EtapesProcess;
        public List<LigneFabrication> LignesImpression;

        public RubriqueImpression rubriqueImpression { get; set; }

        public bool ImpressionAConstituer { get; set; }
        public bool ImpressionAValoriser { get; set; }
        public bool CadenceARecalculer { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public int NombreDePassagesImpression
        {
            get
            {
                if (SansImpression) return 0;

                if (CouleursRecto + CouleursVerso == 0) return 1;

                // TODO Traiter les machines de type continu
                if (machineImpression.Type == TypeMachineImpression.plat)
                {
                    if (machineImpression.RectoVersoEnUnPassage)
                    {
                        if (CouleursRecto + CouleursVerso <= machineImpression.NombreDeGroupes) return 1;

                    }
                    else
                    {
                        if (CouleursBascule > 0)
                        {
                            Decimal result = Math.Max(CouleursRecto, CouleursVerso) / machineImpression.NombreDeGroupes;
                            return (int)Math.Ceiling(result);
                        }
                        else
                        {
                            int passR = Convert.ToInt32(Decimal.Ceiling(Decimal.Divide(CouleursRecto , machineImpression.NombreDeGroupes)));
                            int passV = Convert.ToInt32(Decimal.Ceiling(Decimal.Divide(CouleursVerso , machineImpression.NombreDeGroupes)));
                            return passR + passV;
                        }
                    }
                }

                return 1;
            }
        }

        public int ToursGacheFixe { get; set; }
        
        public int GetQuantite()
        {
            if (this.Quantite > 0) return this.Quantite;
            else
                return this.devis.Quantite;
        }

        public FormatFini GetFormatFini()
        {
            if (this.formatFini.Largeur > 0) return this.formatFini;
            else
                return this.devis.formatFini;
        }

        public decimal GetTauxGacheVariable()
        {
            if (this.GacheVariable > 0) return this.GacheVariable;
            else
                return this.devis.GacheVariable;
        }

        public String GetLibelleColoris()
        {
            String lib = this.CouleursRecto + "R°";
            if (this.CouleursVerso > 0) lib = lib + " " + this.CouleursVerso + "V°";
            if (this.CouleursBascule>0) lib = lib + " " + this.CouleursBascule + "BAS";
            return lib;
            
        }

        public int ToursUtiles
        {
            get
            {

                decimal tours = this.GetQuantite() * this.ElementsIdentiques;
                if (this.Poses > 1) tours = Math.Ceiling(tours / this.Poses);
                return Decimal.ToInt32(tours);
            }

        }

        public int GetToursUtilesMP()
        {
            decimal tours = 1000.0M * this.ElementsIdentiques;
            if (this.Poses > 1) tours = Math.Ceiling(tours / this.Poses);
            return Decimal.ToInt32(tours);

        }

        public decimal MetrageUtile
        {
            get
            {

                return ToursUtiles * this.formatTirage.GetHauteurM();
            }
            
        }

        public decimal GetMetrageUtileMP()
        {
            return GetToursUtilesMP() * this.formatTirage.GetHauteurM();
            
        }

        public int ToursGacheVariable
        {
            get
            {
                return Decimal.ToInt32(Math.Ceiling(this.ToursUtiles * this.GetTauxGacheVariable() / 100.0M));
            }

        }

        public int GetToursGacheVariableMP()
        {
            return Decimal.ToInt32(Math.Ceiling(this.GetToursUtilesMP() * this.GetTauxGacheVariable() / 100.0M));

        }

        public decimal MetrageGacheVariable
        {
            get
            {
                return ToursGacheVariable * this.formatTirage.GetHauteurM();
            }
            
        }

        public decimal GetMetrageGacheVariableMP()
        {
            return GetToursUtilesMP() * this.formatTirage.GetHauteurM();
            
        }

        public decimal GetMetrageGacheFixe()
        {
            return ToursGacheFixe * this.formatTirage.GetHauteurM();

        }

        public decimal FeuillesUtiles
        {
            get
            {
                if (this.Refente > 1) return ToursUtiles / this.Refente;
                else return ToursUtiles;
            }
        }

        public decimal GetFeuillesUtilesMP()
        {
            if (this.Refente > 1) return GetToursUtilesMP() / this.Refente;
            else return GetToursUtilesMP();
        }

        public decimal FeuillesGacheVariable
        {
            get
            {

                if (this.Refente > 1) return ToursGacheVariable / this.Refente;
                else return ToursGacheVariable;
            }
        }

        public decimal GetFeuillesGacheVariableMP()
        {
            if (this.Refente > 1) return GetToursGacheVariableMP() / this.Refente;
            else return GetToursGacheVariableMP();
        }

        public decimal GetFeuillesGacheFixe()
        {
            if (this.Refente > 1) return ToursGacheFixe / this.Refente;
            else return ToursGacheFixe;
        }

        public int ToursTotal
        {
            get
            {
                return ToursUtiles + ToursGacheFixe + ToursGacheVariable;
            }
        }
        public decimal FeuillesTotal
        {
            get
            { 
                return FeuillesUtiles + GetFeuillesGacheFixe() + FeuillesGacheVariable; 
            }
        }
        public decimal MetrageTotal
        {
            get
            {
                return MetrageUtile + GetMetrageGacheFixe() + MetrageGacheVariable;
            }
        }


        public decimal PoidsLivre
        {
            get
            {
                if (SansPapier) return 0.0M;
                if (support.type == TypeSupport.Feuille)
                {
                    return Support.ConvertQuantite(support, "UN", "KG", FeuillesUtiles);
                }
                else
                {
                    return Support.ConvertQuantite(support, "ML", "KG", MetrageUtile);
                }
            }
        }

        public decimal PoidsAchete
        {
            get
            {
                if (SansPapier) return 0.0M;
                if (support.Provenance == "C") return 0.0M;

                //TODO Traiter la gache fixe
                //TODO Traiter l'arrondi à l'unité de conditionnement
                //TODO Ajouter le % du type de produit
                if (support.type == TypeSupport.Feuille)
                {
                    switch (support.Provenance)
                    {
                        case "F": return Support.ConvertQuantite(support, "UN", "KG", FeuillesTotal);
                        default: return Support.ConvertQuantite(support, "UN", "KG", FeuillesTotal);
                    }

                }
                else
                {
                    return Support.ConvertQuantite(support, "ML", "KG", MetrageTotal);
                }
            }
        }

        public decimal ValeurSupport
        {
            get
            {
                if (SansPapier) return 0.0M;
                if (support.Provenance == "C") return 0.0M;

                if (support.PrixAchat == 0.0M) return 0.0M;
                if (support.UniteAchat==String.Empty) return 0.0M;

                //TODO Traiter la gache fixe
                //TODO Traiter l'arrondi à l'unité de conditionnement
                //TODO Ajouter le % du type de produit
                if (support.type == TypeSupport.Feuille)
                {
                    switch (support.Provenance)
                    {
                        case "F": return Support.ConvertQuantite(support, "UN", support.UniteAchat, FeuillesTotal)*support.PrixAchat;
                        default: return Support.ConvertQuantite(support, "UN", support.UniteAchat, FeuillesTotal)*support.PrixAchat;
                    }

                }
                else
                {
                    return Support.ConvertQuantite(support, "ML", support.UniteAchat, MetrageTotal)*support.PrixAchat;
                }
            }
        }



        

        public ElementDevis()
        {
            this.EtapesProcess = new List<EtapeProcess>();
            this.LignesImpression = new List<LigneFabrication>();

            //this.PropertyChanged += ElementDevis_PropertyChanged;
        }

        void ElementDevis_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "Quantite")
            //{
           //     (sender as ElementDevis).ImpressionAValoriser = true;
           // }
            //if (e.PropertyName == "machineImpression")
            //{
            //    (sender as ElementDevis).ImpressionAConstituer = true;
            //    foreach( RubriqueChoisie rubc in (sender as ElementDevis).RubriquesChoisies.Where(rubc=>rubc.LieMachine))
            //    {
            //        rubc.DetailAConstituer = true;
            //    }
            //}
        }

        public ElementDevis(Devis _devis)
            : this()
        {
            this.devis = _devis;

        }

        // Constructeur pour prepresse et façonnage
        public ElementDevis(Devis _devis, int _id, String _nom)
            : this()
        {
            this.devis = _devis;
            this.Designation = _nom;
            this.Id = _id;
            this.formatFini = new FormatFini(0.0M, 0.0M, 0.0M);
            this.formatOuvert = new FormatBase(0.0M, 0.0M);
            this.formatTirage = new FormatBase(0.0M, 0.0M);
            this.support = new SupportDevis { grammage = 0, Format = new FormatBase(0.0M, 0.0M), coloris=String.Empty, ConditionnePar=0, PrixAchat=0, Provenance=String.Empty, UniteAchat=String.Empty, epaisseur = new Epaisseur { Longueur = 0, Unite = String.Empty } };
            this.support.sousSorte = new SousSorte { code = String.Empty, natureSupport = 0, nom = String.Empty, sorte = new Sorte { code = String.Empty, nom = String.Empty } };
            this.machineImpression=new MachineImpression{Id=""};

        }

        public List<LigneFabrication> GetDetailElement()
        {
            List<LigneFabrication> detail = new List<LigneFabrication>();
            foreach (var etape in this.EtapesProcess)
            {
                foreach (var rubc in etape.RubriquesChoisies)
                {
                    detail.AddRange(rubc.LignesFabrication.Where(e => e.Resultat > 0));
                }
            }
            detail.AddRange(this.LignesImpression.Where(e => e.Resultat > 0));

            //return detail.OrderBy(e=>e.Sequence).ToList();
            //return new BindListWithRemoving<LigneFabrication>(detail.OrderBy(e => e.Sequence).ToList());
            return detail.OrderBy(e => e.Sequence).ToList();
            
            //tblList=(ThreadedBindingList<LigneFabrication>) detail.OrderBy(e => e.Sequence);

            
        }

        

        public static ElementDevis Clone(ElementDevis _elt)
        {
            ElementDevis cpy = new ElementDevis();
            cpy.Id = _elt.Id;
            cpy.Designation = _elt.Designation;
            cpy.Quantite = _elt.Quantite;
            cpy.CouleursRecto = _elt.CouleursRecto;
            cpy.formatFini = new FormatFini(_elt.formatFini.Largeur, _elt.formatFini.Hauteur, _elt.formatFini.Cote3);
            cpy.formatOuvert = new FormatBase(_elt.formatOuvert.Largeur, _elt.formatOuvert.Hauteur);
            cpy.machineImpression = _elt.machineImpression;
            cpy.Poses = _elt.Poses;
            cpy.Pages = _elt.Pages;

            //TODO traiter toutes les zones

            //TODO traiter le clonage des lignes
            foreach (LigneFabrication lig in _elt.LignesImpression)
            {
            }

            //TODO traiter le clonage d'une rubrique choisie
            foreach (EtapeProcess ep in _elt.EtapesProcess)
            {
            }

            return cpy;
        }

    }

    public class EtapeProcess
    {
        public ElementDevis element { get; private set; }
        public int Ordre { get; set; }
        public String Etape { get; set; }

        public List<RubriqueChoisie> RubriquesChoisies;

        public EtapeProcess()
        {
            this.RubriquesChoisies = new List<RubriqueChoisie>();
            //this.LignesImpression = new List<LigneFabrication>();
         
        }

        public EtapeProcess(ElementDevis _elem)
            : this()
        {
            this.element = _elem;

        }

        public RubriqueChoisie[] GetRubriquesChoisiesAsArray()
        {
            return RubriquesChoisies.ToArray();
        }

    }
    public class RubriqueChoisie
    {
        public Rubrique rubrique { get; set; }

        public EtapeProcess etape { get; private set; }

        public decimal[] Reponses { get; set; }

        public decimal TauxGacheVariable { get; set; }
        public int GacheFixe { get; set; }

        public int SequenceGache { get; set; }

        public int ReductionCadence { get; set; }
        public string typeReductionCadence { get; set; }

        public bool ReponsesAdonner { get; set; }
        public bool DetailAConstituer { get; set; }
        public bool ASupprimer { get; set; }
        public bool AValoriser { get; set; }
        public bool LieMachine { get; set; }

        public LienRubriqueMachine lienMachineUtilise { get; set; }

        public List<LigneFabrication> LignesFabrication;

        public RubriqueChoisie()
        {
            this.LignesFabrication = new List<LigneFabrication>();
            this.Reponses = new decimal[5];
        }

        public RubriqueChoisie(EtapeProcess _etape)
            : this()
        {
            this.etape = _etape;

        }

        public String getReponse(int idx)
        {
            return Reponses[idx].ToString("F", CultureInfo.InvariantCulture);
        }

    }

}
