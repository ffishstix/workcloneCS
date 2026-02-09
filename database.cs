using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WorkCloneCS;




static class database
{
    #region variables
    
    // local database variables
    public static int cloudVNum;
    private const bool extraSafe = true;
    public static bool pullCloudStarted = false;
    private static bool DBExists;
    private static int localVNum; // this is the variable gotten from local file
                                   // and updated when sync occurs.
    private static readonly JsonSerializerOptions SnapshotJsonOptions = new()
    {
    WriteIndented = true,
    ReferenceHandler = ReferenceHandler.Preserve,
    PropertyNameCaseInsensitive = false,
    IncludeFields = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.Never,
    NumberHandling = JsonNumberHandling.Strict,
    AllowTrailingCommas = false,
    ReadCommentHandling = JsonCommentHandling.Disallow
    };
    //table sql results
    public static Dictionary<int, allergy> allergies {get; private set;}
    private static Dictionary<int, item> items;
    private static Dictionary<int, dbCategory> categories;
    private static Dictionary<int, header> headers;
    private static Dictionary<int, List<orderLine>> orderLines;
    private static Dictionary<int, order> orders;
    private static Dictionary<int, staff> staff;
    
    private static Dictionary<int, table> tables; //future shizzle
    
    //links
    private static Dictionary<int, HashSet<int>> catItemLinks;
    private static Dictionary<int, HashSet<int>> allergyItemLinks;
    #endregion
    
    #region functions
    public static bool pullCloudDatabase()
    {
        if (pullCloudStarted) return false;
        pullCloudStarted = true;
        
        while (!SQL.initCompleted)
        {
            
            if (!SQL.initStarted) SQL.initSQL();
            else Thread.Sleep(10000); // i chose 10 seconds cus that is reasonable for a database
        }
        
        cloudVNum = SQL.getDatabaseVNum();
        saveLocalDbVNum(cloudVNum);
        localVNum = SQL.getLocalDBVNum();
        //local Database Variables
        DBExists = databaseExists();
        if(DBExists) checkDBVNum();
        
        staff = SQL.getStaff().ToDictionary(s => s.Id);    
        
        headers = SQL.getHeaders().ToDictionary(h => h.Id);
        
        orders = SQL.getOrders().ToDictionary(o => o.Id);
        orderLines = SQL.getOrderLines().
            GroupBy(ol => ol.orderId)
            .ToDictionary(g => g.Key, g => g.ToList());
        
        
        updateItems();
        updateCategories();
        
        //links linear (access-->staff-->header-->orders<--orderLine<--item
        addHeadersAndOrderLinesToOrders();
    
        // in theory this should be all of the data from the database
        //that you can grab and assign.

        
        saveLocalDatabase();
        DBExists = true;
    }

    private static void saveLocalDbVNum(int n)
    {
        try 
        {
            File.WriteAllText(SQL.sqlDir + "DBvNum.txt"  , $"{n}");
        } catch (Exception ex)
        {
            Logger.Log($"exception in saveLocalDbVNum {ex.Message}");
        }
    }
    
    
    public static List<dbCategory> getCategories()
    {
        ensureLocalDatabaseLoaded();
        List<dbCategory> cats = new();
        if (categories == null) return cats;
        foreach (dbCategory cat in categories.Values) cats.Add(cat);
        return cats;
    }
    
    public static List<staff> getStaffList()
    {
        ensureLocalDatabaseLoaded();
        List<staff> list = new();
        if (staff == null) return list;
        foreach (staff s in staff.Values) list.Add(s);
        return list;
    }

    public static List<item> getCategoryItems(int categoryId)
    {
        ensureLocalDatabaseLoaded();
        List<item> result = new();
        if (categories == null || items == null) return result;
        if (!categories.TryGetValue(categoryId, out var cat) || cat.itemIds == null) return result;

        foreach (int itemId in cat.itemIds)
        {
            if (!items.TryGetValue(itemId, out var it)) continue;
            result.Add(cloneItemForOrder(it));
        }
        return result;
    }

