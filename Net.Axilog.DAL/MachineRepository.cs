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
    public static class MachineRepository
    {
        public static List<MachineImpression> MachinesImpression { get; set; }
        public static List<OperationMachine> OperationsMachines { get; set; }

        public static void LoadMachinesImpression(String _societe)
        {
            iDB2Connection cn1 = DBConnection.GetDBConnection();

            string _sql = "SELECT A.RSECT, trim(A.RSLIB), A.RSLB6, A.RSATE, cast(B.MTTYPE as int) , B.MTPOUM, " +
                        " B.MTLRMI, b.mtHAMI, B.MTLRMX, b.mthamx, B.MTLRFT, B.MTHAFT, " +
                        " B.MTPRPI, B.MTFIPR , B.MTBOFL, " +
                        " MTGRM1,MTGRM2, MTGRM3, MTGRM4, MTGRM5, MTGRM6 ," +
                        " MTqte1,MTqte2, MTqte3, MTqte4, MTqte5, MTqte6 ," +
                        " mtpc1, mtpc2, mtpc3, mtpc4, mtpc5, " +
                        " mtpc6, mtpc7, mtpc8, mtpc9, mtpc10, " +
                        " mtpc11, mtpc12, mtpc13, mtpc14, mtpc15, " +
                        " mtpc16, mtpc17, mtpc18, mtpc19, mtpc20, " +
                        " mtpc21, mtpc22, mtpc23, mtpc24, mtpc25 , MTROVO , MTGRX " +
                        " FROM c800f a join aa000j b on  " +
                        " a.rsoc=b.rsoc and a.rsect=b.rsect and mttype<>' ' WHERE a.rsoc="+ _societe + " and A.RSMCHT ='O' " +
                        "and A.RSTAG= ' ' ORDER BY rsect "; 
                         
                         

            iDB2Command _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();

            iDB2DataReader read = _command.ExecuteReader();

            MachinesImpression = new List<MachineImpression>();

            while (read.Read())
            {
                MachineImpression machine = new MachineImpression();
                machine.Id = read.GetString(0);
                machine.Nom = read.GetString(1);
                machine.NomCourt = read.GetString(2);
                machine.Atelier = read.GetString(3);
                machine.Type = (TypeMachineImpression)read.GetInt32(4);
                machine.FormatMiniSupport=new FormatBase(read.GetDecimal(6), read.GetDecimal(7), FormatBase.UNITEMM);
                machine.FormatMaxiSupport = new FormatBase(read.GetDecimal(8), read.GetDecimal(9), FormatBase.UNITEMM);
                machine.FormatMaxiImpression = new FormatBase(read.GetDecimal(10), read.GetDecimal(11), read.GetString(5));
                machine.PriseDePince = read.GetInt32(12);
                machine.FinDePression = read.GetInt32(13);
                machine.BordDeFeuille = read.GetInt32(14);
                machine.RectoVersoEnUnPassage = read.GetString(52) == "O";
                machine.NombreDeGroupes = read.GetInt32(53);

                               
                machine.grammages = new int[6] { read.GetInt32(15), read.GetInt32(16), read.GetInt32(17), read.GetInt32(18), read.GetInt32(19), read.GetInt32(20) };
                machine.quantites = new int[6] { read.GetInt32(21), read.GetInt32(22), read.GetInt32(23), read.GetInt32(24), read.GetInt32(25), read.GetInt32(26) };
                machine.abattements = new int[25] {read.GetInt32(27), read.GetInt32(28), read.GetInt32(29), read.GetInt32(30), read.GetInt32(31), read.GetInt32(32),
                                                read.GetInt32(33), read.GetInt32(34), read.GetInt32(35), read.GetInt32(36), read.GetInt32(37), read.GetInt32(38),
                                                read.GetInt32(39), read.GetInt32(40), read.GetInt32(41), read.GetInt32(42), read.GetInt32(43), read.GetInt32(44),
                                                read.GetInt32(45), read.GetInt32(46), read.GetInt32(47), read.GetInt32(48), read.GetInt32(49), read.GetInt32(50), read.GetInt32(51) };

                MachinesImpression.Add(machine);

            }

            read.Close();
            _command.Dispose();

        }

        public static void LoadOperationsMachines(String _societe)
        {
            iDB2Connection cn1 = DBConnection.GetDBConnection();

            string _sql = "SELECT A.RSECT, trim(A.RSLIB), A.RSLB6, A.RSATE , B.ROPE, trim(c.olib), c.olib6 " +
                        " , otypfr, B.SOPRMA, B.SOPRMO, B.SOTPST, B.SOCAST, " +          
                        " B.SOTPMI, trim(B.SOCOMM), B.SOGACH, B.SOGACF , SOSEQF FROM c800f a join d100f b on " +
                        " a.rsoc=b.rsoc and a.rsect=b.rsect and SOTAG=' ' join d000f c on b.rsoc=c.rsoc and " +
                        " b.rope=c.rope and onatop in ('C', 'T', 'A', 'L') and OTAG=' ' " +
                        " WHERE a.rsoc=" + _societe + " and A.RSTAG= ' '  " +                        
                        " and (b.rsect concat digits(b.rope) in " +   
                        " (select lcref from c50011 where rsoc=" + _societe + " and lctypl='O') " +  
                        " or b.rsect concat digits(b.rope) in  " +
                        " (select liref from c50021 where rsoc=" + _societe + " and litypl='O')) " + 
                        " order by b.rsect, b.rope ";



            iDB2Command _command = new iDB2Command(_sql, cn1);
            _command.DeriveParameters();

            iDB2DataReader read = _command.ExecuteReader();

            OperationsMachines = new List<OperationMachine>();
            String savSect = String.Empty;
            Section newSect = new Section(); ;

            while (read.Read())
            {
                if (read.GetString(0) != savSect)
                {
                    newSect = new Section { Id = read.GetString(0), Nom = read.GetString(1), NomCourt=read.GetString(2), Atelier=read.GetString(3) };
                    savSect = read.GetString(0);
                }
                
                OperationMachine ope = new OperationMachine();
                ope.section = newSect;
                ope.Operation = read.GetInt32(4);
                ope.Nom = read.GetString(5);
                ope.TypeFrais = read.GetString(7);
                ope.TauxHoraireMachine = read.GetDecimal(8);
                ope.TauxHoraireMO = read.GetDecimal(9);
                ope.TempsStandard = read.GetDecimal(10);
                ope.CadenceStandard = read.GetInt32(11);
                ope.TempsMini = read.GetDecimal(12);
                ope.Commentaire = read.GetString(13);
                ope.AvecGacheFixe = read.GetString(15) == "N";
                ope.AvecGacheVariable = read.GetString(14) == "N";
                ope.Sequence = read.GetInt32(16);

                OperationsMachines.Add(ope);

            }

            read.Close();
            _command.Dispose();

        }


        public static MachineImpression GetMachineImpression(String _code)
        {
            
            try
            {
                return MachinesImpression.Single(e => e.Id == _code);
            }
            catch (Exception)
            {
                Trace.TraceError(   "Machine " + _code + " non trouvée");
                return new MachineImpression { Id = "---", Atelier = "1", Nom = "Non Trouvée", NomCourt = "INEXST", Type = TypeMachineImpression.plat };
            }
        }

        public static OperationMachine GetOperationMachine(String _section, int _ope)
        {
            try
            {
            return OperationsMachines.Single(e => e.section.Id == _section && e.Operation==_ope);
            }
            catch (Exception)
            {
                Trace.TraceError("Operation " + _section + _ope.ToString().PadLeft(3) + " non trouvée");
                return null;
            }
        }
        public static OperationMachine GetOperationMachine(String _sectionope)
        {
            try
            {
                String _section = _sectionope.Substring(0, 3);
                int _ope = int.Parse(_sectionope.Substring(3, 3));
                return GetOperationMachine(_section, _ope);
            }
            catch (Exception)
            {
                Trace.TraceError("Operation " + _sectionope + "non trouvée");
                return null;
            }
        }

    }
}
