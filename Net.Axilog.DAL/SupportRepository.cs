using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Axilog.Model.Devis;
using Net.Axilog.Model.Base;
using IBM.Data.DB2.iSeries;


namespace Net.Axilog.DAL
{
    public static class SupportRepository
    {
        public static List<SousSorte> SousSortes { get; set; }

        public static void LoadSousSortes(String _societe)
        {
            iDB2Connection cn1 = DBConnection.GetDBConnection();

            string _sql = "select trim(rsorte), trim(rssort), trim(a.tlib1), trim(b.tlib1), tnorm, tnatsu  " +    
                            " from aa00081 a join ae0004 b  " +                          
                            " on a.rsoc=b.rsoc and a.rtable='SP' and a.rarg=b.rsorte and a.ttag=' ' " +
                          " WHERE a.rsoc=" + _societe +
                          " and b.ttag=' '" +
                          " order by rsorte, rssort ";

            iDB2Command _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();

            iDB2DataReader read = _command.ExecuteReader();

            String savSorte = String.Empty;

            Sorte newSorte = new Sorte(); ;
            SousSorte newSousSorte ;
            SousSortes = new List<SousSorte>();

            while (read.Read())
            {
                if (read.GetString(0) != savSorte)
                {
                    newSorte = new Sorte { code = read.GetString(0), nom = read.GetString(2) };
                    savSorte = read.GetString(0);
                }

                newSousSorte = new SousSorte { code = read.GetString(1), nom = read.GetString(3), natureSupport = read.GetInt32(5), sorte = newSorte };
                SousSortes.Add(newSousSorte);

            }

            // Ajout de la sorte spéciale "*"
            newSousSorte = new SousSorte { code = String.Empty, nom = "sans", natureSupport = 0, sorte = new Sorte{code="*", nom="sans" }};
            SousSortes.Add(newSousSorte);

            read.Close();
            _command.Dispose();

        }

        public static SousSorte GetSousSorte(String _codeSorte, String _codeSousSorte)
        {
            return SousSortes.Single(e => e.code==_codeSousSorte && e.sorte.code==_codeSorte);
        }

        public static List<SousSorte> GetSousSortesBySorte(String _codeSorte)
        {
            return SousSortes.Where(e => e.sorte.code == _codeSorte).ToList();
        }
    }
}
