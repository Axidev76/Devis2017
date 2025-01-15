using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Axilog.DAL;
using Net.Axilog.Model.Base;


namespace Net.Axilog.BLL
{
    public class SupportBLL
    {
        public static List<SousSorte> GetSousSortes()
        {
            return SupportRepository.SousSortes;
        }
    }
}
