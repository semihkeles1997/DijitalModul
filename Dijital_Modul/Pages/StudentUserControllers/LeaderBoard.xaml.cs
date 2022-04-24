using Dijital_Modul.Pages.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dijital_Modul.Pages.UserController
{
    /// <summary>
    /// Interaction logic for LeaderBoard.xaml
    /// </summary>
    public partial class LeaderBoard : UserControl
    {
        List<string> kullaniciAdiList;
        List<int> puanList;
        int kullaniciSirası = -1;
        public LeaderBoard()
        {
            InitializeComponent();
            Prm.pageName = "LiderTahtasi";
            kullaniciAdiList = new List<string>();
            puanList = new List<int>();
        }
        private void liderTahtasiCombox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            kullaniciAdiList.Clear();
            puanList.Clear();
            stc.Children.Clear();
            kullaniciSirası = -1;
            General general = new General();

            if (liderTahtasiCombox.SelectedIndex == 0)
            {
                general.LeaderBoardEdit(kullaniciAdiList, puanList,0, Convert.ToInt32(Prm.sinif_id));
            }
            else if (liderTahtasiCombox.SelectedIndex == 1)
            {
                general.LeaderBoardEdit(kullaniciAdiList, puanList, 1, Convert.ToInt32(Prm.sinif_id));
            }
            else if (liderTahtasiCombox.SelectedIndex == 2)
            {
                general.LeaderBoardEdit(kullaniciAdiList, puanList, 2, Convert.ToInt32(Prm.sinif_id));
            }

            for (int i = 0; i < kullaniciAdiList.Count; i++)
            {
                if (Prm.ad == kullaniciAdiList[i])
                {
                    kullaniciSirası = i;
                }
            }
            viewCreate();

        }

        public void viewCreate()
        {

            // The screen view to setting object here
            for (int i = 0; i < kullaniciAdiList.Count; i++)
            {

                Border border = new Border();
                border.Height = 70;
                border.Width = 750;
                border.CornerRadius = new CornerRadius(30);
                border.Background = Brushes.White;


                DockPanel dockPanel = new DockPanel();
                dockPanel.Margin = new Thickness(10, 0, 0, 0);

                TextBlock siraText = new TextBlock();
                siraText.Foreground = Brushes.Black;
                siraText.FontSize = 40;
                siraText.Width = 40;
                siraText.Height = 60;
                siraText.HorizontalAlignment = HorizontalAlignment.Left;
                siraText.VerticalAlignment = VerticalAlignment.Center;
                siraText.Margin = new Thickness(20, 0, 0, 0);
                siraText.Text = (i + 1).ToString();

                Image siraImage = new Image();
                siraImage.Width = 60;
                siraImage.Height = 60;

                TextBlock adText = new TextBlock();
                adText.Foreground = Brushes.Black;
                adText.FontSize = 40;
                adText.Text = kullaniciAdiList[i];
                adText.HorizontalAlignment = HorizontalAlignment.Center;
                adText.VerticalAlignment = VerticalAlignment.Center;
                adText.Margin = new Thickness(20, 0, 0, 0);

                Image puanresim = new Image();
                puanresim.Height = 60;
                puanresim.Width = 60;
                puanresim.VerticalAlignment = VerticalAlignment.Center;
                puanresim.Margin = new Thickness(0, 15, 0, 0);

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(@"../../Pages/Images/puanLogo.png", UriKind.Relative);
                bitmapImage.EndInit();

                puanresim.Source = bitmapImage;

                TextBlock puanText = new TextBlock();
                puanText.Foreground = Brushes.Black;
                puanText.FontSize = 40;
                puanText.VerticalAlignment = VerticalAlignment.Center;
                puanText.Text = puanList[i].ToString();

                DockPanel puanDock = new DockPanel();
                puanDock.HorizontalAlignment = HorizontalAlignment.Right;
                puanDock.Children.Add(puanText);
                puanDock.Children.Add(puanresim);

                BitmapImage bitmapImage2 = new BitmapImage();
                bitmapImage2.BeginInit();
                bitmapImage2.UriSource = null;
                if (i == 0)
                {
                    bitmapImage2.UriSource = new Uri(@"../../Pages/Images/leaderBoard1.png", UriKind.Relative);
                    bitmapImage2.EndInit();
                }
                else if (i == 1)
                {
                    bitmapImage2.UriSource = new Uri(@"../../Pages/Images/leaderBoard2.png", UriKind.Relative);
                    bitmapImage2.EndInit();
                }
                else if (i == 2)
                {
                    bitmapImage2.UriSource = new Uri(@"../../Pages/Images/leaderBoard3.png", UriKind.Relative);
                    bitmapImage2.EndInit();
                }

                siraImage.Source = bitmapImage2;

                if (i < 3 && i != kullaniciSirası)
                {

                    dockPanel.Children.Add(siraImage);
                    dockPanel.Children.Add(adText);
                    puanresim.Source = bitmapImage;
                    dockPanel.Children.Add(puanDock);
                    border.Child = dockPanel;
                    stc.Children.Add(border);
                }
                else if (i == kullaniciSirası)
                {

                    border.Background = Brushes.LightBlue;
                    if (i < 4)
                    {
                        dockPanel.Children.Add(siraImage);
                    }
                    else
                    {
                        dockPanel.Children.Add(siraText);
                    }
                    dockPanel.Children.Add(adText);
                    puanresim.Source = bitmapImage;
                    dockPanel.Children.Add(puanDock);
                    border.Child = dockPanel;
                    stc.Children.Add(border);

                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            liderTahtasiCombox.SelectedIndex = 0;

        }


    }





}

