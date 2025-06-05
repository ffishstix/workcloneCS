using System.Text.Json;
using Microsoft.Data.SqlClient;
namespace WorkCloneCS
{


    public partial class FirstRunWindow : Form
    {
        private string lastWorkingConnection;
        public string connectionString;
        public FirstRunWindow()
        {
            InitializeComponent();
            progressBar = new ProgressBar
            {
                Location = new Point(95, 157), // Adjust location as needed
                Name = "progressBar",
                Size = new Size(177, 23),
                Style = ProgressBarStyle.Continuous
            };

            Logger.Log("first run window created");
            IPTextBox.GotFocus += RemoveTempTextIP;
            IPTextBox.LostFocus += AddTempTextIP;
        
            PortTextBox.GotFocus += RemoveTempTextPort;
            PortTextBox.LostFocus += AddTempTextPort;
        
            databaseTextBox.GotFocus += RemoveTempTextDatabase;
            databaseTextBox.LostFocus += AddTempTextDatabase;
        
            UserNameTextBox.GotFocus += RemoveTempTextId;
            UserNameTextBox.LostFocus += AddTempTextId;
        
            PasswordTextBox.GotFocus += RemoveTempTextPassword;
            PasswordTextBox.LostFocus += AddTempTextPassword;
            
            AddTempTextDatabase(null, null);
            AddTempTextId(null, null);
            AddTempTextIP(null, null);
            AddTempTextPassword(null, null);
            AddTempTextPort(null, null);
            
        }


        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            IPTextBox = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            PortTextBox = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            UserNameTextBox = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            PasswordTextBox = new System.Windows.Forms.TextBox();
            CheckBtn = new System.Windows.Forms.Button();
            CancelBtn = new System.Windows.Forms.Button();
            ApplyBtn = new System.Windows.Forms.Button();
            databaseTextBox = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            InfoLabel = new System.Windows.Forms.Label();
            LastBtn = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // IPTextBox
            // 
            IPTextBox.Location = new System.Drawing.Point(95, 12);
            IPTextBox.Name = "IPTextBox";
            IPTextBox.Size = new System.Drawing.Size(177, 23);
            IPTextBox.TabIndex = 2;
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(9, 10);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(77, 24);
            label1.TabIndex = 1;
            label1.Text = "IP";
            // 
            // label2
            // 
            label2.Location = new System.Drawing.Point(9, 41);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(77, 24);
            label2.TabIndex = 3;
            label2.Text = "Port";
            // 
            // PortTextBox
            // 
            PortTextBox.Location = new System.Drawing.Point(95, 41);
            PortTextBox.Name = "PortTextBox";
            PortTextBox.Size = new System.Drawing.Size(177, 23);
            PortTextBox.TabIndex = 4;
            // 
            // label3
            // 
            label3.Location = new System.Drawing.Point(9, 69);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(77, 24);
            label3.TabIndex = 5;
            label3.Text = "database";
            // 
            // UserNameTextBox
            // 
            UserNameTextBox.Location = new System.Drawing.Point(95, 99);
            UserNameTextBox.Name = "UserNameTextBox";
            UserNameTextBox.Size = new System.Drawing.Size(177, 23);
            UserNameTextBox.TabIndex = 8;
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(9, 99);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(77, 24);
            label4.TabIndex = 7;
            label4.Text = "UserId";
            // 
            // PasswordTextBox
            // 
            PasswordTextBox.Location = new System.Drawing.Point(95, 128);
            PasswordTextBox.Name = "PasswordTextBox";
            PasswordTextBox.Size = new System.Drawing.Size(177, 23);
            PasswordTextBox.TabIndex = 10;
            // 
            // CheckBtn
            // 
            CheckBtn.Location = new System.Drawing.Point(9, 217);
            CheckBtn.Name = "CheckBtn";
            CheckBtn.Size = new System.Drawing.Size(77, 32);
            CheckBtn.TabIndex = 11;
            CheckBtn.Text = "Check";
            CheckBtn.UseVisualStyleBackColor = true;
            CheckBtn.Click += CheckBtn_Click;
            // 
            // CancelBtn
            // 
            CancelBtn.Location = new System.Drawing.Point(115, 217);
            CancelBtn.Name = "CancelBtn";
            CancelBtn.Size = new System.Drawing.Size(77, 32);
            CancelBtn.TabIndex = 12;
            CancelBtn.Text = "Cancel";
            CancelBtn.UseVisualStyleBackColor = true;
            CancelBtn.Click += CancelBtn_Click;
            // 
            // ApplyBtn
            // 
            ApplyBtn.Location = new System.Drawing.Point(198, 217);
            ApplyBtn.Name = "ApplyBtn";
            ApplyBtn.Size = new System.Drawing.Size(77, 32);
            ApplyBtn.TabIndex = 13;
            ApplyBtn.Text = "Apply";
            ApplyBtn.UseVisualStyleBackColor = true;
            ApplyBtn.Visible = false;
            ApplyBtn.Click += Apply_Click;
            // 
            // databaseTextBox
            // 
            databaseTextBox.Location = new System.Drawing.Point(95, 70);
            databaseTextBox.Name = "databaseTextBox";
            databaseTextBox.Size = new System.Drawing.Size(177, 23);
            databaseTextBox.TabIndex = 6;
            // 
            // label6
            // 
            label6.Location = new System.Drawing.Point(9, 128);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(77, 35);
            label6.TabIndex = 9;
            label6.Text = "password";
            // 
            // InfoLabel
            // 
            InfoLabel.Location = new System.Drawing.Point(7, 157);
            InfoLabel.Name = "InfoLabel";
            InfoLabel.Size = new System.Drawing.Size(248, 36);
            InfoLabel.TabIndex = 14;
            // 
            // LastBtn
            // 
            LastBtn.Location = new System.Drawing.Point(198, 180);
            LastBtn.Name = "LastBtn";
            LastBtn.Size = new System.Drawing.Size(77, 31);
            LastBtn.TabIndex = 14;
            LastBtn.Text = "Revert";
            LastBtn.UseVisualStyleBackColor = true;
            LastBtn.Visible = false;
            LastBtn.Click += LastBtn_Click;
            var tooltip = new ToolTip();
            tooltip.SetToolTip(LastBtn, "Changes all data fields to last known working connection");

