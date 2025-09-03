﻿using System.Text.Json;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Data.SqlClient;
namespace WorkCloneCS
{


    public partial class FirstRunWindow : Form
    {
        private readonly ConnectionSettingsValidator _validator = new ConnectionSettingsValidator();
        private string lastWorkingConnection;
        public string connectionString;
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
            SQL.connectionString = lastWorkingConnection;
            sync.syncAll();
            if (!t)
            {
                Form1 form1 = new Form1();
                form1.Show();
                Hide();
            }
            
            else Close();
        }

        private async void CheckBtn_Click(object sender, EventArgs e)
        {
            InfoLabel.Text = "Checking connection string";
            string anyErrors = "";
            bool valid = true;
            IPTextBox.BackColor = SystemColors.Window;
            PortTextBox.BackColor = SystemColors.Window;
            databaseTextBox.BackColor = SystemColors.Window;
            UserNameTextBox.BackColor = SystemColors.Window;
            PasswordTextBox.BackColor = SystemColors.Window;

            var settings = new ConnectionSettings
            {
                IP = IPTextBox.Text,
                Port = PortTextBox.Text,
                Database = databaseTextBox.Text,
                Username = UserNameTextBox.Text,
                Password = PasswordTextBox.Text
            };

            // Validate each property individually
            var ipValidation = _validator.TestValidate(settings, options => 
                options.IncludeProperties(x => x.IP));
            var portValidation = _validator.Validate(settings, options => 
                options.IncludeProperties(x => x.Port));
            var dbValidation = _validator.Validate(settings, options => 
                options.IncludeProperties(x => x.Database));
            var userValidation = _validator.Validate(settings, options => 
                options.IncludeProperties(x => x.Username));
            var passValidation = _validator.Validate(settings, options => 
                options.IncludeProperties(x => x.Password));

            var errorMessages = new List<string>();

            // Check each validation result and update colors/messages
            if (!ipValidation.IsValid)
            {
                IPTextBox.BackColor = Color.MistyRose;
                errorMessages.AddRange(ipValidation.Errors.Select(e => e.ErrorMessage));
                valid = false;
            }

            if (!portValidation.IsValid)
            {
                PortTextBox.BackColor = Color.MistyRose;
                errorMessages.AddRange(portValidation.Errors.Select(e => e.ErrorMessage));
                valid = false;
            }

            if (!dbValidation.IsValid)
            {
                databaseTextBox.BackColor = Color.MistyRose;
                errorMessages.AddRange(dbValidation.Errors.Select(e => e.ErrorMessage));
                valid = false;
            }

            if (!userValidation.IsValid)
            {
                UserNameTextBox.BackColor = Color.MistyRose;
                errorMessages.AddRange(userValidation.Errors.Select(e => e.ErrorMessage));
                valid = false;
            }

            if (!passValidation.IsValid)
            {
                PasswordTextBox.BackColor = Color.MistyRose;
                errorMessages.AddRange(passValidation.Errors.Select(e => e.ErrorMessage));
                valid = false;
            }

            if (!valid)
            {
                anyErrors = string.Join("\n", errorMessages);
                InfoLabel.Text = anyErrors;
                Logger.Log(anyErrors);
            }

            if (valid)
            {
                connectionString = $"Server={IPTextBox.Text},{PortTextBox.Text};" +
                                   $"Database={databaseTextBox.Text};" +
                                   $"User Id={UserNameTextBox.Text};" +
                                   $"Password={PasswordTextBox.Text};" +
                                   $"Encrypt=False";
                //now we try the connection
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        await conn.OpenAsync();
                        InfoLabel.Text = "connection successful";
                        Logger.Log("connection successful");
                        lastWorkingConnection = connectionString;
                        LastBtn.Visible = false;
                    }

                }
                catch (Exception ex)
                {
                    InfoLabel.Text = "failed to connect to database";
                    Logger.Log($"user inputted invalid string {ex.Message}, {connectionString}");
                    valid = false;
                    connectionString = "";
                    if (lastWorkingConnection != null)
                    {
                        Logger.Log("last working connection string was: " + lastWorkingConnection +
                                   "so gonna give option to return");
                        LastBtn.Visible = true;
                    }

                }
            }
            else
            {
                InfoLabel.Text = anyErrors;
                if (lastWorkingConnection != null)
                {
                    Logger.Log("last working connection string was: " + lastWorkingConnection +
                               "so gonna give option to return");
                    LastBtn.Visible = true;
                }
            }
            
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