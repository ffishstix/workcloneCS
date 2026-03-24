using System.Reflection;

namespace WorkCloneCS;

public partial class Form1 : Form
{
    private List<string> alergies;
    private List<string> availableAllergies;
    private int lineId = 1;
    public table tableSelected = new();
    private staff currentStaff = new();
    private List<dbCategory> cat = new();

    // Loads local data, initializes UI, then starts async category rendering.
    public Form1()
    {
        alergies = new List<string>();
        availableAllergies = new List<string>();
        database.tryLoadLocalDatabase();
        availableAllergies = database.allergies?.Values.Select(a => a.Name).ToList() ?? new List<string>();

        cat = database.getCategories();
        Logger.Log("inside the Form1 constructor");
        InitializeComponent();
        textMsgBtn.Click += textMsgBtn_Click;
        Logger.Log("initialized components");
        InitFoodList();
        Logger.Log("initialized food list");
        Task.Run(async () => { await LoadCategories(); });
        Logger.Log("started loading open tables");
        Logger.Log("added categories");
        Visible = true;
        Show();
    }


    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        database.saveLocalDatabase(false);
    }

    private static item cloneItemForOrder(item source)
    {
        return new item
        {
            Id = source.Id,
            Name = source.Name,
            extraInfo = source.extraInfo,
            itemCount = source.itemCount > 0 ? source.itemCount : 1,
            price = source.price,
            chosenColour = source.chosenColour,
            ordered = false,
            hasSubItems = source.hasSubItems,
            allergies = source.allergies != null ? new List<allergy>(source.allergies) : new List<allergy>(),
            subItems = source.subItems != null ? new List<dbSubCat>(source.subItems) : new List<dbSubCat>(),
            messages = source.messages != null ? new List<string>(source.messages) : new List<string>()
        };
    }


    private void generalItem_Click(object sender, EventArgs e)
    {
        // Queues the clicked item for the current table and updates totals immediately.
        if (currentStaff == new staff() || currentStaff.Id == 0)
        {
            NameForm name = new();
            name.ShowDialog();
            updateCurrentStaff(name.staffSelected);
        }

        if (currentStaff == new staff() || currentStaff.Id == 0)
        {
            MessageBox.Show("you need to log in to select an item");
            return;
        }

        if (!currentStaff.staffAccess.canSendThroughItems)
        {
            MessageBox.Show("you do not have the required permissions to send through items");
            return;
        }

        if (sender is not Control clickedControl || clickedControl.Tag is not item selectedItem)
        {
            Logger.Log("generalItem_Click called without a valid item tag");
            return;
        }

        item queuedItem = cloneItemForOrder(selectedItem);
        queuedItem.lineId = lineId++;
        tableSelected.itemsToOrder.Add(queuedItem);

        // Append the new row directly; no full panel rebuild needed.
        addItem(queuedItem);
        updateTotalItems(1);
        updateTotalPrice(queuedItem.price);
    }

    private void backBtn_Click(object sender, EventArgs e)
    {
        deleteChildbox();
        addCategories();
        allPannelsBlank();
    }


    private void ConfigBtn_Click(object sender, EventArgs e)
    {
        if (catPan.Height < 10)
        {
            int tempInt = maximiseSelectPanel();
            finallyPanelCode(tempInt);
        }

        bool temp = !ConfigPannel.Visible;
        allPannelsBlank();
        if (temp) addCategories();
        ConfigPannel.Visible = temp;
        ConfigPannel.BringToFront();
    }

    private void SignOffBtn_Click(object sender, EventArgs e)
    {
        tableSelected = new table();
        updateCurrentStaff(new staff());
        deleteAllItemsOrdered();
        allPannelsBlank();
        LoadCategories();
    }

    private void FinalBtn_Click(object sender, EventArgs e)
    {
        if (catPan.Height < 10)
        {
            int tempInt = maximiseSelectPanel();
            finallyPanelCode(tempInt);
        }

        bool temp = !finalPanel.Visible;
        allPannelsBlank();
        finalPanel.Visible = temp;
        finalPanel.BringToFront();
    }

    private void tableBottomBtn_Click(object sender, EventArgs e)
    {
        if (catPan.Height < 10)
        {
            int tempInt = maximiseSelectPanel();
            finallyPanelCode(tempInt);
        }

        bool temp = !tablePanel.Visible;
        allPannelsBlank();
        tablePanel.Visible = temp;
        tablePanel.BringToFront();
    }

    private void OrderBtn_Click(object sender, EventArgs e)
    {
        orderBtn_Click_Code(sender, e);
    }

    private void miscBtn_Click(object sender, EventArgs e)
    {
        if (catPan.Height < 10)
        {
            int tempInt = maximiseSelectPanel();
            finallyPanelCode(tempInt);
        }

        bool temp = !miscPanel.Visible;
        allPannelsBlank();
        miscPanel.Visible = temp;
        miscPanel.BringToFront();
    }

    private void tableBtn_Click(object sender, EventArgs e)
    {
        if (database.isConnectedToRemoteServer) tableBtn_Click_Code(sender, e);
        else
        {
            Logger.Log("you are not connected to the database and so you cannot access tabnles");
            MessageBox.Show("you are not connected to the database please contact the IT admin");
        }
    }

    private void AllergiesBtn_Click(object sender, EventArgs e)
    {
        allergiesForm al = new allergiesForm(availableAllergies, alergies);
        al.ShowDialog();
        alergies = al.SelectedAllergies;
        refreshVisibleItemAllergyStyles();
    }

    private void textMsgBtn_Click(object sender, EventArgs e)
    {
        textMsgBtn_Click_Code();
    }

    private void infoBtn_Click(object sender, EventArgs e)
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        Logger.Log($"Running version: {version}");
    }

    private void nameBtn_Click(object sender, EventArgs e)
    {
        nameBtn_Click_Code(sender, e);
    }

    private void PricingBtn_Click(object sender, EventArgs e)
    {
        MessageBox.Show("unfortunately this is not developed so far");
    }

    private void ConfigSideBtn_Click(object sender, EventArgs e)
    {
        ConfigSideBtn_Click_Code(sender, e);
    }

    private void syncBtn_Click(object sender, EventArgs e)
    {
        try
        {
            database.pullCloudDatabase();
            SignOffBtn_Click(null, null);
        }
        catch (Exception ex)
        {
            Logger.Log($"Error in syncBtn_Click: {ex.Message}");
        }
    }

    private void paymentBtn_Click(object sender, EventArgs e)
    {
        // Marks the table as paid in SQL, mirrors closure locally, then signs off.
        bool payWithCard = ((Button)sender).Text.ToLower() == "card"; // not used
        Logger.Log("user tried to send through a payment but this is not within scope" +
                   "\nclearing items off table now");

        if (tableSelected == null || tableSelected.tableId <= 0)
        {
            Logger.Log("paymentBtn_Click called with no table selected");
            MessageBox.Show("Please select a table before taking payment.");
            return;
        }

        int tableId = tableSelected.tableId;
        int rowsAffected = SQL.updateHeadersFinishedForTable(tableId, 2);
        Logger.Log(
            $"paymentBtn_Click updated finished=2 for table {tableId}. Rows affected: {rowsAffected}");

        if (rowsAffected <= 0)
        {
            Logger.Log($"paymentBtn_Click did not clear table {tableId} locally because SQL update returned 0.");
            MessageBox.Show("Payment update failed in the database. Table was not cleared locally.");
            return;
        }

        int localHeadersUpdated = database.closeTableLocally(tableId, 2);
        Logger.Log(
            $"paymentBtn_Click closed local table cache for table {tableId}. Local headers updated: {localHeadersUpdated}");

        SignOffBtn_Click(null, null);
    }
}
