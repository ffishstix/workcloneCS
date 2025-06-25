using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
namespace WorkCloneCS;

public partial class Form1 : Form
{
    private int lineId = 1;
    public table tableSelected = new table();
    private staff currentStaff;        
    private List<catagory> cat = new();
    private List<item> currentlyDisplayedItems = new();
    private List<item> itemsToBeOrdered = new();

    public Form1()
    {
        cat = sync.catagories;
        Logger.Log("inside the Form1 constructor");
        InitializeComponent();
        Logger.Log("initialized components");
        InitFoodList();
        Logger.Log("initialized food list");
        Task.Run(async () => {
            await LoadCategories();

            // Use Invoke to update UI from background thread
            if (IsHandleCreated)
            {
                Invoke(new Action(() => {
                    addCatagory();
                    Logger.Log("added catagories");
                    
                }));
            }
        });
        
        Logger.Log("added catagories");
        Visible = true;
        Show();
    }
    private async Task LoadCategories()
    {
        // Create a TaskCompletionSource to wait for categories
        var tcs = new TaskCompletionSource<bool>();
        Logger.Log("inside syncAll just gonna run syncAll rn");

        // Wait until categories are loaded or timeout
        int attempts = 0;
        while (attempts < 10 && (sync.catagories == null || sync.catagories.Count < 10))
        {
            await Task.Delay(3000);
            sync.getFiles();
            cat = sync.catagories;
            attempts++;
        }

        if (cat != null && cat.Count > 0)
        {
            tcs.SetResult(true);
        }
        else
        {
            Logger.Log("Failed to load categories after timeout");
            tcs.SetResult(false);
        }

        await tcs.Task;
    }

    private void formClosing(object sender, FormClosingEventArgs e)
    {
        Application.Exit();
    }

    private void deleteChildbox() { if (catPan != null) catPan.Controls.Clear(); }

    private void addCatagory()
    {
        if (!IsHandleCreated) return;
        deleteChildbox();

        // Make sure we have the latest categories
        cat = sync.catagories;

        if (cat == null || cat.Count == 0)
        {
            Logger.Log("Categories list is null or empty");
            return;
        }

        try
        {
            foreach (var category in cat)
            {
                if (category != null)
                {
                    Label item = new Label
                    {
                        Text = category.catName,
                        AutoSize = false,
                        BackColor = Color.Gray,
                        Width = (catPan.Width / 8) - 2,
                        Height = 50,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Segoe UI", 12, FontStyle.Regular),
                        Margin = new Padding(1),
                        Tag = cat.IndexOf(category),
                        Visible = true
                    };
                    item.Click += catClick;
                    catPan.Controls.Add(item);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Log($"Error in addCatagory: {ex.Message}");
        }
    }

    private void catClick(object sender, EventArgs e)
    {
        deleteChildbox();
        InitItemList((int)((Control)sender).Tag);
    }

    private void addItem(item item)
    {
        priceTotal += item.price;
        int rowHeight = 40;
        int countLabelWidth = 30;
        int priceLabelWidth = 60;
        rowOfItem row = new rowOfItem()
        {
            
            itemName = item.itemName,
            price = item.price,
            itemCount = item.itemCount,
            itemId = item.itemId,
            chosenColour = item.chosenColour,
            extraInfo = item.extraInfo,
            
        };
        row.updateText();
        EnableSwipeToDelete(row);
        row.SetHeight(rowHeight);
        scrollPanel.Controls.Add(row.rowPannel);
        scrollPanel.VerticalScroll.Value = scrollPanel.VerticalScroll.Maximum;
        scrollPanel.PerformLayout();
    }

    private void EnableSwipeToDelete(rowOfItem row)
    {
        Point mouseDownLocation = Point.Empty;
        bool isDragging = false;

        row.Middle.MouseDown += (s, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDownLocation = e.Location;
                isDragging = true;
            }
        };

        row.Middle.MouseMove += (s, e) =>
        {
            if (!isDragging) return;

            int deltaX = e.X - mouseDownLocation.X;

            if (Math.Abs(deltaX) > 5)
                row.Middle.BackColor = Color.LightCoral;
        };

        row.Middle.MouseUp += (s, e) =>
        {
            FlowLayoutPanel rowPannel = (FlowLayoutPanel)row.Middle.Parent;
            if (!isDragging) return;
            isDragging = false;

            int deltaX = e.X - mouseDownLocation.X;
            if (rowPannel != null)
            {
                if (deltaX < -100 && rowPannel != null) // Swipe left threshold
                {
                    Label left = row.Left;
                    if (row.itemCount > 1)
                    {
                        row.itemCount--;
                        row.updateText();


                    }
                    else
                    {
                        itemsToBeOrdered.Remove(itemsToBeOrdered.Find(x => x.itemName == row.itemName));
                        refreshScrollPanel();
                    }
                    updateTotalPrice(-row.price);
                    updateTotalItems(-row.itemCount);
                }
                else if (deltaX > 100 && rowPannel != null)
                {
                    row.itemCount++;
                    row.updateText();
                    itemsToBeOrdered[itemsToBeOrdered.FindIndex(x => x.itemName == row.itemName)].itemCount = row.itemCount;
                    refreshScrollPanel();
                    updateTotalItems(1);
                    updateTotalPrice(row.price);
                    row.updateText();

                }

                row.Middle.BackColor = SystemColors.Control;
            }
            else
            {
                MessageBox.Show($"it is null here is the thing innit: {rowPannel.Controls}");
            }

        };

    }

    private void updateTotalItems(int number)
    {
        leftLabel.Tag = (int)leftLabel.Tag + number;
        leftLabel.Text = $"Items: {leftLabel.Tag}";
    }

    private void updateTotalPrice(decimal change)
    {
        decimal total = (decimal)rightLabel.Tag + change;
        rightLabel.Tag = (total);
        string t = total.ToString("c");
        rightLabel.Text = $"Price: {t}";
    }

    private void createScrollPanel()
    {
        scrollPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            Location = new Point(13, 43),
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = false,
            Padding = new Padding(0)
        };

        panel1.Controls.Add(scrollPanel);
    }
    
