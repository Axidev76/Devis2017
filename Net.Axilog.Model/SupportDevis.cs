using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Axilog.Model.Base;

namespace Net.Axilog.Model.Base
{
    public class SupportDevis : Support
    {
        public bool NonCode { get; set; }
        public String Provenance { get; set; }
        public decimal PrixAchat { get; set; }
        public String UniteAchat { get; set; }
        public int ConditionnePar { get; set; }
    }
}
