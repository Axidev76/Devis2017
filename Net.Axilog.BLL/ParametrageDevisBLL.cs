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
    public class ParametrageDevisBLL
    {
        public static Rubrique GetRubrique(int _id)
        {
            return ParametrageDevisRepository.GetRubrique(_id);
        }

        public static UniteVente GetUniteVente(string _id)
        {
            return ParametrageDevisRepository.GetUniteVente(_id);
        }

        public static ParametresGeneraux GetParametresGeneraux()
        {
            return ParametrageDevisRepository.Parametre;
        }

        public static RegleCalcul GetRegle(String _id)
        {
            return ParametrageDevisRepository.GetRegle(_id);
        }

        public static List<TableauCalcul> GetTableauCalcul(String _regle, String _reference)
        {
            return ParametrageDevisRepository.GetTableauCalcul(_regle, _reference);

        }

        public static List<TableauCalcul> GetTableauxCalculPourGache(String _reference)
        {
            return ParametrageDevisRepository.GetTableauxCalculPourGache(_reference);
        }

        public static RubriqueImpression GetRubriqueImpression (String  _codeMachine, int _recto, int _verso, int _basculee)
        {
            return ParametrageDevisRepository.GetRubriqueImpression(_codeMachine, _recto, _verso, _basculee);
        }

        public static List<Rubrique> GetRubriquesByProcess(String _opeProcess)
        {
            return ParametrageDevisRepository.GetRubriquesByProcess(_opeProcess);
        }

        public static Rubrique[] GetRubriquesByProcessAsArray(String _opeProcess)
        {
            return ParametrageDevisRepository.GetRubriquesByProcessAsArray(_opeProcess);
        }

    }
}