    private void InitFoodList()
    {
        createScrollPanel();
        

        // Sample data - to be changed
        leftLabel.Tag = 0;
        rightLabel.Tag = 0m;
        leftLabel.Dock = DockStyle.Left;
        rightLabel.Dock = DockStyle.Right;
        panel2.Controls.Add(leftLabel);
        panel2.Controls.Add(rightLabel);
    }

    private void deleteAllItemsOrdered()
    {
        itemsToBeOrdered.Clear();
        scrollPanel.Controls.Clear();
        leftLabel.Tag = 0;
        leftLabel.Text = "Items: 0";
        rightLabel.Tag = 0m;
        rightLabel.Text = "Price: 0.00";
    }

    private void generalItem_Click(object sender, EventArgs e)
    {
        item item = (item)((Control)sender).Tag;
        itemsToBeOrdered.Add(item);
        
        //updating middle row shizzle
        leftLabel.Tag = (int)leftLabel.Tag + 1;
        leftLabel.Text = leftLabel.Tag.ToString();
        updateTotalPrice(item.price);
        refreshScrollPanel();
    }

    private void refreshScrollPanel()
    {
        List<item> itemsCusIdkWhatIDid = itemsToBeOrdered;
        List<item> items = itemsToBeOrdered;
        itemsToBeOrdered = new List<item>();
        foreach (Control ctrl in scrollPanel.Controls)
        {
            if (ctrl is FlowLayoutPanel panel && panel.Tag is item t && t != null && t.lineId != null)
            {
                bool valid = false;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].lineId == t.lineId)
                    {
                        itemsToBeOrdered.Add(items[i]);
                        items.RemoveAt(i);
                        valid = true;
                        break;
                    }
                }

