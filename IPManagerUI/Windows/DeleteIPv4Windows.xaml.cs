using System.ComponentModel;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

using DatabaseLibrary;
using DatabaseLibrary.Models;

namespace IPManagerUI.Windows;

/// <summary>
/// Interaction logic for DeleteIPv4Windows.xaml
/// </summary>
public partial class DeleteIPv4Windows : Window, INotifyPropertyChanged
{
    public IDatabase Database { get; }

    private IPv4Database _IPv4Database;

    public IPv4Database IPv4Database
    {
        get { return _IPv4Database; }
        set
        {
            _IPv4Database = value;
            OnPropertyChanged(nameof(IPv4Database));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public DeleteIPv4Windows(IDatabase database, IPv4Database ipv4Database)
    {
        InitializeComponent();
        Database = database;
        IPv4Database = ipv4Database;
    }

    public void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    public void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (IPv4Database.Description != string.Empty && IPv4Database.IP != string.Empty)
            {
                Database.DeleteIPSettings(IPv4Database);
            }
            else
            {
                MessageBox.Show("Please fill in the required data.", "Error");
            }

            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{ex.Message}", "Error");
        }
    }
}
