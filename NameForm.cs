using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace WorkCloneCS
{
    public partial class NameForm : Form
    {
        private int currentID = 0;
        private List<staff> x;
        public staff staffSelected;
        public NameForm()
        {
            InitializeComponent();
            x = SQL.getStaffData();
            if (x == null) x = getStaff($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/workclonecs/sql/staff.txt");
            else File.WriteAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/workclonecs/sql/staff.txt", 
            JsonSerializer.Serialize(x, new JsonSerializerOptions { WriteIndented = true }));
            
            if (x == null) { this.Close(); }
            displayBtn.Text = "";
        }


        private List<staff> getStaff(string filePath)
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);

                // Deserialize the JSON string into a List of User objects
                List<staff> staff = JsonSerializer.Deserialize<List<staff>>(jsonString);
                return staff;
            } catch (Exception ex) {
                Logger.Log(ex.Message);
            }
            return null;
        }
        public int returnUserID()
        {
            this.Show();

            return 0;
        }

        private void updateDisplayBtnText()
        {
            if (currentID == 0) displayBtn.Text = "";
            else displayBtn.Text = currentID.ToString();
        }

        private void numberBtn_Click(object sender, EventArgs e)
        {
            if (currentID == 0) currentID = int.Parse(((Button)sender).Text);
            else currentID = (currentID * 10) + int.Parse(((Button)sender).Text);

                
            updateDisplayBtnText();
        }

        private void btnEsc_Click(object sender, EventArgs e)
        {
            currentID = 0;
            updateDisplayBtnText();
        }



        private void btnOK_Click(object sender, EventArgs e)
        {
            staffSelected = null;
            foreach(staff staff in x)
            {
                if (staff.Id == currentID)
                {
                    staffSelected = staff;
                    this.Close();
                    break;
                } 

            }
            if (staffSelected == null)
            {
                MessageBox.Show("invalid id");
            }
        }
    }
}
