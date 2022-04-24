using Dijital_Modul.Pages.TeacherWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Dijital_Modul.Pages.Class
{
    public class Prm
    {

        public static TeacherMain gk = (TeacherMain)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

        public static string BilgiMesajiAlani;
        public static sbyte Hata;

        public static string pageName;
        public static string BelgelerimYolu = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString();


        public static string soruNo;
        public static string cevap;
        public static string soru_acilma_zamani;

        #region Oturum
        public static DateTime sisteme_giris_zamani;


        public static Grid anaGrid;

        public static TextBlock txbMyScore;
        public static double myScore;

        // Öğretmen
        public static List<classAndBranches> sinifVeSubelerim;

        
        public static int kullanici_ID;
        public static string kullanici_No;
        public static string ad;
        public static string soyad;
        public static string cinsiyet;
        public static string okul_id;
        public static string yetki_id;
        public static string sinif_id;
        public static int ogretmenimID;
        #endregion

    }
}
