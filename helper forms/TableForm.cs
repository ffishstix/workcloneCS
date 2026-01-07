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
