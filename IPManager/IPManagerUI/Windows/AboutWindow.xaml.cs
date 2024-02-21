using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace IPManagerUI.Windows
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        private string _WebPage = "https://adamsautomation.be";

        public string WebPage
        {
            get { return _WebPage; }
        }

        public string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private string _GitHubPage = "https://github.com/Adams-Automation/IPManager";
        public string GitHubPage
        {   
            get { return _GitHubPage; } 
        }

        public AboutWindow()
        {
            InitializeComponent();
        }

        private void GithubTextBlock_Click(object sender, RoutedEventArgs e)
        {
            ShowWebPage(GitHubPage);
        }

        private void WebPageTextBlock_Click(object sender, RoutedEventArgs e)
        {
            ShowWebPage(WebPage);
        }

        private static void ShowWebPage(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }
    }
}
