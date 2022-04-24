using System;
using System.Collections.Generic;
using System.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Dijital_Modul.Pages.Class
{
    class Functions
    {

        public string[] FunctionControl(string gelenkod, string kod, string parametre)
        {
            kod = ""; 
            parametre = "";
            int baslangic = 0, bitis = 0;
            for (int i = 0; i < gelenkod.Length - 1; i++)
            {
                if (gelenkod[i] == '(')
                {
                    baslangic = i;
                }
                else if (gelenkod[i] == ')')
                {
                    bitis = i;
                }
            }

            for (int i = 0; i < gelenkod.Length - 1; i++)
            {
                if (i > baslangic && i < bitis)
                {
                    if (gelenkod[i] != '"')
                    {
                        parametre += gelenkod[i];
                    }
                }
                else
                {
                    if (gelenkod[i] != '(' && gelenkod[i] != '"' && gelenkod[i] != ')')
                    {
                        kod += gelenkod[i];
                    }
                }
            }

            return new string[] { kod, parametre };
        }


        public void SayfaAcik(Label txtMetotlar, Label txtIlerlemelerim, Label txtLiderTahtasi, Label txtRozetlerim, Label txtKullanimKilavuzu)
        {
            txtMetotlar.Foreground = Brushes.White;
            txtIlerlemelerim.Foreground = Brushes.White;
            txtLiderTahtasi.Foreground = Brushes.White;
            txtRozetlerim.Foreground = Brushes.White;
            txtKullanimKilavuzu.Foreground = Brushes.White;

            if (Prm.pageName == "Metotlar")
            {
                txtMetotlar.Foreground = Brushes.LightGreen;
            }
            else if (Prm.pageName == "Ilerlemelerim")
            {
                txtIlerlemelerim.Foreground = Brushes.LightGreen;
            }
            else if (Prm.pageName == "LiderTahtasi")
            {
                txtLiderTahtasi.Foreground = Brushes.LightGreen;
            }
            else if (Prm.pageName == "Rozetlerim")
            {
                txtRozetlerim.Foreground = Brushes.LightGreen;
            }
            else if (Prm.pageName == "KullanimKilavuzu")
            {
                txtKullanimKilavuzu.Foreground = Brushes.LightGreen;
            }

        }

        
        public void OgretmenSayfaAcik(Label txtMetotlar, Label txtOgrencilerim, Label txtLiderTahtasi, Label txtRozetler, Label txtSorular, Label txtKullanimKilavuzu)
        {
            txtMetotlar.Foreground = Brushes.White;
            txtOgrencilerim.Foreground = Brushes.White;
            txtLiderTahtasi.Foreground = Brushes.White;
            txtRozetler.Foreground = Brushes.White;
            txtSorular.Foreground = Brushes.White;
            txtKullanimKilavuzu.Foreground = Brushes.White;

            if (Prm.pageName == "Metotlar")
            {
                txtMetotlar.Foreground = Brushes.LightGreen;
            }
            else if (Prm.pageName == "Ogrencilerim")
            {
                txtOgrencilerim.Foreground = Brushes.LightGreen;
            }
            else if (Prm.pageName == "Lider_Tahtasi")
            {
                txtLiderTahtasi.Foreground = Brushes.LightGreen;
            }
            else if (Prm.pageName == "Rozetler")
            {
                txtRozetler.Foreground = Brushes.LightGreen;
            }
            else if (Prm.pageName == "Sorular")
            {
                txtSorular.Foreground = Brushes.LightGreen;
            }
            else if (Prm.pageName == "Kullanim_Kilavuzu")
            {
                txtKullanimKilavuzu.Foreground = Brushes.LightGreen;
            }

        }


        public string OgretmenSayfaAdiDegistir(string gelenSayfaAdi)
        {
            string gercekSayfaAdi = "";
            if (gelenSayfaAdi == "METOTLAR")
            {
                gercekSayfaAdi = "Metotlar";
            }
            else if (gelenSayfaAdi == "ÖĞRENCİLERİM")
            {
                gercekSayfaAdi = "Ogrencilerim";
            }
            else if (gelenSayfaAdi == "LİDER TAHTASI")
            {
                gercekSayfaAdi = "Lider_Tahtasi";
            }
            else if (gelenSayfaAdi == "ROZETLER")
            {
                gercekSayfaAdi = "Rozetler";
            }
            else if (gelenSayfaAdi == "KULLANIM KILAVUZU")
            {
                gercekSayfaAdi = "Kullanim_Kilavuzu";
            }
            return gercekSayfaAdi;
        }

    }
}
