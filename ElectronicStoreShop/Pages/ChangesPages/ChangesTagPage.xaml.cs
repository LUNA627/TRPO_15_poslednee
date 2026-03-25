using ElectronicStoreShop.Models;
using ElectronicStoreShop.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ElectronicStoreShop.Pages.ChangesPages
{
    public partial class ChangesTagPage : Page
    {
        private readonly ElectronicStoreContext _context;
        private readonly int? _tagId;
        private readonly bool _isEditMode;

        public string TagName { get; set; }
        public string TagDescription { get; set; }

        public ChangesTagPage(int? tagId = null)
        {
            InitializeComponent();
            _context = DBService.Instance.Context;

            _tagId = tagId;  
            _isEditMode = tagId.HasValue;

            if (_isEditMode)
            {
                var existingTag = _context.Tags
                    .AsNoTracking()
                    .FirstOrDefault(t => t.Id == _tagId.Value);

                if (existingTag == null)
                {
                    MessageBox.Show("Тег не найден!", "Ошибка");
                    NavigationService?.GoBack();
                    return;
                }

                TagName = existingTag.Name;
            }
            else
            {
                TagName = string.Empty;
                TagDescription = string.Empty;
            }

            DataContext = this;
        }


        private bool ValidateTag()
        {
            if (string.IsNullOrWhiteSpace(TagName))
            {
                MessageBox.Show("Введите название тега!");
                return false;
            }

            if (TagName.Trim().Length < 2)
            {
                MessageBox.Show("Название должно содержать минимум 2 символа!");
                return false;
            }

            if (TagName.Trim().Length > 50)
            {
                MessageBox.Show("Название должно содержать максимум 50 символов!");
                return false;
            }

            var existingTag = _context.Tags
                .AsNoTracking() 
                .FirstOrDefault(t => t.Name.ToLower() == TagName.Trim().ToLower());

            if (existingTag != null && (!_isEditMode || existingTag.Id != _tagId))
            {
                MessageBox.Show("Тег с таким названием уже существует!");
                return false;
            }

            return true;
        }

        private void SaveTag_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateTag())
            {
                return;
            }
            if (_isEditMode)
            {
                using (var saveContext = new ElectronicStoreContext())
                {
                    var existingTag = saveContext.Tags.Find(_tagId.Value);
                    if (existingTag != null)
                    {
                        existingTag.Name = TagName.Trim();
                        saveContext.SaveChanges();

                        MessageBox.Show("Тег обновлён!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                var newTag = new Tag
                {
                    Name = TagName.Trim(),
                };
                _context.Tags.Add(newTag);
                _context.SaveChanges();
            }

            MessageBox.Show("Тег добавлен!",
                "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

            NavigationService?.Navigate(new ViewChangesTags());
        }

  

 

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void CancelTag_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Отменить изменения?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                NavigationService?.Navigate(new ViewChangesTags());
        }
    }
}