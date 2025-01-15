using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Axilog.Model.Devis;
using Net.Axilog.DAL;

namespace Net.Axilog.BLL
{
    public class MatiereBLL
    {
        public static Matiere GetMatiereById(int _id)
        {
            return MatiereRepository.GetMatiere(_id);
        }

        public static List<Matiere> GetMatieres()
        {
            return MatiereRepository.Matieres;
        }

        public static List<Matiere> GetMatieresByCodeRecherche(string _rech)
        {
            return MatiereRepository.GetMatieresByCodeRecherche(_rech);
        }

    }
}
