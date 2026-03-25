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
    /// Логика взаимодействия для ChangesBrandPage.xaml
    /// </summary>
    public partial class ChangesBrandPage : Page
    {
        private readonly ElectronicStoreContext _context;
        private readonly int? _brandId;
        private readonly bool _isEditMode;

        public string BrandName { get; set; }
        public ChangesBrandPage(int? brandId = null)
        {
            InitializeComponent();
            _context = DBService.Instance.Context;

            _brandId = brandId;
            _isEditMode = brandId.HasValue;

            if (_isEditMode)
            {
                var brand = _context.Brands
                    .AsNoTracking()
                    .FirstOrDefault(c => c.Id == _brandId.Value);

                if (brand == null)
                {
                    MessageBox.Show("Бренд не найден!", "Ошибка");
                    NavigationService?.GoBack();
                    return;
                }

                BrandName = brand.Name;
            }
            else
            {
                BrandName = string.Empty;
            }

            DataContext = this;
        }

        private bool ValidateBrand()
        {
            if (string.IsNullOrWhiteSpace(BrandName))
            {
                MessageBox.Show("Введите название бренда!");
                return false;
            }


            if (BrandName.Length > 10000)
            {
                MessageBox.Show("Название должно содержать максимум 10000 символов!");
                return false;
            }

            var existingBrand = _context.Brands
                .AsNoTracking()
                .FirstOrDefault(c => c.Name.ToLower() == BrandName.Trim().ToLower());

            if (existingBrand != null && (!_isEditMode || existingBrand.Id != _brandId))
            {
                MessageBox.Show("Категория с таким названием уже существует!");
                return false;
            }

            return true;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ViewСhangesPage());
        }
        private void SaveBrand_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateBrand())
                return;

            try
            {
                if (_isEditMode)
                {
                    using (var saveContext = new ElectronicStoreContext())
                    {
                        var existingCBrand = saveContext.Brands.Find(_brandId.Value);
                        if (existingCBrand != null)
                        {
                            existingCBrand.Name = BrandName.Trim();
                            saveContext.SaveChanges();
                        }
                    }
                    MessageBox.Show("Бренд обновлен!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                    NavigationService?.Navigate(new ViewChangesBrands());
                }
                else
                {
                    var newBrand = new Brand
                    {
                        Name = BrandName.Trim(),
                    };

                    _context.Brands.Add(newBrand);
                    _context.SaveChanges();

                    MessageBox.Show("Бренд добавлен!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService?.Navigate(new ViewChangesBrands());

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void CancelBrand_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Отменить изменения?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                NavigationService?.Navigate(new ViewChangesBrands());
        }
    }
}
