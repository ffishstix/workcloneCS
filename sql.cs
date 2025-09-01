using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Text.Json;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace WorkCloneCS;

class SQL
{

    private static bool testFiles = false;
    private static List<catagory> catagoriesFromFile;
    private static string jsonDir;
    private static string jsonstaffDir;
    private static string connectionString;
    
    public static string ConnectionString { set{  connectionString = value; } }
    
    public static void initSQL()
    {
        
        string sql = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/workCloneCs/sql/";
        if (!Directory.Exists(sql)) Directory.CreateDirectory(sql);
        jsonDir = sql + "catagoryJson.txt";
        jsonstaffDir = sql + "staff.txt";
        catagoriesFromFile = pullCatFile();
        if (!testFiles && connectionString == null)
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(sql + "ConnectionStringsConfiguration.json")
                    .Build();
                connectionString = configuration.GetConnectionString("DefaultConnection");
            }
            catch (Exception ex)
            {
                created = true;
                connectionString = null;
                Logger.Log($"something went wrong here insite initSQL {ex}");
            }
            
            
        }

        if (connectionString != null){
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Logger.Log("connected to database");
                
                }
                catch (Exception ex)
                {
                    ErrorCallIS(ex);

                }
            }

        }
        else
        {
            ErrorCallIS(null);
        }
        sync.allStaff = getStaffFromFile();
        if (sync.allStaff == null)
        {
            Logger.Log("staff didnt staff");
        }
        
    }
    

    private static bool created = false;

    private static List<staff> getStaffFromFile()
    {
        try
        {
            if (File.Exists(jsonstaffDir))
            {
                string json = File.ReadAllText(jsonstaffDir);
                List<staff> fileJson = JsonSerializer.Deserialize<List<staff>>(json);
                Logger.Log("json was read");
                return fileJson;

            }

            Logger.Log(jsonstaffDir + " doesn't exist");
            return null;
        }
        catch (Exception ex)
        {
            Logger.Log($"getStaffFromFile() failed: {ex.Message}");
            return null;
        }
    }
    
    private static void ErrorCallIS(Exception ex)
    {
        if (ex != null) Logger.Log(ex.Message);
        if (!created)
        {
            created = true;
            Task.Run(() => MessageBox.Show(
                "it is recomened for you to go through the config settings," +
                "\n you are currently using the backup database on your local device," +
                "\n you will not be able to send an order through in this state"));
        }
    }


    public static (int, int) getRangeOfCatagoryID()
    {
        string query = "SELECT top 1 catagoryId " +
            "from catagories " +
            "order by catagoryId ";
        int min = 0;
        int max = 0;
        try
        {
            if (connectionString != null)
            {

                //main method
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(query + " desc ", connection);
                        Logger.Log(command.Parameters.ToString());
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                max = reader.GetInt32(0);
                                Logger.Log($"max: {max}");
                            }
                        }

                        command = new SqlCommand(query + "asc ", connection);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                min = reader.GetInt32(0);
                                Logger.Log($"max: {min}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return errorCallCI(ex);
                    }
                }
                return (min, max);
            }

            return errorCallCI(null);
        }
        catch (Exception ex)
        {
            return errorCallCI(ex);

        }

        
    }
    
    public static List<staff> getStaffData()
    {
        Logger.Log("inside getStaffData");
        string query = "select * from staff order by id desc";
        List<staff> values= new List<staff>();
        if (connectionString != null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        Logger.Log("connected to database");
                        using (SqlCommand command = new SqlCommand(query, connection))
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            try
                            {
                                while (reader.Read())
                                {
                                    int id = reader.GetInt32(0);
                                    string name = reader.GetString(1);
                                    int access = reader.GetInt32(2);
                                    values.Add(new staff
                                    {
                                        Id = id,
                                        Access = access,
                                        Name = name
                                    });
                                    Logger.Log($"ID: {id}, Name: {name}, accessLevel: {access}");


                                }

                                try
                                {
                                    // logging it just incase it cannot pull it next time
                                    string jsonString = JsonSerializer.Serialize(values,
                                        new JsonSerializerOptions { WriteIndented = true });

                                    File.WriteAllText(jsonstaffDir, jsonString);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    Logger.Log(ex.Message);
                                }

                                return values;
                            }
                            catch (Exception ex)
                            {
                                return errorCallSD(ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return errorCallSD(ex);
                    }
                }
            }
            catch (SqlException ex)
            {
                return errorCallSD(ex);
            }
        }

        return errorCallSD(null);


    }

    
    public static List<catagory> pullCatFile()
    {
        if (sync.checkCatFile())
        {
            try
            {
                string json = File.ReadAllText(jsonDir); // Read file contents
                List<catagory> fileJson = JsonSerializer.Deserialize<List<catagory>>(json); // Deserialize JSON text
                Logger.Log("json was read and was valid");
                return fileJson;
            }
            catch (Exception ex)
            {
                Logger.Log($"pullCatFile() failed: {ex.Message}");
                return null;
            }
        }
        Logger.Log(jsonDir + " doesn't exist or is null/corrupt");
        return null;
    }

    public static catagory getCatagory(int catagoryChosen)
    {
        catagory currentCatagory = new catagory();
        List<item> values = new List<item>();
        string query = 
        "SELECT ai.itemId, ai.itemName, ISNULL(ai.extraInfo, ''), ai.price, ISNULL(ai.chosenColour, 'grey'), " +
        "cat.catName, cat.catagoryId, ISNULL(cat.extraCatInfo, '') " +
        "FROM allItems ai " +
        "JOIN [foodCatagory] foo ON ai.itemID = foo.itemId " +
        "JOIN [catagories] cat ON cat.catagoryId = foo.catagoryId " +
        "WHERE cat.catagoryId = @catagoryId " +
        "AND cat.catagoryId IS NOT NULL " +
        "AND catName IS NOT NULL " +
        "AND itemName IS NOT NULL " +
        "AND price IS NOT NULL " +
        "ORDER BY cat.catagoryId ";
        
        try
        {
            if (connectionString != null)
            {

                //main method
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        Logger.Log("opened connection for catagories");
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@catagoryId", catagoryChosen);
                        using (command)
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Logger.Log("reading currently catagories btw");
                                currentCatagory.catagoryId = reader.GetInt32(6);
                                currentCatagory.catName = reader.GetString(5);
                                currentCatagory.catagoryExtraInfo = reader.IsDBNull(7) ? null : reader.GetString(7);
                                values.Add(new item()
                                {
                                    /*
                                     * items......
                                     * itemId
                                     * itemName
                                     * extraInfo
                                     * itemCount - input manually
                                     * price
                                     * chosenColour
                                     * lineId = blank
                                     *
                                     * catagories...
                                     * connected - true or false
                                     * catName
                                     * catId
                                     * catExtraInfo
                                     * list of items
                                     */
                                    itemName = reader.GetString(1),
                                    price = (decimal)reader.GetInt32(3) / 100,
                                    extraInfo = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    chosenColour = reader.GetString(4),
                                    itemId = reader.GetInt32(0),
                                    itemCount = 1
                                });
                                Logger.Log($"got: item: {reader.GetString(1)}");
                            }

                            currentCatagory.items = values;
                            currentCatagory.connected = false;
                            try
                            {
                                List<catagory> fileJson = new List<catagory>();



                                if (File.Exists(jsonDir))
                                {
                                    Logger.Here();
                                    fileJson = pullCatFile();

                                    if (fileJson == null)
                                    {
                                        Logger.Log("file was null or weird so I'm starting again");
                                        fileJson = new List<catagory>();
                                    }
                                }
                                else
                                {
                                    Logger.Log($"{jsonDir} doesn't exist, creating a new one");
                                    fileJson = new List<catagory>();
                                }

                                bool ah = false;
                                foreach (catagory cat in fileJson)
                                {
                                    if (cat != null)
                                    {
                                        if ((cat.catName == currentCatagory.catName ||
                                             cat.catagoryId == currentCatagory.catagoryId))
                                        {
                                            ah = true;
                                        }
                                    }

                                }

                                if (!ah) fileJson.Add(currentCatagory);
                                string jsonStrings = JsonSerializer.Serialize(fileJson,
                                    new JsonSerializerOptions { WriteIndented = true });
                                File.WriteAllText(jsonDir, jsonStrings);
                            }
                            catch (Exception ex)
                            {
                                Logger.Log($"error while storing items {ex.Message}");
                            }

                            currentCatagory.connected = true;
                            return currentCatagory;

                        }
                    }
                    catch (Exception ex)
                    {
                        //backup method just incase server down

                        return errorCallGC(ex, catagoryChosen);

                    }

                }
            }
            return errorCallGC(null, catagoryChosen);
        }
        catch (Exception ex) {
            return errorCallGC(ex, catagoryChosen);

        }

    }
    
    
    // only used in getCatagory when catch is called
    private static catagory errorCallGC(Exception ex, int catagoryChosen)
    {
        Logger.Log("\ncouldn't connect so am resorting to backup\n");
        Logger.Log(ex.Message);
        Logger.Here();
        if (catagoriesFromFile != null)
        {
            Logger.Log("tbf i think it worked just have a quick look tbf");
            foreach (catagory cat in catagoriesFromFile) 
            {
                Logger.Log($"catID {cat.catagoryId}, chosen cat: {catagoryChosen}");
                if (cat.catagoryId == catagoryChosen) return cat;
            }
        }
                    
        Logger.Log("catagories file or cat doesnt exist so :(");
        return null;
    }
    
    //only using in getRangeOfCatagoryID when catch is called
    private static (int, int) errorCallCI(Exception ex)
    {
        int min = 0;
        int max = 0;
        Logger.Log(ex.Message);
        List<int> d = new List<int>();
        //couldnt connect or something so 
        if (catagoriesFromFile != null)
        {
            foreach (catagory cat in catagoriesFromFile)
            {
                d.Add(cat.catagoryId);
            }

            min = d.Min();
            max = d.Max();    
        }
        // else basically means we are screwed icl
        else
        {
            min = 0;
            max = 0;
        }
        return (min, max);
    }
    
    //only used in getStaffData when catch is called
    private static List<staff> errorCallSD(Exception ex)
    {
        Logger.Log(ex.Message);
        Console.WriteLine("Error: " + ex.Message);
        string file = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/workclonecs/sql/staff.txt";
        return staffreturnthing(file);
    }

    public static List<staff> staffreturnthing(string file)
    {
        if (File.Exists(file)) {
            List<staff> staff = JsonSerializer.Deserialize<List<staff>>(File.ReadAllText(file));
            Logger.Log("file exists so im gonna try and read it staff btw");
            return staff;

        }

        return null;
    }
    private static int getHighestidFromTable(string tableName)
    {
        string sqlcommand = $"select max(Id) from {tableName}";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlcommand, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read() && ! reader.IsDBNull(0)){
                       
                        int max = reader.GetInt32(0);
                        Logger.Log($"got this number in the getHigest HeaderId function : {max}");
                        return max;
                    }

                    return 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return -1;
            }
        }
    }

    private static void modifyTableSql(string sqlCommand)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                SqlCommand sql = new SqlCommand(sqlCommand, connection);
                sql.ExecuteNonQuery();
                Logger.Log($"Executed {sqlCommand} successfully");
                }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
             
            }
        }
    }
         
    public static void pushItemsToTables(table table, staff staff) {
        int headerId = getHighestidFromTable("headers") + 1;
        int orderId = getHighestidFromTable("orders") + 1;
        int LineId = getHighestidFromTable("orderLine");
        if (headerId == 0 || orderId == 0 || LineId == 0)
        {
            Logger.Log("one of the ids was 0 so it errored the fuck out ngl cheieve this shouldnt happen but fuck me ig");
        }
        if (table.tableId < 1)
        {
            table.tableId = 4000;
            // this is specifically for when using the till and you dont want to have to add a table number
        }
        //header table
        string command = $"insert into headers(Id, staffId, tableNumber) values({headerId}, {staff.Id}, {table.tableId})";
        modifyTableSql(command);
        //order table
        command = $"insert into orders(Id, headerId) values ({orderId}, {headerId})";
        
        modifyTableSql(command);
        //orderLine table
        foreach (item item in table.itemsToOrder)
        {
            command = $"insert into orderLine(Id, orderId, itemId) values({LineId++}, {orderId}, {item.itemId})";
            modifyTableSql(command);
            Logger.Log("added item");
        }
        Logger.Log("end of items being ordered");
        
 

    }
    
}

