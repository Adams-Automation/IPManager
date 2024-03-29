﻿using System.Reflection;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

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

        private string _LinkedInPage = "https://www.linkedin.com/company/adamsautomation/";

        public string LinkedInPage
        {
            get { return _LinkedInPage; }
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

        private void CopyWebsiteButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(WebPage);
        }

        private void CopyGithubButton_Click(object sender, RoutedEventArgs e)
        {            
            System.Windows.Clipboard.SetText(GitHubPage);
        }

        private void CopyLinkedInButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(LinkedInPage);
        }
    }
}
