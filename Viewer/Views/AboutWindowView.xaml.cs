using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace Viewer.Views
{
    /// <summary>
    /// Interaction logic for AboutWindowView.xaml
    /// </summary>
    public partial class AboutWindowView : Window
    {
        public AboutWindowView()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Hyperlink_MailTo(object sender, RequestNavigateEventArgs e)
        {
            var hyperlink = sender as Hyperlink;
            var address = "mailto:" + hyperlink.NavigateUri;
            try
            {
                Process.Start(address);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("That e-mail address is invalid.", "E-mail error");
            }
        }
    }
}
