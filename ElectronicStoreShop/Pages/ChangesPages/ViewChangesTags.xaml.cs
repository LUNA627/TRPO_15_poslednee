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
    /// Логика взаимодействия для ViewChangesTags.xaml
    /// </summary>
    public partial class ViewChangesTags : Page
    {

        private readonly ElectronicStoreContext _context;
        public ObservableCollection<Tag> Tags { get; set; } = new();

        public ViewChangesTags()
        {
            InitializeComponent();
            _context = DBService.Instance.Context;
            DataContext = this;
            LoadTags();
        }

        private void LoadTags()
        {
            Tags.Clear();

            var tags = _context.Tags
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

        private void AddTag_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ChangesTagPage(null));
        }

        private void EditTag_Click(object sender, RoutedEventArgs e)
        {
            if (TagsListView.SelectedItem is Tag selectedTag)
            {
                NavigationService?.Navigate(new ChangesTagPage(selectedTag.Id));
            }
            else
            {
                MessageBox.Show("Выберите тег для редактирования!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteTag_Click(object sender, RoutedEventArgs e)
        {
            if (TagsListView.SelectedItem is Tag selectedTag)
            {

                bool hasDelete = _context.ProductTags.Any(pt => pt.TagId == selectedTag.Id);
                if (hasDelete)
                {
                    MessageBox.Show("Нельзя удалить тег, в которой есть товар.",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show($"Удалить тег {selectedTag.Name} ?", "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                {
                    return;
                }

                try
                {
                    using (var deleteContext = new ElectronicStoreContext())
                    {
                        var productTags = deleteContext.ProductTags
                            .Where(pt => pt.TagId == selectedTag.Id)
                            .ToList();

                        if (productTags.Any())
                        {
                            deleteContext.ProductTags.RemoveRange(productTags);
                        }

                        var tagToDelete = deleteContext.Tags
                            .Find(selectedTag.Id);

                        if (tagToDelete != null)
                        {
                            deleteContext.Tags.Remove(tagToDelete);
                            deleteContext.SaveChanges();
                        }
                    }

                    MessageBox.Show("Тег успешно удалён!", "Успешно",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadTags();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите тег для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void TagsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ViewСhangesPage());
        }
    }
}
