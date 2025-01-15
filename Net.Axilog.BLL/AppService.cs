using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ini;
using Net.Axilog.DAL;
using System.Threading;

namespace Net.Axilog.BLL
{
    public static class AppService
    {
        internal static Dictionary<string, string> GlobalVariables = new Dictionary<string, string>();

        public static void LoadContext()
        {
            Thread myThread;

            // Instanciation du thread, on spécifie dans le 
            // délégué ThreadStart le nom de la méthode qui
            // sera exécutée lorsque l'on appele la méthode
            // Start() de notre thread.
            myThread = new Thread(new ThreadStart(LoadParametres));

            // Lancement du thread
            myThread.Start();

        }
        
        private static void LoadParametres() {
            Console.WriteLine("Demarrage du thread");

            IniFile ini = new IniFile(".\\Netprint.ini");
            string iniPath = ini.path;
            GlobalVariables["installDir"]=ini.IniReadValue("AXILOG", "InstallDir");
            string IP = ini.IniReadValue("GUISys TN5250", "Host");
            GlobalVariables["IPServeur"]= IP;
            ini = new IniFile(".\\Parametre.ini");
            String bibliothequePgm = ini.IniReadValue("PARAM", "BibliothequePgm");
            String profil = ini.IniReadValue("PARAM", "Profil");
            String bibliothequeFichier = ini.IniReadValue("PARAM", "Bibliotheque");
            String societe=ini.IniReadValue("PARAM", "SOCIETE");
            String mdp = ini.IniReadValue("PARAM", "MotDePasse");
            if (bibliothequePgm == String.Empty) bibliothequePgm = "NP11P";
            GlobalVariables["bibliotheque"]=bibliothequeFichier;
            GlobalVariables["bibliothequePgm"]= bibliothequePgm;
            GlobalVariables["profil"]=profil;
            GlobalVariables["sourceODBC"]=ini.IniReadValue("PARAM", "NomSource");
            GlobalVariables["societe"]=societe;
            GlobalVariables["cpteGen"]=ini.IniReadValue("PARAM", "CPTEGEN");
            GlobalVariables["enseigne"]=ini.IniReadValue("PARAM", "ENSEIGNE");
            string _connectionstring = @"Datasource=" + IP + "; UserID=" + profil.Trim().ToUpper() + "; Password=" + mdp.Trim().ToUpper() + "; DefaultCollection=" + bibliothequeFichier + "; LibraryList=" + bibliothequeFichier + "; pooling=True; ";
            GlobalVariables["connectionString"]=_connectionstring;

            DBConnection.OpenDBConnection(_connectionstring);

            //TODO Charger société, paramétre général, coefficients devis
            //TODO Charger type de produit, catégorie ? sorte sous-sorte
            //TODO Charger profil + autorisation
            //TODO Charger fabricants, représentant, transporteurs + tarif
            ParametrageDevisRepository.LoadParametresGeneraux(societe);
            ParametrageDevisRepository.LoadTypesProduit(societe);
            ParametrageDevisRepository.LoadUnitesVente(societe);
            SupportRepository.LoadSousSortes(societe);
            MachineRepository.LoadMachinesImpression(societe);
            MachineRepository.LoadOperationsMachines(societe);
            MatiereRepository.LoadMatieres(societe);
            SousTraitanceRepository.LoadOperationsSousTraitance(societe);
            ParametrageDevisRepository.LoadReglesCalcul(societe);
            ParametrageDevisRepository.LoadRubriques(societe);
            ParametrageDevisRepository.LoadRubriquesImpression(societe);
            ParametrageDevisRepository.LoadTableauxCalcul(societe);
            ParametrageDevisRepository.LoadTableauxRubrique(societe);

            Console.WriteLine("Fin du thread");
        }
    }
}
