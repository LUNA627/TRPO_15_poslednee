using ElectronicStoreShop.Models;
using ElectronicStoreShop.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ElectronicStoreShop.Pages.ChangesPages
{
    /// <summary>
    /// Логика взаимодействия для ViewChangesProducts.xaml
    /// </summary>
    public partial class ViewChangesProducts : Page
    {
        private readonly ElectronicStoreContext context;
        public ObservableCollection<Product> Products { get; set; } = new();

        public ViewChangesProducts()
        {
            InitializeComponent();
            context = DBService.Instance.Context;
            DataContext = this;
            LoadProducts();
        }

        private void LoadProducts()
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


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ViewСhangesPage());
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            Product selectedProduct = null;

            if (ProductsList.SelectedItem is Product product)
            {
                selectedProduct = product;
            }
            else if (sender is Button button && button.DataContext is Product btnProduct)
            {
                selectedProduct = btnProduct;
            }

            if (selectedProduct == null)
            {
                MessageBox.Show(
                    "Выберите товар для удаления!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
               $"Вы уверены, что хотите удалить товар {selectedProduct.Name} ?",
               "Подтверждение удаления",
               MessageBoxButton.YesNo,
               MessageBoxImage.Warning,
               MessageBoxResult.No);

            if (result != MessageBoxResult.Yes)
                return;

            context.Products.Remove(selectedProduct);
            context.SaveChanges();

            LoadProducts();

            MessageBox.Show("Товар успешно удалён!", "Успешно",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            Product selectedProduct = null;

            if (ProductsList.SelectedItem is Product product)
            {
                selectedProduct = product;
            }
            else if (sender is Button button && button.DataContext is Product btnProduct)
            {
                selectedProduct = btnProduct;
            }

            if (selectedProduct == null)
            {
                MessageBox.Show(
                    "Выберите товар для редактирования!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            NavigationService.Navigate(new ChangesProductPage(selectedProduct.Id));
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ChangesProductPage(null));
        }
    }
}
