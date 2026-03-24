namespace WorkCloneCS;

public partial class Form1
{
    #region default code

    private void Form1_Load(object sender, EventArgs e)
    {
    }

    private void formClosing(object sender, FormClosingEventArgs e)
    {
        Application.Exit();
    }

    #endregion

    #region displayed categories code

    // Rebuilds category buttons in the category panel.
    private void addCategories()
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(addCategories));
            return;
        }

        if (!IsHandleCreated || IsDisposed) return;
        deleteChildbox();


        if (cat == null || cat.Count == 0)
        {
            Logger.Log("Categories list is null or empty (add category)");
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
                        BackColor = Color.FromName(category.catColour),
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


    private void deleteChildbox()
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(deleteChildbox));
            return;
        }

        if (catPan != null && !catPan.IsDisposed) catPan.Controls.Clear();
    }

    private void catClick(object sender, EventArgs e)
    {
        deleteChildbox();
        InitItemList((int)((Control)sender).Tag);
    }


    // Loads categories/allergies and updates category buttons on screen.
    private async Task LoadCategories()
    {
        var tcs = new TaskCompletionSource<bool>();

        cat = database.getCategories();
        availableAllergies = database.allergies?.Values.Select(a => a.Name).ToList() ?? new List<string>();
        alergies = alergies
            .Where(a => availableAllergies.Contains(a, StringComparer.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        Logger.Log("got categories");
        if (cat != null && cat.Count > 0)
        {
            addCategories();
            tcs.SetResult(true);
        }
        else
        {
            Logger.Log("Failed to load categories after timeout LoadCategories");
            tcs.SetResult(false);
        }


        await tcs.Task;
    }

    private void InitItemList(int e)
    {
        // Loads all items for the selected category and appends them to the UI.
        if (cat == null || e < 0 || e >= cat.Count)
        {
            Logger.Log("InitItemList called with invalid category index");
            return;
        }

        List<item> catItems = database.getCategoryItems(cat[e].catId);
        if (catItems != null)
        {
            for (int i = 0; i < catItems.Count; i++)
            {
                try
                {
                    item item = catItems[i];
                    item.lineId = lineId++;
                    addLabel(item);
                }
                catch (Exception ex)
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
            Label item = new Label
            {
                Text = tag.Name,
                Tag = tag,
                AutoSize = false,
                Width = (catPan.Width / 8) - 2,
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Margin = new Padding(1)
            };

            applyAllergyDisplay(item, tag);
            item.Click += generalItem_Click;

            catPan.Controls.Add(item);
        }
        catch (Exception ex)
        {
            Logger.Log($"got an error inside addLabel which is called by InitItemList: {ex.Message}");
        }
    }

    private Color getBaseItemColour(item tag)
    {
        if (string.IsNullOrWhiteSpace(tag.chosenColour) ||
            string.Equals(tag.chosenColour, "grey", StringComparison.OrdinalIgnoreCase))
            return Color.Gray;

        Color colour = Color.FromName(tag.chosenColour);
        return colour.A == 0 ? Color.Gray : colour;
    }

    private bool itemContainsSelectedAllergy(item tag)
    {
        if (alergies == null || alergies.Count == 0) return false;
        if (tag.allergies == null || tag.allergies.Count == 0) return false;

        HashSet<string> selected = new(alergies, StringComparer.OrdinalIgnoreCase);
        return tag.allergies.Any(a => a != null && !string.IsNullOrWhiteSpace(a.Name) && selected.Contains(a.Name));
    }

    private void applyAllergyDisplay(Label itemLabel, item tag)
    {
        bool hasSelectedAllergy = itemContainsSelectedAllergy(tag);
        itemLabel.BackColor = hasSelectedAllergy ? Color.DarkRed : getBaseItemColour(tag);
        allergyToolTip.SetToolTip(itemLabel, hasSelectedAllergy ? "item contains selected allergies" : null);
    }

    private void refreshVisibleItemAllergyStyles()
    {
        if (catPan == null || catPan.IsDisposed) return;

        foreach (Control control in catPan.Controls)
        {
            if (control is not Label label || label.Tag is not item tag) continue;
            applyAllergyDisplay(label, tag);
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

    private rowOfItem selectedRow = null;
    private readonly Dictionary<int, rowOfItem> rowsByLineId = new();
    private readonly Dictionary<FlowLayoutPanel, rowOfItem> rowsByPanel = new();

    private void selectRow(rowOfItem row)
    {
        if (selectedRow != null)
        {
            selectedRow.rowPannel.BackColor = SystemColors.Control;
            selectedRow.rowPannel.Padding = new Padding(0);
            selectedRow.rowPannel.Invalidate();
        }

        selectedRow = row;

        selectedRow.rowPannel.Padding = new Padding(2);
        selectedRow.rowPannel.BackColor = Color.LightBlue;
        selectedRow.rowPannel.Invalidate();
    }

    private int globalLineId = 0;

    // Adds one item row to the order list and wires selection/delete behavior.
    private void addItem(item item, bool autoScrollToBottom = true)
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
            lineId = item.lineId != 0 ? item.lineId : globalLineId++,
            chosenColour = item.chosenColour,
            extraInfo = item.extraInfo,
        };

        row.updateText();
        if (item.messages != null && item.messages.Count > 0)
        {
            foreach (string message in item.messages)
            {
                row.addMessage(message);
            }
        }

        if (!item.ordered) EnableSwipeToDelete(row);
        row.SetHeight(rowHeight);
        row.Middle.Click += (s, e) => selectRow(row);
        rowsByLineId[row.lineId] = row;
        rowsByPanel[row.rowPannel] = row;
        scrollPanel.SuspendLayout();
        scrollPanel.Controls.Add(row.rowPannel);
        if (autoScrollToBottom)
        {
            scrollPanel.VerticalScroll.Value = scrollPanel.VerticalScroll.Maximum;
        }

        scrollPanel.ResumeLayout();
    }


    // Enables horizontal drag gestures to decrement or increment a row quantity.
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
                        tableSelected.itemsToOrder.Remove(tableSelected.itemsToOrder.Find(x => x.lineId == row.lineId));
                        refreshScrollPanel();
                    }

                    updateTotalPrice(-row.price);
                    updateTotalItems(-1);
                }
                else if (deltaX > 100 && rowPannel != null)
                {
                    row.itemCount++;
                    row.updateText();
                    tableSelected.itemsToOrder[tableSelected.itemsToOrder.FindIndex(x => x.lineId == row.lineId)]
                        .itemCount = row.itemCount;
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
        tableSelected.ordered.Clear();
        selectedRow = null;
        rowsByLineId.Clear();
        rowsByPanel.Clear();
        scrollPanel.Controls.Clear();
        leftLabel.Tag = 0;
        leftLabel.Text = "Items: 0";
        rightLabel.Tag = 0m;
        rightLabel.Text = "Price: 0.00";
    }

    // Rebuilds the visible order list from ordered and queued table items.
    private void refreshScrollPanel()
    {
        if (scrollPanel == null || scrollPanel.IsDisposed)
        {
            createScrollPanel();
        }

        if (scrollPanel == null)
        {
            return;
        }

        FlowLayoutPanel currentScrollPanel = scrollPanel;
        currentScrollPanel.SuspendLayout();
        selectedRow = null;
        rowsByLineId.Clear();
        rowsByPanel.Clear();
        currentScrollPanel.Controls.Clear();

        if (tableSelected.ordered.Count != 0 || tableSelected.itemsToOrder.Count != 0)
        {
            if (tableSelected.ordered.Count != 0)
            {
                foreach (item existingItem in tableSelected.ordered)
                {
                    if (currentScrollPanel.InvokeRequired)
                        currentScrollPanel.Invoke((MethodInvoker)(() => addItem(existingItem, false)));
                    else addItem(existingItem, false);
                }
            }

            if (tableSelected.itemsToOrder.Count != 0)
            {
                foreach (item queuedItem in tableSelected.itemsToOrder)
                {
                    if (currentScrollPanel.InvokeRequired)
                        currentScrollPanel.Invoke((MethodInvoker)(() => addItem(queuedItem, false)));
                    else addItem(queuedItem, false);
                }
            }
        }

        currentScrollPanel.ResumeLayout();
        currentScrollPanel.PerformLayout();
    }

    private void textMsgBtn_Click_Code()
    {
        // Adds a message to the selected/latest item and persists it for ordered lines.
        item? targetItem = getMessageTargetItem();
        if (targetItem == null)
        {
            MessageBox.Show("No item available to attach a message to.");
            return;
        }

        string existingText = targetItem.messages != null && targetItem.messages.Count > 0
            ? targetItem.messages[^1]
            : string.Empty;
        using MessageForm messageForm = new MessageForm("Item Message", existingText);
        if (messageForm.ShowDialog(this) != DialogResult.OK) return;

        string newMessage = messageForm.MessageText.Trim();
        if (string.IsNullOrWhiteSpace(newMessage)) return;

        targetItem.messages ??= new List<string>();
        targetItem.messages.Add(newMessage);

        if (rowsByLineId.TryGetValue(targetItem.lineId, out rowOfItem? row))
        {
            row.addMessage(newMessage);
        }

        if (targetItem.ordered && targetItem.lineId > 0)
        {
            SQL.updateOrderLineMessage(targetItem.lineId, targetItem.messages);
        }
    }

    // Chooses the best item target for message entry based on selection and recency.
    private item? getMessageTargetItem()
    {
        if (selectedRow != null)
        {
            item? selectedItem = findItemByLineId(selectedRow.lineId);
            if (selectedItem != null) return selectedItem;
        }

        rowOfItem? lastVisibleRow = getLastVisibleRow();
        if (lastVisibleRow != null)
        {
            item? lastVisibleItem = findItemByLineId(lastVisibleRow.lineId);
            if (lastVisibleItem != null) return lastVisibleItem;
        }

        if (tableSelected.itemsToOrder.Count > 0) return tableSelected.itemsToOrder[^1];
        if (tableSelected.ordered.Count > 0) return tableSelected.ordered[^1];
        return null;
    }

    private rowOfItem? getLastVisibleRow()
    {
        if (scrollPanel == null || scrollPanel.IsDisposed || scrollPanel.Controls.Count == 0) return null;

        for (int i = scrollPanel.Controls.Count - 1; i >= 0; i--)
        {
            if (scrollPanel.Controls[i] is FlowLayoutPanel panel &&
                rowsByPanel.TryGetValue(panel, out rowOfItem? row))
                return row;
        }

        return null;
    }

    private item? findItemByLineId(int lineId)
    {
        item? queued = tableSelected.itemsToOrder.FirstOrDefault(x => x.lineId == lineId);
        if (queued != null) return queued;
        return tableSelected.ordered.FirstOrDefault(x => x.lineId == lineId);
    }

    private int maximiseSelectPanel()
    {
        // Expands item-row labels when collapsing category panel space.
        int panel1Height = 279;
        foreach (Control ctrl in scrollPanel.Controls)
        {
            if (ctrl is FlowLayoutPanel panel && panel.Tag is item t && t != null)
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
        // Restores category panel space while keeping row label widths aligned.
        int panel1Height = 0;
        foreach (Control ctrl in scrollPanel.Controls)
        {
            if (ctrl is FlowLayoutPanel panel && panel.Tag is item t && t != null)
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

    // Opens table selection or sends queued items depending on current table state.
    private void tableBtn_Click_Code(object sender, EventArgs e)
    {
        if (currentStaff == null || currentStaff.Id == new staff().Id)
        {
            Logger.Log("staff is null inside tableBtn_Click_Code");
            NameForm name = new();
            name.ShowDialog();

            updateCurrentStaff(name.staffSelected);
        }

        if (currentStaff == null || currentStaff.Id == 0) return;
        if (currentStaff.Id != 0 && tableSelected != null)
        {
            if (tableSelected.tableId == 0)
            {
                TableForm table = new TableForm();
                table.ShowDialog();
                if (table.tableSelected != 0)
                {
                    tableSelected.tableId = table.tableSelected;
                    tableSelected.openStaff = currentStaff;
                    tableBtn.Text = $"Table {tableSelected.tableId}";
                    List<item> items =
                        database.getOpenTableItemsFromSqlAndUpdateLocal(tableSelected.tableId, currentStaff);
                    if (items != null)
                    {
                        tableSelected.ordered = items;
                        refreshScrollPanel();
                    }
                    else Logger.Log("empty table loaded tableBtn_Click_Code");
                }

                return;
            }

            if (tableSelected.itemsToOrder != null)
            {
                sendToTable();
                refreshScrollPanel();
                return;
            }

            Logger.Log(
                "had a table open and then closed it but didnt send any orders through so just continuing anyways");
        }
        else
        {
            MessageBox.Show("No Staff selected");
            Logger.Log("pressed table button without being logged in please log in to continue");
        }
    }

    private void updateCurrentStaff(staff staff)
    {
        // Sets current user context and refreshes name button state.
        if (staff == new staff())
        {
            nameBtn.Text = "Name";
            nameBtn.Tag = new staff();
            return;
        }

        if (staff.staffAccess == new accessLevel()) staff.staffAccess = database.getAccessLevelFromId(staff.Id);
        currentStaff = staff;
        nameBtn.Text = staff.Name;
        nameBtn.Tag = staff;
    }

    // Sends queued items to SQL, mirrors them locally, then resets the current table draft.
    private void sendToTable()
    {
        if (tableSelected == null || tableSelected.itemsToOrder == null || tableSelected.itemsToOrder.Count == 0)
        {
            Logger.Log("sendToTable() called with no queued items.");
            return;
        }

        if (currentStaff == null || currentStaff.Id == 0)
        {
            Logger.Log("sendToTable() aborted because no staff is selected.");
            return;
        }

        int headerId = Math.Max(1, SQL.getHighestidFromTable("headers") + 1);
        int orderId = Math.Max(1, SQL.getHighestidFromTable("orders") + 1);
        int lineId = Math.Max(1, SQL.getHighestidFromTable("orderLine") + 1);

        SQL.pushItemsToTables(tableSelected, currentStaff, headerId, orderId, lineId);
        database.addTableOrder(tableSelected, currentStaff);

        refreshScrollPanel();
        leftLabel.Text = "Items: 0";
        leftLabel.Tag = 0;
        rightLabel.Text = "Price: 0.00";
        rightLabel.Tag = 0m;

        tableSelected = new table();
        tableBtn.Text = "Table";
        Logger.Log("sent through all items and cleared them");
    }

    private void nameBtn_Click_Code(object sender, EventArgs e)
    {
        NameForm name = new NameForm();
        if (name != null && !name.IsDisposed) name.ShowDialog();
        updateCurrentStaff(name.staffSelected);


        Logger.Log($"user: {currentStaff.Name} just logged in with id: {currentStaff.Id}");
    }

    // Reopens setup and reloads local category/allergy state after config changes.
    private void ConfigSideBtn_Click_Code(object sender, EventArgs e)
    {
        FirstRunWindow reLoad = new FirstRunWindow(true);
        reLoad.FormClosed += async (s, args) =>
        {
            try
            {
                allPannelsBlank();
                deleteChildbox();

                // Wait for local database to load
                await Task.Run(() =>
                {
                    database.tryLoadLocalDatabase();
                    cat = database.getCategories();
                    availableAllergies = database.allergies?.Values.Select(a => a.Name).ToList() ?? new List<string>();
                    alergies = alergies
                        .Where(a => availableAllergies.Contains(a, StringComparer.OrdinalIgnoreCase))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
                });


                // Update UI on the main thread
                if (IsHandleCreated)
                {
                    Invoke(new Action(() =>
                    {
                        if (cat != null && cat.Count > 0)
                        {
                            addCategories();
                            deleteAllItemsOrdered();
                        }
                        else
                        {
                            Logger.Log("Failed to load categories after config update ConfigSideBtn_Click_Code");
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