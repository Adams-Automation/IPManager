using System.Data.SQLite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using NetworkInterfaceLibrary.Models;
using DatabaseLibrary.Models;

namespace DatabaseLibrary;

public class Database : IDatabase
{
    private ILogger<Database>? Logger;

    public string DatabasePath { get; set; }

    #region Constructors

    public Database(IConfiguration configuration)
    {
        string? databasePath = configuration.GetConnectionString("DatabasePath");
        if (databasePath == null)
        {
            databasePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,"Resources",
                                        "IPManager.db");
        }
        DatabasePath = $"Data Source={databasePath};Version=3;Compress=True;";
    }

    public Database(IConfiguration configuration, ILogger<Database> logger) : this(configuration)
    {
        Logger = logger;
    }

    public Database(string databasePath)
    {
        DatabasePath = $"Data Source={databasePath};Version=3;Compress=True;";
    }

    #endregion

    #region Private methods

    private void WriteQuery(string querry)
    {
        if (Logger != null)
        {
            Logger.LogInformation($"Database: {DatabasePath}" +
            $"Writing querry: {querry}");
        }
        try
        {
            using (SQLiteConnection Db = new SQLiteConnection(DatabasePath))
            {
                Db.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(querry, Db))
                {
                    Console.WriteLine(querry);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            if (Logger != null) { Logger.LogError(ex, $"Trying to write querry: {querry}"); }
            throw;
        }
    }

    #endregion

    #region Public methods

    #region Create querries
    public void CreateNewIPSettings(IPv4Database ipv4settings)
    {
        string Querry = $"INSERT INTO IPList (Description, IP, SubnetMask, DefaultGateway)" +
                        $"VALUES('{ipv4settings.Description}','{ipv4settings.IP}','{ipv4settings.SubnetMask}','{ipv4settings.DefaultGateway}')";

        WriteQuery(Querry);
    }

    #endregion

    #region Get querries
    public List<IPv4Database> GetAllIPs()
    {
        List<IPv4Database> IPList = new();
        using (SQLiteConnection Db = new SQLiteConnection(DatabasePath))
        {
            Db.Open();

            string Querry = $"SELECT * FROM IPList";

            using (SQLiteCommand cmd = new SQLiteCommand(Querry, Db))
            {
                using (SQLiteDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        if (dataReader.GetInt32(0) != 0)
                        {
                            IPv4Database ipv4 = new(dataReader.GetString(1),
                                                    dataReader.GetString(2),
                                                    dataReader.GetString(3),
                                                    dataReader.GetString(4));
                            ipv4.ID = dataReader.GetInt32(0);

                            IPList.Add(ipv4);
                        }
                    }
                }
            }
        }
        return IPList;
    }

    #endregion

    #region Delete querries

    public void DeleteIPSettings(int? referenceID)
    {
        if (referenceID == null)
        {
            return;
        }

        string Querry = $"DELETE FROM ReferenceTags \n" +
                        $"WHERE ID={referenceID}";

        WriteQuery(Querry);
    }

    #endregion

    #region Update querries

    public void UpdateIPSettings(string reference)
    {
        string Querry = $"UPDATE ReferenceTags SET Tag = '{reference}', Location = '{reference}' \n" +
                        $"WHERE ID = '{reference}'";

        WriteQuery(Querry);
    }

    #endregion

    #region settings

    public void SetDatabaseFilePath(string path)
    {
        DatabasePath = $"Data Source={path};Version=3;Compress=True;";
    }
    #endregion

    #endregion
}