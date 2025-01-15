using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Axilog.Model.Devis;
using Net.Axilog.Model.Base;
using IBM.Data.DB2.iSeries;
using System.Diagnostics;

namespace Net.Axilog.DAL
{
    public static class MatiereRepository
    {
        public static List<Matiere> Matieres { get; set; }

        public static void LoadMatieres(String _societe)
        {
            iDB2Connection cn1 = DBConnection.GetDBConnection();

            string _sql = "SELECT RMATIE, trim(GNOM), GCOEF1, GPA1, GCRECD , " +
               " digits(GDA1) , substr(gopt03, 1, 1)" +
                " FROM c000f " +
               " WHERE rsoc=" + _societe +
               " and GTAG= ' ' and  (digits(rmatie)  in (select lcref " +
               " from c50011 where rsoc=" + _societe + " and lctypl='M') or digits(rmatie) " +
               " in (select liref from c50021 where rsoc=" + _societe + " and " +
               " litypl='M') or gcrecd<>' ') ";
        
            iDB2Command _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();

            iDB2DataReader read = _command.ExecuteReader();
            Matieres = new List<Matiere>();

            while (read.Read())
            {
                Matiere mat = new Matiere();
                mat.Id = read.GetInt32(0);
                mat.Nom = read.GetString(1);
                mat.Coefficient = read.GetDecimal(2);
                mat.PrixAchat = read.GetDecimal(3);
                mat.CodeRecherche = read.GetString(4);
                mat.DateTarif = DateTime.ParseExact(read.GetString(5), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                mat.AvecGacheVariable = read.GetString(6) == "N";
               
                Matieres.Add(mat);

            }

            read.Close();
            _command.Dispose();
        }

        public static Matiere GetMatiere(int _code)
        {
            try
            {
                return Matieres.Single(e => e.Id == _code);
            }
            catch (Exception)
            {

                Trace.TraceError("Matiere " + _code.ToString() + " non trouvée");
                return null;
            }
        }

        public static Matiere GetMatiere(String _scode)
        {
            try
            {
                int _code = int.Parse(_scode);
                return GetMatiere(_code);
            }
            catch (Exception)
            {
                
                Trace.TraceError("Matiere " + _scode + " non trouvée");
                return null;
            }
        }

        public static List<Matiere> GetMatieresByCodeRecherche(String _codeRecherche)
        {
            return Matieres.Where(e => e.CodeRecherche == _codeRecherche).ToList();
        }
    }

}
