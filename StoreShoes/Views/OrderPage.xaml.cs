using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StoreShoes.Views
{
    public partial class OrderPage : Page
    {
        StoreShoesEntities db = new StoreShoesEntities();
        public static OrderEditWindow openEditWindow = null;

        public OrderPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Проверка прав: Только Администратор (RolesID == 1) видит кнопку добавления
            if (AppSession.CurrentUser != null && AppSession.CurrentUser.RolesID == 1)
            {
                btnAddOrder.Visibility = Visibility.Visible;
            }

            UpdateData();
        }

        public void UpdateData()
        {
            if (dgOrders == null) return;

            db = new StoreShoesEntities(); // Обновляем контекст
            dgOrders.ItemsSource = db.Orders.ToList();
        }

        private void btnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            OpenEditWindow(null);
        }

        private void dgOrders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Только администратор может открывать окно редактирования
            if (AppSession.CurrentUser != null && AppSession.CurrentUser.RolesID == 1)
            {
                if (dgOrders.SelectedItem is Orders selectedOrder)
                {
                    OpenEditWindow(selectedOrder);
                }
            }
        }

        private void OpenEditWindow(Orders order)
        {
            if (openEditWindow != null)
            {
                MessageBox.Show("Окно редактирования заказа уже открыто!", "Внимание");
                openEditWindow.Focus();
                return;
            }

            openEditWindow = new OrderEditWindow(order, this);
            openEditWindow.Show();
        }
    }
}