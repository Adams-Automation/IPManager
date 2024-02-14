using NetworkInterfaceLibrary.Models;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace NetworkInterfaceLibrary;

public class NetworkHelper
{

    public static List<NetworkInterface> GetAllNetworkInterfaces()
    {
        List<NetworkInterface> NICList = new();

        foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if(nic.OperationalStatus == OperationalStatus.Up && 
                (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                nic.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet ||
                nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet3Megabit ||
                nic.NetworkInterfaceType == NetworkInterfaceType.FastEthernetFx ||
                nic.NetworkInterfaceType == NetworkInterfaceType.FastEthernetT))
            {
                NICList.Add(nic);
            }
        }

        return NICList;
    }

    public static IPv4Settings GetIPv4Settings(NetworkInterface NIC)
    {
        IPv4Settings ipv4Setting = new();

        if (NIC == null) { return ipv4Setting; }

        foreach (UnicastIPAddressInformation info in NIC.GetIPProperties().UnicastAddresses)
        {
            if (info.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                //Assign name
                ipv4Setting.Name = NIC.Name;

                //Assign IP address and add it to the list
                ipv4Setting.IPAddress = info.Address.ToString();
                ipv4Setting.IPList.Add(ipv4Setting.IPAddress);

                //Read subnet mask
                ipv4Setting.SubnetMask = info.IPv4Mask.ToString();

                //Look for gateway properties
                foreach (GatewayIPAddressInformation Gatewayinfo in NIC.GetIPProperties().GatewayAddresses)
                {
                    if (Gatewayinfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        ipv4Setting.DefaultGateway = Gatewayinfo.Address.ToString();
                    }
                }

                ipv4Setting.Mode = info.PrefixOrigin.ToString();

            }
        }

        if(ipv4Setting.IPList.Count > 1)
        {
            ipv4Setting.MultipleIPs = true;
        }

        return ipv4Setting;
    }

    public static void SetIpSettings(string Name, string IP, string SubnetMask = "255.255.255.0", string DefaultGateway = "")
    {
        Process p = new Process();
        ProcessStartInfo psi = new ProcessStartInfo("netsh", $"interface ip set address {Name} static {IP} {SubnetMask} {DefaultGateway} 1");
        psi.UseShellExecute = true;
        p.StartInfo = psi;
        psi.Verb = "runas";
        p.Start();
    }

    public static void SetIpDHCP(string Name)
    {
        Process p = new Process();
        ProcessStartInfo psi = new ProcessStartInfo("netsh", $"interface ip set address {Name} dhcp");
        psi.UseShellExecute = true;
        p.StartInfo = psi;
        psi.Verb = "runas";
        p.Start();
    }
}