            // 
            // FirstRunWindow
            // 
            BackColor = System.Drawing.SystemColors.Control;
            ClientSize = new System.Drawing.Size(284, 261);
            Controls.Add(LastBtn);
            Controls.Add(InfoLabel);
            Controls.Add(label6);
            Controls.Add(databaseTextBox);
            Controls.Add(ApplyBtn);
            Controls.Add(CancelBtn);
            Controls.Add(CheckBtn);
            Controls.Add(label4);
            Controls.Add(PasswordTextBox);
            Controls.Add(label3);
            Controls.Add(UserNameTextBox);
            Controls.Add(label2);
            Controls.Add(PortTextBox);
            Controls.Add(label1);
            Controls.Add(IPTextBox);
            ForeColor = System.Drawing.SystemColors.Desktop;
            Location = new System.Drawing.Point(15, 15);
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Button LastBtn;

        private System.Windows.Forms.Label InfoLabel;

        private System.Windows.Forms.Label label6;

        private System.Windows.Forms.TextBox databaseTextBox;

        private System.Windows.Forms.Button CheckBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button ApplyBtn;

        private System.Windows.Forms.TextBox IPTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PortTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox UserNameTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private ProgressBar progressBar;

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Apply_Click(object sender, EventArgs e)
        {
            File.Create(firstRun.basestr + "/ranbefore.txt").Dispose();
            try
            {
                // Create proper configuration object structure
                var configObject = new
                {
                    ConnectionStrings = new
                    {
                        DefaultConnection = lastWorkingConnection
                    }
                };

                var jsonString = JsonSerializer.Serialize(configObject,
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(firstRun.basestr + "/sql/ConnectionStringsConfiguration.json", jsonString);
            }
            catch (Exception ex)
            {
                Logger.Log($"error while writing connection string to file {ex.Message}");
                
            }
            Logger.Log("applying settings");
            SQL.ConnectionString = lastWorkingConnection;
            sync.syncAll();
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private async void CheckBtn_Click(object sender, EventArgs e)
        {
            InfoLabel.Text = "Checking connection string";
            progressBar.Value = 0;
            bool valid = true;
            connectionString = $"Server={IPTextBox.Text},{PortTextBox.Text};" +
                                      $"Database={databaseTextBox.Text};" +
                                      $"User Id={UserNameTextBox.Text};" +
                                      $"Password={PasswordTextBox.Text};" +
                                      $"Encrypt=False";
            //now we try the connection
            try
            {
                progressBar.Show();
                progressBar.Value = 10;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    progressBar.Value = 20;
                    await conn.OpenAsync();
                    progressBar.Value = 40;
                    InfoLabel.Text = "connection successful";
                    Logger.Log("connection successful");
                    progressBar.Value = 100;
                    lastWorkingConnection = connectionString;
                    LastBtn.Visible = false;
                }
                
            }
            catch (Exception ex)
            {
                InfoLabel.Text = "failed to connect to database";
                progressBar.Value = 100;
                Logger.Log($"user inputted invalid string {ex.Message}, {connectionString}");
                valid = false;
                connectionString = "";
                if (lastWorkingConnection != null)
                {
                    Logger.Log("last working connection string was: " + lastWorkingConnection + "so gonna give option to return");
                    LastBtn.Visible = true;
                }
                
            }

            progressBar.Value = 0;
            CheckBtn.Visible = true;
            CancelBtn.Visible = true;
            ApplyBtn.Visible = valid;
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
            if (PortTextBox.Text == "*12346")
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
                PortTextBox.Text = "*12346";
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
            IPTextBox.Text = lastWorkingConnection.Split(';')[0].Split('=')[1].Split(',')[0];
            PortTextBox.Text = lastWorkingConnection.Split(';')[0].Split('=')[1].Split(',')[1];
            databaseTextBox.Text = lastWorkingConnection.Split(';')[1].Split('=')[1];
            UserNameTextBox.Text = lastWorkingConnection.Split(';')[2].Split('=')[1];
            PasswordTextBox.Text = lastWorkingConnection.Split(';')[3].Split('=')[1];
            Logger.Log("replaced last known shit");
            connectionString = lastWorkingConnection;
        }



    }
    
    public class firstRun
    {
        public static string basestr = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/workclonecs";
        

        public static bool ranBefore()
        {
            Logger.Here();
            if (!Directory.Exists(basestr+"/sql"))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(basestr+"/sql"));
            }
            if (Directory.Exists(basestr + "/sql"))
            {
                try
                {
                    if (File.Exists(basestr + "/ranbefore.txt") && File.Exists(basestr + "/sql/ConnectionStringsConfiguration.json"))
                    {
                        return true;
                    }
                    
                }
                catch (Exception ex)
                {
                    Logger.Log("error while creating / existing ranbefore.txt " + ex.Message);
                }
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(basestr + "/sql");
                    Logger.Log("created sql folder");
                    
                }
                catch (Exception ex)
                {
                    Logger.Log("error while creating sql folder " + ex.Message);
                }
            }
                
            Logger.Log("program hasnt ran before");
            return false;

            
        }
    }
}