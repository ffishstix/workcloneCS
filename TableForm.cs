namespace WorkCloneCS;

public partial class TableForm : Form
{
    public int tableSelected;
    public TableForm()
    {
        InitializeComponent();
        tableSelected = 0;
        clickSection();
    }

    private void clickSection()
    {
        // For buttons 1-87
        for (int i = 1; i <= 87; i++)
        {
            var button = (Button)this.Controls.Find($"button{i}", true)[0];
            button.Click += tableBtn_Click;
            button.DoubleClick += tableBtn_DoubleClick;
        }

        for (int i = 90; i <= 98; i++)
        {
            var button = (Button)this.Controls.Find($"button{i}", true)[0];
            button.Click += tableBtn_Click;
            button.DoubleClick += tableBtn_DoubleClick;
        }
        button300.Click += tableBtn_Click;
        button500.Click += tableBtn_Click;
        button700.Click += tableBtn_Click;
        button800.Click += tableBtn_Click;
        button300.DoubleClick += tableBtn_DoubleClick;
        button500.DoubleClick += tableBtn_DoubleClick;
        button700.DoubleClick += tableBtn_DoubleClick;
        button800.DoubleClick += tableBtn_DoubleClick;
    }
    private void tableBtn_DoubleClick(object sender, EventArgs e)
    {
        tableSelected = int.Parse(((Button)sender).Text);
        Close();
    }

    private void tableBtn_Click(object sender, EventArgs e)
    {
        tableSelected = int.Parse(((Button)sender).Text);
    }
    
    private void escapeBtn_Click(object sender, EventArgs e)
    {
        tableSelected = 0;
        Close();
    }

    private void printBillBtn_Click(object sender, EventArgs e)
    {
        Logger.Log("printBillBtn clicked but i havent implemented it yet lol");
    }

    private void tableNumBtn_Click(object sender, EventArgs e)
    {
        tableNumberForm tableNumberForm = new tableNumberForm();
        tableNumberForm.ShowDialog();
        tableSelected = tableNumberForm.tableSelected;
        if (tableSelected != 0) Close();
    }

    private void openTableBtn_Click(object sender, EventArgs e)
    {
        if (tableSelected != 0) Close();
        Logger.Log("openTableBtn clicked but there is no table actually selected so not closing :)");
    }

    private void defaultTableBtn_Click(object sender, EventArgs e)
    {
        tableSelected = 4000;
        Close();
    }
}
