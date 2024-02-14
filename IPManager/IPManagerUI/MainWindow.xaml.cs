using NetworkInterfaceLibrary;
using NetworkInterfaceLibrary.Models;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Management;
using System.Diagnostics;

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

    #region Visibility Variables

    private string _ProcessInfo = "Test.";

    public string ProcessInfo { 
        get { return _ProcessInfo; } 
        set 
        { 
            _ProcessInfo = value;
            OnPropertyChanged(nameof(ProcessInfo));
        } 
    }

    private Visibility _ProcessInfoVisibility = Visibility.Hidden;

    public Visibility ProcessInfoVisibility { 
        get { return _ProcessInfoVisibility; }
        set
        {
            _ProcessInfoVisibility |= value;
            OnPropertyChanged(nameof(ProcessInfoVisibility));
        } 
    }

    #endregion

    #endregion

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        NICList = NetworkHelper.GetAllNetworkInterfaces();
        SelectedNic = NICList.FirstOrDefault()!;

        IPv4Settings = NetworkHelper.GetIPv4Settings(SelectedNic);

        NetworkChange.NetworkAddressChanged += new
        NetworkAddressChangedEventHandler(AddressChangedCallback);
    }

    public async void AddressChangedCallback(object? sender, EventArgs e)
    {
        ProcessInfoVisibility = Visibility.Visible;
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

    public void NetworkAdaptorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

    public async void setIPAddres_click(object sender, EventArgs e)
    {
        ProcessInfo = "Setting IP Address.";
        if (SelectedNic != null)
        {
            await Task.Run(() =>
            {
                NetworkHelper.SetIpSettings(SelectedNic.Name, "12.43.56.98", "255.255.255.0", "12.43.56.1");
            });
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
}