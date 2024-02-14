using System.Windows;

using DatabaseLibrary;
using DatabaseLibrary.Models;

namespace IPManagerUI.Windows;

/// <summary>
/// Interaction logic for AddIPv4Window.xaml
/// </summary>
public partial class AddIPv4Window : Window
{
    public IPv4Database IPv4Database { get; set; } = new(string.Empty, string.Empty);
    public IDatabase Database { get; }

    public AddIPv4Window(IDatabase database)
    {
        InitializeComponent();
        Database = database;
    }

    public void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    public void CreateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (IPv4Database.Description != string.Empty && IPv4Database.IP != string.Empty)
            {
                Database.CreateNewIPSettings(IPv4Database);
            }
            else
            {
                MessageBox.Show("Please fill in the required data.", "Error");
            }

            this.Close();
        } 
        catch(Exception ex)
        {
            MessageBox.Show($"{ex.Message}", "Error");
        }
    }
}
