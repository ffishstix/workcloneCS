using System.ComponentModel.DataAnnotations.Schema;

namespace WorkCloneCS;

public partial class Form1
{
    #region default code
    // <summary>
    // this is the default generated code with each form and is not important
    // </summary>
    private void Form1_Load(object sender, EventArgs e)
    {

    }
    private void formClosing(object sender, FormClosingEventArgs e)
    {
        Application.Exit();
    }

    #endregion

    #region displayed categories code

    private void addCategories()
    {
        if (!IsHandleCreated) return;
        deleteChildbox();

        // Make sure we have the latest categories
        cat = database.getCategories();

        if (cat == null || cat.Count == 0)
        {
            Logger.Log("Categories list is null or empty (add category)");
            return;
        }
        
        try
        {
            // sort the categories using a specific sort to get exta marks...... there is a column in the table that tells you which order they want to be displayed in, sort the categories based on that. /////
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
            Logger.Log($"Error in addCategory: {ex.Message}");
        }
    }


    private void deleteChildbox() { if (catPan != null) catPan.Controls.Clear(); }
    
    private void catClick(object sender, EventArgs e)
    {
        deleteChildbox();
        InitItemList((int)((Control)sender).Tag);
    }
    
    
    
    private async Task LoadCategories()
    {
        // Create a TaskCompletionSource to wait for categories
        var tcs = new TaskCompletionSource<bool>();

        // Wait until categories are loaded or timeout
        sync.getFiles();
        cat = database.getCategories();
        if (cat != null && cat.Count > 0)
        {
            tcs.SetResult(true);
        }
        else
        {
            Logger.Log("Failed to load categories after timeout LoadCategories");
            tcs.SetResult(false);
        }
        addCategories();
        await tcs.Task;
    }

    private void InitItemList(int e)
    {
        List<item> catItems = sync.categories[e].items;
        if (catItems != null)
        {
            for (int i = 0; i < catItems.Count; i++)
            {
                try
                {
                    item item = catItems[i];
                    item.lineId = lineId++;
                    addLabel(item);
                } catch (Exception ex)
                {
                    Logger.Log($"error in InitItemList: {ex.Message}    {e}");
                }
                
            }
        }
    }
    
    private void addLabel(item tag)
    {
        try
        {
            Color colour = Color.Gray;
            if (tag.chosenColour == "grey") colour = Color.Gray;
            else if (tag.chosenColour != null) colour = Color.FromName(tag.chosenColour);
            if (alergies != null && tag.allergies != null)
            {
                var tagAllergyNames = new HashSet<string>(
                    tag.allergies.Select(a => a.Name),
                    StringComparer.OrdinalIgnoreCase
                );

                foreach (string s in alergies)
                {
                    if (tagAllergyNames.Contains(s))
                    {
                        colour = Color.DarkRed;
                        break;
                    }
                }
            }

            Label item = new Label
            {
                Text = tag.Name,
                Tag = tag,
                AutoSize = false,
                BackColor = colour,
                Width = (catPan.Width / 8) - 2,
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Margin = new Padding(1)
            
            };
            allergyToolTip.SetToolTip(item, "item contains selected allergies");
            item.Click += generalItem_Click;

            catPan.Controls.Add(item);
        }
        catch (Exception ex)
        {
            Logger.Log($"got an error inside addLabel which is called by InitItemList: {ex.Message}");
        }
        
    }
    
    #endregion
    
    
    
    

    private void allPannelsBlank()
    {
        
        ConfigPannel.Visible = false;
        finalPanel.Visible = false;
        tablePanel.Visible = false;
        orderPanel.Visible = false;
        miscPanel.Visible = false;
    }

    
    

