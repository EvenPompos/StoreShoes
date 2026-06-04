using StoreShoes.Views;
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

namespace StoreShoes
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(AppSession.CurrentUser == null)
            {
                txtFIO.Text = "Гость";

                Tovarbut.Visibility = Visibility.Collapsed;
                Orderbut.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtFIO.Text = AppSession.CurrentUser.FIO;
                if (AppSession.CurrentUser.RolesID == 3)
                {
                    Tovarbut.Visibility = Visibility.Collapsed;
                    Orderbut.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Tovarbut.Visibility = Visibility.Visible;
                    Orderbut.Visibility = Visibility.Visible;
                }
            }
            MainFrame.Navigate(new MainPage());

        }

        private void Catalogbut_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MainPage());
        }

        private void Tovarbut_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TovarPage());
        }

        private void Orderbut_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrderPage());
        }

        private void GoBackbut_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }

        private void Exitbut_Click(object sender, RoutedEventArgs e)
        {
            AppSession.CurrentUser = null;
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Hide();
        }
    }
}
