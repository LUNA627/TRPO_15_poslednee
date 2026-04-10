using ElectronicStoreShop.Models;
using ElectronicStoreShop.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace ElectronicStoreShop.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainProductPage.xaml
    /// </summary>
    public partial class MainProductPage : Page, INotifyPropertyChanged
    {
        private ElectronicStoreContext context; // Поключение к бд

        public ObservableCollection<Product> Products { get; set; } = new(); // Коллекция всех продуктов
        public ICollectionView ProductsView { get; set; } // Коллекция для фильтрации и сортировки продуктов


        public string SearchQuery { get; set; } = string.Empty; // поиск

        // Свойства для фильтра
        public string FilterCategory { get; set; } = string.Empty;
        public string FilterBrand { get; set; } = string.Empty;
        public string FilterPriceFrom { get; set; } = string.Empty;
        public string FilterPriceTo { get; set; } = string.Empty;


        // Свойства для сортировки
        public string SortField { get; set; } = string.Empty;
        public bool IsSortAscending { get; set; } = true;



        public ObservableCollection<Category> Categories { get; set; } = new();
        public ObservableCollection<Brand> Brands { get; set; } = new();


        public bool IsManager => App.CurrentUserType == App.UserType.Manager;


        public string CountInfo => $"Всего: {Products.Count} | Показано: {ProductsView?.Cast<Product>().Count()}";


        public MainProductPage()
        {
            InitializeComponent();
            Products = new ObservableCollection<Product>();

            context = DBService.Instance.Context;

            ProductsView = CollectionViewSource.GetDefaultView(Products);
            ProductsView.Filter = FilterProducts;

            DataContext = this;

            LoadList();
            LoadCategoriesAndBrands();
            LoadSortOptions();
            UpdateCountInfo();

        }

        private void UpdateCountInfo()
        {
            OnPropertyChanged(nameof(CountInfo)); 
        }

        private void LoadSortOptions()
        {
            SortComboBox.Items.Clear();
            SortComboBox.Items.Add(new ComboBoxItem { Content = "По наименованию", Tag = "Name" });
            SortComboBox.Items.Add(new ComboBoxItem { Content = "По цене", Tag = "Price" });
            SortComboBox.Items.Add(new ComboBoxItem { Content = "По количеству", Tag = "Stock" });
            SortComboBox.SelectedIndex = -1; 
        }



        private void LoadList()
        {
            Products.Clear();

            var products = context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.ProductTags)  
            .ThenInclude(pt => pt.Tag)
        .ToList();

            foreach (var product in products)
            {
                Products.Add(product);
            }

        }

        private bool FilterProducts(object obj)
        {
            if (obj is not Product product)
                return false;

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                if (!product.Name.Contains(SearchQuery, StringComparison.CurrentCultureIgnoreCase))
                    return false;
            }

            if (!string.IsNullOrWhiteSpace(FilterCategory) && FilterCategory != "Все категории")
            {
                if (product.Category == null)
                    return false;

                if (product.Category.Name != FilterCategory)
                    return false;
            }

            if (!string.IsNullOrWhiteSpace(FilterBrand) && FilterBrand != "Все бренды")
            {
                if (product.Brand == null)
                    return false;

                if (product.Brand.Name != FilterBrand)
                    return false;
            }

            if (!string.IsNullOrWhiteSpace(FilterPriceFrom))
            {
                if (decimal.TryParse(FilterPriceFrom, out decimal priceFrom))
                {
                    if (product.Price < priceFrom)
                        return false;
                }
            }

            

            if (!string.IsNullOrWhiteSpace(FilterPriceTo))
            {
                if (decimal.TryParse(FilterPriceTo, out decimal priceTo))
                {
                    if (product.Price > priceTo)
                        return false;
                }
            }

            return true;
        }

        private void LoadCategoriesAndBrands()
        {
            var categories = context.Categories.ToList();
            CategoryFilterComboBox.Items.Clear();
            CategoryFilterComboBox.Items.Add(new ComboBoxItem { Content = "Все категории", Tag = null });
            foreach (var cat in categories)
            {
                CategoryFilterComboBox.Items.Add(new ComboBoxItem { Content = cat.Name, Tag = cat.Id });
            }
            CategoryFilterComboBox.SelectedIndex = 0;

            var brands = context.Brands.ToList();
            BrandFilterComboBox.Items.Clear();
            BrandFilterComboBox.Items.Add(new ComboBoxItem { Content = "Все бренды", Tag = null });
            foreach (var brand in brands)
            {
                BrandFilterComboBox.Items.Add(new ComboBoxItem { Content = brand.Name, Tag = brand.Id });
            }
            BrandFilterComboBox.SelectedIndex = 0;
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryFilterComboBox.SelectedItem is ComboBoxItem catItem)
            {
                FilterCategory = catItem.Content?.ToString();
            }

            if (BrandFilterComboBox.SelectedItem is ComboBoxItem brandItem)
            {
                FilterBrand = brandItem.Content?.ToString();
            }

            FilterPriceFrom = PriceFromTextBox.Text;

            FilterPriceTo = PriceToTextBox.Text;

            if (SortComboBox.SelectedItem is ComboBoxItem sortItem)
            {
                SortField = sortItem.Tag?.ToString();
            }

            ApplySorting();
            ProductsView.Refresh();

            if (ProductsView.IsEmpty)
            {
                MessageBox.Show(
                    "Товары с выбранными параметрами не найдены.",
                    "Нет результатов",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                
            }
            if (!string.IsNullOrEmpty(PriceFromTextBox.Text))
            {
                if (!decimal.TryParse(PriceFromTextBox.Text, out decimal priceFrom))
                {
                    MessageBox.Show(
                        "Некорректный ввод данных в поле 'от'. Введите число.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                if (priceFrom < 0)
                {
                    MessageBox.Show(
                        "Цена не может быть ниже 0.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                if (priceFrom > 10000000)
                {
                    MessageBox.Show(
                        "Цена не может быть больше 10 000 000.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(PriceToTextBox.Text))
            {
                if (!decimal.TryParse(PriceToTextBox.Text, out decimal priceTo))
                {
                    MessageBox.Show(
                        "Некорректный ввод данных в поле 'до'. Введите число.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                if (priceTo < 0)
                {
                    MessageBox.Show(
                        "Цена не может быть ниже 0.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                if (priceTo > 10000000)
                {
                    MessageBox.Show(
                        "Цена не может быть больше 10 000 000.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(PriceFromTextBox.Text) && !string.IsNullOrEmpty(PriceToTextBox.Text))
            {
                decimal from = decimal.Parse(PriceFromTextBox.Text);
                decimal to = decimal.Parse(PriceToTextBox.Text);

                if (from > to)
                {
                    MessageBox.Show(
                        "Цена 'от' не может быть больше цены 'до'.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
            }
            UpdateCountInfo();
        }


        private void RemoveFilter_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
               "Сбросить все фильтры и сортировку?",
               "Подтверждение",
               MessageBoxButton.YesNo,
               MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            SearchQuery = string.Empty;

            FilterCategory = string.Empty;
            FilterBrand = string.Empty;
            FilterPriceFrom = string.Empty;
            FilterPriceTo = string.Empty;
            SortField = string.Empty;
            IsSortAscending = true;

            if (FindName("SearchTextBox") is TextBox searchBox)
                searchBox.Text = string.Empty;

            if (CategoryFilterComboBox.Items.Count > 0)
            {
                CategoryFilterComboBox.SelectedIndex = 0;
            }

            if (BrandFilterComboBox.Items.Count > 0)
            {
                BrandFilterComboBox.SelectedIndex = 0;
            }

            if (PriceFromTextBox != null)
            {
                PriceFromTextBox.Text = string.Empty;
            }
            if (PriceToTextBox != null)
            {
                PriceToTextBox.Text = string.Empty;
            }

            if (SortComboBox != null)
            {
                SortComboBox.SelectedIndex = -1;
            }

            
            ProductsView.SortDescriptions.Clear();
            IsSortAscending = true;
            SortField = "";

            ProductsView.Refresh();
            UpdateCountInfo();

        }

       
        private void ApplySorting()
        {
            ProductsView.SortDescriptions.Clear();

            if (string.IsNullOrWhiteSpace(SortField))
                return;

            var direction = IsSortAscending ? ListSortDirection.Ascending : ListSortDirection.Descending;

            ProductsView.SortDescriptions.Add(new SortDescription(SortField, direction));
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortComboBox.SelectedItem is ComboBoxItem sortItem && sortItem.Tag != null)
            {
                SortField = sortItem.Tag.ToString();
                ApplySorting();
                ProductsView.Refresh();
                UpdateCountInfo();
            }
        }

        private void SortDirection_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                IsSortAscending = radioButton.Tag?.ToString() == "Ascending";

                if (ProductsView != null && ProductsView.SortDescriptions.Count > 0)
                {
                    ApplySorting();
                    ProductsView.Refresh();
                    UpdateCountInfo();
                }
            }
        }




        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                SearchQuery = textBox.Text;
            }
            ProductsView.Refresh();
            UpdateCountInfo();
        }

        private void GoChangesPage_Click(object sender, RoutedEventArgs e)
        {
            if (!IsManager)
            {
                MessageBox.Show("Вы не являетесь менеджером", "Ошибка");
                return;
            }

            NavigationService.Navigate(new ViewСhangesPage());
            Window window = Application.Current.MainWindow;
            window.Title = "Изменения параметров товара";

        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
