using System.Configuration;
using System.Data;
using System.Windows;

namespace ElectronicStoreShop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public enum UserType { Visitor, Manager };
        public static UserType CurrentUserType { get; set; } = UserType.Visitor;
    }

}
