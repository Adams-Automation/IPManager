using System.ComponentModel;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

using DatabaseLibrary;
using DatabaseLibrary.Models;

namespace IPManagerUI.Windows;

/// <summary>
/// Interaction logic for EditIPv4Window.xaml
/// </summary>
public partial class EditIPv4Window : Window, INotifyPropertyChanged
{

    public IDatabase Database { get; }

    private IPv4Database _IPv4Database;

    public IPv4Database IPv4Database { 
        get { return _IPv4Database; }
        set { 
            _IPv4Database = value;
            OnPropertyChanged(nameof(IPv4Database));
        } 
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    public EditIPv4Window(IDatabase database, IPv4Database selectedIP)
    {
        InitializeComponent();
        Database = database;
        IPv4Database = selectedIP;
    }

    public void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    public void EditButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (IPv4Database.Description != string.Empty && IPv4Database.IP != string.Empty)
            {
                Database.UpdateIPSettings(IPv4Database);
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
