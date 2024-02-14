using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLibrary.Models;

public class IPv4Database(string description, string ip)
{
    public int? ID { get; set; }
    public string Description { get; set; } = description;
    public string IP { get; set; } = ip;
    public string SubnetMask { get; set; } = "255.255.255.0";
    public string DefaultGateway { get; set; } = string.Empty;

    public IPv4Database(string description, string ip, string subnetmask) : this(description, ip)
    {
        SubnetMask = subnetmask;
    }

    public IPv4Database(string description, string ip, string subnetmask, string defaultgateway) : this(description, ip, subnetmask)
    {
        DefaultGateway = defaultgateway;
    }
}
