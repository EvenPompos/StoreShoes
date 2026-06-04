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

namespace StoreShoes.Views
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        StoreShoesEntities db = new StoreShoesEntities();
        public MainPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var list = db.Tovars
                .Include("TovarNames")
                .Include("Postavshikes")
                .Include("Categories")
                .Include("Manufactures")
                .ToList();
            ListMain.ItemsSource = list;
        }
    }
}
