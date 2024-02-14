using DatabaseLibrary.Models;

namespace DatabaseLibrary;

public interface IDatabase
{
    string DatabasePath { get; set; }

    void CreateNewIPSettings(IPv4Database ipv4settings);
    void DeleteIPSettings(int? referenceID);
    List<IPv4Database> GetAllIPs();
    void SetDatabaseFilePath(string path);
    void UpdateIPSettings(string reference);
}