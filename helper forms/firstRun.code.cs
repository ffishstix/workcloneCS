using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace WorkCloneCS
{
    public partial class FirstRunWindow : Form
    {
        private void Apply_Click_Code(object sender, EventArgs e)
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
                Logger.Log($"error while writing connection string to file {ex.Message} apply_Click_code");

            }
            Logger.Log("applying settings");
            database.setConnectionString(lastWorkingConnection);
            database.pullCloudDatabase();
            if (!t)
            {
                Form1 form1 = new Form1();
                form1.Show();
                Hide();
            }

            else Close();
        }
    
        private async void CheckBtn_Click_Code(object obj, EventArgs e)
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
            if (settings.IP.IsNullOrEmpty()) settings.IP = defaultPort.ToString();
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
                Logger.Log(anyErrors + "  CheckBtn_Click_Code ");
            }

            if (valid)
            {
                connectionString = $"Server={settings.IP},{settings.Port};" +
                                   $"Database={settings.Database};" +
                                   $"User Id={settings.Username};" +
                                   $"Password={settings.Password};" +
                                   $"Encrypt=False";
                //now we try the connection
                string allPat = @"Server=((?!-)[A-Za-z0-9-]{1,63}(?<!-)(\.[A-Za-z0-9-]{1,63})*(\.[A-Za-z]{2,})),((6553[0-5])|(655[0-2][0-9])|(65[0-4][0-9]{2})|(6[0-4][0-9]{3})|([1-5][0-9]{4})|([0-5]{0,5})|([0-9]{1,4}));Database=(\w*);User Id=([a-zA-Z0-9][a-zA-Z0-9_-]{0,127});Password=(\w{8,128});Encrypt=False";
                Logger.Log(connectionString + " to be removed in prod");
                bool isAllCorrect = System.Text.RegularExpressions.Regex.IsMatch(connectionString, allPat);
                if (isAllCorrect)
                {
                    var (ok, errorMessage) = await database.tryOpenConnectionAsync(connectionString);
                    if (ok)
                    {
                        InfoLabel.Text = "connection successful";
                        Logger.Log("connection successful");
                        lastWorkingConnection = connectionString;
                        LastBtn.Visible = false;
                    }
                    else
                    {
                        InfoLabel.Text = "failed to connect to database";
                        Logger.Log($"user inputted invalid string {errorMessage}, {connectionString}");
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

                else Logger.Log("connectionstring isnt valid");
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
    
        private void LastBtn_Click_Code(object sender, EventArgs e)
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
            if (!Directory.Exists(basestr + "/sql"))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(basestr + "/sql"));
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
