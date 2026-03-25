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
using ElectronicStoreShop.Pages;

namespace ElectronicStoreShop.Pages
{
    /// <summary>
    /// Логика взаимодействия для ManagerPasswordPage.xaml
    /// </summary>
    public partial class ManagerPasswordPage : Page
    {
        public ManagerPasswordPage()
        {
            InitializeComponent();
        }

        private void BtnEnterManager_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(PassBoxInput.Password))
            {
                MessageBox.Show("Введите пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (PassBoxInput.Password == "1234")
            {
                App.CurrentUserType = App.UserType.Manager;
                NavigationService.Navigate(new MainProductPage());


            }
            else
            {
                PassBoxInput.Clear();
                MessageBox.Show("Неправильный пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void BtnBackManager_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
