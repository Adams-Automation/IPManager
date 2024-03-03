using IPManagerUI.Properties;
using MaterialDesignThemes.Wpf;
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

namespace IPManagerUI.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            InitializeDarkModeToggleButton();
        }

        private void InitializeDarkModeToggleButton()
        {
            var paletteHelper = new PaletteHelper();
            //Retrieve the app's existing theme
            var theme = paletteHelper.GetTheme();

            if (theme.GetBaseTheme() == BaseTheme.Dark)
            {
                DarkModeToggleButton.IsChecked = true;
            }
            else { DarkModeToggleButton.IsChecked = false; }

        }

        private void DarkModeToggleButton_Pressed(object sender, RoutedEventArgs e)
        {
            var paletteHelper = new PaletteHelper();
            //Retrieve the app's existing theme
            var theme = paletteHelper.GetTheme();

            if ((bool)DarkModeToggleButton.IsChecked!)
            {
                theme.SetBaseTheme(BaseTheme.Dark);
                UserSettings.Default.DarkTheme = true;
                UserSettings.Default.Save();
            }
            else if (!(bool)DarkModeToggleButton.IsChecked!)
            {
                theme.SetBaseTheme(BaseTheme.Light);
                UserSettings.Default.DarkTheme = false;
                UserSettings.Default.Save();
            }

            //Change the app's current theme
            paletteHelper.SetTheme(theme);
        }
    }
}