    public static List<item> getTableItems(int tableId)
    {
        ensureLocalDatabaseLoaded();
        List<item> result = new();
        if (headers == null || orders == null || orderLines == null || items == null) return result;

        foreach (header h in headers.Values)
        {
            if (h.tableId != tableId || h.finished != 0) continue;

            foreach (order o in orders.Values)
            {
                if (o.headerId != h.Id) continue;
                if (!orderLines.TryGetValue(o.Id, out var lines) || lines == null) continue;

                foreach (orderLine line in lines)
                {
                    if (!items.TryGetValue(line.itemId, out var it)) continue;
                    item copy = cloneItemForOrder(it);
                    copy.lineId = line.Id;
                    result.Add(copy);
                }
            }
        }

        return result;
    }

    public static void addTableOrder(table table, staff staffMember)
    {
        if (table == null || staffMember == null)
        {
            Logger.Log("addTableOrder called with null table or staff");
            return;
        }

        ensureLocalDatabaseLoaded();
        headers ??= new Dictionary<int, header>();
        orders ??= new Dictionary<int, order>();
        orderLines ??= new Dictionary<int, List<orderLine>>();

        if (table.tableId < 1) table.tableId = 4000;

        int headerId = getNextId(headers);
        int orderId = getNextId(orders);
        int lineId = getNextOrderLineId();

        header newHeader = new()
        {
            Id = headerId,
            sentDateTime = DateTime.Now,
            headerStaff = staffMember,
            tableId = table.tableId,
            finished = 0
        };
        headers[headerId] = newHeader;

        order newOrder = new()
        {
            Id = orderId,
            headerId = headerId,
            header = newHeader
        };
        orders[orderId] = newOrder;

        List<orderLine> lines = new();
        if (table.itemsToOrder != null)
        {
            foreach (item it in table.itemsToOrder)
            {
                lines.Add(new orderLine
                {
                    Id = lineId++,
                    orderId = orderId,
                    itemId = it.Id
                });
            }
        }
        orderLines[orderId] = lines;
        newOrder.orderLines = lines;

        saveLocalDatabase(false);
    }

    public static void setConnectionString(string value)
    {
        SQL.connectionString = value;
    }

