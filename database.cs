using Microsoft.Data.SqlClient;

namespace WorkCloneCS;

static class database
{
    #region variables
    
    // local database variables
    public static readonly int databaseVersion;
    private static bool DBExists;
    
    //table sql results
    public static List<allergy> allergies;
    private static List<item> items;
    private static List<dbCategory> categories;
    private static Dictionary<int, header> headers;
    private static Dictionary<int, List<orderLine>> orderLines;
    private static List<order> orders;
    private static List<staff> staff;
    
    private static List<table> tables; //future shizzle
    
    //links
    private static Dictionary<int, HashSet<int>> catItemLinks;
    private static Dictionary<int, HashSet<int>> allergyItemLinks;
    #endregion
    
    #region functions
    public static void initLocalDatabase()
    {
        while (!SQL.initCompleted)
        {
            
            if (!SQL.initStarted) SQL.initSQL();
            else Thread.Sleep(10000); // i chose 10 seconds cus that is reasonable for a database
        }
        
        //local Database Variables
        DBExists = databaseExists();
        if (DBExists) checkDBVNum();
        
        
        //table sql results
        items = SQL.getAllItems();
        categories = SQL.getAllCategories();
        updateCategories();
        allergies = SQL.getAllergies();
        staff = SQL.getStaff(); //getStaff is updated specifically for this function
        // bit of a bigger explanation bassically it gives the individual staffs their 
        // access levels so we dont need to do voodoo dictionaries.
        headers = SQL.getHeaders().ToDictionary(h => h.Id);
        
        orders = SQL.getOrders();
        orderLines = SQL.getOrderLines().
            GroupBy(ol => ol.orderId)
            .ToDictionary(g => g.Key, g => g.ToList());
        
        
        //links (junction tables only
        allergyItemLinks = new basicJunctionTable
        {
            tableName = "allergyItem",
            leftCol = "Id",
            rightCol = "allergyId"
            
        }.combined;
        catItemLinks = new basicJunctionTable
        {
            tableName = "foodCategory",
            leftCol = "catId",
            rightCol = "Id"
            
        }.combined; 
        
        
        //links linear (access-->staff-->header-->orders<--orderLine<--item
        addHeadersAndOrderLinesToOrders();
    
        // in theory this should be all of the data from the database
        //that you can grab and assign.
        
        

    }

    private static void getTables()
    {
        // tables and shizzlez
    }
    
    private static void addHeadersAndOrderLinesToOrders() 
    {
        foreach (var o in orders)
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
        if (Directory.Exists(dir)) return File.Exists(dir + "DBvNum.txt");
        Logger.Log($"static Classes databaseExists() {dir} directory doesnt exist");
        return false;

    }

    private static void checkDBVNum()
    {
        int cloud = SQL.getDatabaseVNum();
        int local = SQL.getLocalDBVNum();
        int syncCount = 0;
        do
        {
            Logger.Log("local database version is different from cloud database version updating now");
            sync.syncAll();
            syncCount++;
            cloud = SQL.getDatabaseVNum();
            local = SQL.getLocalDBVNum();
        } while (local != cloud);
        
        Logger.Log("local database is up to date");
        Logger.Log($"it took {syncCount} syncs to update if this number " +
                   $"is more than one your system is being updated very often");



    }

    private static void updateCategories()
    {
        if (categories == null)
        {
            Logger.Log("updateCategories(): categories is null (SQL.getAllCategories returned null).");
            return;
        }

        if (items == null)
        {
            Logger.Log("updateCategories(): items is null (SQL.getAllItems returned null).");
            return;
        }

        if (catItemLinks == null)
        {
            Logger.Log("updateCategories(): catItemLinks is null.");
            return;
        }

        // Build fast lookup: categoryId → category
        var catById = new Dictionary<int, dbCategory>(categories.Count);
        foreach (dbCategory cat in categories)
        {
            cat.Ids ??= new List<int>();
            cat.Ids.Clear();                  // important: prevent duplicates on refresh
            catById[cat.catId] = cat;
        }

        // Build fast lookup: Id → Id (or item if you later need object)
        var itemById = new HashSet<int>(items.Count);
        foreach (item it in items)
            itemById.Add(it.Id);

        // Now walk the junction map
        foreach (var kvp in catItemLinks)
        {
            int categoryId = kvp.Key;
            HashSet<int> Ids = kvp.Value;

            if (!catById.TryGetValue(categoryId, out var cat))
                continue;

            foreach (int Id in Ids)
            {
                if (!itemById.Contains(Id))
                    continue;

                cat.Ids.Add(Id);
            }
        }
    }

    private static void initSubClasses()
    {
        
        
    }

    //function to be called after all allergies and items have been initialised.
    private static void updateItems()
    {
        var itemById = new Dictionary<int, item>(items.Count);
        foreach (var it in items)
        {
            it.allergies ??= new List<allergy>();
            it.allergies.Clear();       // critical when reloading
            itemById[it.Id] = it;
        }

        var allergyById = new Dictionary<int, allergy>(allergies.Count);
        foreach (var al in allergies)
        {
            allergyById[al.Id] = al;
        }


    }
    #endregion
}