                if (!valid)
                {
                    scrollPanel.Controls.Remove(ctrl);
                }
            }
        }

        if (items != null)
        {
            foreach (item item in items)
            {
                itemsToBeOrdered.Add(item);
                addItem(item);
            }
        }
        
        itemsToBeOrdered = itemsCusIdkWhatIDid;
    }

    //in the bottom right
    private void backBtn_Click(object sender, EventArgs e)
    {
        deleteChildbox();


        addCatagory();
        allPannelsBlank();
    }


    //this happens when shit is open
    private void Form1_Load(object sender, EventArgs e)
    {

    }


    //called when catagory clicked on, gets items from file called "{catagoryName}".txt -
    //should probs change for an api call icl but you never know yk 
    // then just goes through each and adds each value and what not
    private void InitItemList(int e)
    {
        catagory cat = sync.catagories[e];
        List<item> foodItems = cat.items;
        if (foodItems != null)
        {
            for (int i = 0; i < foodItems.Count; i++)
            {
                try
                {
                    var d = foodItems[i];
                    d.lineId = lineId++;
                    addLabel(d);
                } catch (Exception ex)
                {
                    Logger.Log($"{ex.Message}    {e}");
                }
                
            }
        }
    }


    //add all of the items made clickeable that all take them to the gereralItem_Click()
    private void addLabel(item tag)
    {
        Color colour = Color.Gray;
        if (tag.chosenColour == "grey") colour = Color.Gray;
        else if (tag.chosenColour != null) colour = Color.FromName(tag.chosenColour);


            Label item = new Label
            {
                Text = tag.itemName,
                Tag = tag,
                AutoSize = false,
                BackColor = colour,
                Width = (catPan.Width / 8) - 2,
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Margin = new Padding(1)
            };

        item.Click += generalItem_Click;

        catPan.Controls.Add(item);
    }


    //for the config panels should always be called before toggling the visibility of a panel
    //probably useful for me to explain that this is a requirement else we could run into multiple 
    //click issues and other annoying shite
    private void allPannelsBlank()
    {
        ConfigPannel.Visible = false;
        finalPanel.Visible = false;
        tablePanel.Visible = false;
        orderPanel.Visible = false;
        miscPanel.Visible = false;
    }

    //the following 5 functions are to bring up user specific panels
    private void ConfigBtn_Click(object sender, EventArgs e)
    {
        bool temp = !ConfigPannel.Visible;
        allPannelsBlank();
        ConfigPannel.Visible = temp;
        ConfigPannel.BringToFront();

    }

    private void SignOffBtn_Click(object sender, EventArgs e)
    {
        currentStaff = null;
        nameBtn.Text = "name";
        nameBtn.Tag = currentStaff;
        deleteAllItemsOrdered();
        allPannelsBlank();
        addCatagory();
        
    }
    private void FinalBtn_Click(object sender, EventArgs e)
    {
        bool temp = !finalPanel.Visible;
        allPannelsBlank();
        finalPanel.Visible = temp;
        finalPanel.BringToFront();
    }

    private void tableBottomBtn_Click(object sender, EventArgs e)
    {
        bool temp = !tablePanel.Visible;
        allPannelsBlank();
        tablePanel.Visible = temp;
        tablePanel.BringToFront();
    }

    private void OrderBtn_Click(object sender, EventArgs e)
    {
        bool temp = !orderPanel.Visible;
        allPannelsBlank();
        orderPanel.Visible = temp;
        orderPanel.BringToFront();
        int totalHeight = flowLayoutPanel1.Height;
        int panel1Height;
        if (!temp)
        {
            panel1Height = 279;
            foreach (Control ctrl in scrollPanel.Controls)
            {
                if (ctrl is FlowLayoutPanel panel && panel.Tag is item t && t != null) // or check Name, Tag, etc.
                {
                    foreach (Control ctrl2 in panel.Controls)
                    {
                        if (ctrl2 is Label lbl && lbl.Name == $"foodLabel{t.itemCount}")
                        {
                            lbl.Width += 92;
                        }
                    }
                }
            }
        }


        else
        {
            panel1Height = 0;
            foreach (Control ctrl in scrollPanel.Controls)
            {
                if (ctrl is FlowLayoutPanel panel && panel.Tag is item t && t != null) // or check Name, Tag, etc.
                {
                    foreach (Control ctrl2 in panel.Controls)
                    {
                        if (ctrl2 is Label lbl && lbl.Name == $"foodLabel{t.itemCount}")
                        {
                            lbl.Width += 92;
                        }
                    }
                }
            }
        }

        int panel2Height = 33;
        int catpanHeight = totalHeight - panel2Height - panel1Height - 20;
        panel1.Height = catpanHeight;
        catPan.Height = panel1Height;




    }

    private void miscBtn_Click(object sender, EventArgs e)
    {
        bool temp = !miscPanel.Visible;
        allPannelsBlank();
        miscPanel.Visible = temp;
        miscPanel.BringToFront();

    }

    private void tableBtn_Click(object sender, EventArgs e)
    {
        if (tableSelected.tableId == 0)
        {
            //select table logic displaying if it is open currently
            //probs sql db of currently open tables with the items stored on it
            
            TableForm table = new TableForm();
            table.ShowDialog();
            if (table.tableSelected != 0)
            {
                tableSelected.tableId = table.tableSelected;
            }
            tableBtn.Text = $"Table {tableSelected.tableId}";
            return;
        }
        //this section runs
        sentToTable();
        refreshScrollPanel();
        
    }

    private void sentToTable()
    {
        tableSelected.ordered = itemsToBeOrdered;
        Logger.Log("sent through all items and cleared them");
        itemsToBeOrdered.Clear();
        refreshScrollPanel();
        leftLabel.Text = "Items: 0";
        leftLabel.Tag = 0;
        rightLabel.Text = "Price: 0.00";
        rightLabel.Tag = 0m;
        tableSelected = new table();
        tableBtn.Text = "Table";

        Logger.Log(
            "probs best to ignore the last one however i am now going to try and call the sql to inser the values, " +
            "still not sure what to do with no table number tbh");
        SQL.pushItemsToTables(tableSelected.tableId, currentStaff.Id, itemsToBeOrdered);
    }

    private void nameBtn_Click(object sender, EventArgs e)
    {
        NameForm name = new NameForm();
        if (name != null && ! name.IsDisposed)  name.ShowDialog();
        if (name.staffSelected == null)
        {
            currentStaff = new staff()
            {
                Id = 0,
                Name = "name",
                Access = 0
            };
            
        }
        else
        {
            currentStaff = name.staffSelected;
        }
        nameBtn.Tag = currentStaff;
        nameBtn.Text = currentStaff.Name.ToUpper();
    }

    private void PricingBtn_Click(object sender, EventArgs e)
    {
        MessageBox.Show("basically this is a thing but is honestly never used" +
            "\n in practice i have never ever pressed this button" +
            "\n and it is something you would only ever change on the tills anyways sooo" +
            "\n return not implemented lol");
    }

    private void CancelBtn_Click(object sender, EventArgs e)
    {
        deleteAllItemsOrdered();
        allPannelsBlank();
        addCatagory();
    }


    private async void ConfigSideBtn_Click(object sender, EventArgs e)
    {
        FirstRunWindow reLoad = new FirstRunWindow(true);
        reLoad.FormClosed += async (s, args) =>
        {
            try
            {
                allPannelsBlank();
                deleteChildbox();
                
                // Wait for sync to complete
                await Task.Run(async () =>
                {
                    sync.catagories = null;
                    sync.syncAll();
                    
                    // Wait for categories to be populated
                    int attempts = 0;
                    while (attempts < 10 && (sync.catagories == null || sync.catagories.Count == 0))
                    {
                        await Task.Delay(1000);
                        attempts++;
                        Logger.Log($"Waiting for categories, attempt {attempts}");
                    }
                });
                

                // Update UI on the main thread
                if (IsHandleCreated)
                {
                    Invoke(new Action(() =>
                    {
                        if (sync.catagories != null && sync.catagories.Count > 0)
                        {
                            addCatagory();
                            deleteAllItemsOrdered();
                        }
                        else
                        {
                            Logger.Log("Failed to load categories after config update");
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error in ConfigSideBtn_Click: {ex.Message}");
            }
        };
        Logger.Log("showed catagories");
        reLoad.Show();
    }


    }