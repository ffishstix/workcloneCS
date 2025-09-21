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
        private int currentID;
        private List<staff> x;
        public staff staffSelected;
        private DateTime timeSinceLastClick;
        public NameForm()
        {
            currentID = 0;
            InitializeComponent();
            timeSinceLastClick = DateTime.MinValue;
            x = sync.allStaff;
            if (x == null)
            {
                x = SQL.getStaffData();
            }
            if (x == null)
            {
                
                this.Close();
                Logger.Log("staff was null so closing window");
            }
            else
            {
                Logger.Log("staff be staffing icl ");
                sync.allStaff = x;
            }
            displayBtn.Text = "";
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
            if (timeSinceLastClick >= (DateTime.Now) - TimeSpan.FromSeconds(0.2))
            {
                staffSelected.Id = 0;
                this.Close();
                
            }
            timeSinceLastClick = DateTime.Now;
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
