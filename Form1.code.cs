namespace WorkCloneCS;

public partial class Form1
{
    
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
    
    private async Task LoadCategories()
    {
        // Create a TaskCompletionSource to wait for categories
        var tcs = new TaskCompletionSource<bool>();

        // Wait until categories are loaded or timeout
        sync.getFiles();
        cat = sync.catagories;
        if (cat != null && cat.Count > 0)
        {
            tcs.SetResult(true);
        }
        else
        {
            Logger.Log("Failed to load categories after timeout");
            tcs.SetResult(false);
        }
        addCatagory();
        await tcs.Task;
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
                        itemsToBeOrdered.Remove(itemsToBeOrdered.Find(x => x.itemName == row.itemName));
                        refreshScrollPanel();
                    }
                    updateTotalPrice(-row.price);
                    updateTotalItems(-1);
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
        tableBtn.Text = "Table";
        itemsToBeOrdered.Clear();
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
            if (itemsToBeOrdered.Count != 0) foreach (item itemt in itemsToBeOrdered) combinedItems.Append(itemt);
            
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
                    item item = foodItems[i];
                    item.lineId = lineId++;
                    addLabel(item);
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

    private void orderBtn_Click_Code(object sender, EventArgs e)
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

    private void tableBtn_Click_Code(object sender, EventArgs e)
    {
        
        if ((currentStaff != null || currentStaff.Id != 0) && tableSelected != null)
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
                    else Logger.Log("empty table loaded");
               
                }
            
                return;
            }

            if (itemsToBeOrdered != null)
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
        tableSelected.itemsToOrder = itemsToBeOrdered;
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
        itemsToBeOrdered.Clear();
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