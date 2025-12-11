using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Text.Json;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace WorkCloneCS;
public class table
{
    public staff openStaff { get; set; }
    public int tableId { get; set; }
    public List<item> ordered { get; set; }
    public List<item> itemsToOrder { get; set; }
    public table()
    {
        tableId = 0;
        openStaff = new staff();
        ordered = new List<item>();
        itemsToOrder = new List<item>();
    }
}


public class staff
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Access { get; set; }

    public staff()
    {
        Id = 0;
        Name = "";
        Access = 0;
    }
}

class catagory
{
    // this is purely for purposes such as how in the locally saved files 
    // there might not be certain properties from the database so i am just putting this here so i can 
    public bool connected { get; set; }
    public string catName { get; set; }
    public int catagoryId { get; set; }
    public string catagoryExtraInfo { get; set; }

    public string catColour { get; set; }
    public List<item> items { get; set; }
}

public class item
{
    public int itemId { get; set; }
    public string itemName { get; set; }
    public string extraInfo { get; set; }
    public int itemCount { get; set; }
    public decimal price { get; set; }
    public string chosenColour { get; set; }
    public int lineId { get; set; }
    public bool ordered { get; set; }
    public List<string> containedAllergies  { get; set; }
}


public class rowOfItem : item
{
    private int rowHeight = 40;
    private Label left, middle, right;

    private item Tag;
    public int lineId;
    public int maxWidth = 850;
    public FlowLayoutPanel rowPannel;


    public Label Left { get { return left; } set { left = value; } }
    public Label Right { get { return right; } set { right = value; } }
    public Label Middle { get { return middle; } set { middle = value; } }

    public rowOfItem()
    {
        itemName = "litterallly anythingelse";
        int countLabelWidth = 30;
        int priceLabelWidth = 60;
        itemId = 0;
        lineId = 0;
        price = 0;
        itemCount = 1;
        Tag = new item()
        {
            itemCount = itemCount,
            itemName = itemName,
            price = price,
            itemId = itemId,
            lineId = lineId,
            ordered = ordered

        };


        rowPannel = new()
        {
            Height = rowHeight,
            Tag = Tag,
            Padding = new Padding(0),
            Margin = new Padding(0),
            Width = maxWidth - SystemInformation.VerticalScrollBarWidth,
            AutoSize = false,
            AutoScroll = false,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = Color.Green
        };

        left = new Label
        {
            Text = itemCount.ToString(),
            Width = countLabelWidth,
            Height = rowHeight,
            AutoSize = false,
            Padding = new Padding(0),
            Margin = new Padding(0),
            Font = new Font("Segoe UI", 12, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = Color.Yellow

        };

        middle = new Label
        {
            Text = itemName,
            Height = rowHeight,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 12, FontStyle.Regular),
            Padding = new Padding(10, 0, 0, 0),
            Tag = Tag,
            Width = maxWidth - countLabelWidth - priceLabelWidth - SystemInformation.VerticalScrollBarWidth - 9
        };

        right = new Label
        {
            Text = price.ToString("c"),
            Tag = Tag,
            Width = priceLabelWidth,
            Height = rowHeight,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 12, FontStyle.Regular),
            Padding = new Padding(0),
            BackColor = Color.Red,

        };

        rowPannel.Controls.Add(left);
        rowPannel.Controls.Add(middle);
        rowPannel.Controls.Add(right);

    }
    public void SetHeight(int height)
    {
        rowHeight = height;
        left.Height = height;
        middle.Height = height;
        right.Height = height;
        rowPannel.Height = height;



    }


    public void updateText()
    {
        left.Text = itemCount.ToString();
        middle.Text = itemName;
        right.Text = (itemCount * price).ToString("c");
    }

    public void Dispose()
    {
        foreach (Control control in rowPannel.Controls)
        {
            if (control == Left || control == Right || control == middle)
            {
                foreach (Control control2 in control.Controls)
                {
                    control.Controls.Remove(control2);
                    control2.Dispose();
                }
            }
            rowPannel.Controls.Remove(control);
            control.Dispose();
        }
    }
    
    
}


///
/// this will be where i put the daatabase class i am going to redo the waya i store my local database.
/// this will make it unbelieably more efficient in the long run but gonna need to cook for a few hours
///

class dbCatagories
{
    private List<int> itemIds;
    private string catName;
    private int catId;

    dbCatagories()
    {
        catId = 0;
        catName = "";
    }
}


class database
{
    private static SqlConnection connection;
    private static List<item> items;
    private static List<catagory> catagories;
    private static bool DBExists;
    private static List<List<int>> catItemLinks;

    public static void initLocalDatabase()
    {
        DBExists = databaseExists();
        if(DBExists) checkDBVNum();
        items = SQL.getAllItems();
        catagories = SQL.getAllCatagories();
        catItemLinks = SQL.getCatItemLinks(); // this is to compare the items and catagories
        updateCatagories();
        
        
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
        if (local != cloud)
        {
            Logger.Log("local database version is different from cloud database version updating now");
            sync.syncAll();
        }
        else Logger.Log("local database is up to date");
        
        

    }

    private static void updateCatagories()
    {
        if (catagories == null)
        {
            Logger.Log("updateCatagories(): catagories is null (SQL.getAllCatagories returned null).");
            return;
        }

        if (items == null)
        {
            Logger.Log("updateCatagories(): items is null (SQL.getAllItems returned null).");
            return;
        }

        if (catItemLinks == null)
        {
            Logger.Log("updateCatagories(): catItemLinks is null (SQL.getCatItemLinks returned null).");
            return;
        }
        var catById = new Dictionary<int, catagory>(catagories.Count);
        foreach (catagory cat in catagories)
        {
            cat.items ??= new List<item>();
            catById[cat.catagoryId] = cat;
        }

        var itemById = new Dictionary<int, item>(items.Count);
        foreach (item it in items)
            itemById[it.itemId] = it;
        
        foreach (var link in catItemLinks)
        {
            if (link == null || link.Count < 2)
                continue;

            int itemId = link[1];
            int catagoryId = link[0];

            if (!catById.TryGetValue(catagoryId, out var cat))
                continue;

            if (!itemById.TryGetValue(itemId, out var it))
                continue;

            cat.items.Add(it);
        }
        Logger.Log("finished updating catagories");
    }
    
}


