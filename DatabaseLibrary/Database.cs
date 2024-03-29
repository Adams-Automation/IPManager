﻿using System.Data.SQLite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using NetworkInterfaceLibrary.Models;
using DatabaseLibrary.Models;
using System.CodeDom;
using System;

namespace DatabaseLibrary;

public class Database : IDatabase
{
    private ILogger<Database>? Logger;

    public string DatabasePath { get; set; }

    #region Events
    public event EventHandler DatabaseChanged;
    public event EventHandler IPListChanged;
    public event EventHandler IgnoreListChanged;
    #endregion


    #region Constructors

    public Database(IConfiguration configuration)
    {
        string? databasePath = configuration.GetConnectionString("DatabasePath");
        if (databasePath == null)
        {
            databasePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                                        "IPManager.db");
        }
        SetDatabasePath(databasePath);
    }

    public Database(IConfiguration configuration, ILogger<Database> logger) : this(configuration)
    {
        Logger = logger;
    }

    public Database(string databasePath)
    {
        SetDatabasePath(databasePath);
    }

    #endregion

    #region Private methods

    private void WriteQuery(string querry)
    {
        Logger?.LogInformation($"Database: {DatabasePath} Writing querry: {querry}");
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
            Logger?.LogError(ex, $"Trying to write querry: {querry}");
            throw;
        }
    }

    private void SetDatabasePath(string databasePath)
    {
        if(!File.Exists(databasePath))
        {
            Logger?.LogError("Database path does not exist.");
            CreateNewDatabase(databasePath);
        }
        else if (Path.GetExtension(databasePath) != ".db")
        {
            Logger?.LogError("Database extension is incorrect.");
            CreateNewDatabase(databasePath);
        }else
        {
            Logger?.LogInformation($"Using following database : {databasePath}.");
            DatabasePath = $"Data Source={databasePath};Version=3;Compress=True;";

            //Push event when new database is set
            DatabaseChanged?.Invoke(this, EventArgs.Empty);
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

        //Trigger event so dependancies knows to reload list
        IPListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void CreateIgnoreNetworkName(string name)
    {
        string Querry = $"INSERT INTO IgnoreList (Name)" +
                        $"VALUES('{name}')";

        WriteQuery(Querry);

        //Trigger event for dependancies
        IgnoreListChanged?.Invoke(this, EventArgs.Empty);
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

    public List<string> GetIgnoreNames()
    {
        List<string> IgnoreList = new();
        using (SQLiteConnection Db = new SQLiteConnection(DatabasePath))
        {
            Db.Open();

            string Querry = $"SELECT * FROM IgnoreList";

            using (SQLiteCommand cmd = new SQLiteCommand(Querry, Db))
            {
                using (SQLiteDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        if (dataReader.GetInt32(0) != 0)
                        {
                            IgnoreList.Add(dataReader.GetString(1));
                        }
                    }
                }
            }
        }
        return IgnoreList;
    }

    #endregion

    #region Delete querries

    public void DeleteIPSettings(IPv4Database IPv4Database)
    {
        string Querry = $"DELETE FROM IPList \n" +
                        $"WHERE ID={IPv4Database.ID}";

        WriteQuery(Querry);

        //Trigger event so dependancies knows to reload list
        IPListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void DeleteIgnoreListName(string name)
    {
        string Querry = $"DELETE FROM IgnoreList \n" +
                        $"WHERE Name='{name}'";

        WriteQuery(Querry);

        //Trigger event so that dependancies knows to reload list
        IgnoreListChanged?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Update querries

    public void UpdateIPSettings(IPv4Database IPv4Database)
    {
        string Querry = $"UPDATE IPList SET Description = '{IPv4Database.Description}', " +
                        $"IP = '{IPv4Database.IP}', " +
                        $"SubnetMask = '{IPv4Database.SubnetMask}', " +
                        $"DefaultGateway = '{IPv4Database.DefaultGateway}' \n" +
                        $"WHERE ID = '{IPv4Database.ID}'";

        WriteQuery(Querry);

        //Trigger event so UI knows to reload list
        IPListChanged?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region settings

    public void SetDatabaseFilePath(string path)
    {
        SetDatabasePath(path);

        //Trigger event so UI knows to reload list
        IPListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void CreateNewDatabase(string path)
    {
        //Check if the extension is correct, if not add extension to the file
        if (Path.GetExtension(path) != ".db") { Path.ChangeExtension(path, ".db"); }

        //Delete existing file so that the new file can be copied to the existing location
        if(File.Exists(path)) { File.Delete(path);}

        //Copy the local empty database
        File.Copy(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                               "Resources",
                               "IPManager.db"), 
                  path);

        //Set database path
        DatabasePath = $"Data Source={path};Version=3;Compress=True;";

        //Trigger event so dependancies know that the database is changed
        DatabaseChanged?.Invoke(this, EventArgs.Empty);
    }
    
    #endregion

    #endregion
}