    public static async Task<(bool ok, string errorMessage)> tryOpenConnectionAsync(string connectionString)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public static bool tryLoadLocalDatabase()
    {
        string path = SQL.sqlDir + "database.json";
        if (!File.Exists(path))
        {
            Logger.Log($"local database file missing: {path}");
            pullCloudDatabase();
            return true;
        }
        else
        {
            try
            {
                loadLocalDatabase();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"failed to load local database: {ex.Message}");
                return false;
            }
        }
        
    }
    
    
    public static void updateStaff()
    {
        try
        {
            staff = SQL.getStaff().ToDictionary(s => s.Id);
        }
        catch (Exception ex)
        {
            Logger.Log($"error in updateStaff: {ex.Message}");
        }
        
        
    }

    public static item getItemFromId(int itemId)
    {
        if (items == null) return null;
        return items[itemId];
    }
    
    public static void saveLocalDatabase(bool allowCloudRefresh = true)
    {
        int temp = SQL.getDatabaseVNum();
        if (allowCloudRefresh && (cloudVNum != temp || cloudVNum != localVNum)) pullCloudDatabase();
        if (!( // this is ensuring that all possible vairables are assigned
                cloudVNum == null || cloudVNum == new int() ||
            DBExists == false ||
            allergies == null || allergies == new Dictionary<int, allergy>() ||
            items == null || items == new Dictionary<int, item>() ||
            categories == null || categories == new Dictionary<int, dbCategory>() ||
            headers == null || headers == new Dictionary<int, header>() ||
            orderLines == null || orderLines == new Dictionary<int, List<orderLine>>() ||
            orders == null || orderLines == new Dictionary<int, List<orderLine>>() ||
            staff == null || staff == new Dictionary<int, staff>() ||
            //tables == null || tables == new Dictionary<int, table>() ||
            catItemLinks == null || catItemLinks == new Dictionary<int, HashSet<int>>() ||
            allergyItemLinks == null || allergyItemLinks == new Dictionary<int, HashSet<int>>() ||
            localVNum == null || localVNum == new int()

            ))
        {
            Logger.Log("all checks are valid can begin to serialise and save datbase saveLocalDatabase()");
            dbSnapShot snap = getDatabaseSnapShot();
            string json = JsonSerializer.Serialize(snap, SnapshotJsonOptions);
            File.WriteAllText(SQL.sqlDir + "database.json", json);

            if (extraSafe)
            {
                // we now do a solid test to see if the written json is = to the one we just made:
                string json2 = File.ReadAllText(SQL.sqlDir + "database.json");
                if (json == json2) Logger.Log("database saved successfully");
                else Logger.Log("database failed to save");
                return;
            }
            Logger.Log("extrasafe not enabled so we arent going to check if it saved correctly");
        }
        Logger.Log("database failed to save a variable isnt initialised correctly");
    }

    private static bool ensureLocalDatabaseLoaded()
    {
        if (allergies != null &&
            items != null &&
            categories != null &&
            headers != null &&
            orders != null &&
            orderLines != null &&
            staff != null &&
            catItemLinks != null &&
            allergyItemLinks != null)
        {
            return true;
        }

        return tryLoadLocalDatabase();
    }

    private static item cloneItemForOrder(item source)
    {
        return new item
        {
            Id = source.Id,
            Name = source.Name,
            extraInfo = source.extraInfo,
            itemCount = 1,
            price = source.price,
            chosenColour = source.chosenColour,
            ordered = source.ordered,
            allergies = source.allergies == null ? null : new List<allergy>(source.allergies),
            hasSubItems = source.hasSubItems,
            subItems = source.subItems
        };
    }

    private static int getNextId<T>(Dictionary<int, T> dict)
    {
        if (dict == null || dict.Count == 0) return 1;
        return dict.Keys.Max() + 1;
    }

    private static int getNextOrderLineId()
    {
        if (orderLines == null || orderLines.Count == 0) return 1;
        int maxId = 0;
        foreach (List<orderLine> lines in orderLines.Values)
        {
            foreach (orderLine line in lines)
            {
                if (line.Id > maxId) maxId = line.Id;
            }
        }
        return maxId + 1;
    }

    public static void loadLocalDatabase()
    {
        
        dbSnapShot snap = JsonSerializer.Deserialize<dbSnapShot>(
            File.ReadAllText(SQL.sqlDir + "database.json"), SnapshotJsonOptions);
        cloudVNum = snap.cloudVNum;
        DBExists = snap.DBExists;
        localVNum = snap.localVNum;
        allergies = snap.allergies;
        items = snap.items;
        categories = snap.categories;
        headers = snap.headers;
        orders = snap.orders;
        orderLines = snap.orderLines;
        staff = snap.staff;
        tables = snap.tables;
        catItemLinks = snap.catItemLinks;
        allergyItemLinks = snap.allergyItemLinks;
        Logger.Log("database loaded successfully");
    }
    
    private static void getTables()
    {
        var x = SQL.getTables();
        List<int> orderIds = x.orderId;
        List<int> tableIds = x.tableId;
        
    }
    
    private static void addHeadersAndOrderLinesToOrders() 
    {
        foreach (var o in orders.Values)
        {
            if (headers.TryGetValue(o.headerId, out var h))
                o.header = h;

            if (orderLines.TryGetValue(o.Id, out var lines))
                o.orderLines = lines;
        }

    }
    
    private static bool databaseExists()
    {
        string dir = @$"{SQL.dir}sql/";
        if (Directory.Exists(dir)) return File.Exists(dir + "DBvNum.txt") && File.Exists(SQL.dir + "database.json");
        Logger.Log($"static Classes databaseExists() {dir} directory doesnt exist");
        return false;

    }

    private static void checkDBVNum()
    {
        cloudVNum = SQL.getDatabaseVNum();
        localVNum = SQL.getLocalDBVNum();
        int syncCount = 0;
            while (localVNum != cloudVNum)
            {
                
                Logger.Log("local database version is different from cloud database version updating now");
                sync.syncAll();
                syncCount++;
                cloudVNum = SQL.getDatabaseVNum();
                localVNum = SQL.getLocalDBVNum();
            }
        
        Logger.Log("local database is up to date");
        Logger.Log($"it took {syncCount} syncs to update if this number " +
                   $"is more than one your system is being updated very often");



    }

    private static void updateCategories()
    {
        if (items == null)
        {
            Logger.Log("updateCategories(): items is null (SQL.getAllItems returned null).");
            return;
        }
        
        categories = SQL.getAllCategories().ToDictionary(c => c.catId);
        if (categories == null)
        {
            Logger.Log("updateCategories(): categories is null (SQL.getAllCategories returned null).");
            return;
        }
        basicJunctionTable t  = new basicJunctionTable 
        (
            "foodCategory", 
            "categoryId", 
            "itemId"
        );
        t.populateTable();
        catItemLinks = t.combined;
        
        
        if (catItemLinks == null)
        {
            Logger.Log("updateCategories(): catItemLinks is null.");
            return;
        }
        
        foreach (dbCategory cat in categories.Values)
        {
            cat.itemIds ??= new List<int>();
            cat.itemIds.Clear();                  // important: prevent duplicates on refresh
        }

        // Build fast lookup: Id → Id (or item if you later need object)
       

        // Now walk the junction map
        foreach (var kvp in catItemLinks)
        {
            int categoryId = kvp.Key;
            HashSet<int> Ids = kvp.Value;

            if (!categories.TryGetValue(categoryId, out var cat))
                continue;

            foreach (int Id in Ids)
            {
                if (!items.ContainsKey(Id))
                    continue;

                cat.itemIds.Add(Id);
            }
        }
    }

    private static void initSubClasses()
    {
        
        
    }

    //function to be called after all allergies and items have been initialised.
    private static void updateItems()
    {
        items = SQL.getAllItems().ToDictionary(i => i.Id);
        allergies = SQL.getAllergies().ToDictionary(a => a.Id);
        basicJunctionTable t = new basicJunctionTable(
            "allergyItem",
            "itemId",
            "allergyId"
        );
        t.populateTable();
        allergyItemLinks = t.combined;
        
        foreach (item it in items.Values)
        {
            it.allergies ??= new List<allergy>();
            it.allergies.Clear();
        }
        foreach (var kvp in allergyItemLinks)
        {
            int itemId = kvp.Key;
            HashSet<int> allergyIds = kvp.Value;
            
            if (!items.TryGetValue(itemId, out var it))
                continue;

            foreach (int allergyId in allergyIds)
            {
                if (!allergies.TryGetValue(allergyId, out var al))
                    continue;

                it.allergies.Add(al);
            }
        }
    }
    
    private static dbSnapShot getDatabaseSnapShot()
    {
        return new dbSnapShot
        {
            cloudVNum = database.cloudVNum,
            DBExists = database.DBExists,
            localVNum = database.localVNum,

            allergies = database.allergies,
            items = database.items,
            categories = database.categories,
            headers = database.headers,
            orders = database.orders,
            orderLines = database.orderLines,
            staff = database.staff,
            tables = database.tables,

            catItemLinks = database.catItemLinks,
            allergyItemLinks = database.allergyItemLinks
        };
    }

    
    #endregion
}