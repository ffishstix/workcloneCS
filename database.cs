using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Data.SqlClient;

namespace WorkCloneCS;

static class database
{
    #region variables

    // local database variables
    public static bool isConnectedToRemoteServer = false;
    public static int cloudVNum = 0;
    private const bool extraSafe = true;
    public static bool pullCloudStarted = false;
    public static bool DBExists = false;

    private static int localVNum = -1; // this is the variable gotten from local file

    // and updated when sync occurs.
    private static readonly JsonSerializerOptions SnapshotJsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = false,
        IncludeFields = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.Strict,
        AllowTrailingCommas = false,
        ReadCommentHandling = JsonCommentHandling.Disallow
    };

    //table sql results
    public static Dictionary<int, allergy> allergies { get; private set; } = new();
    private static Dictionary<int, item> items = new();
    private static Dictionary<int, dbCategory> categories = new();
    private static Dictionary<int, header> headers = new();
    private static Dictionary<int, List<orderLine>> orderLines = new();
    private static Dictionary<int, order> orders = new();
    private static Dictionary<int, staff> staff = new();
    private static Dictionary<int, table> tables = new(); 

    //links
    private static Dictionary<int, HashSet<int>> catItemLinks = new();
    private static Dictionary<int, HashSet<int>> allergyItemLinks = new();

    #endregion

    #region functions

    public static accessLevel getAccessLevelFromId(int Id)
    {
        accessLevel acc = staff[Id].staffAccess;
        if (acc == new accessLevel())
        {
            return SQL.getAccessLevelFromId(Id);
        }

        return acc;
    }


    public static bool pullCloudDatabase()
    {
        if (!SQL.checkDBConnection())
        {
            Logger.Log("pullCloudDatabase(): no database connection available.");
            return false;
        }

        if (!isConnectedToRemoteServer)
        {
            Logger.Log("this function should not have been called " +
                       "as there is no connection to the database");
            return false;
        }

        while (!SQL.initCompleted && !pullCloudStarted)
        {
            if (!SQL.initStarted) SQL.initSQL();
            else Thread.Sleep(10000); // i chose 10 seconds cus that is reasonable for a database
        }

        if (pullCloudStarted) return false;
        pullCloudStarted = true;
        cloudVNum = SQL.getDatabaseVNum();
        saveLocalDbVNum(cloudVNum);
        localVNum = SQL.getLocalDBVNum();
        //local Database Variables
        DBExists = databaseExists();
        if (DBExists) checkDBVNum();

        staff = SQL.getStaff().ToDictionary(s => s.Id);
        headers = SQL.getHeaders().ToDictionary(h => h.Id);
        orders = SQL.getOrders().ToDictionary(o => o.Id);
        orderLines = SQL.getOrderLines().GroupBy(ol => ol.orderId)
            .ToDictionary(g => g.Key, g => g.ToList());
        // following code must be written in this order to avoid null references.
        items = SQL.getAllItems().ToDictionary(i => i.Id);
        allergies = SQL.getAllergies().ToDictionary(a => a.Id);
        updateItems();

        // categories section, must be placed under updateItems.
        categories = SQL.getAllCategories().ToDictionary(c => c.catId);
        updateCategories();

        //links linear (access-->staff-->header-->orders<--orderLine<--item
        addHeadersAndOrderLinesToOrders();
        updateOpenTablesFromLoadedData();

        // in theory this should be all of the data from the database
        //that you can grab and assign.
        pullCloudStarted = false;
        saveLocalDatabase();
        DBExists = true;
        return true;
    }

    private static void saveLocalDbVNum(int n)
    {
        try
        {
            if (!Directory.Exists(SQL.sqlDir)) Directory.CreateDirectory(SQL.sqlDir);
            File.WriteAllText(SQL.sqlDir + "DBvNum.txt", $"{n}");
        }
        catch (Exception ex)
        {
            Logger.Log($"exception in saveLocalDbVNum {ex.Message}");
        }
    }


    public static List<dbCategory> getCategories()
    {
        ensureLocalDatabaseLoaded();
        List<dbCategory> cats = new();
        if (categories == new Dictionary<int, dbCategory>()) return SQL.getAllCategories();
        foreach (dbCategory cat in categories.Values) cats.Add(cat);
        return cats;
    }

    public static List<staff> getStaffList()
    {
        ensureLocalDatabaseLoaded();
        List<staff> list = new();
        if (staff == new Dictionary<int, staff>()) return SQL.getStaff();
        foreach (staff s in staff.Values) list.Add(s);
        return list;
    }

    public static List<item> getCategoryItems(int categoryId)
    {
        ensureLocalDatabaseLoaded();
        List<item> result = new();
        if (categories == new Dictionary<int, dbCategory>() || items == new Dictionary<int, item>()) return result;
        if (!categories.TryGetValue(categoryId, out var cat) || cat.itemIds == new List<int>()) return result;

        foreach (int itemId in cat.itemIds)
        {
            if (!items.TryGetValue(itemId, out var it)) continue;
            result.Add(cloneItemForOrder(it));
        }

        return result;
    }

    public static table getTableItems(int tableId)
    {
        ensureLocalDatabaseLoaded();
        List<item> result = new();
        if (headers == new Dictionary<int, header>() ||
            orders == new Dictionary<int, order>() ||
            orderLines == new Dictionary<int, List<orderLine>>() ||
            items == new Dictionary<int, item>())
            return new();
        headers.Values.ToList();
        staff openStaff = new staff();
        foreach (header h in headers.Values)
        {
            if (h.tableId != tableId) continue;
            foreach (order o in orders.Values)
            {
                if (o.headerId != h.Id) continue;
                if (!orderLines.TryGetValue(o.Id, out var lines) || lines == null) continue;
                if (openStaff == new staff()) openStaff = h.headerStaff;
                foreach (orderLine line in lines)
                {
                    if (!items.TryGetValue(line.itemId, out var it)) continue;
                    item copy = cloneItemForOrder(it);
                    copy.lineId = line.Id;
                    copy.ordered = true;
                    copy.messages = parseLineMessages(line.lineMessage);
                    result.Add(copy);
                }
            }
        }

        table table = new table()
        {
            ordered = result,
            openStaff = openStaff,
            tableId = tableId
        };
        return table;
    }

    public static List<item> getOpenTableItemsFromSqlAndUpdateLocal(int tableId, staff fallbackStaff = null)
    {
        ensureLocalDatabaseLoaded();

        List<item> sqlItems = SQL.getOpenTableItemsFromDatabase(tableId) ?? new List<item>();
        tables ??= new Dictionary<int, table>();

        if (sqlItems.Count == 0)
        {
            tables.Remove(tableId);
            saveLocalDatabase(false);
            return new List<item>();
        }

        table updatedTable = new table
        {
            tableId = tableId,
            openStaff = resolveOpenTableStaff(tableId, fallbackStaff)
        };

        foreach (item sqlItem in sqlItems)
        {
            item copy = cloneItemForOrder(sqlItem);
            copy.ordered = true;
            updatedTable.ordered.Add(copy);
        }

        tables[tableId] = updatedTable;
        saveLocalDatabase(false);

        List<item> response = new();
        foreach (item localItem in updatedTable.ordered)
        {
            response.Add(cloneItemForOrder(localItem));
        }

        return response;
    }

    public static Dictionary<int, (int itemCount, decimal totalPrice)> getOpenTableSummaries()
    {
        ensureLocalDatabaseLoaded();
        Dictionary<int, (int itemCount, decimal totalPrice)> summaries = new();

        if (headers == null || orders == null || orderLines == null || items == null) return summaries;

        Dictionary<int, int> openHeaderToTable = new();
        foreach (header h in headers.Values)
        {
            if (h == null) continue;
            if (h.tableId <= 0) continue;
            if (h.finished != 0) continue;
            openHeaderToTable[h.Id] = h.tableId;
        }

        if (openHeaderToTable.Count == 0) return summaries;

        foreach (order o in orders.Values)
        {
            if (o == null) continue;
            if (!openHeaderToTable.TryGetValue(o.headerId, out int tableId)) continue;
            if (!orderLines.TryGetValue(o.Id, out var lines) || lines == null) continue;

            foreach (orderLine line in lines)
            {
                if (line == null) continue;
                if (!items.TryGetValue(line.itemId, out var orderedItem) || orderedItem == null) continue;

                (int itemCount, decimal totalPrice) summary =
                    summaries.TryGetValue(tableId, out var existing)
                        ? existing
                        : (0, 0m);

                summary.itemCount += 1;
                summary.totalPrice += orderedItem.price;
                summaries[tableId] = summary;
            }
        }

        return summaries;
    }

    public static void addTableOrder(table table, staff staffMember)
    {
        if (table == new table() || staffMember == new staff())
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
        if (table.itemsToOrder != new List<item>())
        {
            foreach (item it in table.itemsToOrder)
            {
                lines.Add(new orderLine
                {
                    Id = lineId++,
                    orderId = orderId,
                    itemId = it.Id,
                    lineMessage = serialiseLineMessages(it.messages)
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
            if (string.IsNullOrWhiteSpace(SQL.connectionString))
            {
                Logger.Log("tryLoadLocalDatabase(): connection string not set yet; skipping cloud pull.");
                return false;
            }

            return pullCloudDatabase();
        }

        try
        {
            loadLocalDatabase();
            DBExists = true;
            return true;
        }
        catch (Exception ex)
        {
            Logger.Log($"failed to load local database: {ex.Message}");
            return false;
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
        if (items == new Dictionary<int, item>()) return new();
        return items[itemId];
    }


    //<summary>
    // checks all variables are not null then stores them. 
    // specifically in json format.
    // </summary>

    public static void saveLocalDatabase(bool allowCloudRefresh = true)
    {
        if (!DBExists) Logger.Log("db file doesnt exist however i am creating one");
        if (!Directory.Exists(SQL.sqlDir)) Directory.CreateDirectory(SQL.sqlDir);
        int temp = SQL.getDatabaseVNum();
        if (allowCloudRefresh && (cloudVNum != temp || cloudVNum != localVNum)) pullCloudDatabase();
        if (!( // this is ensuring that all possible vairables are assigned
                cloudVNum == 0 || localVNum == -1 ||
                allergies == new Dictionary<int, allergy>() ||
                items == new Dictionary<int, item>() ||
                categories == new Dictionary<int, dbCategory>() ||
                headers == new Dictionary<int, header>() ||
                orderLines == new Dictionary<int, List<orderLine>>() ||
                orderLines == new Dictionary<int, List<orderLine>>() ||
                staff == new Dictionary<int, staff>() ||
                //tables == null || tables == new Dictionary<int, table>() ||
                catItemLinks == new Dictionary<int, HashSet<int>>() ||
                allergyItemLinks == new Dictionary<int, HashSet<int>>()))
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
        if (allergies != new Dictionary<int, allergy>() &&
            items != new Dictionary<int, item>() &&
            categories != new Dictionary<int, dbCategory>() &&
            headers != new Dictionary<int, header>() &&
            orders != new Dictionary<int, order>() &&
            orderLines != new Dictionary<int, List<orderLine>>() &&
            staff != new Dictionary<int, staff>() &&
            catItemLinks != new Dictionary<int, HashSet<int>>() &&
            allergyItemLinks != new Dictionary<int, HashSet<int>>())

            return true;
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
            lineId = source.lineId,
            ordered = source.ordered,
            allergies = source.allergies == null ? null : new List<allergy>(source.allergies),
            hasSubItems = source.hasSubItems,
            subItems = source.subItems,
            messages = source.messages == null ? new List<string>() : new List<string>(source.messages)
        };
    }

    private static staff resolveOpenTableStaff(int tableId, staff fallbackStaff = null)
    {
        foreach (header hdr in headers.Values)
        {
            if (hdr.tableId != tableId || hdr.finished != 0) continue;
            if (staff.TryGetValue(hdr.headerStaff.Id, out var assignedStaff)) return assignedStaff;
            if (hdr.headerStaff != null && hdr.headerStaff.Id != 0) return hdr.headerStaff;
        }

        if (tables.TryGetValue(tableId, out var existingTable) &&
            existingTable?.openStaff != null &&
            existingTable.openStaff.Id != 0)
            return existingTable.openStaff;

        return fallbackStaff ?? new staff();
    }

    private static string serialiseLineMessages(List<string>? messages)
    {
        if (messages == null || messages.Count == 0) return string.Empty;
        string combined = string.Join("\n", messages.Where(m => !string.IsNullOrWhiteSpace(m)).Select(m => m.Trim()));
        return combined.Length > 1000 ? combined[..1000] : combined;
    }

    private static List<string> parseLineMessages(string? lineMessage)
    {
        List<string> parsed = new();
        if (string.IsNullOrWhiteSpace(lineMessage)) return parsed;
        string[] split = lineMessage.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        foreach (string message in split)
        {
            string trimmed = message.Trim();
            if (trimmed.Length > 0) parsed.Add(trimmed);
        }

        return parsed;
    }

    private static int getNextId<T>(Dictionary<int, T> dict)
    {
        if (dict == null || dict.Count == 0) return 1;
        return dict.Keys.Max() + 1;
    }

    private static int getNextOrderLineId()
    {
        if (orderLines.Count == 0) return 1;
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


    private static void updateOpenTablesFromLoadedData()
    {
        tables = new Dictionary<int, table>();

        foreach (order ord in orders.Values)
        {
            if (!headers.TryGetValue(ord.headerId, out var hdr)) continue;
            if (hdr.finished != 0 || hdr.tableId <= 0) continue;

            if (!tables.TryGetValue(hdr.tableId, out var openTable))
            {
                openTable = new table
                {
                    tableId = hdr.tableId,
                    openStaff = staff.TryGetValue(hdr.headerStaff.Id, out var s) ? s : hdr.headerStaff
                };
                tables[hdr.tableId] = openTable;
            }

            if (!orderLines.TryGetValue(ord.Id, out var lines) || lines == null) continue;
            foreach (orderLine line in lines)
            {
                if (!items.TryGetValue(line.itemId, out var orderedItem)) continue;

                item copy = cloneItemForOrder(orderedItem);
                copy.lineId = line.Id;
                copy.ordered = true;
                copy.messages = parseLineMessages(line.lineMessage);
                openTable.ordered.Add(copy);
            }
        }
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
        if (Directory.Exists(dir)) return File.Exists(dir + "DBvNum.txt") && File.Exists(SQL.sqlDir + "database.json");
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
            if (pullCloudDatabase())
            {
                Logger.Log("updateSuccessFul");
                break;
            }

            Logger.Log("if you see this message twice something has gone wrong");
        }
    }

    private static void updateCategories()
    {
        if (items == new Dictionary<int, item>())
        {
            Logger.Log("updateCategories(): items is null (SQL.getAllItems returned null).");
            return;
        }

        if (categories == new Dictionary<int, dbCategory>())
        {
            Logger.Log("updateCategories(): categories is null (SQL.getAllCategories returned null).");
            return;
        }

        basicJunctionTable t = new basicJunctionTable
        (
            "foodCategory",
            "categoryId",
            "itemId"
        );
        t.populateTable();
        catItemLinks = t.combined;


        if (catItemLinks == new Dictionary<int, HashSet<int>>())
        {
            Logger.Log("updateCategories(): catItemLinks is null.");
            return;
        }

        foreach (dbCategory cat in categories.Values)
        {
            cat.itemIds ??= new List<int>();
            cat.itemIds.Clear(); // important: prevent duplicates on refresh
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
            cloudVNum = cloudVNum,
            DBExists = DBExists,
            localVNum = localVNum,

            allergies = allergies,
            items = items,
            categories = categories,
            headers = headers,
            orders = orders,
            orderLines = orderLines,
            staff = staff,
            tables = tables,

            catItemLinks = catItemLinks,
            allergyItemLinks = allergyItemLinks
        };
    }


    public static void loadLocalDatabase()
    {
        dbSnapShot snap = JsonSerializer.Deserialize<dbSnapShot>(
            File.ReadAllText(SQL.sqlDir + "database.json"), SnapshotJsonOptions);
        if (snap == null) throw new InvalidDataException("database snapshot was null after deserialize.");
        cloudVNum = snap.cloudVNum;
        DBExists = true;
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

    #endregion
}
