using ElectronicStoreShop.Models;
using ElectronicStoreShop.Service;
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
using Microsoft.EntityFrameworkCore;

namespace ElectronicStoreShop.Pages.ChangesPages
{
    /// <summary>
    /// Логика взаимодействия для ChangesCategoryPage.xaml
    /// </summary>
    public partial class ChangesCategoryPage : Page
    {
        private readonly ElectronicStoreContext _context;
        private readonly int? _categoryId;
        private readonly bool _isEditMode;

        public string CategoryName { get; set; }

        public ChangesCategoryPage(int? categoryId = null)
        {
            InitializeComponent();
            _context = DBService.Instance.Context;

            _categoryId = categoryId;
            _isEditMode = categoryId.HasValue;

            if (_isEditMode)
            {
                var category = _context.Categories
                    .AsNoTracking()
                    .FirstOrDefault(c => c.Id == _categoryId.Value);

                if (category == null)
                {
                    MessageBox.Show("Категория не найдена!", "Ошибка");
                    NavigationService?.GoBack();
                    return;
                }

                CategoryName = category.Name;
            }
            else
            {
                CategoryName = string.Empty;
            }

            DataContext = this;
        }

       

        private bool ValidateCategory()
        {
            if (string.IsNullOrWhiteSpace(CategoryName))
            {
                MessageBox.Show("Введите название категории!");
                return false;
            }


            if (CategoryName.Length > 10000)
            {
                MessageBox.Show("Название должно содержать максимум 10000 символов!");
                return false;
            }

            var existingCategory = _context.Categories
                .AsNoTracking()
                .FirstOrDefault(c => c.Name.ToLower() == CategoryName.Trim().ToLower());

            if (existingCategory != null && (!_isEditMode || existingCategory.Id != _categoryId))
            {
                MessageBox.Show("Категория с таким названием уже существует!");
                return false;
            }

            return true;
        }

        private void SaveCategory_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateCategory())
                return;

            try
            {
                if (_isEditMode)
                {
                    using (var saveContext = new ElectronicStoreContext())
                    {
                        var existingCategory = saveContext.Categories.Find(_categoryId.Value);
                        if (existingCategory != null)
                        {
                            existingCategory.Name = CategoryName.Trim();
                            saveContext.SaveChanges();
                        }
                    }
                    MessageBox.Show("Категория обновлена!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                    NavigationService?.Navigate(new ViewChangesCategories());
                }
                else
                {
                    var newCategory = new Category
                    {
                        Name = CategoryName.Trim(),
                    };

                    _context.Categories.Add(newCategory);
                    _context.SaveChanges();

                    MessageBox.Show("Категория добавлена!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService?.Navigate(new ViewChangesCategories());

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

 

        private void CancelCategory_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Отменить изменения?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                NavigationService?.Navigate(new ViewChangesCategories());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

    }
}
