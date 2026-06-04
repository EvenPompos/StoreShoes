using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace StoreShoes.Views
{
    public partial class TovarEditWindow : Window
    {
        private Tovars _currentTovar;
        private TovarPage _parentPage;
        private StoreShoesEntities db = new StoreShoesEntities();
        private string _selectedPhotoPath = null;
        private string _oldPhotoName = null;

        public TovarEditWindow(Tovars tovar, TovarPage page)
        {
            InitializeComponent();
            _parentPage = page;
            _currentTovar = tovar;

            // Загрузка списков
            cbName.ItemsSource = db.TovarNames.ToList();
            cbCategory.ItemsSource = db.Categories.ToList();
            cbPostavshik.ItemsSource = db.Postavshikes.ToList();
            cbManufacture.ItemsSource = db.Manufactures.ToList();

            if (_currentTovar != null)
            {
                this.Title = "Редактирование товара";
                tbArticle.Text = _currentTovar.Article;
                tbPrice.Text = _currentTovar.Price.ToString();
                tbSkidka.Text = _currentTovar.Skidka.ToString();
                tbOstatok.Text = _currentTovar.Ostatok.ToString();
                tbEd.Text = _currentTovar.EdIzmereniya;
                tbOpisanie.Text = _currentTovar.Opisanie;

                cbName.SelectedItem = db.TovarNames.FirstOrDefault(x => x.TovarNamesID == _currentTovar.TovarNamesID);
                cbCategory.SelectedItem = db.Categories.FirstOrDefault(x => x.CategoriesID == _currentTovar.CategoriesID);
                cbPostavshik.SelectedItem = db.Postavshikes.FirstOrDefault(x => x.PostavshikesID == _currentTovar.PostavshikesID);
                cbManufacture.SelectedItem = db.Manufactures.FirstOrDefault(x => x.ManufacturesID == _currentTovar.ManufacturesID);

                _oldPhotoName = _currentTovar.Images;
                LoadImage(_currentTovar.Images);
            }
            else
            {
                this.Title = "Добавление нового товара";
                btnDelete.Visibility = Visibility.Collapsed;
                LoadImage(null); // Загрузит заглушку
            }
        }

        private void LoadImage(string photoName)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory + "Images\\";
            if (!string.IsNullOrEmpty(photoName) && File.Exists(basePath + photoName))
                imgPhoto.Source = new BitmapImage(new Uri(basePath + photoName));
            else
                imgPhoto.Source = new BitmapImage(new Uri("pack://application:,,,/Images/picture.png"));
        }

        private void btnSelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Картинки|*.jpg;*.jpeg;*.png" };
            if (ofd.ShowDialog() == true)
            {
                var img = BitmapFrame.Create(new Uri(ofd.FileName));
                if (img.PixelWidth > 300 || img.PixelHeight > 200)
                {
                    MessageBox.Show($"Размер превышает 300x200! Текущий: {img.PixelWidth}x{img.PixelHeight}");
                    return;
                }
                _selectedPhotoPath = ofd.FileName;
                imgPhoto.Source = new BitmapImage(new Uri(_selectedPhotoPath));
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Валидация (допускаются сотые доли, > 0)
            if (!decimal.TryParse(tbPrice.Text, out decimal price) || price < 0) { MessageBox.Show("Некорректная цена!"); return; }
            if (!int.TryParse(tbOstatok.Text, out int ostatok) || ostatok < 0) { MessageBox.Show("Некорректный остаток!"); return; }
            int.TryParse(tbSkidka.Text, out int skidka);

            if (_currentTovar == null)
            {
                _currentTovar = new Tovars();
                db.Tovars.Add(_currentTovar);
            }

            _currentTovar.Article = tbArticle.Text;
            _currentTovar.Price = (int)price;
            _currentTovar.Skidka = skidka;
            _currentTovar.Ostatok = ostatok;
            _currentTovar.EdIzmereniya = tbEd.Text;
            _currentTovar.Opisanie = tbOpisanie.Text;

            // Проверяем, что во всех ComboBox что-то выбрано
            if (cbName.SelectedItem == null || cbCategory.SelectedItem == null ||
                cbPostavshik.SelectedItem == null || cbManufacture.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите значения во всех выпадающих списках!", "Ошибка валидации");
                return;
            }
            _currentTovar.TovarNamesID = (int)(cbName.SelectedItem as TovarNames).TovarNamesID;
            _currentTovar.CategoriesID = (int)(cbCategory.SelectedItem as Categories).CategoriesID;
            _currentTovar.PostavshikesID = (int)(cbPostavshik.SelectedItem as Postavshikes).PostavshikesID;
            _currentTovar.ManufacturesID = (int)(cbManufacture.SelectedItem as Manufactures).ManufacturesID;

            // Логика сохранения фото
            if (_selectedPhotoPath != null)
            {
                string imagesFolder = AppDomain.CurrentDomain.BaseDirectory + "Images\\";
                if (!Directory.Exists(imagesFolder)) Directory.CreateDirectory(imagesFolder);

                string newName = Path.GetFileName(_selectedPhotoPath);
                File.Copy(_selectedPhotoPath, imagesFolder + newName, true);
                _currentTovar.Images = newName;

                if (!string.IsNullOrEmpty(_oldPhotoName) && _oldPhotoName != newName)
                {
                    if (File.Exists(imagesFolder + _oldPhotoName)) File.Delete(imagesFolder + _oldPhotoName);
                }
            }

            db.SaveChanges();
            MessageBox.Show("Успешно сохранено!");
            _parentPage.UpdateData();
            this.Close();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (db.OrderItems.Any(oi => oi.TovarsID == _currentTovar.TovarsID))
            {
                MessageBox.Show("Нельзя удалить товар, так как он есть в заказах!");
                return;
            }

            if (MessageBox.Show("Удалить?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var del = db.Tovars.Find(_currentTovar.TovarsID);
                db.Tovars.Remove(del);
                db.SaveChanges();
                _parentPage.UpdateData();
                this.Close();
            }
        }

        private void Window_Closed(object sender, EventArgs e) => TovarPage.openEditWindow = null;
    }
}