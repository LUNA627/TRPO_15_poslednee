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
    /// Логика взаимодействия для ViewChangesBrands.xaml
    /// </summary>
    public partial class ViewChangesBrands : Page
    {

        private readonly ElectronicStoreContext _context;
        public ObservableCollection<Brand> Brands { get; set; } = new();
        public ViewChangesBrands()
        {
            InitializeComponent();
            _context = DBService.Instance.Context;
            DataContext = this;
            LoadBrands();
        }

        private void LoadBrands()
        {
            Brands.Clear();

            var brands = _context.Brands
                .Include(c => c.Products)
                .Select(c => new Brand
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .OrderBy(c => c.Name)
                .ToList();

            foreach (var brand in brands)
            {
                Brands.Add(brand);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ViewСhangesPage());
        }

        private void AddBrand_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ChangesBrandPage(null));
            Window window = Application.Current.MainWindow;
            window.Title = "Управление брендами";
        }

        private void EditBrand_Click(object sender, RoutedEventArgs e)
        {
            if (BrandsListView.SelectedItem is Brand selectedBrand)
            {
                NavigationService?.Navigate(new ChangesBrandPage(selectedBrand.Id));
                Window window = Application.Current.MainWindow;
                window.Title = "Управление брендами";
            }
            else
            {
                MessageBox.Show("Выберите бренд для редактирования!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteBrand_Click(object sender, RoutedEventArgs e)
        {
            if (BrandsListView.SelectedItem is Brand selectedBrand)
            {

                bool hasDelete = _context.Products.Any(p => p.BrandId == selectedBrand.Id);

                if (hasDelete)
                {
                    MessageBox.Show("Нельзя удалить бренд, в которой есть товар.",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show($"Вы уверены, что хотите удалить {selectedBrand.Name} бренд ?", "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return;

                try
                {
                    using (var deleteContext = new ElectronicStoreContext())
                    {

                        var products = deleteContext.Products
                            .Where(p => p.BrandId == selectedBrand.Id)
                            .ToList();

                        foreach (var product in products)
                        {
                            product.BrandId = null;  
                        }

                        var brandToDelete = deleteContext.Brands
                            .Find(selectedBrand.Id);

                        if (brandToDelete != null)
                        {
                            deleteContext.Brands.Remove(brandToDelete);
                            deleteContext.SaveChanges();
                        }
                    }

                    MessageBox.Show("Бренд успешно удалена!", "Успешно",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadBrands();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите бренд для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
