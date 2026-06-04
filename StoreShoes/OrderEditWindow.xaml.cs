using System;
using System.Linq;
using System.Windows;

namespace StoreShoes.Views
{
    public partial class OrderEditWindow : Window
    {
        private Orders _currentOrder;
        private OrderPage _parentPage;
        private StoreShoesEntities db = new StoreShoesEntities();

        public OrderEditWindow(Orders order, OrderPage page)
        {
            InitializeComponent();
            _parentPage = page;
            _currentOrder = order;

            // Загрузка данных для ComboBox
            cbUser.ItemsSource = db.Users.ToList();
            cbStatus.ItemsSource = db.OrderStatuses.ToList();
            cbPoint.ItemsSource = db.PointVudachies.ToList();

            if (_currentOrder != null) 
            {
                this.Title = "Редактирование заказа";
                tbCode.Text = _currentOrder.Code.ToString();
                dpOrderDate.SelectedDate = _currentOrder.DateOrders;
                dpDeliveryDate.SelectedDate = _currentOrder.DateDelivery;

                cbUser.SelectedItem = db.Users.FirstOrDefault(u => u.UsersID == _currentOrder.ClientID);
                cbStatus.SelectedItem = db.OrderStatuses.FirstOrDefault(s => s.OrderStatusesID == _currentOrder.OrderStatusesID);
                cbPoint.SelectedItem = db.PointVudachies.FirstOrDefault(p => p.PointVudachiesID == _currentOrder.PointVudachiesID);
            }
            else 
            {
                this.Title = "Добавление заказа";
                btnDelete.Visibility = Visibility.Collapsed;
                dpOrderDate.SelectedDate = DateTime.Now; 
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbCode.Text) || !int.TryParse(tbCode.Text, out int code))
            {
                MessageBox.Show("Введите корректный числовой Код заказа!", "Ошибка");
                return;
            }
            if (cbUser.SelectedItem == null || cbStatus.SelectedItem == null || cbPoint.SelectedItem == null)
            {
                MessageBox.Show("Заполните все выпадающие списки (Клиент, Статус, Пункт выдачи)!", "Ошибка");
                return;
            }
            if (dpOrderDate.SelectedDate == null)
            {
                MessageBox.Show("Укажите дату заказа!", "Ошибка");
                return;
            }

            // Инициализация объекта, если это добавление
            if (_currentOrder == null)
            {
                _currentOrder = new Orders();
                db.Orders.Add(_currentOrder);
            }

            _currentOrder.Code = code;
            _currentOrder.DateOrders = dpOrderDate.SelectedDate;
            _currentOrder.DateDelivery = dpDeliveryDate.SelectedDate;

            _currentOrder.ClientID = (int)(cbUser.SelectedItem as Users).UsersID;
            _currentOrder.OrderStatusesID = (int)(cbStatus.SelectedItem as OrderStatuses).OrderStatusesID;
            _currentOrder.PointVudachiesID = (int)(cbPoint.SelectedItem as PointVudachies).PointVudachiesID;

            try
            {
                db.SaveChanges();
                MessageBox.Show("Заказ успешно сохранен!", "Успех");
                _parentPage.UpdateData();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Защита от удаления заказа, в котором уже есть товары (OrderItems)
            if (db.OrderItems.Any(oi => oi.OrdersID == _currentOrder.OrdersID))
            {
                MessageBox.Show("Нельзя удалить заказ, так как к нему привязаны товары (OrderItems)!\nСначала удалите состав заказа.", "Запрет удаления");
                return;
            }

            if (MessageBox.Show("Вы уверены, что хотите удалить этот заказ?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var orderToDelete = db.Orders.Find(_currentOrder.OrdersID);
                if (orderToDelete != null)
                {
                    db.Orders.Remove(orderToDelete);
                    db.SaveChanges();
                    _parentPage.UpdateData();
                    this.Close();
                }
            }
        }

        // При закрытии окна освобождаем статическую переменную
        private void Window_Closed(object sender, EventArgs e)
        {
            OrderPage.openEditWindow = null;
        }
    }
}