using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Text.Json;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace WorkCloneCS;

static partial class SQL
{

    private static bool testFiles = false;
    private static bool created = false;
    private static List<category> categoriesFromFile;
    private static string jsonDir;
    private static string jsonstaffDir;
    public static string connectionString;
    public static bool initCompleted = false;
    public static string dir = @$"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\workclonecs\";
    public static string sqlDir = dir + "sql/";
    private static SqlConnection sqlCon;
    
    public static void initSQL()
    {
        //database connection section
        dir = @$"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\workclonecs\";
        if (!Directory.Exists(sqlDir)) Directory.CreateDirectory(sqlDir);
        jsonDir = sqlDir + "categoryJson.txt";
        jsonstaffDir = sqlDir + "staff.txt";
        categoriesFromFile = pullCatFile();
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
            
            catch (System.IO.InvalidDataException ex)
            {
                Logger.Log($"couldnt access file and threw {ex.Message} most likely open in another aplication");
            }
            
            catch (Exception ex)
            {
                created = true;
                connectionString = null;
                Logger.Log($"something went wrong here insite initSQL {ex}");
            }
            
            
        }

        if (connectionString == null) ErrorCallIS(null);
        else {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Logger.Log("connected to database");
                    connection.Close();
                
                }
                catch (Exception ex)
                {
                    ErrorCallIS(ex);

                }
            }

        }
        
        //staff section
        sync.allStaff = getStaffFromFile();
        if (sync.allStaff == null)
        {
            Logger.Log("staff didnt staff");
        }

        sqlCon = new SqlConnection(connectionString);
        initCompleted = true;
    }
    
    public static (int, int) getRangeOfCategoryID()
    {
        string query = "SELECT top 1 categoryId " +
            "from categories " +
            "order by categoryId ";
        int min = -1;
        int max = -1;
        SqlCommand com = new SqlCommand(query + "desc ", sqlCon);
        SqlCommand com2 = new SqlCommand(query + "asc ", sqlCon); 
        sqlCon.Open();
        object? descResult = com.ExecuteScalar();
        object? ascResult = com.ExecuteScalar();
        sqlCon.Close();
        if (descResult != null || descResult != DBNull.Value) max = Convert.ToInt32(descResult);
        if (ascResult != null || ascResult != DBNull.Value) min = Convert.ToInt32(ascResult);


        return (min, max);
    }
    
    public static List<staff> getStaffDataCloud()
    {
        Logger.Log("inside getStaffData");
        string query = "select * from staff order by id desc";
        List<staff> values= new List<staff>();
        if (connectionString == null) return null;
        SqlCommand com = new SqlCommand(query, sqlCon);
        sqlCon.Open();
        SqlDataReader reader = com.ExecuteReader();
        while (reader.Read())
        {
            Logger.Here();
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);
            int access = reader.GetInt32(2);
            values.Add(new staff
            {
                Id = id,
                Access = access,
                Name = name
            });
        }

        sqlCon.Close();
        if (values.Count == 0) return null;
        return values;

    }
    
    public static List<category> pullCatFile()
    {
        if (sync.checkCatFile())
        {
            try
            {
                string json = File.ReadAllText(jsonDir); // Read file contents
                List<category> fileJson = JsonSerializer.Deserialize<List<category>>(json); // Deserialize JSON text
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

    public static category getCategory(int categoryChosen)
    {
        category currentCategory = new category();
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
                       	,cat.categoryId
                       	,ISNULL(cat.extraCatInfo, '') 
                       FROM
                       	allItems ai 
                       	JOIN [foodCategory] foo ON ai.itemID = foo.itemId 
                       	JOIN [categories] cat ON cat.categoryId = foo.categoryId 
                       	LEFT JOIN [allergyItem] ali ON ai.itemId = ali.itemId 
                       	LEFT JOIN [allergies] al ON ali.allergyId = al.allergyId 
                       WHERE
                       		cat.categoryId = @categoryId 
                       	AND cat.categoryId IS NOT NULL 
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
                       	,cat.categoryId
                       	,cat.extraCatInfo 
                       ORDER BY
                       	cat.categoryId 
                       
                       """;
        
        SqlCommand com = new SqlCommand(query, sqlCon);
        SqlDataReader reader = com.ExecuteReader();
        while (reader.Read())
            {
                
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
                currentCategory.categoryId = catId;
                currentCategory.catName = catName;
                currentCategory.categoryExtraInfo = catExtraInfo;

                // Split allergy string into a list
                List<string> containedAllergies = new List<string>();
                if (!string.IsNullOrWhiteSpace(allergyString))
                    containedAllergies = allergyString
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(a => a.Trim())
                        .ToList();
                
                values.Add(new item()
                {
                    itemId = itemId,
                    itemName = itemName,
                    extraInfo = extraInfo,
                    price = price,
                    chosenColour = chosenColour,
                    itemCount = 1,
                    containedAllergies = containedAllergies
                });
                
            }

            currentCategory.items = values;
            currentCategory.connected = false;
            try
            {
                List<category> fileJson = new List<category>();

                if (File.Exists(jsonDir))
                {
                    Logger.Here();
                    fileJson = pullCatFile();

                    if (fileJson == null)
                    {
                        Logger.Log("file was null or weird so I'm starting again");
                        fileJson = new List<category>();
                    }
                }
                else
                {
                    Logger.Log($"{jsonDir} doesn't exist, creating a new one");
                    fileJson = new List<category>();
                }

                bool ah = false;
                foreach (category cat in fileJson)
                {
                    if (cat != null && (cat.catName == currentCategory.catName ||
                                        cat.categoryId == currentCategory.categoryId)) 
                        ah = true;
                }

                if (!ah) fileJson.Add(currentCategory);
                string jsonStrings = JsonSerializer.Serialize(fileJson,
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(jsonDir, jsonStrings);
            }
            catch (Exception ex)
            {
                Logger.Log($"error while storing items {ex.Message}");
            }

            currentCategory.connected = true;
            return currentCategory;
        
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
        SqlCommand com = new SqlCommand(sqlcommand, sqlCon);
        sqlCon.Open();
        object? result = com.ExecuteScalar();
        sqlCon.Close();
        int x = -1;
        if (result != null || result != DBNull.Value) x = Convert.ToInt32(result);
        return x;
    }

    private static void modifyTableSql(string sqlCommand)
    {
        SqlCommand sql = new SqlCommand(sqlCommand, sqlCon);
        try
        {
            sqlCon.Open();
            sql.ExecuteNonQuery();
            sqlCon.Close();
            Logger.Log($"Executed {sqlCommand} successfully");
        }
        catch (Exception ex)
        {
            Logger.Log($"error in modifyTableSql with {sqlCommand} {ex}"); 
        }
        
        
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
        sqlCon.Open();
        SqlCommand sql = new SqlCommand(sqlCommand, sqlCon);
        SqlDataReader reader = sql.ExecuteReader();
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
        string query = """
                       select itemId, itemName, price, chosenColour, 
                              extraInfo, subCatId, leadsToCategoryId, subItemOrder 
                       from allItems
                       """;
        using var cmd = new SqlCommand(query, sqlCon);
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
                containedAllergies = null,
                hasSubItems = reader.GetInt32(6) > -1,
            });
        }
        sqlCon.Close();
        
        return localItems;
    }
    

    public static List<dbCategory> getAllCategories()
    {
        List<dbCategory> cats = new List<dbCategory>();
        string command = "select categoryId, catName, ISNULL(chosenColour, '') from categories";
        SqlCommand com = new SqlCommand(command, sqlCon);
        sqlCon.Open();
        using SqlDataReader reader = com.ExecuteReader();
        while (reader.Read())
        {
            cats.Add(new dbCategory()
            {
                catId = reader.GetInt32(0),
                catName = reader.GetString(1),
                catColour = reader.GetString(2)
            });
        }
        sqlCon.Close();
        return cats;
    }

    public static List<List<int>> getCatItemLinks()
    {
        
        int count = 0;
        List<List<int>> itemCat = new List<List<int>>();
        SqlCommand com = new SqlCommand("select categoryId, itemId from foodCategory order by categoryId asc", sqlCon);
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
        return itemCat; // this returns a list of categories that has a list of items that has the categoryId and itemId in that order
    }

    public static List<dbSubParent> getSubParentPairs()
    {
        
        //this command gets all of the item Id's that has categories but isnt a category itself. 
        string query = "select itemId, leadsToCategoryId from allItems where subCatID = -1 and leadsToCategoryId != -1";

        string query2 = "select itemId, subCatId, leadsToCategoryId from allItems where subCatId != -1 and subItemOrder != -1";
        
        SqlCommand com = new SqlCommand(query, sqlCon);
        sqlCon.Open();
        SqlDataReader reader = com.ExecuteReader();
        List<dbSubParent> lis = new List<dbSubParent>();
        while (reader.Read())
        {
            
            
        }

        return null;
    }

    public static (List<int>, List<int>) getJunctionTableValues(string tableName, string leftId, string rightId)
    {
        List<int> leftIds = new();
        List<int> rightIds = new();
        string query = $"""
                     select {leftId}, {rightId} 
                     from {tableName}
                     """;
        SqlCommand com = new SqlCommand(query, sqlCon);
        sqlCon.Open();
        SqlDataReader reader = com.ExecuteReader();
        while (reader.Read())
        {
            leftIds.Add(reader.GetInt32(0));
            rightIds.Add(reader.GetInt32(1));
        }
        sqlCon.Close();
        return (leftIds, rightIds);
    }
    
    #endregion
    
    
    
}

