using System.Data.SQLite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace SyntaxCheckerDatabaseLibrary;

public class Database
{
    private ILogger<Database>? Logger;

    public string DatabasePath { get; set; }

    public Database(IConfiguration configuration)
    {
        string? databasePath = configuration.GetConnectionString("DatabasePath");
        if (databasePath == null)
        {
            databasePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
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
    public void CreateNewIPSettings(string Name, string Prefix)
    {
        string Querry = $"INSERT INTO Project (Name, Prefix)" +
                        $"VALUES('{Name}','{Prefix}')";

        WriteQuery(Querry);
    }

    #endregion

    #region Get querries
    public int GetIPList(string Name)
    {
        using (SQLiteConnection Db = new SQLiteConnection(DatabasePath))
        {
            Db.Open();

            string Querry = $"SELECT ID FROM Project WHERE Name = '{Name}'";

            using (SQLiteCommand cmd = new SQLiteCommand(Querry, Db))
            {
                using (SQLiteDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        if (dataReader.GetInt32(0) != 0)
                        {
                            return dataReader.GetInt32(0);
                        }
                    }
                    //TODO create exception
                    return -1;
                }
            }
        }

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