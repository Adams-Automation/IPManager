using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;

using NetworkInterfaceLibrary;
using NetworkInterfaceLibrary.Models;
using DatabaseLibrary.Models;
using DatabaseLibrary;
using IPManagerUI.Windows;

using MessageBox = System.Windows.MessageBox;
using System.Reflection;

namespace IPManagerUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    #region Variables

    #region Network Variables

    private List<NetworkInterface> _NICList = new();

    public List<NetworkInterface> NICList
    {
        get { return _NICList; }
        set
        {
            _NICList = value;
            OnPropertyChanged(nameof(NICList));
        }
    }

    private NetworkInterface _SelectedNic;

    public NetworkInterface SelectedNic
    {
        get { return _SelectedNic; }
        set
        {
            _SelectedNic = value;
            OnPropertyChanged(nameof(SelectedNic));
        }
    }

    private IPv4Settings _IPv4Settings;

    public IPv4Settings IPv4Settings
    {
        get { return _IPv4Settings; }
        set
        {
            _IPv4Settings = value;
            OnPropertyChanged(nameof(IPv4Settings));
        }
    }

    private string NICName = string.Empty;

    #endregion

    #region User Information Variables

    private string _ProcessInfo = "Test.";

    public string ProcessInfo { 
        get { return _ProcessInfo; } 
        set 
        { 
            _ProcessInfo = value;
            OnPropertyChanged(nameof(ProcessInfo));
        } 
    }

    #endregion

    #region Database Variables

    private List<IPv4Database> _IPList;

    public List<IPv4Database> IPList
    {
        get { return _IPList; }
        set 
        { 
            _IPList = value; 
            OnPropertyChanged(nameof(IPList));
        }
    }

    public IPv4Database? SelectedIP { get; set; }

    #endregion

    #endregion

    #region Interfaces

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private IDatabase _Database;

    #endregion

    public MainWindow(IDatabase database)
    {
        InitializeComponent();

        //Set interfaces
        _Database = database;

        //Populate variables
        NICList = NetworkHelper.GetAllNetworkInterfaces();
        SelectedNic = NICList.FirstOrDefault()!;
        IPv4Settings = NetworkHelper.GetIPv4Settings(SelectedNic);
        IPList = _Database.GetAllIPs();

        //Connect event
        NetworkChange.NetworkAddressChanged += new
        NetworkAddressChangedEventHandler(AddressChangedCallback);

        _Database.IPListChanged += RepopulateIPList;
    }

    #region Private methods

    private async void AddressChangedCallback(object? sender, EventArgs e)
    {
        ProcessInfo = "Reading new data ...";

        await Task.Run(() =>
        {
            NICList = NetworkHelper.GetAllNetworkInterfaces();

            var temp = NICList.Where(x => x.Name.ToString() == NICName).FirstOrDefault();

            if (temp != null)
            {
                SelectedNic = temp;
            }
            else
            {
                SelectedNic = NICList.FirstOrDefault()!;
            }
        });

        ProcessInfo = "Done.";
    }

    private void NetworkAdaptorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ProcessInfo = "Selecting interface.";

        //Read IP settings from the selected NIC
        IPv4Settings = NetworkHelper.GetIPv4Settings(SelectedNic);

        //Save name for auto select NIC when NetworkAddressChanged event comes up
        if (SelectedNic != null) 
        { 
            if (SelectedNic.Name != string.Empty && SelectedNic.Name != null)
            {
                NICName = SelectedNic.Name;
            }
        }

        ProcessInfo = "Done.";
    }

    private void RepopulateIPList(object? sender, EventArgs e)
    {
        IPList = _Database.GetAllIPs();
    }

    #endregion

    #region Network Commands

    public async void setIPAddres_click(object sender, EventArgs e)
    {
        ProcessInfo = "Setting IP Address.";
        if (SelectedNic != null)
        {
            if(SelectedIP != null)
            {
                await Task.Run(() =>
                {
                    NetworkHelper.SetIpSettings(SelectedNic.Name, SelectedIP.IP, SelectedIP.SubnetMask, SelectedIP.DefaultGateway);
                });
            }
            else
            {
                MessageBox.Show("Please select an IP from the list.", "Error");
            }
        }
        else
        {
            MessageBox.Show("Please select an network adapter.", "Error");
        }
        ProcessInfo = "Done.";
    }

    public async void setDHCP_Click(object sender, EventArgs e)
    {
        ProcessInfo = "Setting DHCP.";
        if (SelectedNic != null)
        {
            await Task.Run(() =>
            {
                NetworkHelper.SetIpDHCP(SelectedNic.Name);
            });
        }
        else
        {
            MessageBox.Show("Please select an network adapter.", "Error");
        }
        ProcessInfo = "Done.";
    }

    #endregion

    #region Database Commands

    private void AddNewIPButton_Click(object sender, RoutedEventArgs e)
    {
        AddIPv4Window addIPv4Window = new(_Database);
        addIPv4Window.Show();
    }

    private void EditIPButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedIP != null)
        {
            EditIPv4Window editIPv4Window = new(_Database, SelectedIP);
            editIPv4Window.Show();
        }
        else
        {
            MessageBox.Show("Please select an IP from the list.");
        }
    }

    private void DeleteIPButton_Click(object sender, RoutedEventArgs e)
    {
        if(SelectedIP != null)
        {
            DeleteIPv4Windows deleteIPv4Window = new(_Database, SelectedIP);
            deleteIPv4Window.Show();
        }
        else
        {
            MessageBox.Show("Please select an IP from the list.");
        }
    }

    #endregion

    #region Info Commands

    private void AboutButton_Click(object sender, RoutedEventArgs e)
    {
        AboutWindow aboutWindow = new AboutWindow();
        aboutWindow.Show();
    }

    private void HelpButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            System.Windows.Forms.Help.ShowHelp(null, "Help/PowerCollections.chm");
        } catch(Exception ex) { MessageBox.Show(ex.Message); }
    }

    #endregion

}