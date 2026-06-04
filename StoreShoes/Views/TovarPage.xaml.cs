using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StoreShoes.Views
{
    public partial class TovarPage : Page
    {
        StoreShoesEntities db = new StoreShoesEntities();
        public static TovarEditWindow openEditWindow = null;

        public TovarPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var postavshiks = db.Postavshikes.ToList();
            postavshiks.Insert(0, new Postavshikes { PostavshikesName = "Все поставщики" });
            cbFilter.ItemsSource = postavshiks;
            cbFilter.SelectedIndex = 0;

            // Проверка прав (предполагаем: 1 - Админ, 2 - Менеджер)
            if (AppSession.CurrentUser != null && AppSession.CurrentUser.RolesID == 1)
            {
                btnAdd.Visibility = Visibility.Visible;
            }

            UpdateData();
        }

        public void UpdateData()
        {

            if (dgTovars == null) return;

            db = new StoreShoesEntities();
            var currentTovars = db.Tovars.ToList();

            // 1. ФИЛЬТРАЦИЯ
            if (cbFilter.SelectedIndex > 0)
            {
                var selectedPostavshik = cbFilter.SelectedItem as Postavshikes;
                currentTovars = currentTovars.Where(t => t.PostavshikesID == selectedPostavshik.PostavshikesID).ToList();
            }

            // 2. ПОИСК (по артикулу, названию, описанию и т.д.)
            string search = txtSearch.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(search))
            {
                currentTovars = currentTovars.Where(t =>
                    t.Article.ToLower().Contains(search) ||
                    (t.Opisanie != null && t.Opisanie.ToLower().Contains(search)) ||
                    (t.TovarNames != null && t.TovarNames.TovarName.ToLower().Contains(search)) ||
                    (t.Manufactures != null && t.Manufactures.ManufacturesName.ToLower().Contains(search))
                ).ToList();
            }

            // 3. СОРТИРОВКА (по остатку)
            if (cbSort.SelectedIndex == 1)
                currentTovars = currentTovars.OrderBy(t => t.Ostatok).ToList();
            else if (cbSort.SelectedIndex == 2)
                currentTovars = currentTovars.OrderByDescending(t => t.Ostatok).ToList();

            dgTovars.ItemsSource = currentTovars;
        }

        private void FilterElements_Changed(object sender, SelectionChangedEventArgs e) => UpdateData();
        private void FilterElements_Changed(object sender, TextChangedEventArgs e) => UpdateData();

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenEditWindow(null);
        }

        private void dgTovars_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Только администратор может редактировать
            if (AppSession.CurrentUser != null && AppSession.CurrentUser.RolesID == 1)
            {
                if (dgTovars.SelectedItem is Tovars selectedTovar)
                {
                    OpenEditWindow(selectedTovar);
                }
            }
        }

        private void OpenEditWindow(Tovars tovar)
        {
            if (openEditWindow != null)
            {
                MessageBox.Show("Окно редактирования уже открыто!", "Внимание");
                openEditWindow.Focus();
                return;
            }

            openEditWindow = new TovarEditWindow(tovar, this);
            openEditWindow.Show();
        }
    }
}