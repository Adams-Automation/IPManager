using DatabaseLibrary.Models;

namespace DatabaseLibrary
{
    public interface IDatabase
    {
        string DatabasePath { get; set; }

        void CreateNewIPSettings(IPv4Database ipv4settings);
        void DeleteIPSettings(IPv4Database IPv4Database);
        List<IPv4Database> GetAllIPs();
        void SetDatabaseFilePath(string path);
        void UpdateIPSettings(IPv4Database IPv4Database);

        //Events
        public event EventHandler IPListChanged;
    }
}