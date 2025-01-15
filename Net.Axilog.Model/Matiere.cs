using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Axilog.Model.Devis
{
    public class Matiere
    {
        public int Id { get; set; }
        public String Nom { get; set; }
        public String CodeRecherche { get; set; }
        public Decimal PrixAchat { get; set; }
        public Decimal Coefficient { get; set; }
        public DateTime DateTarif { get; set; }
        public bool AvecGacheVariable { get; set; }

        public override String ToString() { return String.Concat(Id, " ", Nom); }
    }
}
