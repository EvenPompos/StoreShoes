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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data.Sql;

namespace StoreShoes
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        StoreShoesEntities db = new StoreShoesEntities();
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginBut_Click(object sender, RoutedEventArgs e)
        {
            var login = txtLogin.Text.Trim();
            var password = txtPassword.Password.Trim();

            if(string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполни все поля!", "!Предупреждение!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                var user = db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

                if(user != null)
                {
                    AppSession.CurrentUser = user;
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Неверный пароль", "!Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подклюения к базе данных {ex.Message}", "!Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GuestBut_Click(object sender, RoutedEventArgs e)
        {
            AppSession.CurrentUser = null;
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Hide();

        }
    }
}
