using LockManagementUI.Model;
using Ninject;
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
using System.Windows.Shapes;

namespace LockManagementUI.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        [Inject]
        public IKernel Kernel { private get; set; }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var userName = txtUserName.Text;
                var pwd = txtPassword.Password;

                var loginViewModel = DataContext as LoginViewModel;

                if (await loginViewModel.Login(userName, pwd))
                {
                    var form = new MainPage();
                    form.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Login Failed !", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Internal Error. Please Check With Support", "Error", MessageBoxButton.OK, MessageBoxImage.Error); ;
            }
            
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
