using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WorkCloneCS;



class dbSnapShot
{
    
    // meta
    public int cloudVNum { get; set; }
    public bool DBExists { get; set; }
    public int localVNum { get; set; }

    // tables
    public Dictionary<int, allergy> allergies { get; set; }
    public Dictionary<int, item> items { get; set; }
    public Dictionary<int, dbCategory> categories { get; set; }
    public Dictionary<int, header> headers { get; set; }
    public Dictionary<int, order> orders { get; set; }
    public Dictionary<int, List<orderLine>> orderLines { get; set; }
    public Dictionary<int, staff> staff { get; set; }
    public Dictionary<int, table> tables { get; set; }

    // link tables
    public Dictionary<int, HashSet<int>> catItemLinks { get; set; }
    public Dictionary<int, HashSet<int>> allergyItemLinks { get; set; }
    

}

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
        localVNum = SQL.getLocalDBVNum(); 
        //local Database Variables
        DBExists = databaseExists();
        checkDBVNum();
        
        
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

        DBExists = true;
        saveLocalDatabase();
        return true;
    }

    public static List<dbCategory> getCategories()
    {
        List<dbCategory> cats = new();
        foreach (dbCategory cat in categories.Values) cats.Add(cat);
        return cats;
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
    
    public static void saveLocalDatabase()
    {
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
                //tables == null || tables == new Dictionary<int, table>() to be added
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