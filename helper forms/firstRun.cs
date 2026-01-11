using System.Text.RegularExpressions;
namespace WorkCloneCS
{


    public partial class FirstRunWindow : Form
    {
        private readonly ConnectionSettingsValidator _validator = new ConnectionSettingsValidator();
        private string lastWorkingConnection;
        public string connectionString;
        private const int templatePort = 12346;
        private const int defaultPort = 1433;
        private bool t;
        public FirstRunWindow(bool te = false)
        {
            t = te;
            InitializeComponent();
            extraInitShit();
            
        }
        
        
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Apply_Click(object sender, EventArgs e)
        {
            Apply_Click_Code(sender, e);
        }

        private void CheckBtn_Click(object sender, EventArgs e)
        {
           CheckBtn_Click_Code(sender, e);
        }

        private void RemoveTempTextId(object sender, EventArgs e)
        {
            if (UserNameTextBox.Text == "*client")
            {
                UserNameTextBox.Text = "";
                UserNameTextBox.ForeColor = Color.Black;
            }
        }

        private void AddTempTextId(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(UserNameTextBox.Text))
            {
                UserNameTextBox.ForeColor = Color.Gray;
                UserNameTextBox.Text = "*client";
            }
        }

        private void RemoveTempTextIP(object sender, EventArgs e)
    {
        if (IPTextBox.Text == "*bd.fishstix.uk")
        {
            IPTextBox.Text = "";
            IPTextBox.ForeColor = Color.Black;
        }
    }

        private void AddTempTextIP(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(IPTextBox.Text))
            {
                IPTextBox.ForeColor = Color.Gray;
                IPTextBox.Text = "*bd.fishstix.uk";
            }
        }

        private void RemoveTempTextPort(object sender, EventArgs e)
        {
            if (PortTextBox.Text == $"*{templatePort}")
            {
                PortTextBox.Text = "";
                PortTextBox.ForeColor = Color.Black;
            }
        }

        private void AddTempTextPort(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(PortTextBox.Text))
            {
                PortTextBox.ForeColor = Color.Gray;
                PortTextBox.Text = $"*{templatePort}";
            }
        }

        private void RemoveTempTextDatabase(object sender, EventArgs e)
        {
            if (databaseTextBox.Text == "*workclonecs")
            {
                databaseTextBox.Text = "";
                databaseTextBox.ForeColor = Color.Black;
            }
        }

        private void AddTempTextDatabase(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(databaseTextBox.Text))
            {
                databaseTextBox.ForeColor = Color.Gray;
                databaseTextBox.Text = "*workclonecs";
            }
        }
        

        private void RemoveTempTextPassword(object sender, EventArgs e)
        {
            if (PasswordTextBox.Text == "*password")
            {
                PasswordTextBox.Text = "";
                PasswordTextBox.ForeColor = Color.Black;
            }
        }

        private void AddTempTextPassword(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordTextBox.Text))
            {
                PasswordTextBox.ForeColor = Color.Gray;
                PasswordTextBox.Text = "*password";
            }
        }


        private void LastBtn_Click(object sender, EventArgs e)
        {
            LastBtn_Click_Code(sender, e);
        }



    }
    

}