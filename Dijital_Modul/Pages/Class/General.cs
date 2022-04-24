using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using Dijital_Modul.Pages.UserController;
using Dijital_Modul.Pages.TeacherUserControllers;
using System.IO;
using System.Windows.Media.Imaging;

namespace Dijital_Modul.Pages.Class
{
    
    public class General
    {
        MySqlConnection connect = new MySqlConnection("Server=2.59.117.106;Database=DijitalModul_05_06_2021;Uid=usr_DijitalModul;Pwd=DijitalModul2021;Charset=utf8");
        public static MySqlCommand cmd;
        public static MySqlDataReader reader;
        public static MySqlDataAdapter adapter;

        public void Add_User_Log()
        {

            TimeSpan fark = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd H:m:s")).Subtract(Convert.ToDateTime(Prm.sisteme_giris_zamani));

            cmd = new MySqlCommand($@"insert into kullanici_log (Giris_Zamani, Cikis_Zamani, 
                                        Sistemde_Kalma_Suresi, Kullanici_No) 
                                        values ('{Prm.sisteme_giris_zamani.ToString("yyyy-MM-dd H:m:s")}', '{DateTime.Now.ToString("yyyy-MM-dd H:m:s")}',
                                        '{fark}','{Prm.kullanici_No}')",connect);
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0001 - {ex.GetType()}","Hata",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }
            
        }
        public string[] Login(string userNo, string password)
        {
            MySqlCommand cmd2, cmd3;
            string[] donenDeger = new string[2];
            cmd = new MySqlCommand($@"Select * from kullanicilar where Kullanici_No='{userNo}' and Sifre='{password}'", connect);
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    donenDeger[0] = "1";
                    Prm.kullanici_ID = Convert.ToInt32(reader["ID"]);
                    Prm.kullanici_No = reader["Kullanici_No"].ToString();
                    Prm.ad = reader["Ad"].ToString();
                    Prm.soyad = reader["Soyad"].ToString();
                    Prm.cinsiyet = reader["Cinsiyet"].ToString();
                    Prm.okul_id = reader["Okul_ID"].ToString();
                    Prm.yetki_id = reader["Yetki_ID"].ToString();
                    connect.Dispose();
                    cmd2 = new MySqlCommand($@"select ks.Kullanicilar_ID, ks.Siniflar_ID 'Sinif_ID' 
                                                from kullanici_sinif ks inner join kullanicilar k 
                                                on ks.Kullanicilar_ID = k.ID 
                                                where k.Kullanici_No='{userNo}'", connect);
                    connect.Open();
                    reader = cmd2.ExecuteReader();
                    if (reader.Read())
                    {
                        Prm.sinif_id = reader["Sinif_ID"].ToString();
                    }
                    connect.Dispose();

                    cmd3 = new MySqlCommand($"select k.ID from kullanici_sinif ks " +
                        $"inner join kullanicilar k on k.ID = ks.Kullanicilar_ID " +
                        $"where ks.Siniflar_ID={Prm.sinif_id} and k.Yetki_ID=1",connect);
                    connect.Open();
                    reader = cmd3.ExecuteReader();
                    if (reader.Read())
                    {
                        Prm.ogretmenimID = Convert.ToInt32(reader["ID"]);
                    }
                    connect.Dispose();
                }
                else
                {
                    donenDeger[0] = "0";
                    connect.Dispose();
                }
                connect.Dispose();
            }
            catch(MySqlException)
            {
                donenDeger[1] = "Veritabanına bağlanılamadı...";
            }
            finally
            {
                connect.Dispose();
            }
            return donenDeger;
        }
        public List<question> Unanswered_Questions(string kullaniciNo, bool quesStatus)
        {
            List<question> questionList = new List<question>();
            if (quesStatus)
            {
                cmd = new MySqlCommand($@" Select s.ID,s.Soru_No, s.Resim,s.ResimSize, s.Aciklama, s.Max_Puan, s.Acilma_Zamani, 
                                            s.Ekleme_Zamani, s.Soru_Turu, s.Soru_Acik_Mi, kb.Konu_Basligi_Adi 'Baslik'
                                            from sorular s inner join Konu_Basliklari kb 
                                            on kb.ID = s.Konu_Basliklari_ID where s.Soru_No 
                                            Not In (Select Soru_no from cevaplar where Kullanici_No='{kullaniciNo}') and 
                                            s.Soru_Acik_Mi=1 and s.Ilerlemelerime_Ekle = 1 and s.Ekleyen in (0,{Prm.ogretmenimID}) 
                                            group by s.Soru_No  order by s.ID desc", connect);
            }
            else
            {
                cmd = new MySqlCommand($@" Select s.ID,s.Soru_No, s.Resim,s.ResimSize, s.Aciklama, s.Max_Puan, s.Acilma_Zamani, 
                                            s.Ekleme_Zamani, s.Soru_Turu, s.Soru_Acik_Mi, kb.Konu_Basligi_Adi 'Baslik'
                                            from sorular s inner join Konu_Basliklari kb 
                                            on kb.ID = s.Konu_Basliklari_ID where s.Soru_No 
                                            Not In (Select Soru_no from cevaplar where Kullanici_No='{kullaniciNo}') and 
                                            s.Soru_Acik_Mi=0 and s.Ilerlemelerime_Ekle = 1 and s.Ekleyen in (0, {Prm.ogretmenimID})
                                            group by s.Soru_No  order by s.ID desc", connect);
            }
            
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    question ques = new question();
                    ques.soruNo = reader["Soru_No"].ToString();
                    ques.baslik = reader["Baslik"].ToString();
                    if(reader["ResimSize"].ToString() != null)
                        if (Convert.ToInt32(reader["ResimSize"]) > 0)
                        {
                            int FilseSize = Convert.ToInt32(reader["ResimSize"].ToString());
                            ques.imgArray = new byte[FilseSize];
                            reader.GetBytes(reader.GetOrdinal("Resim"), 0, ques.imgArray, 0, FilseSize);
                        }
                    ques.aciklama = reader["Aciklama"].ToString();
                    ques.maxPuan = Convert.ToInt32(reader["Max_Puan"]);
                    if (!string.IsNullOrEmpty(reader["Acilma_Zamani"].ToString()))
                    {
                        ques.acilmaZamani = Convert.ToDateTime(reader["Acilma_Zamani"]);
                    }
                    ques.eklemeZamani = Convert.ToDateTime(reader["Ekleme_Zamani"]);
                    ques.soruTuru = reader["Soru_turu"].ToString();
                    ques.soruAcikMi = Convert.ToBoolean(reader["Soru_Acik_Mi"]);
                    
                    questionList.Add(ques);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0003 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }
            return questionList;
        }

        public void Update_Score()
        {
            string score_query = $"select Sum(Puan) 'Puan' from cevaplar where Kullanici_No='{Prm.kullanici_No}'";
            Prm.myScore = TekilVeriCekInt(score_query, "Puan");
            Prm.txbMyScore.Text = Prm.myScore.ToString();
        }
        public List<question> Answered_Questions(string kullaniciNo, bool badgeQueryStatus)
        {
            List<question> questionList = new List<question>();
            if(badgeQueryStatus)
            {
                cmd = new MySqlCommand($@" Select s.ID, s.Soru_No, s.Resim,s.ResimSize, s.Aciklama, s.Max_Puan, s.Acilma_Zamani, s.Ekleme_Zamani, s.Soru_Turu, s.Soru_Acik_Mi, kb.Konu_Basligi_Adi 'Baslik'
                                        from sorular s 
                                        inner join Konu_Basliklari kb on kb.ID = s.Konu_Basliklari_ID 
                                        inner join cevaplar c on c.Soru_No = s.Soru_No
                                        where s.Soru_No In (Select Soru_no from cevaplar where Kullanici_No = '{kullaniciNo}') 
                                        and c.Kullanici_No='{kullaniciNo}' and 
                                        s.Ekleyen in (0,{Prm.ogretmenimID}) and c.Puan>=s.Max_Puan*.7 group by s.Soru_No order by s.Soru_No", connect);
            }
            else
            {
                cmd = new MySqlCommand($@" Select s.ID, s.Soru_No, s.Resim,s.ResimSize, s.Aciklama, s.Max_Puan, s.Acilma_Zamani, 
                                        s.Ekleme_Zamani, s.Soru_Turu, s.Soru_Acik_Mi, kb.Konu_Basligi_Adi 'Baslik'
                                        from sorular s 
                                        inner join Konu_Basliklari kb on kb.ID = s.Konu_Basliklari_ID 
                                        inner join cevaplar c on c.Soru_No = s.Soru_No
                                        where s.Soru_No In (Select Soru_no from cevaplar where Kullanici_No = '{kullaniciNo}') and 
                                        c.Kullanici_No='{kullaniciNo}' and 
                                        s.Ekleyen in (0,{Prm.ogretmenimID}) group by s.Soru_No order by s.Soru_No", connect);
            }
          
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    question ques = new question();
                    ques.soruID = reader["ID"].ToString();
                    ques.soruNo = reader["Soru_No"].ToString();
                    ques.baslik = reader["Baslik"].ToString();
                    if (Convert.ToInt32(reader["ResimSize"]) > 0)
                    {
                        int FilseSize = Convert.ToInt32(reader["ResimSize"].ToString());
                        ques.imgArray = new byte[FilseSize];
                        reader.GetBytes(reader.GetOrdinal("Resim"), 0, ques.imgArray, 0, FilseSize);
                    }
                    ques.aciklama = reader["Aciklama"].ToString();
                    ques.maxPuan = Convert.ToInt32(reader["Max_Puan"]);
                    if (!string.IsNullOrEmpty(reader["Acilma_Zamani"].ToString()))
                    {
                        ques.acilmaZamani = Convert.ToDateTime(reader["Acilma_Zamani"]);
                    }
                    ques.eklemeZamani = Convert.ToDateTime(reader["Ekleme_Zamani"]);
                    ques.soruTuru = reader["Soru_turu"].ToString();
                    ques.soruAcikMi = Convert.ToBoolean(reader["Soru_Acik_Mi"]);

                    questionList.Add(ques);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0004 - {ex.GetType()}","Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                connect.Dispose();
            }
            return questionList;
            
        }


        public List<question> A_Asking_Question(string query)
        {
            List<question> quesList = new List<question>();
            cmd = new MySqlCommand(query,connect);
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    question ques = new question();
                    ques.soruNo = reader["Soru_No"].ToString();
                    ques.baslik = reader["Baslik"].ToString();
                    //ques.resim = reader["Resim"].ToString();
                     if ( Convert.ToInt32(reader["ResimSize"])>0)
                    {
                        int FilseSize = Convert.ToInt32(reader["ResimSize"].ToString());
                        ques.imgArray = new byte[FilseSize];
                        reader.GetBytes(reader.GetOrdinal("Resim"), 0, ques.imgArray, 0, FilseSize);
                    } 
                    ques.aciklama = reader["Aciklama"].ToString();
                    ques.maxPuan = Convert.ToInt32(reader["Max_Puan"]);
                    if (!string.IsNullOrEmpty(reader["Acilma_Zamani"].ToString()))
                    {
                        ques.acilmaZamani = Convert.ToDateTime(reader["Acilma_Zamani"]);
                    }
                    ques.eklemeZamani = Convert.ToDateTime(reader["Ekleme_Zamani"]);
                    ques.soruTuru = reader["Soru_turu"].ToString();
                    ques.soruAcikMi = Convert.ToBoolean(reader["Soru_Acik_Mi"]);
                    quesList.Add(ques);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0005 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }
            return quesList;
        }
        public List<feedback> A_Asking_FeedBack(string soruNo, string kullaniciNo)
        {
            List<feedback> fbList = new List<feedback>();
            cmd = new MySqlCommand($@"select * from donutler where Soru_No='{soruNo}' and Kullanici_No='{kullaniciNo}'",connect);
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    feedback feed = new feedback();
                    feed.soruNo = reader["Soru_No"].ToString();
                    feed.cevapNo = reader["Cevap_ID"].ToString();
                    feed.donut = reader["Donut"].ToString();
                    feed.kullaniciNo = reader["Kullanici_No"].ToString();
                    feed.donutZamani = Convert.ToDateTime(reader["Donut_Zamani"]);
                    fbList.Add(feed);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0006 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }

            return fbList;
        }
        public List<answer> A_Asking_Answer(string soruNo, string kullaniciNo)
        {
            List<answer> ansList = new List<answer>();
            cmd = new MySqlCommand($@"select * from cevaplar where Soru_No='{soruNo}' and Kullanici_No='{kullaniciNo}'",connect);
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    answer ans = new answer();
                    ans.answerID = reader["ID"].ToString();
                    ans.soruNo = reader["Soru_No"].ToString();
                    ans.kullaniciNo = reader["Kullanici_No"].ToString();
                    ans.cevap = reader["Cevap"].ToString();
                    ans.soruAcilmaZamani = Convert.ToDateTime(reader["Soru_Acilma_Zamani"]);
                    ans.cevapGondermeZamani = Convert.ToDateTime(reader["Cevap_Gonderme_Zamani"]);
                    ans.cevapGondermeSuresi = reader["Cevap_Gonderme_Suresi"].ToString();
                    if (!string.IsNullOrEmpty(reader["Puan"].ToString()))
                    {
                        ans.puan = Convert.ToInt32(reader["Puan"]);
                    }
                    ansList.Add(ans);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0007 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }
            return ansList;
        }
        public void LoadPageStudent(Grid grd,string pn)
        {
           switch(pn)
            {
                case "Metotlar":
                    Prm.pageName = "Metotlar";
                    uc_cagir.uc_Ekle(grd, new ucStudentMethods());
                    break;

                case "Ilerlemelerim":
                    Prm.pageName = "Ilerlemelerim";
                    uc_cagir.uc_Ekle(grd, new MyProgress());
                    break;

                case "Cevaplarim":
                    Prm.pageName = "Cevaplarim";
                    uc_cagir.uc_Ekle(grd, new MyProgress());
                    break;

                case "Cevap_Vermediklerim":
                    Prm.pageName = "Cevap_Vermediklerim";
                    uc_cagir.uc_Ekle(grd, new MyProgress());
                    break;


                case "Lider_Tahtasi":
                    Prm.pageName = "Lider_Tahtasi";
                    uc_cagir.uc_Ekle(grd, new LeaderBoard());
                    break;

                case "Rozetlerim":
                    Prm.pageName = "Rozetlerim";
                    uc_cagir.uc_Ekle(grd, new MyBadges());
                    break;

                case "Kullanim_Kilavuzu":
                    Prm.pageName = "Kullanim_Kilavuzu";
                    uc_cagir.uc_Ekle(grd,new UserGuide());
                    break;

                   
                default:
                    MessageBox.Show("Sayfa bulunamadı","Uyarı",MessageBoxButton.OK,MessageBoxImage.Warning);
                    break;
            }
        }
        public void LoadPageTeacher(Grid grd, string pn)
        {
            switch (pn)
            {
                case "METOTLAR":
                case "Metotlar":
                    Prm.pageName = "Metotlar";
                    uc_cagir.uc_Ekle(grd, new ucTeacherMethods());
                    break;

                case "Ogrencilerim":
                case "ÖĞRENCİLERİM":
                    Prm.pageName = "Ogrencilerim";
                    List<classAndBranches> clsList = new List<classAndBranches>();
                    clsList = getMyClassesAndBranches();
                    if (clsList.Count() > 1) // Birden fazla şubesi varsa
                    {
                        uc_cagir.uc_Ekle(grd, new ucTeacherMyClasses());
                    }
                    else
                    {
                        uc_cagir.uc_Ekle(grd, new ucTeacherMyStudents(0));
                    }
                    break;

                case "Lider_Tahtasi":
                case "LİDER TAHTASI":
                    Prm.pageName = "Lider_Tahtasi";
                    uc_cagir.uc_Ekle(grd, new ucTeacherLeaderBoard());
                    break;

                case "Rozetler":
                case "ROZETLER":
                    Prm.pageName = "Rozetler";
                    uc_cagir.uc_Ekle(grd, new ucTeacherBadges());
                    break;


                case "Sorular":
                case "SORULAR":
                case "Tum_Sorular":
                    Prm.pageName = "Sorular";
                    uc_cagir.uc_Ekle(grd, new ucTeacherQuestions());
                    break;

                case "Sordugum_Sorular":
                    Prm.pageName = "Sordugum_Sorular";
                    uc_cagir.uc_Ekle(grd, new ucTeacherQuestions());
                    break;

                case "Mufredat_Sorulari":
                    Prm.pageName = "Mufredat_Sorulari";
                    uc_cagir.uc_Ekle(grd, new ucTeacherQuestions());
                    break;

                case "Kullanim_Kilavuzu":
                case "KULLANIM KILAVUZU":
                    Prm.pageName = "Kullanim_Kilavuzu";
                    uc_cagir.uc_Ekle(grd, new ucTeacherUserGuide());
                    break;

               

                default:
                    
                    MessageBox.Show("Sayfa bulunamadı!","Uyarı",MessageBoxButton.OK,MessageBoxImage.Warning);
                    break;
            }
        }
        public bool Add_Answer(string soruNo, string cevap)
        {
            TimeSpan fark = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd H:m:s")).Subtract(Convert.ToDateTime(Prm.soru_acilma_zamani));


            bool status = false;
            
            
            cmd = new MySqlCommand($@"insert into cevaplar 
                                        (Soru_No, Cevap, Kullanici_No, Soru_Acilma_Zamani, 
                                            Cevap_Gonderme_Zamani, Cevap_Gonderme_Suresi) 
                                        values('{soruNo}','{cevap}','{Prm.kullanici_No}',
                                                '{Prm.soru_acilma_zamani}',
                                                '{DateTime.Now.ToString("yyyy-MM-dd H:m:s")}',
                                                    '{fark}');",connect);
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                status = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0008 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            finally
            {
                connect.Dispose();
            }
            
            return status;
        }
        public void LeaderBoardEdit(List<String> kullaniciAdiList, List<int> kullaniciPuanList, int secim, int sinifID)
        {
            // 0 = Genel, 1 = Haftanın, 2 = Ayın
            if (secim == 0) // Genel sıralama
            {
                cmd = new MySqlCommand("SELECT  k.Ad as Ogrenci_adi ,sum(Puan) as puan, s.Sinif,s.Sube, o.Okul_Adi FROM cevaplar as c " +
                    "INNER JOIN kullanicilar as k on k.Kullanici_No = c.Kullanici_No " +
                    "inner join kullanici_sinif ks on ks.Kullanicilar_ID = k.ID " +
                    "inner join siniflar s on s.ID = ks.Siniflar_ID " +
                    "inner join okul_sinif os on os.Sinif_ID = ks.Siniflar_ID " +
                    $"inner join okullar o on o.ID = os.Okul_ID where puan is not null and s.ID = {sinifID} " +
                    "group by c.Kullanici_No order by puan desc ", connect);
            }
            else
            {
                string baslangic;
                string bugun = DateTime.Today.ToString("yyyy-MM-dd 23:59:59");
                if (secim == 1) // Haftanın öğrencisi
                {
                    baslangic = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd 00:00:00");
                    
                }
                else // Ayın öğrencisi
                {
                    baslangic = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd 00:00:00");
                }


                cmd = new MySqlCommand("SELECT  k.Ad as Ogrenci_adi ,sum(c.Puan) as puan, s.Sinif,s.Sube, o.Okul_Adi FROM cevaplar as c " +
                    "INNER JOIN kullanicilar as k on k.Kullanici_No = c.Kullanici_No " +
                    "inner join kullanici_sinif ks on ks.Kullanicilar_ID = k.ID " +
                    "inner join siniflar s on s.ID = ks.Siniflar_ID " +
                    "inner join okul_sinif os on os.Sinif_ID = ks.Siniflar_ID " +
                    "inner join okullar o on o.ID = os.Okul_ID " +
                    $"where puan is not null and c.Cevap_Gonderme_Zamani between '{baslangic}' and '{bugun}' and s.ID = {sinifID} " +
                    "group by c.Kullanici_No order by puan desc LIMIT 1", connect);
            }

            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["puan"] != null || reader["puan"].ToString() != "")
                    {
                        kullaniciAdiList.Add(reader["Ogrenci_adi"].ToString());
                        kullaniciPuanList.Add(Convert.ToInt32(reader["puan"]));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0009 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }
        }
        public List<badge> studentMyAllBadges()
        {
            List<badge> badgeList = new List<badge>();
            cmd = new MySqlCommand($"Select * from rozetler where Ekleyen={Prm.ogretmenimID}",connect);
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
               
                while (reader.Read())
                {
                    int rozetID;
                    badge b = new badge();
                    b.rozetID = Convert.ToInt32(reader["ID"]);
                    b.rozetAdi = reader["Rozet_Adi"].ToString();

                    if (Convert.ToInt32(reader["FileSize"]) > 0)
                    {
                        int FilseSize = Convert.ToInt32(reader["FileSize"].ToString());
                        b.denemeImageByte = new byte[FilseSize];
                        reader.GetBytes(reader.GetOrdinal("Resim"), 0, b.denemeImageByte, 0, FilseSize);
                        byte[] a = b.denemeImageByte;

                    }

                    b.resim = reader["Resim"].ToString();
                    b.aciklama = reader["Aciklama"].ToString();
                    if (!string.IsNullOrEmpty(reader["Bitis_Suresi"].ToString()))
                    {
                        b.bitisSuresi = Convert.ToDateTime(reader["Bitis_Suresi"]);
                    }
                    b.eklemeZamani = Convert.ToDateTime(reader["Ekleme_Zamani"]);

                    rozetID = b.rozetID;
                    MySqlConnection connect2 = new MySqlConnection("Server=2.59.117.106;Database=DijitalModul_05_06_2021;Uid=usr_DijitalModul;Pwd=DijitalModul2021;Charset=utf8");
                   
                    MySqlCommand cmd2 = new MySqlCommand($"Select s.Soru_No from sorular s inner join soru_rozet sr on s.ID = sr.Sorular_ID where sr.Rozetler_ID={rozetID}",connect2);
                    connect2.Open();
                    MySqlDataReader reader2;
                    reader2 = cmd2.ExecuteReader();
                    b.soruListesi = new List<string>();
                    while (reader2.Read())
                    {
                        b.soruListesi.Add(reader2["Soru_No"].ToString());
                    }
                    
                    badgeList.Add(b);
                    connect2.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0010 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }

            return badgeList;
        }
        public List<badge> teacherMyAllBadges()
        {
            List<badge> badgeList = new List<badge>();
            cmd = new MySqlCommand($"Select * from rozetler where Ekleyen={Prm.ogretmenimID}", connect);
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int rozetID;
                    badge b = new badge();
                    b.rozetID = Convert.ToInt32(reader["ID"]);
                    b.rozetAdi = reader["Rozet_Adi"].ToString();


                    //b.denemeImageByte = Encoding.ASCII.GetBytes(reader["Resim"].ToString()); // reader.GetBytes();
                    if ( Convert.ToInt32(reader["FileSize"])>0)
                    {
                        int FilseSize = Convert.ToInt32(reader["FileSize"].ToString());
                        b.denemeImageByte = new byte[FilseSize];
                        reader.GetBytes(reader.GetOrdinal("Resim"), 0, b.denemeImageByte, 0, FilseSize);
                        byte[] a =b.denemeImageByte;
                        
                    }
                    

                    
                    b.resim = reader["Resim"].ToString();
                    
                    b.aciklama = reader["Aciklama"].ToString();
                    if (!string.IsNullOrEmpty(reader["Bitis_Suresi"].ToString()))
                    {
                        b.bitisSuresi = Convert.ToDateTime(reader["Bitis_Suresi"]);
                    }
                    b.eklemeZamani = Convert.ToDateTime(reader["Ekleme_Zamani"]);

                    rozetID = b.rozetID;
                    MySqlConnection connect2 = new MySqlConnection("Server=2.59.117.106;Database=DijitalModul_05_06_2021;Uid=usr_DijitalModul;Pwd=DijitalModul2021;Charset=utf8");

                    MySqlCommand cmd2 = new MySqlCommand($"Select s.Soru_No from sorular s inner join soru_rozet sr on s.ID = sr.Sorular_ID where sr.Rozetler_ID={rozetID}", connect2);
                    connect2.Open();
                    MySqlDataReader reader2= cmd2.ExecuteReader();
                   
                    b.soruListesi = new List<string>();
                    while (reader2.Read())
                    {
                        b.soruListesi.Add(reader2["Soru_No"].ToString());
                    }

                    badgeList.Add(b);
                    connect2.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0011 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }

            return badgeList;
        }
        public byte[] converSmallerImageByteArray(string imagePath)
        {
            byte[] data;
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(@"" + imagePath);
            bitmap.EndInit();
            bitmap = makeSmallerBitmap(bitmap, imagePath);

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }
        private BitmapImage makeSmallerBitmap(BitmapImage image, string imagePath)
        {
            double height = image.Height;
            double width = image.Width;
            int maximumSize = 200;

            double bitmapRatio = width / height;
            if (bitmapRatio > 1)
            {
                //Yatay Görsel
                width = maximumSize;
                double scaleHeight = width / bitmapRatio;
                height = scaleHeight;
            }
            else
            {
                //Dikey Görsel
                height = maximumSize;
                double scaleWidth = height / bitmapRatio;
                width = Convert.ToInt32(scaleWidth);
            }

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.DecodePixelWidth = Convert.ToInt32(width);
            bitmap.DecodePixelHeight = Convert.ToInt32(height);
            bitmap.UriSource = new Uri(@"" + imagePath);
            bitmap.EndInit();
            return bitmap;
        }
        public bool badgeQuesStatus(string soruNo, string kullaniciNo)
        {
            bool status = false;
            cmd = new MySqlCommand($"Select * from cevaplar where Soru_No='{soruNo}' and Kullanici_No='{kullaniciNo}'",connect);
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    status = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0012 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }
            return status;
        }
        public List<question> quesBadges(int badgeID) // Rozet için gerekli sorular
        {
            List<question> quesList = new List<question>();
            cmd = new MySqlCommand("Select sr.Sorular_ID, s.Soru_No, sr.Rozetler_ID from soru_rozet sr " +
                "inner join sorular s on s.ID = sr.Sorular_ID " +
                $"where sr.Rozetler_ID = {badgeID}",connect);
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    question ques = new question();
                    ques.soruID = reader["Sorular_ID"].ToString();
                    ques.soruNo = reader["Soru_No"].ToString();
                    quesList.Add(ques);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0013 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }

            return quesList;
        }
        public List<student> getMyStudents()
        {
            List<student> studentList = new List<student>();
            cmd = new MySqlCommand("Select k.ID, k.Kullanici_No, k.Ad, k.Soyad, k.Cinsiyet, s.Sinif, s.Sube " +
                "from kullanicilar k inner join okullar o on k.Okul_ID = o.ID " +
                "inner join kullanici_sinif ks on ks.Kullanicilar_ID = k.ID " +
                "inner join siniflar s on s.ID = ks.Siniflar_ID " +
                $"where k.Yetki_ID = 2 and k.Okul_ID = {Prm.okul_id} and ks.Siniflar_ID in (Select Siniflar_ID from kullanici_sinif where Kullanicilar_ID = {Prm.kullanici_ID})",connect);

            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    student s = new student();
                    s.id = Convert.ToInt32(reader["ID"]);
                    s.kullaniciNo = Convert.ToInt32(reader["Kullanici_No"]);
                    s.ad = reader["Ad"].ToString();
                    s.soyad = reader["Soyad"].ToString();
                    s.cinsiyet = reader["Cinsiyet"].ToString();
                    s.sinif = reader["Sinif"].ToString();
                    s.sube = reader["Sube"].ToString();
                    studentList.Add(s);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0014 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                MessageBox.Show(ex.ToString()); ;
            }
            finally
            {
                connect.Dispose();
            }

            return studentList;
        }
        public int TekilVeriCekInt(string sorgu, string istenen)
        {
            int sonuc = 0;
            cmd = new MySqlCommand(sorgu, connect);
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if(!string.IsNullOrEmpty(reader[istenen].ToString()))
                        sonuc = Convert.ToInt32(reader[istenen]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0015 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }
            return sonuc;
        }
        public List<classAndBranches> getMyClassesAndBranches()
        {
            List<classAndBranches> clsList = new List<classAndBranches>();
            cmd = new MySqlCommand("Select count(*) Adet, s.Sinif, s.Sube, s.ID 'Sinif_ID' from kullanicilar k " +
                "inner join okullar o on k.Okul_ID = o.ID " +
                "inner join kullanici_sinif ks on ks.Kullanicilar_ID = k.ID " +
                "inner join siniflar s on s.ID = ks.Siniflar_ID " +
                $"where k.Yetki_ID = 2 and k.Okul_ID = {Prm.okul_id} and " +
               $"ks.Siniflar_ID in (Select Siniflar_ID from kullanici_sinif where Kullanicilar_ID = {Prm.kullanici_ID}) " +
                "group by s.Sinif,s.Sube ", connect);

            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    classAndBranches cl = new classAndBranches();
                    cl.sinifID = Convert.ToInt32(reader["Sinif_ID"]);
                    cl.adet = Convert.ToInt32(reader["Adet"]);
                    cl.sinif = reader["Sinif"].ToString();
                    cl.sube = reader["Sube"].ToString();
                    clsList.Add(cl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0016 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }

            return clsList;
        }
        public List<student> getMyStudents(int sinifID, int okulID)
        {
            List<student> studentList = new List<student>();
            cmd = new MySqlCommand("Select k.ID, k.Kullanici_No, k.Ad, k.Soyad, " +
                "k.Cinsiyet, k.Okul_ID, k.Yetki_ID, s.Sinif, s.Sube from kullanicilar k " +
                "inner join kullanici_sinif ks " +
                "on ks.Kullanicilar_ID = k.ID " +
                "inner join siniflar s " +
                "on s.ID = ks.Siniflar_ID " +
                $"where s.ID = {sinifID} and k.Okul_ID = {okulID} and k.Yetki_ID = 2",connect);
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    student s = new student();
                    s.id = Convert.ToInt32(reader["ID"]);
                    s.kullaniciNo = Convert.ToInt32(reader["Kullanici_No"]);
                    s.ad = reader["Ad"].ToString();
                    s.soyad = reader["Soyad"].ToString();
                    s.cinsiyet = reader["Cinsiyet"].ToString();
                    s.sinif = reader["Sinif"].ToString();
                    s.sube = reader["Sube"].ToString();
                    studentList.Add(s);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0017 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }

            return studentList;
        }
        public List<question> getTeacherAllQuestion(int secim, int sinifID)
        {
            List<question> questionList = new List<question>();
            if (secim == 0) // Açık sorular Öğretmen + Sistem
            {
                cmd = new MySqlCommand($@" select s.ID, s.Soru_No, s.Resim, s.ResimSize, s.Aciklama, s.Max_Puan, s.Acilma_Zamani, s.Ekleme_Zamani, s.Soru_Turu, s.Soru_Acik_Mi, kb.Konu_Basligi_Adi 'Baslik' 
                                            from Soru_Sinif ss inner join sorular s on ss.Soru_ID = s.ID
                                            inner join Konu_Basliklari kb on kb.ID = s.Konu_Basliklari_ID
                                            where ss.Sinif_ID={sinifID} and s.Ilerlemelerime_Ekle=1 and s.Soru_Acik_Mi=1", connect);
                }
            else if (secim == 1) // Kapalı sorular Öğretmen + Sistem
            {
                cmd = new MySqlCommand($@"select s.ID, s.Soru_No, s.Resim, s.ResimSize,s.Aciklama, s.Max_Puan, s.Acilma_Zamani, s.Ekleme_Zamani, s.Soru_Turu, s.Soru_Acik_Mi, kb.Konu_Basligi_Adi 'Baslik' 
                                            from Soru_Sinif ss inner join sorular s on ss.Soru_ID = s.ID
                                            inner join Konu_Basliklari kb on kb.ID = s.Konu_Basliklari_ID
                                            where ss.Sinif_ID={sinifID} and s.Ilerlemelerime_Ekle=1 and s.Soru_Acik_Mi=0", connect);
            }
            else if (secim == 2) // Öğretmenin sorduğu açık sorular
            {
                cmd = new MySqlCommand($@" select s.ID, s.Soru_No, s.Resim,s.ResimSize, s.Aciklama, s.Max_Puan, s.Acilma_Zamani, s.Ekleme_Zamani, s.Soru_Turu, s.Soru_Acik_Mi, kb.Konu_Basligi_Adi 'Baslik' 
                                            from Soru_Sinif ss inner join sorular s on ss.Soru_ID = s.ID
                                            inner join Konu_Basliklari kb on kb.ID = s.Konu_Basliklari_ID
                                            where ss.Sinif_ID={sinifID} and s.Ilerlemelerime_Ekle=1 and s.Soru_Acik_Mi=1 and ss.Ekleyen={Prm.kullanici_ID}", connect);
            }
            else if (secim == 3) // Öğretmenin sorduğu kapalı sorular
            {
                cmd = new MySqlCommand($@" select s.ID, s.Soru_No, s.Resim,s.ResimSize, s.Aciklama, s.Max_Puan, s.Acilma_Zamani, s.Ekleme_Zamani, s.Soru_Turu, s.Soru_Acik_Mi, kb.Konu_Basligi_Adi 'Baslik' 
                                            from Soru_Sinif ss inner join sorular s on ss.Soru_ID = s.ID
                                            inner join Konu_Basliklari kb on kb.ID = s.Konu_Basliklari_ID
                                            where ss.Sinif_ID={sinifID} and s.Ilerlemelerime_Ekle=1 and s.Soru_Acik_Mi=0 and ss.Ekleyen={Prm.kullanici_ID}", connect);
            }
            else if (secim == 4) // Müfredattaki açık sorular
            {
                cmd = new MySqlCommand($@" select s.ID, s.Soru_No, s.Resim,s.ResimSize, s.Aciklama, s.Max_Puan, s.Acilma_Zamani, s.Ekleme_Zamani, s.Soru_Turu, s.Soru_Acik_Mi, kb.Konu_Basligi_Adi 'Baslik' 
                                            from Soru_Sinif ss inner join sorular s on ss.Soru_ID = s.ID
                                            inner join Konu_Basliklari kb on kb.ID = s.Konu_Basliklari_ID
                                            where ss.Sinif_ID={sinifID} and s.Ilerlemelerime_Ekle=1 and s.Soru_Acik_Mi=1 and ss.Ekleyen=0", connect);
            }
            else if (secim == 5) // Müfredattaki kapalı sorular
            {
                cmd = new MySqlCommand($@" select s.ID, s.Soru_No, s.Resim,s.ResimSize, s.Aciklama, s.Max_Puan, s.Acilma_Zamani, s.Ekleme_Zamani, s.Soru_Turu, s.Soru_Acik_Mi, kb.Konu_Basligi_Adi 'Baslik' 
                                            from Soru_Sinif ss inner join sorular s on ss.Soru_ID = s.ID
                                            inner join Konu_Basliklari kb on kb.ID = s.Konu_Basliklari_ID
                                            where ss.Sinif_ID={sinifID} and s.Ilerlemelerime_Ekle=1 and s.Soru_Acik_Mi=0 and ss.Ekleyen=0", connect);
            }
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    question ques = new question();
                    ques.soruID = reader["ID"].ToString();
                    ques.soruNo = reader["Soru_No"].ToString();
                    ques.baslik = reader["Baslik"].ToString();
                    if (Convert.ToInt32(reader["ResimSize"]) > 0)
                    {
                        int FilseSize = Convert.ToInt32(reader["ResimSize"].ToString());
                        ques.imgArray = new byte[FilseSize];
                        reader.GetBytes(reader.GetOrdinal("Resim"), 0, ques.imgArray, 0, FilseSize);
                    }
                    ques.aciklama = reader["Aciklama"].ToString();
                    ques.maxPuan = Convert.ToInt32(reader["Max_Puan"]);
                    ques.acilmaZamani = Convert.ToDateTime(reader["Acilma_Zamani"]);
                    ques.eklemeZamani = Convert.ToDateTime(reader["Ekleme_Zamani"]);
                    ques.soruTuru = reader["Soru_turu"].ToString();
                    ques.soruAcikMi = Convert.ToBoolean(reader["Soru_Acik_Mi"]);

                    questionList.Add(ques);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0018 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }
            return questionList;
        }
        public List<konubasligi> KonuBasliklariniGetir(string gelenKonuBasligi = "")
        {
            List<konubasligi> konuBasliklari = new List<konubasligi>();
            if (gelenKonuBasligi != "")
            {
                cmd = new MySqlCommand($"Select * from Konu_Basliklari Where Konu_Basligi_Adi='{gelenKonuBasligi}'", connect);
            }
            else
            {
                cmd = new MySqlCommand($"Select * from Konu_Basliklari", connect);
            }


           
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    konubasligi kb = new konubasligi();
                    kb.kbID = Convert.ToInt32(reader["ID"]);
                    kb.kbAdi = reader["Konu_Basligi_Adi"].ToString();
                    konuBasliklari.Add(kb);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0019 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }

            return konuBasliklari;
        }
        public bool TeacherUpdateQuestion(question ques, int konubasligiID, string duzenlenmisAcilmaZamani, string resimAdi="")
        {
            bool guncellemeDurum = false;
            int soruDurum;
            if (ques.soruAcikMi == true)
            {
                soruDurum = 1;
            }
            else
            {
                soruDurum = 0;
            }
            /*
            if (resimAdi.Length>0)
            {

                cmd = new MySqlCommand($"update sorular Set  Resim='{resimAdi}'," +
                                        $"Aciklama = '{ques.aciklama}'," +
                                        $"Max_Puan = {ques.maxPuan}, Acilma_Zamani = '{duzenlenmisAcilmaZamani}', Soru_Acik_Mi = {soruDurum}, " +
                                        $"Konu_Basliklari_ID = {konubasligiID} where Soru_No = '{ques.soruNo}'",connect);
            }
            else
            {
                cmd = new MySqlCommand($"update sorular Set Aciklama = '{ques.aciklama}',Max_Puan = {ques.maxPuan}, Acilma_Zamani = '{duzenlenmisAcilmaZamani}', Soru_Acik_Mi = {soruDurum}, Konu_Basliklari_ID = {konubasligiID} where Soru_No = '{ques.soruNo}'",connect);
            }
            */

            cmd = new MySqlCommand($"update sorular Set  Resim= @image, ResimSize= {ques.imgArray.Length}, "  + 
                                        $"Aciklama = '{ques.aciklama}'," +
                                        $"Max_Puan = {ques.maxPuan}, Acilma_Zamani = '{duzenlenmisAcilmaZamani}', Soru_Acik_Mi = {soruDurum}, " +
                                        $"Konu_Basliklari_ID = {konubasligiID} where Soru_No = '{ques.soruNo}'", connect);
            try
            {
                connect.Open();
                cmd.Parameters.Add(new MySqlParameter("@image", ques.imgArray));
                cmd.ExecuteNonQuery();
                guncellemeDurum = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0020 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                guncellemeDurum = false;
            }
            finally
            {
                connect.Dispose();
            }
            return guncellemeDurum;
        } 
        //Sorguda , yanlış koyuldu yapıldı
        public List<cevapVerenKullanici> SoruyaCevapVerenler(int sinifID, string soruNo)
        {
            List<cevapVerenKullanici> cvkList = new List<cevapVerenKullanici>();

            cmd = new MySqlCommand($"select k.ID 'Kullanici_ID', k.Kullanici_No, k.Ad, k.Soyad, k.Cinsiyet ,c.ID, c.Puan, " +
                $"'Cevap_ID', c.Soru_No, c.Cevap, c.Kullanici_No, c.Soru_Acilma_Zamani, c.Cevap_Gonderme_Zamani ,c.Cevap_Gonderme_Suresi " +
                $"from cevaplar c inner " +
                $"join sorular s on s.Soru_No = c.Soru_No " +
                $"inner join kullanicilar k on k.Kullanici_No = c.Kullanici_No " +
                $"inner join kullanici_sinif ks on ks.Kullanicilar_ID = k.ID " +
                $"where ks.Siniflar_ID = {sinifID} and k.Okul_ID = {Prm.okul_id} and c.Soru_No = '{soruNo}'", connect);
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cevapVerenKullanici cvk = new cevapVerenKullanici();
                    cvk.ogrenci.id = Convert.ToInt32(reader["Kullanici_ID"]);
                    cvk.ogrenci.kullaniciNo = Convert.ToInt32(reader["Kullanici_No"]);
                    cvk.ogrenci.ad = reader["Ad"].ToString();
                    cvk.ogrenci.soyad = reader["Soyad"].ToString();
                    cvk.ogrenci.cinsiyet = reader["Cinsiyet"].ToString();
                    cvk.cevap.answerID = reader["Cevap_ID"].ToString();
                    cvk.cevap.soruNo = reader["Soru_No"].ToString();
                    cvk.cevap.cevap = reader["Cevap"].ToString();
                    if (!string.IsNullOrEmpty(reader["Puan"].ToString()))
                    {
                        cvk.cevap.puan = Convert.ToInt32(reader["Puan"]);
                    }
                    cvk.cevap.soruAcilmaZamani = Convert.ToDateTime(reader["Soru_Acilma_Zamani"]);
                    cvk.cevap.cevapGondermeZamani = Convert.ToDateTime(reader["Cevap_Gonderme_Zamani"]);
                    cvk.cevap.cevapGondermeSuresi = reader["Cevap_Gonderme_Suresi"].ToString();
                    cvkList.Add(cvk);
                }
              
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0021 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }
            return cvkList;
        }
        
        public List<student> SoruyaCevapVermeyenler(int sinifID, string soruNo)
        {
            List<student> ogrenciListesi = new List<student>();

            cmd = new MySqlCommand($"Select k.ID 'Kullanici_ID', k.Kullanici_No, k.Ad, k.Soyad from kullanicilar k " +
                $"inner join kullanici_sinif ks on ks.Kullanicilar_ID = k.ID " +
                $"where k.Okul_ID = 2 and ks.Siniflar_ID = 1 and k.Yetki_ID = 2 " +
                $"and k.ID not in " +
                $"(Select k.ID from kullanicilar k inner " +
                $"join cevaplar c on c.Kullanici_No = k.Kullanici_No " +
                $"inner join kullanici_sinif ks on ks.Kullanicilar_ID = k.ID " +
                $"where ks.Siniflar_ID = {sinifID} and k.Okul_ID = {Prm.okul_id} and c.Soru_No = '{soruNo}')", connect);
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    student s = new student();
                    s.id = Convert.ToInt32(reader["Kullanici_ID"]);
                    s.kullaniciNo = Convert.ToInt32(reader["Kullanici_No"]);
                    s.ad = reader["Ad"].ToString();
                    s.soyad = reader["Soyad"].ToString();
                    ogrenciListesi.Add(s);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0022 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }
            return ogrenciListesi;
        }
        public student TekilOgrenciGetir(string kullaniciNo)
        {
            student ogrenci = new student();
            cmd = new MySqlCommand($"select k.ID, k.Kullanici_No, k.Ad, k.Soyad, k.Cinsiyet, s.Sinif, s.Sube from kullanicilar k " +
                $"inner join kullanici_sinif ks on ks.Kullanicilar_ID = k.ID " +
                $"inner join siniflar s on s.ID = ks.Siniflar_ID " +
                $"where k.Kullanici_No = {kullaniciNo}",connect);
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    ogrenci.id = Convert.ToInt32(reader["ID"]);
                    ogrenci.kullaniciNo = Convert.ToInt32(reader["Kullanici_No"]);
                    ogrenci.ad = reader["Ad"].ToString();
                    ogrenci.soyad = reader["Soyad"].ToString();
                    ogrenci.sinif = reader["Sinif"].ToString();
                    ogrenci.sube = reader["Sube"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0023 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }
            return ogrenci;
        }
        public bool DonutEkle(string soruNo, int cevapID, string donut, int kullaniciNo, string donutZamani, string ekleyen, int puan)
        {
            bool donutDurum = false;
            cmd = new MySqlCommand($"insert into donutler (Soru_No, Cevap_ID, Donut, Kullanici_No, Donut_Zamani, Ekleyen) " +
                $"values ('{soruNo}',{cevapID},'{donut}','{kullaniciNo}','{donutZamani}',{ekleyen})",connect);
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                connect.Dispose();
                MySqlCommand cmd2;
                cmd2 = new MySqlCommand($"update cevaplar set Puan={puan} where Soru_No = '{soruNo}' and Kullanici_No={kullaniciNo}",connect);
                connect.Open();
                reader = cmd2.ExecuteReader();

                donutDurum = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0024 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }
            return donutDurum;
        }
        public List<student> badgeTakeStudent(string soruNolar, int listCount)
        {
            List<student> badgeTakeStudentList = new List<student>();
            cmd = new MySqlCommand($"SELECT k.Kullanici_No, Count(c.Kullanici_No) as ss, k.ID, k.Kullanici_No, k.Ad, k.Soyad, sinif.Sinif, sinif.Sube, " +
                                   "k.Cinsiyet FROM cevaplar c " +
                                   "inner join kullanicilar k on k.Kullanici_No = c.Kullanici_No " +
                                   "inner join sorular s on s.Soru_No=c.Soru_No " +
                                   "inner join kullanici_sinif ks on ks.Kullanicilar_ID = k.ID " +
                                   "inner join siniflar sinif on sinif.ID = ks.Siniflar_ID " +
                                  $"where {soruNolar} " +
                                  $"group by Kullanici_No HAVING ss = {listCount} order by Kullanici_No ; ", connect);
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    student s = new student();
                    s.id = Convert.ToInt32(reader["ID"]);
                    s.kullaniciNo = Convert.ToInt32(reader["Kullanici_No"]);
                    s.ad = reader["Ad"].ToString();
                    s.soyad = reader["Soyad"].ToString();
                    s.cinsiyet = reader["Cinsiyet"].ToString();
                    s.sinif = reader["Sinif"].ToString();
                    s.sube = reader["Sube"].ToString();
                    badgeTakeStudentList.Add(s);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0025 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }

            return badgeTakeStudentList;
        }      
        public bool addQuestion(string soruNo,byte[] imageByte,string aciklama,int maxPuan,string acilmaZamani,bool soruAcikmi,int konuBasliklarID, bool ilerlemelerimeEkle, int ekleyen_ID, int sinifID )
        {
            string nowDate = DateTime.Now.ToString("yyyy-MM-dd H:m:s").ToString();
            int quesitonImageSize =imageByte.Length;
            bool status = false;
            cmd = new MySqlCommand($@"insert Into sorular(Soru_No, Resim, ResimSize ,Aciklama, Max_Puan, Acilma_Zamani, Ekleme_Zamani, 
	                                 Soru_Turu, Soru_Acik_Mi, Konu_Basliklari_ID, Ilerlemelerime_Ekle, Ekleyen) 
	                                    values('{soruNo}',@image,{quesitonImageSize},'{aciklama}',{maxPuan},'{acilmaZamani}','{nowDate}',
                                        {0},{Convert.ToInt32(soruAcikmi)},{konuBasliklarID},{Convert.ToInt32(ilerlemelerimeEkle)},{ekleyen_ID});",
                                        connect);
            try
            {
                connect.Open();
                cmd.Parameters.Add(new MySqlParameter("@image", imageByte));
                cmd.ExecuteNonQuery();
                connect.Dispose();
                string sorgu = $@"select ID from sorular where Soru_No ='{soruNo}'";
                int questionID = TekilVeriCekInt(sorgu,"ID");
                if (questionID != null && questionID != 0)
                {
                    status = addSoruSinif(sinifID,1,questionID);
                }
                else
                {
                    status = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0026 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }

            return status;
        }
        public bool addSoruSinif(int sinifID, int ekleyen,int soruID)
        {
            bool status = false;
            cmd = new MySqlCommand($@"insert into Soru_Sinif(Sinif_ID, Ekleyen, Soru_ID) values({sinifID},{ekleyen},{soruID});",
                                        connect);
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                status = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0027 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }

            return status;
        }
        public bool addBadges(string badgeName,string resimUrl,string badgePromotion, string finallyDate,List<question> questionList, byte[] imagePath,int fileSize)
        {
            string nowDate = DateTime.Now.ToString("yyyy-MM-dd H:m:s"); 

            bool status = false;
            cmd = new MySqlCommand($@"insert into rozetler (Rozet_Adi, Resim, Aciklama, Bitis_Suresi, Ekleme_Zamani, Ekleyen, FileSize) 
                                        values ('{badgeName}',@image,'{badgePromotion}','{finallyDate}','{nowDate}',{Prm.kullanici_ID}, '{fileSize}');",
                                        connect);
           
            try
            {
                
                connect.Open();
                cmd.Parameters.Add(new MySqlParameter("@image",imagePath));
                cmd.ExecuteNonQuery();
                connect.Dispose();
                //Eklenen rozetin ID sini çekmek için
                string sorgu = $@"select ID from rozetler where Rozet_Adi ='{badgeName}' and Aciklama ='{badgePromotion}' and Ekleme_Zamani ='{nowDate}'";
                int badgeID= TekilVeriCekInt(sorgu,"ID");
                foreach (var question in questionList)
                {
                    if (badgeID != 0)
                    {

                        status = add_question_badge(badgeID, Convert.ToInt32(question.soruID));
                    }
                    else
                    {
                        status = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0028 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }

            return status;
        }
        public bool add_question_badge(int badgeID, int questionID) {
            cmd = new MySqlCommand($@"insert into soru_rozet(Sorular_ID, Rozetler_ID) values ({questionID},{badgeID});",
                                        connect);
            bool status = false;
            try
            {
                connect.Open();
                reader = cmd.ExecuteReader();
                status = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 01x0029 - {ex.GetType()}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connect.Dispose();
            }

            return status;
        }
    }
    





  




    public struct cevapVerenKullanici
    {
        public student ogrenci;
        public answer cevap;
    }

    public struct badge
    {
        public int rozetID;
        public string rozetAdi,resim,aciklama;
        public byte[] denemeImageByte;
        public Nullable<DateTime> bitisSuresi, eklemeZamani;
        public List<string> soruListesi;
    }

    public struct answer
    {
        public string answerID, soruNo, cevap, kullaniciNo, donutID, cevapGondermeSuresi;
        public DateTime soruAcilmaZamani, cevapGondermeZamani;
        public Nullable<int> puan;
    }

    public struct feedback
    {
        public string soruNo, cevapNo, donut, kullaniciNo;
        public DateTime donutZamani;
        public int puan;
    }

    public struct question
    {
        public string soruID, soruNo, baslik,aciklama, soruTuru;
        public Nullable<DateTime> acilmaZamani, eklemeZamani;
        public Nullable<int> maxPuan;
        public bool soruAcikMi;
        public byte[] imgArray;
    }

    public struct student
    {
        public int kullaniciNo, id;
        public string ad, soyad, cinsiyet, sinif, sube;
    }

    public struct classAndBranches
    {
        public int adet, sinifID;
        public string sinif, sube;
    }

    public struct konubasligi
    {
        public int kbID;
        public string kbAdi;
    }

}
