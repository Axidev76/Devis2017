using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Axilog.Model.Devis;

namespace Net.Axilog.BLL
{
    class RechercheTableau
    {
        public static decimal GR(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.support.grammage;
        }
        public static decimal E1(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.support.epaisseur.GetEpaisseurEnMM();
        }
        public static decimal E2(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.support.epaisseur.GetEpaisseurEn10MM();
        }
        public static decimal E3(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.support.epaisseur.GetEpaisseurEnMicron();
        }
        public static decimal NS(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.support.sousSorte.natureSupport;
        }
        public static decimal LF(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.GetFormatFini().Largeur;
        }
        public static decimal HF(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.GetFormatFini().GetHauteurMM();
        }
        public static decimal AB(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.GetFormatFini().Largeur + _elem.GetFormatFini().GetHauteurMM();
        }
        public static decimal H(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.GetFormatFini().Cote3;
        }
        public static decimal LT(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.formatTirage.Largeur;
        }
        public static decimal HT(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.formatTirage.GetHauteurMM();
        }
        public static decimal SF(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.GetFormatFini().GetSurfaceM2()*10000.0M;
        }
        public static decimal ST(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.formatTirage.GetSurfaceM2() * 10000.0M;
        }
        public static decimal TR(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.ToursUtiles;
        }
        public static decimal ML(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.MetrageUtile;
        }
        public static decimal PG(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.Pages;
        }
        public static decimal ID(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.ElementsIdentiques;
        }
        public static decimal QE(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.GetQuantite();
        }
        public static decimal FE(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.FeuillesUtiles;
        }
        public static decimal CR(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.CouleursRecto;
        }
        public static decimal CV(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.CouleursVerso;
        }
        public static decimal CB(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.CouleursBascule;
        }
        public static decimal NP(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.Poses;
        }
        public static decimal R1(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _rub.Reponses[0];
        }
        public static decimal R2(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _rub.Reponses[1];
        }
        public static decimal R3(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _rub.Reponses[2];
        }
        public static decimal R4(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _rub.Reponses[3];
        }
        public static decimal R5(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _rub.Reponses[4];
        }


        //TODO NF
        //TODO CA
        //TODO UC


    }
}
