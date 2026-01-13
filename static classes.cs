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

    public bool paid { get; set; }
    
    

    public table()
    {
        tableId = 0;
        openStaff = new staff();
        ordered = new List<item>();
        itemsToOrder = new List<item>();
    }
}


public class staff : baseItem
{
    
    public accessLevel staffAccess { get; set; }

    public staff()
    {
        Id = 0;
        Name = "";
        staffAccess = new accessLevel()
        {
            Id = 0,
            Name = "",
        };
    }
}

class category
{
    // this is purely for purposes such as how in the locally saved files 
    // there might not be certain properties from the database so i am just putting this here so i can 
    public bool connected { get; set; }
    public string catName { get; set; }
    public int categoryId { get; set; }
    public string categoryExtraInfo { get; set; }

    public string catColour { get; set; }
    public List<item> items { get; set; }
}

public class item : baseItem
{

    public string extraInfo { get; set; }
    public int itemCount { get; set; }
    public decimal price { get; set; }
    public string chosenColour { get; set; }
    public int lineId { get; set; }
    public bool ordered { get; set; }
    public List<allergy> allergies  { get; set; }
    public bool hasSubItems { get; set; }

    public List<dbSubCat> subItems { get; set; }
    


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
        Name = "litterallly anythingelse";
        int countLabelWidth = 30;
        int priceLabelWidth = 60;
        Id = 0;
        lineId = 0;
        price = 0;
        itemCount = 1;
        Tag = new item()
        {
            itemCount = itemCount,
            Name = Name,
            price = price,
            Id = Id,
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
            Text = Name,
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
        middle.Text = Name;
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

class dbSubParent
{
    public int Id { get; set;}
    public string Name { get; set; }
    public int price { get; set; }
    public string chosenColour { get; set; }
    public int leadsToCategoryId { get; set; }
    
    
    
}

class dbSubChild : dbSubParent
{
    public int subItemOrder { get; set; }
    public int parentId { get; set; }
    public int subCatId { get; set; }
    public bool isLeaf { get; set; }
    public bool isRequired { get; set; }
}




class dbCategory
{
    public List<int> itemIds;
    public string catName;
    public int catId;
    public string catColour;

    public dbCategory()
    {
        catId = 0;
        catName = "";
        catColour = "";
        itemIds = new List<int>();
    }
}

public class orderLine : baseItem
{
    public int orderId;
    public int itemId;

}

public class baseItem
{
    public int Id;
    public string Name;
}

public class allergy : baseItem
{
    
}




public class dbSubCat : item
{
    public int parentId { get; set;}
    public bool isLeaf { get; set; }
    public bool isRequired { get; set; }
    
}

public class basicJunctionTable
{
    public List<int> leftIds; 
    public List<int> rightIds;
    public Dictionary<int, HashSet<int>> combined;
    public string tableName;
    public string leftCol;
    public string rightCol;
    private bool hasPopulated;
    private bool hasFinished;
    
    public basicJunctionTable(string TableName, string LeftCol, string RightCol)
    {
        tableName = TableName;
        leftCol = LeftCol;
        rightCol = RightCol;
        hasPopulated = false;
        hasFinished = false;
        if(leftCol != "" && rightCol != "") populateTable();
    }

    public void populateTable()
    {
        (leftIds, rightIds) = SQL.getJunctionTableValues(tableName, leftCol, rightCol);
        hasPopulated = true;
        updateCombined();  
    }

    private void updateCombined()
    {
        combined = new Dictionary<int, HashSet<int>>();

        for (int i = 0; i < leftIds.Count; i++)
        {
            int Id = leftIds[i];
            int allergyId = rightIds[i];

            if (!combined.TryGetValue(Id, out var set))
            {
                set = new HashSet<int>();
                combined[Id] = set;
            }

            set.Add(allergyId);
        }
    }
    
}

public class accessLevel : baseItem
{
    // still has Id
    public bool canSendThroughItems;
    public bool canDelete;
    public bool canNoSale;
    public bool canViewTables;

    public accessLevel()
    {
        canSendThroughItems = false;
        canDelete = false;
        canNoSale = false;
        canViewTables = false;
    }
}

public class header : baseItem
{
    public DateTime sentDateTime;
    public staff headerStaff;
    public int tableId;
    public int finished; // -1 not initialised, 0 not 1 finished 2 error

    public header()
    {
        sentDateTime = DateTime.Now;
        headerStaff = new()
        {
            Id = 0,
            Name = ""
        };
        tableId = 0;
        finished = -1;
    }

}

public class order : baseItem
{
    public int headerId;
    public header header;
    public List<orderLine> orderLines;
}
    