    private void addItem(item item)
    {
        priceTotal += item.price;
        int rowHeight = 40;
        int countLabelWidth = 30;
        int priceLabelWidth = 60;
        rowOfItem row = new rowOfItem()
        {
            
            Name = item.Name,
            price = item.price,
            itemCount = item.itemCount,
            Id = item.Id,
            chosenColour = item.chosenColour,
            extraInfo = item.extraInfo,
        };
        
        row.updateText();
        if (! item.ordered) EnableSwipeToDelete(row);
        row.SetHeight(rowHeight);
        scrollPanel.SuspendLayout();
        scrollPanel.Controls.Add(row.rowPannel);
        scrollPanel.VerticalScroll.Value = scrollPanel.VerticalScroll.Maximum;
        scrollPanel.ResumeLayout();
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
                        tableSelected.itemsToOrder.Remove(tableSelected.itemsToOrder.Find(x => x.Name == row.Name));
                        refreshScrollPanel();
                    }
                    updateTotalPrice(-row.price);
                    updateTotalItems(-1);
                }
                else if (deltaX > 100 && rowPannel != null)
                {
                    row.itemCount++;
                    row.updateText();
                    tableSelected.itemsToOrder[tableSelected.itemsToOrder.FindIndex(x => x.Name == row.Name)-1].itemCount = row.itemCount;
                    //used to have refresh scoll pannel here lol absolutely no need
                    updateTotalItems(1);
                    updateTotalPrice(row.price);


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
        tableBtn.Text = "Table";
        tableSelected.itemsToOrder.Clear();
        scrollPanel.Controls.Clear();
        leftLabel.Tag = 0;
        leftLabel.Text = "Items: 0";
        rightLabel.Tag = 0m;
        rightLabel.Text = "Price: 0.00";
    }
    
    private void refreshScrollPanel()
    {
        foreach (Control ctrl in panel1.Controls) ctrl.Dispose();
        createScrollPanel();
        if (tableSelected.ordered.Count != 0)
        {
            List<item> combinedItems = tableSelected.ordered;
            if (tableSelected.itemsToOrder.Count != 0) foreach (item itemt in tableSelected.itemsToOrder) combinedItems.Append(itemt);
            
            if (combinedItems != null)
            {
                
                foreach (item item in combinedItems)
                {
                    if (scrollPanel.InvokeRequired) scrollPanel.Invoke((MethodInvoker)(() => addItem(item)));
                    else addItem(item);
                }
                scrollPanel.PerformLayout();
            }
        }
        
    }
    
    


    //add all of the items made clickeable that all take them to the gereralItem_Click()
    

    private int maximiseSelectPanel()
    {
        int panel1Height = 279;
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
        return panel1Height;
    }

    private int minimiseSelectPanel()
    {
        int panel1Height = 0;
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
        return panel1Height;
    }

    private void finallyPanelCode(int panel1Height)
    {
        int totalHeight = flowLayoutPanel1.Height;
        int panel2Height = 33;
        int catpanHeight = totalHeight - panel2Height - panel1Height - 20;
        panel1.Height = catpanHeight;
        catPan.Height = panel1Height;
    }
    
    private void orderBtn_Click_Code(object sender, EventArgs e)
    {
        bool temp = orderPanel.Visible;
        allPannelsBlank();
        orderPanel.Visible = !temp;
        orderPanel.BringToFront();
        int panel1Height;
        
        if (temp) panel1Height = maximiseSelectPanel();

        else panel1Height = minimiseSelectPanel();


        finallyPanelCode(panel1Height);

    }

    private void tableBtn_Click_Code(object sender, EventArgs e)
    {
        if (currentStaff == null) Logger.Log("staff is null inside tableBtn_Click_Code");
        if (tableSelected == null) Logger.Log("tableSelected is null inside tableBtn_Click_Code");
        if (currentStaff.Id != 0 && tableSelected != null)
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
                    tableSelected.openStaff = currentStaff;
                    tableBtn.Text = $"Table {tableSelected.tableId}";
                    List<item> items = SQL.getTableItems(tableSelected.tableId); //pulls all items on a table
                    if (items != null)
                    {
                        tableSelected.ordered = items;
                        foreach (item item in items)
                        {
                            addItem(item);
                        }
                    }  
                    else Logger.Log("empty table loaded tableBtn_Click_Code");
               
                }
            
                return;
            }

            if (tableSelected.itemsToOrder != null)
            {
                sentToTable();
                refreshScrollPanel();
                return;
            }
            Logger.Log("had a table open and then closed it but didnt send any orders through so just continuing anyways");
        }
        else
        {
            MessageBox.Show("No Staff selected");
            Logger.Log("pressed table button without being logged in please log in to continue");
        }

    }
    
    private void sentToTable()
    {
        tableSelected.itemsToOrder = tableSelected.itemsToOrder;
        Logger.Log("sent through all items and cleared them");
        refreshScrollPanel();
        leftLabel.Text = "Items: 0";
        leftLabel.Tag = 0;
        rightLabel.Text = "Price: £0.00";
        rightLabel.Tag = 0m;
        Logger.Log(
            "probs best to ignore the last one however i am now going to try and call the sql to inser the values, " +
            "still not sure what to do with no table number tbh");
        int headerId = SQL.getHighestidFromTable("headers") + 1;
        int orderId = SQL.getHighestidFromTable("orders") + 1;
        int lineId = SQL.getHighestidFromTable("orderLine") + 1;
        SQL.pushItemsToTables(tableSelected, currentStaff, headerId, orderId, lineId);
        tableSelected = new table();
        tableBtn.Text = "Table";
        tableSelected.itemsToOrder.Clear();
    }

    private void nameBtn_Click_Code(object sender, EventArgs e)
    {
        NameForm name = new NameForm();
        if (name != null && ! name.IsDisposed)  name.ShowDialog();
        if (name.staffSelected == null)
        {
            currentStaff = new staff()
            {
                Id = 0,
                Name = "name",
                staffAccess = new()
                {
                    Id = 0
                }
            };
            
        }
        else
        {
            currentStaff = name.staffSelected;
        }
        nameBtn.Tag = currentStaff;
        nameBtn.Text = currentStaff.Name.ToUpper();
        
        Logger.Log($"user: {currentStaff.Name} just logged in with id: {currentStaff.Id}");
        
    }

    private void ConfigSideBtn_Click_Code(object sender, EventArgs e)
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
                    sync.categories = null;
                    sync.syncAll();
                    
                    // Wait for categories to be populated
                    int attempts = 0;
                    while (attempts < 10 && (sync.categories == null || sync.categories.Count == 0))
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
                        if (sync.categories != null && sync.categories.Count > 0)
                        {
                            addCategories();
                            deleteAllItemsOrdered();
                        }
                        else
                        {
                            Logger.Log("Failed to load categories after config update ConfigSideBtn_Click_Code" );
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error in ConfigSideBtn_Click: {ex.Message}");
            }
        };
        Logger.Log("showed categories");
        reLoad.Show();
    }

    
    
}


