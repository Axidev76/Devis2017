using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Axilog.Model.Devis;

namespace Net.Axilog.BLL
{
    class Precalculs
    {
                
        // essai avec des delegates. 
        private static decimal Calculer(Func<ElementDevis, RubriqueChoisie, decimal> calcul, ElementDevis _elem, RubriqueChoisie _rub)
        {

            return calcul(_elem, _rub);

        }
        public static decimal NCP(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.Refente;
        }

        public static decimal NPQ(ElementDevis _elem, RubriqueChoisie _rub)
        {
            if (_elem.RectoVersoIdentique)
                return Math.Max(_elem.CouleursRecto , _elem.CouleursVerso);
            else
                return _elem.CouleursRecto + _elem.CouleursVerso - _elem.CouleursBascule;
        }

        public static decimal NCR(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.CouleursRecto;
        }

        public static decimal NCV(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.CouleursVerso;
        }

        public static decimal NLA(ElementDevis _elem, RubriqueChoisie _rub)
        {
            if (_elem.RectoVersoIdentique)
                return Math.Max(_elem.CouleursRecto, _elem.CouleursVerso);
            else
                return _elem.CouleursRecto + _elem.CouleursVerso - _elem.CouleursBascule;
        }

        public static decimal NPO(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.Poses;
        }

        public static decimal NPG(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.Pages;
        }

        public static decimal NP2(ElementDevis _elem, RubriqueChoisie _rub)
        {
            if (_elem.Pages > 1) return _elem.Pages / 2.0M;
            else return 0.0M;
        }

        public static decimal F01(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.Poses + _elem.Pages;
        }

        public static decimal F02(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return (_elem.Poses + _elem.Pages)/2.0M;
        }

        public static decimal F03(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return (_elem.CouleursRecto+ _elem.CouleursVerso)*(_elem.Pages/ 2.0M);
        }

        public static decimal F04(ElementDevis _elem, RubriqueChoisie _rub)
        {
            decimal precalc = (_elem.CouleursRecto + _elem.CouleursVerso) * (_elem.Pages / 2.0M) * _elem.Poses;

            if (_elem.CouleursBascule==0) return precalc;
            else return precalc/2.0M;
        }

        public static decimal HAT(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.formatTirage.GetHauteurMM() + 150.0M;
        }

        public static decimal HT0(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.formatTirage.GetHauteurMM();
        }

        public static decimal HT1(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.formatTirage.GetHauteurMM() + 10.0M;
        }

        public static decimal LT0(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.formatTirage.Largeur;
        }

        public static decimal LT1(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.formatTirage.Largeur + 10.0M;
        }

        public static decimal PPC(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return Math.Min(_elem.formatTirage.Largeur, _elem.formatTirage.GetHauteurMM());
        }

        public static decimal PGC(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return Math.Max(_elem.formatTirage.Largeur, _elem.formatTirage.GetHauteurMM());
        }

        public static decimal EP1(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.support.epaisseur.GetEpaisseurEnMM();
        }

        public static decimal EP2(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.support.epaisseur.GetEpaisseurEn10MM();
        }

        public static decimal EP3(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _elem.support.epaisseur.GetEpaisseurEnMicron();
        }

        public static decimal CAR(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return (_elem.devis.formatFini.Largeur-2.0M)*(_elem.devis.formatFini.GetHauteurMM()-6.0M)*2.0M/1000000.0M;
        }

        public static decimal SU1(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _rub.Reponses[0] * _rub.Reponses[1] / 100.0M;
        }

        public static decimal SU2(ElementDevis _elem, RubriqueChoisie _rub)
        {
            return _rub.Reponses[0] * _rub.Reponses[1] / 1000.0M;
        }


        //TODO NCA
        //TODO NCC
        //TODO REG
        //TODO NFC
        //TODO NLC
        //TODO ETU
        //TODO FOU
        //TODO POC
        //TODO FAU
        //TODO BCO
        





    }
}
