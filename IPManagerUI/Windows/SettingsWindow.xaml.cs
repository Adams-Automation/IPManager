using DatabaseLibrary;
using IPManagerUI.Properties;
using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Windows;

using MessageBox = System.Windows.MessageBox;

namespace IPManagerUI.Windows;

/// <summary>
/// Interaction logic for SettingsWindow.xaml
/// </summary>
/// 

public partial class SettingsWindow : Window, INotifyPropertyChanged
{
    public IDatabase Database { get; }

    private List<string> _IgnoreList;

    public List<string> IgnoreList
    {
        get { return _IgnoreList; }
        set 
        { 
            _IgnoreList = value;
            OnPropertyChanged(nameof(IgnoreList));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public SettingsWindow(IDatabase database)
    {
        InitializeComponent();
        InitializeDarkModeToggleButton();

        DatabaseLocationTextBlock.Text = UserSettings.Default.DatabaseLocation;
        Database = database;

        Database.IgnoreListChanged += RepopulateIgnoreList;
        
        IgnoreList = Database.GetIgnoreNames();
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

    private void RepopulateIgnoreList(object? sender, EventArgs e)
    {
        IgnoreList.Clear();
        IgnoreList = Database.GetIgnoreNames();
    }

    private void RestoreButton_Click(object sender, RoutedEventArgs e)
    {
        if (IgnoreNICListView.SelectedItem != null)
        {
            Database.DeleteIgnoreListName(IgnoreNICListView.SelectedItem.ToString());
        }
        else
        {
            MessageBox.Show("Please select an item before restoring.");
        }
    }

    private void SearchDatabaseButton_Click(object sender, RoutedEventArgs e)
    {
        // Create OpenFileDialog 
        Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

        // Set filter for file extension and default file extension 
        dlg.DefaultExt = ".db";
        dlg.Filter = "Database File (*.db)|*.db";

        // Display OpenFileDialog by calling ShowDialog method 
        Nullable<bool> result = dlg.ShowDialog();


        // Get the selected file name and display in a TextBox 
        if (result == true)
        {
            // Set document location
            string filename = dlg.FileName;
            DatabaseLocationTextBlock.Text = filename;
            Database.SetDatabaseFilePath(filename);
            UserSettings.Default.DatabaseLocation = filename;
            UserSettings.Default.Save();
        }

    }

    private void IgnoreNICListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if(IgnoreNICListView.SelectedItem != null)
        {
            RestoreButton.IsEnabled = false;
        }
        else { RestoreButton.IsEnabled = true; }
    }

}
