using ElectronicStoreShop.Models;
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
using ElectronicStoreShop.Pages.ChangesPages;

namespace ElectronicStoreShop.Pages
{
    /// <summary>
    /// Логика взаимодействия для ViewСhangesPage.xaml
    /// </summary>
    public partial class ViewСhangesPage : Page
    {
        private ElectronicStoreContext context;
        public ObservableCollection<Product> Products { get; set; } = new();

        public ViewСhangesPage()
        {
            InitializeComponent();
            context = new ElectronicStoreContext();
            DataContext = this;
            LoadList();
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

        private void ChangesProduct_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ViewChangesProducts());
        }

        private void ChangesCategory_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ViewChangesCategories());
        }

        private void ChangesTag_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ViewChangesTags());
        }

        private void ChangesBrand_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ViewChangesBrands());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainProductPage());
        }
    }
}
