using System.Text.Json;
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
    public static bool initStarted = false;

    public static void initSQL()
    {
        if (initStarted) return;
        initStarted = true;
        initCompleted = false;
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

            catch (InvalidDataException ex)
            {
                Logger.Log(
                    $"couldnt access file and threw {ex.Message} most likely open in another aplication in init sql");
            }

            catch (Exception ex)
            {
                created = true;
                connectionString = null;
                Logger.Log($"something went wrong here insite initSQL {ex}");
            }
        }

        if (connectionString == null) ErrorCallIS(null);
        else checkDBConnection();

        //staff section
        if (database.getStaffList() == new List<staff>())
        {
            Logger.Log("staff didnt staff after sync.allStaff= getStaffFromFile");
        }

        initCompleted = true;
        initStarted = true;
        Logger.Log("init sql completed");
    }

    public static bool checkDBConnection()
    {
        bool connected = false;
        SqlConnection sqlCon = new SqlConnection(connectionString);
        try
        {
            sqlCon.Open();
            Logger.Log("connected to database");
            sqlCon.Close();

            connected = true;
        }
        catch (Exception ex)
        {
            ErrorCallIS(ex);
        }

        database.isConnectedToRemoteServer = connected;
        return connected;
    }


    public static List<staff> getStaff()
    {
        List<staff> staffs = new();
        string query = """
                       select id, name, staff.accessLevel, canSendThroughItems, 
                              canDelete, canNoSale, canViewTables
                       from staff inner join accessAlowances 
                       on staff.accessLevel = accessAlowances.accessLevel
                       order by id asc
                       """;
        using SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        using SqlCommand com = new SqlCommand(query, con);
        using SqlDataReader reader = com.ExecuteReader();
        while (reader.Read())
        {
            staffs.Add(new staff()
            {
                Id = reader.GetInt32(0),
                staffAccess = new()
                {
                    Id = reader.GetInt32(2),
                    canSendThroughItems = reader.GetBoolean(3),
                    canDelete = reader.GetBoolean(4),
                    canNoSale = reader.GetBoolean(5),
                    canViewTables = reader.GetBoolean(6)
                },
                Name = reader.GetString(1)
            });
        }

        con.Close();

        return staffs;
    }

    public static accessLevel getAccessLevelFromId(int Id)
    {
        string query = $"""
                        select canSendThroughItems, canDelete, canNoSale, canViewTables
                        from accessAlowances, staff
                        where accessAlowances.accessLevel = staff.AccessLevel
                        and staff.id = {Id}
                        """;
        using SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        using SqlCommand com = new SqlCommand(query, con);
        using SqlDataReader reader = com.ExecuteReader();
        reader.Read();
        return new()
        {
            canSendThroughItems = reader.GetBoolean(0),
            canDelete = reader.GetBoolean(1),
            canNoSale = reader.GetBoolean(2),
            canViewTables = reader.GetBoolean(3)
        };
    }

    public static List<staff> getStaffDataCloud()
    {
        Logger.Log("inside getStaffData");
        string query = """
                       select id, name, accessLevel, canSendThroughItems, 
                              canDelete, canNoSale, canViewTables
                       from staff inner join accessAlowances 
                       on staff.accessLevel = accessAlowances.accessLevel
                       order by id asc
                       """;
        List<staff> values = new List<staff>();
        if (connectionString == null) return null;
        using SqlConnection con = new SqlConnection(connectionString);
        using SqlCommand com = new SqlCommand(query, con);
        con.Open();
        using SqlDataReader reader = com.ExecuteReader();
        while (reader.Read())
        {
            Logger.Here();
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);
            int access = reader.GetInt32(2);
            values.Add(new staff
            {
                Id = id,
                staffAccess = new()
                {
                    Id = access
                },
                Name = name
            });
        }

        con.Close();
        if (values.Count == 0) return null;
        return values;
    }

    public static List<category> pullCatFile()
    {
        if (database.DBExists)
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
                Logger.Log($"pullCatFile() doesn't exist or is null/corrupt pullcatfile failed: {ex.Message}");
                return null;
            }
        }

        Logger.Log("sync.CheckCatFile failed: pullCatFile() ");
        return null;
    }

    public static category getCategory(int categoryChosen)
    {
        category currentCategory = new category();
        List<item> values = new List<item>();
        string query = $"""
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
                                JOIN [foodCategory] foo ON ai.itemId = foo.itemId
                                JOIN [categories] cat ON cat.categoryId = foo.categoryId
                                LEFT JOIN [allergyItem] ali ON ai.itemId = ali.itemId
                                LEFT JOIN [allergies] al ON ali.allergyId = al.allergyId
                        WHERE
                            cat.categoryId = {categoryChosen} 
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
        using SqlConnection con = new SqlConnection(connectionString);
        using SqlCommand com = new SqlCommand(query, con);
        con.Open();
        using SqlDataReader reader = com.ExecuteReader();
        while (reader.Read())
        {
            int Id = reader.GetInt32(0);
            string Name = reader.GetString(1);
            string allergyString = reader.IsDBNull(2) ? "" : reader.GetString(2);
            string extraInfo = reader.IsDBNull(3) ? "" : reader.GetString(3);
            decimal price = reader.GetInt32(4);
            string chosenColour = reader.GetString(5);
            string catName = reader.GetString(6);
            int catId = reader.GetInt32(7);
            string catExtraInfo = reader.IsDBNull(8) ? "" : reader.GetString(8);

            // Populate category details once
            currentCategory.categoryId = catId;
            currentCategory.catName = catName;
            currentCategory.categoryExtraInfo = catExtraInfo;

            List<allergy> allergyList =
                string.IsNullOrWhiteSpace(allergyString)
                    ? new List<allergy>()
                    : allergyString
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => new allergy { Name = s.Trim() })
                        .ToList();
            values.Add(new item()
            {
                Id = Id,
                Name = Name,
                extraInfo = extraInfo,
                price = price,
                chosenColour = chosenColour,
                itemCount = 1,
                allergies = allergyList
            });
        }

        con.Close();
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
                    Logger.Log("file was null or weird so I'm starting again getCategory in the logging");
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
        if (File.Exists(file))
        {
            List<staff> staff = JsonSerializer.Deserialize<List<staff>>(File.ReadAllText(file));
            Logger.Log("file exists so im gonna try and read it staff btw");
            return staff;
        }

        return null;
    }

    public static int getHighestidFromTable(string tableName)
    {
        string sqlcommand = $"select max(Id) from {tableName}";
        using SqlConnection con = new SqlConnection(connectionString);
        using SqlCommand com = new SqlCommand(sqlcommand, con);
        con.Open();
        object? result = com.ExecuteScalar();
        con.Close();
        int x = -1;
        if (result != null && result != DBNull.Value) x = Convert.ToInt32(result);
        return x;
    }

    private static void modifyTableSql(string sqlCommand, string functionCalling = "null")
    {
        using SqlConnection con = new SqlConnection(connectionString);
        using SqlCommand sql = new SqlCommand(sqlCommand, con);
        try
        {
            con.Open();
            sql.ExecuteNonQuery();
            con.Close();
            Logger.Log($"Executed {sqlCommand} successfully");
        }
        catch (Exception ex)
        {
            Logger.Log($"error in modifyTableSql called by {functionCalling} with {sqlCommand} {ex}");
        }
    }

    public static List<item> getTableItems(int tableId)
    {
        string sqlCommand = $"""
                                  select
                                      ai.itemid
                                  from tables as tb
                                           inner join tableorder as tor
                                                      on tor.tableid = tb.tableid
                                           inner join orders
                                                      on tor.orderid = orders.id
                                           inner join orderline as ol
                                                      on orders.id = ol.orderid
                                           inner join allitems as ai
                                                      on ol.itemid = ai.itemid
                                  where tb.tableid = {tableId}
                             """;
        List<item> items = new List<item>();
        using SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        using SqlCommand sql = new SqlCommand(sqlCommand, con);
        using SqlDataReader reader = sql.ExecuteReader();
        while (reader.Read())
        {
            if (reader.GetInt32(0) == null)
            {
                Logger.Log("table is null");
                return null;
            }

            item i = database.getItemFromId(reader.GetInt32(0));
            if (i != null) items.Add(i);
            Logger.Log("item added to list");
        }

        con.Close();
        return items;
    }

    public static void pushItemsToTables(table table, staff staff, int headerId, int orderId, int lineId)
    {
        if (headerId == 0 || orderId == 0 || lineId == 0)
        {
            Logger.Log(
                "one of the ids was 0 so it errored the fuck out ngl cheieve this shouldnt happen but fuck me ig");
        }

        if (table.tableId < 1)
        {
            table.tableId = 4000;
            // this is specifically for when using the till and you dont want to have to add a table number
        }

        string[] singleLineCommands =
        [
            $"insert into headers(Id, staffId, tableNumber, finished) " +
            $"values({headerId}, {staff.Id}, {table.tableId}, 0)",

            $"insert into orders(Id, headerId) " +
            $"values ({orderId}, {headerId})",

            // tableOrder has FK to tables.tableId, so tables must exist first.
            $"if not exists (select 1 from tables where tableId = {table.tableId}) " +
            $"insert into tables(tableId, staffId, currentlyActive) " +
            $"values ({table.tableId}, {staff.Id}, 1)",

            $"insert into tableOrder " +
            $"values ({table.tableId}, {orderId})"
        ];

        foreach (string s in singleLineCommands) modifyTableSql(s, "pushItemsToTables");


        //orderLine table
        foreach (item item in table.itemsToOrder)
        {
            string command = $"insert into orderLine(Id, orderId, itemId) values({lineId++}, {orderId}, {item.Id})";
            modifyTableSql(command, "pushItemsToTables later on");
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
        using SqlConnection con = new SqlConnection(connectionString);
        using SqlCommand com = new SqlCommand(sqlCommand, con);
        try
        {
            con.Open();
            object? result = com.ExecuteScalar();
            if (result != null || result != DBNull.Value) x = Convert.ToInt32(result);
            con.Close();
        }
        catch (Exception ex)
        {
            Logger.Log($"error in getDatabaseVNum {ex.Message}");
        }

        return x;
    }

    public static int getLocalDBVNum()
    {
        try
        {
            return int.Parse(File.ReadAllText(dir + "sql/DBvNum.txt"));
        }
        catch (Exception ex)
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
        using SqlConnection con = new SqlConnection(connectionString);
        using SqlCommand cmd = new SqlCommand(query, con);
        con.Open();
        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            localItems.Add(new item()
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                price = reader.GetInt32(2) / 100,
                chosenColour = reader.GetString(3),
                extraInfo = reader.GetString(4),
                itemCount = 1,
                ordered = false,
                allergies = null,
                hasSubItems = reader.GetInt32(6) > -1,
            });
        }

        con.Close();

        return localItems;
    }


    public static List<dbCategory> getAllCategories()
    {
        List<dbCategory> cats = new List<dbCategory>();
        string command =
            "select categoryId, catName, ISNULL(chosenColour, 'grey') from categories order by categoryId asc";
        using SqlConnection con = new SqlConnection(connectionString);
        using SqlCommand com = new SqlCommand(command, con);
        con.Open();
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

        con.Close();
        return cats;
    }

    public static List<dbSubParent> getSubParentPairs()
    {
        //this command gets all of the item Id's that has categories but isnt a category itself. 
        string query = "select itemId, leadsToCategoryId from allItems where subCatID = -1 and leadsToCategoryId != -1";

        string query2 =
            "select itemId, subCatId, leadsToCategoryId from allItems where subCatId != -1 and subItemOrder != -1";
        using SqlConnection con = new SqlConnection(connectionString);
        using SqlCommand com = new SqlCommand(query, con);
        con.Open();
        using SqlDataReader reader = com.ExecuteReader();
        List<dbSubParent> lis = new List<dbSubParent>();
        while (reader.Read())
        {
        }

        con.Close();

        return null;
    }

    public static (List<int>, List<int>) getJunctionTableValues(string tableName, string leftId, string rightId)
    {
        List<int> leftIds = new();
        List<int> rightIds = new();
        string query = $"""
                        select {leftId}, {rightId}  
                        from {tableName} 
                        order by {leftId} asc
                        """;
        using SqlConnection con = new SqlConnection(connectionString);
        using SqlCommand com = new SqlCommand(query, con);
        con.Open();
        using SqlDataReader reader = com.ExecuteReader();
        while (reader.Read())
        {
            leftIds.Add(reader.GetInt32(0));
            rightIds.Add(reader.GetInt32(1));
        }

        con.Close();
        return (leftIds, rightIds);
    }


    public static List<allergy> getAllergies()
    {
        List<allergy> allergies = new();
        string query = """
                       select allergyId, allergyName
                       from allergies
                       order by allergyId asc
                       """;
        using SqlConnection con = new SqlConnection(connectionString);
        using SqlCommand com = new SqlCommand(query, con);
        con.Open();
        using SqlDataReader reader = com.ExecuteReader();
        while (reader.Read())
        {
            allergies.Add(new allergy()
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            });
        }

        con.Close();
        return allergies;
    }

    public static List<header> getHeaders()
    {
        List<header> headers = new();
        string query = """
                       select Id, sentDateTime, staffId, tableNumber, finished
                       from headers
                       where Id >= 0
                       order by Id asc
                       """;

        using SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        using SqlCommand com = new SqlCommand(query, con);
        using SqlDataReader reader = com.ExecuteReader();
        while (reader.Read())
        {
            int temp1 = reader.GetInt32(0);
            DateTime temp2 = reader.GetDateTime(1);
            staff temp3 = new()
            {
                Id = reader.GetInt32(2)
            };
            int temp4 = reader.GetInt32(3);
            int temp5 = reader.GetInt32(4);

            headers.Add(new header()
            {
                Id = reader.GetInt32(0),
                sentDateTime = reader.GetDateTime(1),
                headerStaff = new()
                {
                    Id = reader.GetInt32(2),
                },

                tableId = reader.GetInt32(3),
                finished = reader.GetInt32(4)
            });
        }

        con.Close();
        return headers;
    }

    public static List<order> getOrders()
    {
        List<order> orders = new();
        string query = """
                       select Id, headerId
                       from orders
                       where headerId >= 0
                       order by Id asc
                       """;

        using SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        using SqlCommand com = new SqlCommand(query, con);
        using SqlDataReader reader = com.ExecuteReader();
        while (reader.Read())
        {
            orders.Add(new order()
            {
                Id = reader.GetInt32(0),
                headerId = reader.GetInt32(1)
            });
        }

        con.Close();
        return orders;
    }

    public static List<orderLine> getOrderLines()
    {
        List<orderLine> orderLines = new();
        string query = """
                       select Id, orderId, itemId
                       from orderLine
                       order by Id asc
                       """;
        using SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        using SqlCommand com = new SqlCommand(query, con);
        using SqlDataReader reader = com.ExecuteReader();
        while (reader.Read())
        {
            orderLines.Add(new orderLine()
            {
                Id = reader.GetInt32(0),
                orderId = reader.GetInt32(1),
                itemId = reader.GetInt32(2)
            });
        }

        con.Close();
        return orderLines;
    }

    public static (List<int> orderId, List<int> tableId) getTables()
    {
        (List<int> orderId, List<int> tableId) tables = new();
        string query = """
                       select orders.Id, headers.tableNumber
                       from orders, headers
                       where headers.Id = orders.headerId
                       order by tableNumber asc
                       """;
        using SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        using SqlCommand com = new SqlCommand(query, con);
        using SqlDataReader reader = com.ExecuteReader();
        try
        {
            while (reader.Read())
            {
                tables.orderId.Add(reader.GetInt32(0));
                tables.tableId.Add(reader.GetInt32(1));
            }

            con.Close();
        }
        catch (Exception ex)
        {
            Logger.Log($"error in getTables in SQL {ex}");
        }

        if (tables.tableId.Count != tables.orderId.Count)
            Logger.Log("the amount of tableId's and orderId's dont " +
                       "match which means something went aabsolutely fucky wucky but oh well");
        return tables;
    }

    #endregion
}