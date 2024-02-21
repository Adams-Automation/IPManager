using System.ComponentModel;
using System.Windows;

using DatabaseLibrary;
using DatabaseLibrary.Models;

namespace IPManagerUI.Windows;

/// <summary>
/// Interaction logic for AddIPv4Window.xaml
/// </summary>
public partial class AddIPv4Window : Window, INotifyPropertyChanged
{

    public IDatabase Database { get; }

    private IPv4Database _IPv4Database = new(string.Empty, string.Empty);

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

    private void IPTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        string[] s = IPv4Database.IP.Split(".");
        IPv4Database ip = IPv4Database;
        
        //Validity check IP address
        if(s.Length != 4 )
        {
            return;
        }

        //Validity check default gateway
        if(ip.DefaultGateway.Split(".").Length != 4)
        {
            ip.DefaultGateway = string.Empty;
        }
        else { return; }

        for (int i = 0; i < s.Length; i++)
        {
            //First occurence without a leading dot
            if(i != s.Length && i == 0)
            {
                ip.DefaultGateway += s[i];
            }else if(i != s.Length - 1)
            {
                ip.DefaultGateway += "." + s[i];
            }
            else
            {
                ip.DefaultGateway += ".1";
            }
        }

        IPv4Database = ip;
    }
}
