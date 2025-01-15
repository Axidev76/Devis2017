using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
//using DevExpress.UserSkins;
//using DevExpress.Skins;
using Net.Axilog.BLL;

namespace Devis2017
{
    static class Program
    {
        internal static Dictionary<string, string> GlobalVariables = new Dictionary<string, string>();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //BonusSkins.Register();
            //SkinManager.EnableFormSkins();

            AppService.LoadContext();

            //Application.Run(new Form1());
        }
    }
}
