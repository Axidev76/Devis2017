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
    public class MachineBLL
    {
        public static List<MachineImpression> GetMachinesImpression()
        {
            return MachineRepository.MachinesImpression;
        }

        public static List<OperationMachine> GetOperationsMachines()
        {
            return MachineRepository.OperationsMachines;
        }

        public static MachineImpression GetMachineImpression(string _code)
        {
            return MachineRepository.GetMachineImpression(_code);
        }
    }
}
