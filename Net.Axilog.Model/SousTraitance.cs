using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Axilog.Model.Base;

namespace Net.Axilog.Model.Devis
{
    class SousTraitance
    {
    }

    public class OperationSousTraitance
    {
        public const string FraisFixe = "F";
        public const string FraisProportionnel = "P";

        public Fournisseur fournisseur { get; set; }
        public int Operation { get; set; }
        public String Nom { get; set; }
        public String TypeFrais { get; set; }

        public decimal PrixFixe { get; set; }
        public decimal PrixProp { get; set; }
        public decimal QuantiteProp { get; set; }

        public String Commentaire { get; set; }

        public bool AvecGacheVariable { get; set; }

        public override String ToString() { return String.Concat(fournisseur.MotDirecteur, " ", Nom); }

    }
}
