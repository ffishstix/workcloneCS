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
    public int tableId { get; set; }
    public List<item> ordered { get; set; }

    public table()
    {
        tableId = 0;
        ordered = new List<item>();
    }
}


public class staff
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Access { get; set; }
}

class catagory
{
    // this is purely for purposes such as how in the locally saved files 
    // there might not be certain properties from the database so i am just putting this here so i can 
    public bool connected { get; set; }
    public string catName { get; set; }
    public int catagoryId { get; set; }
    public string catagoryExtraInfo { get; set; }

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
}

public class rowPanelTag
{
    private string name;
    private int count;
    private decimal price;
    private int itemCount;
    public rowPanelTag()
    {
        name = "changeMe";
        count = 1;
        price = 0;
        itemCount = 1;
    }
    public string Name { get { return name; } set { name = value; } }
    public int Count { get { return count; } set { count = value; } }
    public decimal Price { get { return price; } set { price = value; } }
    public int ItemCount { get { return itemCount; } set { itemCount += value; } }
    public decimal TotalPrice
    {
        get { return itemCount * Price; }
    }

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
            lineId = lineId

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


