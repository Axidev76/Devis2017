using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Axilog.Model.Devis;
using Net.Axilog.Model.Base;
using Net.Axilog.DAL;

namespace Net.Axilog.BLL
{
    public class SousTraitanceBLL
    {
        public static List<OperationSousTraitance> GetOperationSousTraitance()
        {
            return SousTraitanceRepository.OperationsSousTraitance;
        }
    }
}
