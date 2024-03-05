using DatabaseLibrary.Models;

namespace DatabaseLibrary
{
    public interface IDatabase
    {
        string DatabasePath { get; set; }

        void CreateNewIPSettings(IPv4Database ipv4settings);
        void CreateIgnoreNetworkName(string name);

        void DeleteIPSettings(IPv4Database IPv4Database);
        void DeleteIgnoreListName(string name);

        List<IPv4Database> GetAllIPs();
        List<string> GetIgnoreNames();

        void SetDatabaseFilePath(string path);
        void CreateNewDatabase(string path);

        void UpdateIPSettings(IPv4Database IPv4Database);

        //Events
        event EventHandler DatabaseChanged;
        event EventHandler IPListChanged;
        event EventHandler IgnoreListChanged;
    }
}