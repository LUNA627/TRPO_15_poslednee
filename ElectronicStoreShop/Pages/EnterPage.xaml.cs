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
using ElectronicStoreShop.Service;

namespace ElectronicStoreShop.Pages
{
    /// <summary>
    /// Логика взаимодействия для EnterPage.xaml
    /// </summary>
    public partial class EnterPage : Page
    {
        public EnterPage()
        {
            InitializeComponent();
        }

        private void BtnEnterVisitor_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUserType = App.UserType.Visitor;
            NavigationService.Navigate(new MainProductPage());
            Window window = Application.Current.MainWindow;
            window.Title = "Каталог товаров (пользователь)";
        }

        private void BtnEnterManager_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManagerPasswordPage());
            Window window = Application.Current.MainWindow;
            window.Title = "Вход для менеджера";
        }
    }
}
