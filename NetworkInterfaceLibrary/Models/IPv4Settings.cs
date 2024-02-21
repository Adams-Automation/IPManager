using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkInterfaceLibrary.Models;

public class IPv4Settings
{
    public List<string> IPList { get; set; }
    public string IPAddress { get; set; }
    public string SubnetMask { get; set; }
    public string DefaultGateway { get; set; }

    public bool MultipleIPs { get; set; }

    public string Mode { get; set; }
    public string Name { get; set; }

    public IPv4Settings()
    {
        IPList = new List<string>();
        IPAddress = string.Empty;
        SubnetMask = "Not set";
        DefaultGateway = "Not set";
        MultipleIPs = false;
        Mode = string.Empty;
        Name = string.Empty;
    }
}
