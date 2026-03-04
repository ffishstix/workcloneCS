using System.Reflection;

namespace WorkCloneCS;

public partial class Form1 : Form
{
    private List<string> alergies;
    private int lineId = 1;
    public table tableSelected = new();
    private staff currentStaff = new();
    private List<dbCategory> cat = new();

    public Form1()
    {
        alergies = new List<string>();
        database.tryLoadLocalDatabase();
        if (database.allergies != null)
        {
            foreach (allergy al in database.allergies.Values) alergies.Add(al.Name);
        }

        cat = database.getCategories();
        Logger.Log("inside the Form1 constructor");
        InitializeComponent();
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
            subItems = source.subItems != null ? new List<dbSubCat>(source.subItems) : new List<dbSubCat>()
        };
    }


    private void generalItem_Click(object sender, EventArgs e)
    {
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
            MessageBox.Show("you do not have the required permissions to send through items")
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

    //in the bottom right
    private void backBtn_Click(object sender, EventArgs e)
    {
        deleteChildbox();
        addCategories();
        allPannelsBlank();
    }


    //<summary>
    //called when category clicked on, gets items from file called "{categoryName}".txt -
    //should probs change for an api call icl but you never know yk 
    // then just goes through each and adds each value and what not
    //
    //
    //for the config panels should always be called before toggling the visibility of a panel
    //probably useful for me to explain that this is a requirement else we could run into multiple 
    //click issues and other annoying stuff
    //
    //the following 5 functions are to bring up user specific panels
    //</summary>
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
        tableSelected = new table(); // currently not working and i honestly dont know why good luck future me ;0
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
        allergiesForm al = new allergiesForm();
        al.ShowDialog();
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
            database.tryLoadLocalDatabase();
            SignOffBtn_Click(null, null);
        }
        catch (Exception ex)
        {
            Logger.Log($"Error in syncBtn_Click: {ex.Message}");
        }
    }
}