using ElectronicStoreShop.Models;
using ElectronicStoreShop.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ElectronicStoreShop.Pages.ChangesPages
{
    public partial class ChangesProductPage : Page
    {
        private readonly ElectronicStoreContext _context;
        private readonly int? _productId;
        private readonly bool _isEditMode;

        public Product Product { get; set; }
        public ObservableCollection<Category> Categories { get; set; } = new();
        public ObservableCollection<Brand> Brands { get; set; } = new();
        public ObservableCollection<Tag> Tags { get; set; } = new();

        private List<int> _selectedTagIds = new();

        public ChangesProductPage(int? productId = null)
        {
            InitializeComponent();
            _context = DBService.Instance.Context;

            _productId = productId;
            _isEditMode = productId.HasValue;

            if (_isEditMode)
            {
                Product = _context.Products
                    .AsNoTracking()  
                    .Include(p => p.ProductTags)
                    .FirstOrDefault(p => p.Id == _productId.Value);

                if (Product == null)
                {
                    MessageBox.Show("Товар не найден!", "Ошибка");
                    NavigationService?.GoBack();
                    return;
                }
            }
            else
            {
                Product = new Product();
            }

            DataContext = this;
            LoadTags();
            LoadCategoriesAndBrands();

            if (_isEditMode && Product?.ProductTags != null)
            {
                SelectExistingTags();
            }
        }


        private void LoadTags()
        {
            Tags.Clear();

            var tags = _context.Tags
                .AsNoTracking()  
                .Include(t => t.ProductTags)
                .Select(t => new Tag
                {
                    Id = t.Id,
                    Name = t.Name,
                })
                .OrderBy(t => t.Name)
                .ToList();

            foreach (var tag in tags)
            {
                Tags.Add(tag);
            }
        }

        private void LoadCategoriesAndBrands()
        {
            Categories = new ObservableCollection<Category>(
                _context.Categories.AsNoTracking().ToList());

            Brands = new ObservableCollection<Brand>(
                _context.Brands.AsNoTracking().ToList());
        }


        private void SelectExistingTags()
        {
            if (Product?.ProductTags == null) return;

            TagsListBox.SelectedItems.Clear();
            foreach (var productTag in Product.ProductTags)
            {
                var tag = Tags.FirstOrDefault(t => t.Id == productTag.TagId);
                if (tag != null)
                {
                    TagsListBox.SelectedItems.Add(tag);
                }
            }
        }


        private bool ValidateFields()
        {
            
            if (string.IsNullOrWhiteSpace(Product.Name))
            {
                MessageBox.Show("Введите название товара!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return false;
            }

            if (Product.Price <= 0)
            {
                MessageBox.Show("Цена должна быть больше 0!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                PriceTextBox.Focus();
                return false;
            }

           
            if (Product.Stock <= 0)
            {
                MessageBox.Show("Остаток не может быть отрицательным и равным 0!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                StockTextBox.Focus();
                return false;
            }

            if (Product.Rating < 0 || Product.Rating > 5)
            {
                MessageBox.Show("Рейтинг должен быть от 0 до 5!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                RatingTextBox.Focus();
                return false;
            }

            if (Product.CategoryId == 0)
            {
                MessageBox.Show("Выберите категорию!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                CategoryComboBox.Focus();
                return false;
            }

            if (Product.BrandId == 0)
            {
                MessageBox.Show("Выберите бренд!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                BrandComboBox.Focus();
                return false;
            }


            return true; 
        }


        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            try
            {
                if (_isEditMode)
                {
                    var existingProduct = _context.Products
                       .Include(p => p.ProductTags)
                       .First(p => p.Id == Product.Id);

                    existingProduct.Name = Product.Name;
                    existingProduct.Price = Product.Price;
                    existingProduct.Stock = Product.Stock;
                    existingProduct.Rating = Product.Rating;
                    existingProduct.CategoryId = Product.CategoryId;
                    existingProduct.BrandId = Product.BrandId;
                    existingProduct.Description = Product.Description;

                    UpdateTags(existingProduct);

                    _context.SaveChanges();


                    MessageBox.Show("Товар обновлён!", "Успешно",
                   MessageBoxButton.OK,
                   MessageBoxImage.Information);

                    NavigationService?.Navigate(new ViewChangesProducts());
                }
                else
                {
                    var newProduct = new Product
                    {
                        Name = Product.Name,
                        Price = Product.Price,
                        Stock = Product.Stock,
                        Rating = Product.Rating,
                        CategoryId = Product.CategoryId,
                        BrandId = Product.BrandId,
                        Description = Product.Description,
                        CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                    };

                    _context.Products.Add(newProduct);
                    _context.SaveChanges(); 

                    if (_selectedTagIds.Any())
                    {
                        foreach (var tagId in _selectedTagIds)
                        {
                            _context.ProductTags.Add(new ProductTag
                            {
                                ProductId = newProduct.Id,
                                TagId = tagId
                            });
                        }
                        _context.SaveChanges();
                    }

                    MessageBox.Show("Товар добавлен!", "Успешно",
                   MessageBoxButton.OK,
                   MessageBoxImage.Information);

                    NavigationService?.Navigate(new ViewChangesProducts());
                }

               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void UpdateTags(Product product)
        {
            var oldTagIds = product.ProductTags.Select(pt => pt.TagId).ToList();
            var newTagIds = _selectedTagIds;

            foreach (var productTag in product.ProductTags.ToList())
            {
                if (!newTagIds.Contains(productTag.TagId))
                {
                    _context.ProductTags.Remove(productTag);
                }
            }

            foreach (var tagId in newTagIds)
            {
                if (!oldTagIds.Contains(tagId))
                {
                    _context.ProductTags.Add(new ProductTag
                    {
                        ProductId = product.Id,
                        TagId = tagId
                    });
                }
            }
        }

        

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Отменить изменения?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                NavigationService?.Navigate(new ViewChangesProducts());
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void TagsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedTagIds = TagsListBox.SelectedItems
                .Cast<Tag>()
                .Select(t => t.Id)
                .ToList();
        }
    }
}