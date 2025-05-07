using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkCloneCS
{
    public partial class NameForm : Form
    {
        private int currentID = 0;
        public NameForm()
        {
            InitializeComponent();
            displayBtn.Text = "";
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

        }
    }
}
