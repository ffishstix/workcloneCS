using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Reflection;
namespace WorkCloneCS;

public partial class Form1 : Form
{
    private int lineId = 1;
    public table tableSelected = new table();
    private staff currentStaff;
    private List<catagory> cat = new();

    public Form1()
    {
        cat = sync.catagories;
        Logger.Log("inside the Form1 constructor");
        InitializeComponent();
        Logger.Log("initialized components");
        InitFoodList();
        Logger.Log("initialized food list");
        Task.Run(async () => {
            await LoadCategories(); });
        Logger.Log("started loading open tables");
        
        
        Logger.Log("added catagories");
        Visible = true;
        Show();
    }
    
    private void formClosing(object sender, FormClosingEventArgs e)
    {
        Application.Exit();
    }

    private void deleteChildbox() { if (catPan != null) catPan.Controls.Clear(); }

    private void catClick(object sender, EventArgs e)
    {
        deleteChildbox();
        InitItemList((int)((Control)sender).Tag);
    }

    
    
    
    private void generalItem_Click(object sender, EventArgs e)
    {
        item item = (item)((Control)sender).Tag;
        item.lineId = lineId++;
        tableSelected.itemsToOrder.Add(item);
        
        //updating middle row shizzle
        leftLabel.Tag = (int)leftLabel.Tag + 1;
        leftLabel.Text = leftLabel.Tag.ToString();
        updateTotalPrice(item.price);
        refreshScrollPanel();
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
        if (catPan.Height < 10)
        {
            int tempInt = maximiseSelectPanel();
            finallyPanelCode(tempInt);
        }
        bool temp = !ConfigPannel.Visible;
        allPannelsBlank();
        if (temp) addCatagory();
        ConfigPannel.Visible = temp;
        ConfigPannel.BringToFront();
        
    }

    private void SignOffBtn_Click(object sender, EventArgs e)
    {
        tableSelected = new table()
        {
            tableId = 0
        }; // currently not working and i honestly dont know why good luck future me ;0
        currentStaff = new staff()
        {
            Id = 24,
            Name = "fin"
        };
        nameBtn.Text = "name";
        nameBtn.Tag = currentStaff;
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
        tableBtn_Click_Code(sender, e);

    }

    private void AllergiesBtn_Click(object sender, EventArgs e)
    {
        invertInfoPanel();
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
        MessageBox.Show("basically this is a thing but is honestly never used" +
            "\n in practice i have never ever pressed this button" +
            "\n and it is something you would only ever change on the tills anyways sooo" +
            "\n return not implemented lol");
    }

    private void ConfigSideBtn_Click(object sender, EventArgs e)
    {
        ConfigSideBtn_Click_Code(sender, e);
    }

    private void syncBtn_Click(object sender, EventArgs e)
    {
        try
        {
            sync.syncAll();
            SignOffBtn_Click(null, null);
        }
        catch (Exception ex)
        {
            Logger.Log($"Error in syncBtn_Click: {ex.Message}");
        }
    }

    
    
    
    private void button1_Click(object sender, EventArgs e)
    {
        invertInfoPanel();
    }
}
