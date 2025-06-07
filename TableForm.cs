namespace WorkCloneCS;

public partial class TableForm : Form
{
    public int tableSelected = 0;
    public TableForm()
    {
        InitializeComponent();
        clickSection();
    }

    private void clickSection()
    {
        // For buttons 1-98
        for (int i = 1; i <= 87; i++)
        {
            var button = (Button)this.Controls.Find($"button{i}", true)[0];
            button.Click += tableBtn_Click;
        }

        for (int i = 90; i <= 98; i++)
        {
            var button = (Button)this.Controls.Find($"button{i}", true)[0];
            button.Click += tableBtn_Click;
        }
        button300.Click += tableBtn_Click;
        button500.Click += tableBtn_Click;
        button700.Click += tableBtn_Click;
        button800.Click += tableBtn_Click;
    }
    private void tableBtn_Click(object sender, EventArgs e)
    {
        tableSelected = int.Parse(((Button)sender).Text);
        Close();
    }
}
