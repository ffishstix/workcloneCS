using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Text.Json;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace WorkCloneCS;

static class SQL
{

    private static bool testFiles = false;
    private static List<catagory> catagoriesFromFile;
    private static string jsonDir;
    private static string jsonstaffDir;
    public static string connectionString;
    public static bool initCompleted = false;
    public static string dir = @$"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\workclonecs\";
    public static string sqlDir = dir + "sql/";
    private static SqlConnection sqlCon;
    public static void initSQL()
    {
        SQL.dir = @$"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\workclonecs\";
        if (!Directory.Exists(sqlDir)) Directory.CreateDirectory(sqlDir);
        jsonDir = sqlDir + "catagoryJson.txt";
        jsonstaffDir = sqlDir + "staff.txt";
        catagoriesFromFile = pullCatFile();
        if (!testFiles && connectionString == null)
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(sqlDir + "ConnectionStringsConfiguration.json")
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

        sqlCon = new SqlConnection(connectionString);
        initCompleted = true;
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
        string query = """
                       SELECT
                       	 ai.itemId
                       	,ai.itemName
                       	,ISNULL(STRING_AGG(al.allergyName, ', '), '') as allergies
                       	,ISNULL(ai.extraInfo, '') 
                       	,ai.price
                       	,ISNULL(ai.chosenColour, 'grey') 
                       	,cat.catName
                       	,cat.catagoryId
                       	,ISNULL(cat.extraCatInfo, '') 
                       FROM
                       	allItems ai 
                       	JOIN [foodCatagory] foo ON ai.itemID = foo.itemId 
                       	JOIN [catagories] cat ON cat.catagoryId = foo.catagoryId 
                       	LEFT JOIN [allergyItem] ali ON ai.itemId = ali.itemId 
                       	LEFT JOIN [allergies] al ON ali.allergyId = al.allergyId 
                       WHERE
                       		cat.catagoryId = @catagoryId 
                       	AND cat.catagoryId IS NOT NULL 
                       	AND catName IS NOT NULL			
                       	AND itemName IS NOT NULL 		
                       	AND price IS NOT NULL 			
                       GROUP BY
                       	 ai.itemId
                       	,ai.itemName
                       	,ai.extraInfo
                       	,ai.price
                       	,ai.chosenColour
                       	,cat.catName
                       	,cat.catagoryId
                       	,cat.extraCatInfo 
                       ORDER BY
                       	cat.catagoryId 
                       
                       """;
        
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
                                /*
                                Logger.Log("reading currently catagories btw");
                                currentCatagory.catagoryId = reader.GetInt32(6);
                                currentCatagory.catName = reader.GetString(5);
                                currentCatagory.catagoryExtraInfo = reader.IsDBNull(7) ? null : reader.GetString(7);

                                values.Add(new item()
                                {

                                     * items......
                                     * itemId
                                     * itemName
                                     * allergies
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
                                     *
                                    itemName = reader.GetString(1),
                                    price = (decimal)reader.GetInt32(3) / 100,
                                    extraInfo = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    chosenColour = reader.GetString(4),
                                    itemId = reader.GetInt32(0),
                                    itemCount = 1



                                });
                            */
                                
                                int itemId = reader.GetInt32(0);
                                string itemName = reader.GetString(1);
                                string allergyString = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                string extraInfo = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                decimal price = reader.GetDecimal(4);
                                string chosenColour = reader.GetString(5);
                                string catName = reader.GetString(6);
                                int catId = reader.GetInt32(7);
                                string catExtraInfo = reader.IsDBNull(8) ? "" : reader.GetString(8);

                                // Populate category details once
                                currentCatagory.catagoryId = catId;
                                currentCatagory.catName = catName;
                                currentCatagory.catagoryExtraInfo = catExtraInfo;

                                // Split allergy string into a list
                                List<string> containedAllergies = new List<string>();
                                if (!string.IsNullOrWhiteSpace(allergyString))
                                {
                                    containedAllergies = allergyString
                                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(a => a.Trim())
                                        .ToList();
                                }
                                
                                item newItem = new item
                                {
                                    itemId = itemId,
                                    itemName = itemName,
                                    extraInfo = extraInfo,
                                    price = price,
                                    chosenColour = chosenColour,
                                    itemCount = 1,
                                    containedAllergies = containedAllergies
                                };

                                values.Add(newItem);
                                
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
    public static int getHighestidFromTable(string tableName)
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
                        Logger.Log($"got this number in the getHighestId function : {max}");
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
                Logger.Log($"message in modifyTableSql in sql.cs sqlCommand: {sqlCommand}" + ex.Message);
             
            }
        }
    }

    private static int betterModifyTableSql(string sqlCommand, string? connectionString)
    {
        int x = -1;
        if (connectionString == null)
        {
            connectionString = SQL.connectionString;
            
        }

        try
        {
            using (SqlConnection con = new SqlConnection((connectionString)))
            {
                SqlCommand com = new SqlCommand(sqlCommand, con);
                x = com.ExecuteNonQuery();
                
            }
        }
        catch (Exception ex)
        {
            Logger.Log("error ~line 610~ sql.cs betterModifyTableSql: " + ex.Message);
        }
        return x;
    } 
    

    public static List<item> getTableItems(int tableId)
    {
        string sqlCommand = $"""
                      SELECT allItems.itemId, allItems.itemName, allItems.Price,
                             ISNULL(allItems.chosenColour, 'grey') as chosenColour, ISNULL(allItems.extraInfo, '') as extraInfo
                      FROM workclonecs.dbo.headers AS headers
                               LEFT JOIN workclonecs.dbo.orders AS orders ON
                          (orders.headerId = headers.Id)
                               LEFT JOIN workclonecs.dbo.orderLine AS orderLines
                                         ON (orderLines.orderId = orders.Id)
                               LEFT JOIN workclonecs.dbo.allItems AS allItems
                                         ON (allItems.itemId = orderLines.itemId)
                      WHERE (headers.finished = 0)
                        AND (headers.tableNumber) = {tableId}
                        AND allItems.itemId IS NOT NULL;
                      """;
        List<item> items = new List<item>();
        using (SqlConnection connection = new SqlConnection(SQL.connectionString))
        {
            try
            {
                connection.Open();
                SqlCommand sql = new SqlCommand(sqlCommand, connection);
                using (SqlDataReader reader = sql.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetInt32(0) == null)
                        {
                            Logger.Log("table is null");
                            return null;
                        }

                        items.Add(new item
                        {
                            itemId = reader.GetInt32(0),
                            itemName = reader.GetString(1),
                            price = reader.GetInt32(2) / 100,
                            chosenColour = reader.GetString(3),
                            extraInfo = reader.GetString(4),
                            itemCount = 1
                        });
                        Logger.Log("item added to list");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error in getTabled, called by tableBtn_Click tried getting " + tableId + ex.Message);
                return null;
            }
        }

        return items;
    }
    

    
    public static void pushItemsToTables(table table, staff staff, int headerId, int orderId, int lineId) {
        
        if (headerId == 0 || orderId == 0 || lineId == 0)
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
            command = $"insert into orderLine(Id, orderId, itemId) values({lineId++}, {orderId}, {item.itemId})";
            modifyTableSql(command);
            Logger.Log("added item");
        }
        Logger.Log("end of items being ordered");
        
 

    }


    
    #region local database initialisation
    ///<summary>
    /// the code in this section only contains the initialisation code
    /// for setting up the local database
    /// this could either be the first time making one or
    /// just adding the modifications 
    /// </summary>
    
    
    public static int getDatabaseVNum()
    {
        int x = -1;
        const string sqlCommand = @"
                            select try_convert(int, value) as databaseVersion
                            from sys.extended_properties 
                            where class = 0
                            and name = N'DatabaseVersion'
                            ";
        
        SqlCommand com = new SqlCommand(sqlCommand, sqlCon);
        sqlCon.Open();
        object? result = com.ExecuteScalar(); 
        sqlCon.Close();
        if (result != null || result != DBNull.Value) x = Convert.ToInt32(result);
        
        return x;
    }
    
    public static int getLocalDBVNum()
    {
        try
        {
            return int.Parse(File.ReadAllText(dir + "sql/DBvNum.txt"));
        } catch (Exception ex)
        {
            Logger.Log(ex.Message);
            return -1;
        }
        
    }

    public static List<item> getAllItems()
    {
        List<item> localItems = new List<item>();
        using var cmd = new SqlCommand("select itemId, itemName, price, " +
                                       "ISNULL(chosenColour, 'grey'), ISNULL(extraInfo, '') from allItems", sqlCon);
        sqlCon.Open();
        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            localItems.Add(new item()
            {
                itemId = reader.GetInt32(0),
                itemName = reader.GetString(1),
                price = reader.GetInt32(2) / 100,
                chosenColour = reader.GetString(3),
                extraInfo = reader.GetString(4),
                itemCount = 1,
                ordered = false,
                containedAllergies = null
            });
        }
        sqlCon.Close();
        
        return localItems;
    }

    public static List<catagory> getAllCatagories()
    {
        List<catagory> cats = new List<catagory>();
        string command = "select catagoryId, catName, ISNULL(extraInfo, ''), ISNULL(chosenColour, '') from catagories";
        SqlCommand com = new SqlCommand(command, sqlCon);
        sqlCon.Open();
        using SqlDataReader reader = com.ExecuteReader();
        while (reader.Read())
        {
            cats.Add(new catagory()
            {
                catagoryId = reader.GetInt32(0),
                catName = reader.GetString(1),
                catagoryExtraInfo = reader.GetString(2),
                catColour = reader.GetString(3)
            });
        }
        sqlCon.Close();
        return cats;
    }

    public static List<List<int>> getCatItemLinks()
    {
        
        int count = 0;
        List<List<int>> itemCat = new List<List<int>>();
        SqlCommand com = new SqlCommand("select catagoryId, itemId from foodCatagory order by catagoryId asc", sqlCon);
        sqlCon.Open();
        SqlDataReader reader = com.ExecuteReader();
        while (reader.Read())
        {
            count++;
            while (itemCat.Count <= reader.GetInt32(0)) itemCat.Add(new List<int>());
            itemCat[reader.GetInt32(0)].Add(reader.GetInt32(1));
        }
        sqlCon.Close();
        Logger.Log($"got {count} links");
        return itemCat; // this returns a list of catagories that has a list of items that has the catagoryId and itemId in that order
    }
    
    
    
    #endregion
    
}

