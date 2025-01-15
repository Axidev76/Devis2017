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
    public static class SousTraitanceRepository
    {
        public static List<OperationSousTraitance> OperationsSousTraitance { get; set; }

        public static void LoadOperationsSousTraitance(String _societe)
        {
            iDB2Connection cn1 = DBConnection.GetDBConnection();

            /* string _sql = "SELECT A.RSECT, trim(A.RSLIB), A.RSLB6, A.RSATE , B.ROPE, trim(c.olib), c.olib6 " +
                        " , otypfr, B.SOPRMA, B.SOPRMO, B.SOTPST, B.SOCAST, " +
                        " B.SOTPMI, trim(B.SOCOMM), B.SOGACH, B.SOGACF FROM c800f a join d100f b on " +
                        " a.rsoc=b.rsoc and a.rsect=b.rsect and SOTAG=' ' join d000f c on b.rsoc=c.rsoc and " +
                        " b.rope=c.rope and onatop in ('C', 'T', 'A', 'L') and OTAG=' ' " +
                        " WHERE a.rsoc=" + _societe + " and A.RSTAG= ' '  " +
                        " and (b.rsect concat digits(b.rope) in " +
                        " (select lcref from c50011 where rsoc=" + _societe + " and lctypl='O') " +
                        " or b.rsect concat digits(b.rope) in  " +
                        " (select liref from c50021 where rsoc=" + _societe + " and litypl='O')) " +
                        " order by b.rsect, b.rope "; */

            string _sql = "SELECT A.RFOURN, trim(A.PMOT), trim(A.PRS), A.PTELEP, A.PEMAIL, B.ROPE, trim(B.HLIB), " +
                        " B.HPRFIX, B.HPRPRO, B.HQTEPR, trim(c.olib), c.olib6 , otypfr, HOPT01 FROM  " +
                        " b500f a join az00f b on a.rsoc=b.rsoc and a.rfourn=b.rfourn and  " +
                        " HTAG=' ' join d000f c on b.rsoc=c.rsoc and b.rope=c.rope and " +
                        " OTAG=' ' WHERE a.rsoc=" + _societe + " and ptag=' ' " +
                        " ORDER BY a.rfourn, b.rope ";   



            iDB2Command _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();

            iDB2DataReader read = _command.ExecuteReader();

            OperationsSousTraitance = new List<OperationSousTraitance>();
            int savFrn = 0;
            Fournisseur newFrn = new Fournisseur(); ;

            while (read.Read())
            {
                if (read.GetInt32(0) != savFrn)
                {
                    newFrn = new Fournisseur { Id = read.GetInt32(0), RaisonSociale = read.GetString(2), MotDirecteur = read.GetString(1), Telephone = read.GetString(3), Email=read.GetString(4) };
                    savFrn = read.GetInt32(0);
                }

                OperationSousTraitance ope = new OperationSousTraitance();
                ope.fournisseur = newFrn;
                ope.Operation = read.GetInt32(5);
                ope.Nom = read.GetString(6);
                ope.TypeFrais = read.GetString(12);
                ope.PrixFixe = read.GetDecimal(7);
                ope.PrixProp = read.GetDecimal(8);
                ope.QuantiteProp = read.GetDecimal(9);
                ope.Commentaire = read.GetString(13);
                
                ope.AvecGacheVariable = read.GetString(13) == "N";


                OperationsSousTraitance.Add(ope);

            }

            read.Close();
            _command.Dispose();

        }

        public static OperationSousTraitance GetOperationSousTraitance(int _frn, int _ope)
        {
            try
            {
                return OperationsSousTraitance.Single(e => e.fournisseur.Id == _frn && e.Operation == _ope);
            }
            catch (Exception)
            {
                Trace.TraceError("Operation de sous-traitance" + _frn + _ope.ToString().PadLeft(3) + " non trouvée");
                return null;
            }
        }
        public static OperationSousTraitance GetOperationSousTraitance(String _frnOpe)
        {
            try
            {
                int frn = int.Parse(_frnOpe.Substring(0, 3));
                int ope = int.Parse(_frnOpe.Substring(3, 3));
                return GetOperationSousTraitance(frn, ope);
            }
            catch (Exception)
            {
                Trace.TraceError("Operation de sous-traitance" + _frnOpe + "incorrecte");
                return null;
            }
        }

    }
}
