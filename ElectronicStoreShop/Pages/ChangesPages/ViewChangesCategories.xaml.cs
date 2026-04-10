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
using System.Windows.Controls.Primitives;
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
    /// Логика взаимодействия для ViewChangesCategories.xaml
    /// </summary>
    public partial class ViewChangesCategories : Page
    {
        private readonly ElectronicStoreContext _context;
        public ObservableCollection<Category> Categories { get; set; } = new();

        public ViewChangesCategories()
        {
            InitializeComponent();
            _context = DBService.Instance.Context;
            DataContext = this;
            LoadCategories();
        }

        private void LoadCategories()
        {
            Categories.Clear();

            var categories = _context.Categories
                .Include(c => c.Products)
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .OrderBy(c => c.Name)
                .ToList();

            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ChangesCategoryPage(null));
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesListView.SelectedItem is Category selectedCategory)
            {
                NavigationService?.Navigate(new ChangesCategoryPage(selectedCategory.Id));
                Window window = Application.Current.MainWindow;
                window.Title = "Управление категориями";
            }
            else
            {
                MessageBox.Show("Выберите категорию для редактирования!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesListView.SelectedItem is Category selectedCategory)
            {

                bool hasDelete = _context.Products.Any(p => p.CategoryId == selectedCategory.Id);

                if (hasDelete)
                {
                    MessageBox.Show("Нельзя удалить категорию, в которой есть товар.",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show($"Вы уверены, что хотите удалить {selectedCategory.Name} категорию ?", "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return;

                try
                {
                    using (var deleteContext = new ElectronicStoreContext())
                    {
                        var products = deleteContext.Products
                            .Where(p => p.CategoryId == selectedCategory.Id)
                            .ToList();

                        foreach (var product in products)
                        {
                            product.CategoryId = null;  
                        }

                        var categoryToDelete = deleteContext.Categories
                            .Find(selectedCategory.Id);

                        if (categoryToDelete != null)
                        {
                            deleteContext.Categories.Remove(categoryToDelete);
                            deleteContext.SaveChanges();
                        }
                    }

                    MessageBox.Show("Категория успешно удалена!", "Успешно",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadCategories();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите категорию для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ViewСhangesPage());
            Window window = Application.Current.MainWindow;
            window.Title = "Управление категориями";
        }

    }
}
