using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
namespace WorkCloneCS
{

    public partial class Form1 : Form
    {

        private staff currentStaff;        
        int globalCount = 0;
        private List<catagory> cat = sync.catagories;

        private string[] catagories = FoodLoader.LoadCatagorys("catagories");
        public Form1()
        {
            InitializeComponent();
            InitFoodList();
            addCatagory();
        }
        
        private void formClosing(object sender, FormClosingEventArgs e)
        {
            Logger.CloseLog();
        }
        
        private void deleteChildbox() { if (catPan != null) catPan.Controls.Clear(); }

        private void addCatagory()
        {
            deleteChildbox();

            int count = 1;
            for (int i = 0; i < cat.Count; i++)
            {
                Label item = new Label
                {
                    Text = sync.catagories[i].catName,
                    AutoSize = false,
                    BackColor = Color.Gray,
                    Width = (catPan.Width / 8) - 2,
                    Height = 50,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 12, FontStyle.Regular),
                    Margin = new Padding(1),
                    Tag = i
                };
                count++;
                item.Click += catClick;

                catPan.Controls.Add(item);
            }
        }

        private void catClick(object sender, EventArgs e)
        {
            deleteChildbox();
            InitItemList((int)((Control)sender).Tag);
        }

        private void addItem(decimal Price, string Name, int rowHeight)
        {
            priceTotal += Price;
            globalCount++;
            int countLabelWidth = 30;
            int priceLabelWidth = 60;
            rowOfItem row = new rowOfItem()
            {
                FoodName = Name,
                Price = Price,
                ItemCount = 1
            };
            row.updateText();
            EnableSwipeToDelete(row);
            row.SetHeight(rowHeight);
            scrollPanel.Controls.Add(row.rowPannel);
            scrollPanel.VerticalScroll.Value = scrollPanel.VerticalScroll.Maximum;
            scrollPanel.PerformLayout();
        }

        private void EnableSwipeToDelete(rowOfItem row)
        {
            Point mouseDownLocation = Point.Empty;
            bool isDragging = false;

            row.Middle.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    mouseDownLocation = e.Location;
                    isDragging = true;
                }
            };

            row.Middle.MouseMove += (s, e) =>
            {
                if (!isDragging) return;

                int deltaX = e.X - mouseDownLocation.X;

                if (Math.Abs(deltaX) > 5)
                    row.Middle.BackColor = Color.LightCoral;
            };

            row.Middle.MouseUp += (s, e) =>
            {
                FlowLayoutPanel rowPannel = (FlowLayoutPanel)row.Middle.Parent;
                if (!isDragging) return;
                isDragging = false;

                int deltaX = e.X - mouseDownLocation.X;
                if (rowPannel != null)
                {
                    if (deltaX < -100 && rowPannel != null) // Swipe left threshold
                    {
                        Label left = row.Left;
                        if (row.ItemCount > 1)
                        {
                            row.ItemCount--;
                            row.updateText();


                        }
                        else
                        {
                            foreach (Control control in panel1.Controls)
                            {
                                if (control is FlowLayoutPanel && control.Tag is rowPanelTag 
                                && ((rowPanelTag)control.Tag).Name == row.FoodName)
                                {
                                    panel1.Controls.Remove(control);
                                    control.Dispose();
                                    break;
                                }
                            }
                        }
                        updateTotalPrice(-row.Price);
                        updateTotalItems(-row.ItemCount);
                    }
                    else if (deltaX > 100 && rowPannel != null)
                    {
                        row.ItemCount++;
                        row.updateText();
                        updateTotalItems(1);
                        updateTotalPrice(row.Price);

                    }

                    row.Middle.BackColor = SystemColors.Control;
                }
                else
                {
                    MessageBox.Show($"it is null here is the thing innit: {rowPannel.Controls}");
                }

            };

        }

        private void updateTotalItems(int number)
        {
            leftLabel.Tag = (int)leftLabel.Tag + number;
            leftLabel.Text = $"Items: {leftLabel.Tag}";
        }

        private void updateTotalPrice(decimal change)
        {
            decimal total = (decimal)rightLabel.Tag + change;
            rightLabel.Tag = (total);
            string t = total.ToString("c");
            rightLabel.Text = $"Price: {t}";
        }

        private void createScrollPanel()
        {
            scrollPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Location = new Point(13, 43),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = false,
                Padding = new Padding(0)
            };

            panel1.Controls.Add(scrollPanel);
        }


        private void InitFoodList()
        {
            createScrollPanel();

            // Sample data - to be changed
            leftLabel.Tag = 0;
            rightLabel.Tag = 0m;
            leftLabel.Dock = DockStyle.Left;
            rightLabel.Dock = DockStyle.Right;
            panel2.Controls.Add(leftLabel);
            panel2.Controls.Add(rightLabel);
        }

        private void deleteAllItemsOrdered()
        {
            scrollPanel.Controls.Clear();
            leftLabel.Tag = 0;
            leftLabel.Text = "Items: 0";
            rightLabel.Tag = 0m;
            rightLabel.Text = "Price: �0.00";
        }

        private void generalItem_Click(object sender, EventArgs e)
        {
            item item = (item)((Control)sender).Tag;
            string name = item.itemName;
            decimal price = item.price;

            addItem(price, name, 40);
            leftLabel.Tag = (int)leftLabel.Tag + 1;
            leftLabel.Text = $"Items: {leftLabel.Tag}";
            updateTotalPrice(price);
        }


        //in the bottom right
        private void backBtn_Click(object sender, EventArgs e)
        {
            deleteChildbox();


            addCatagory();
            allPannelsBlank();
        }


        //this happens when shit is open
        private void Form1_Load(object sender, EventArgs e)
        {

        }


        //called when catagory clicked on, gets items from file called "{catagoryName}".txt -
        //should probs change for an api call icl but you never know yk 
        // then just goes through each and adds each value and what not
        private void InitItemList(int e)
        {
            catagory cat = sync.catagories[e];
            List<item> foodItems = cat.items;
            if (foodItems != null)
            {
                for (int i = 0; i < foodItems.Count; i++)
                {
                    try
                    {
                        
                        addLabel(foodItems[i]);
                    } catch (Exception ex)
                    {
                        Logger.Log($"{ex.Message}    {e}", 3);
                    }
                    
                }
            }
        }


        //add all of the items made clickeable that all take them to the gereralItem_Click()
        private void addLabel(item tag)
        {
            Color colour = Color.Gray;
            if (tag.chosenColour == "grey") colour = Color.Gray;
            else if (tag.chosenColour != null) colour = Color.FromName(tag.chosenColour);


                Label item = new Label
                {
                    Text = tag.itemName,
                    Tag = tag,
                    AutoSize = false,
                    BackColor = colour,
                    Width = (catPan.Width / 8) - 2,
                    Height = 50,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 12, FontStyle.Regular),
                    Margin = new Padding(1)
                };

            item.Click += generalItem_Click;

            catPan.Controls.Add(item);
        }


        //for the config panels should always be called before toggling the visibility of a panel
        //probably useful for me to explain that this is a requirement else we could run into multiple 
        //click issues and other annoying shite
        private void allPannelsBlank()
        {
            ConfigPannel.Visible = false;
            finalPanel.Visible = false;
            tablePanel.Visible = false;
            orderPanel.Visible = false;
            miscPanel.Visible = false;
        }

        //the following 5 functions are to bring up user specific panels
        private void ConfigBtn_Click(object sender, EventArgs e)
        {
            bool temp = !ConfigPannel.Visible;
            allPannelsBlank();
            ConfigPannel.Visible = temp;
            ConfigPannel.BringToFront();

        }


        private void FinalBtn_Click(object sender, EventArgs e)
        {
            bool temp = !finalPanel.Visible;
            allPannelsBlank();
            finalPanel.Visible = temp;
            finalPanel.BringToFront();
        }

        private void tableBottomBtn_Click(object sender, EventArgs e)
        {
            bool temp = !tablePanel.Visible;
            allPannelsBlank();
            tablePanel.Visible = temp;
            tablePanel.BringToFront();
        }

        private void OrderBtn_Click(object sender, EventArgs e)
        {
            bool temp = !orderPanel.Visible;
            allPannelsBlank();
            orderPanel.Visible = temp;
            orderPanel.BringToFront();
            int totalHeight = flowLayoutPanel1.Height;
            int panel1Height;
            if (!temp)
            {
                panel1Height = 279;
                foreach (Control ctrl in scrollPanel.Controls)
                {
                    if (ctrl is FlowLayoutPanel panel && panel.Tag is rowPanelTag t && t != null) // or check Name, Tag, etc.
                    {
                        foreach (Control ctrl2 in panel.Controls)
                        {
                            if (ctrl2 is Label lbl && lbl.Name == $"foodLabel{t.Count}")
                            {
                                lbl.Width += 92;
                            }
                        }
                    }
                }
            }


            else
            {
                panel1Height = 0;
                foreach (Control ctrl in scrollPanel.Controls)
                {
                    if (ctrl is FlowLayoutPanel panel && panel.Tag is rowPanelTag t && t != null) // or check Name, Tag, etc.
                    {
                        foreach (Control ctrl2 in panel.Controls)
                        {
                            if (ctrl2 is Label lbl && lbl.Name == $"foodLabel{t.Count}")
                            {
                                lbl.Width += 92;
                            }
                        }
                    }
                }
            }

            int panel2Height = 33;
            int catpanHeight = totalHeight - panel2Height - panel1Height - 20;
            panel1.Height = catpanHeight;
            catPan.Height = panel1Height;




        }

        private void miscBtn_Click(object sender, EventArgs e)
        {
            bool temp = !miscPanel.Visible;
            allPannelsBlank();
            miscPanel.Visible = temp;
            miscPanel.BringToFront();

        }

        //yet to be implemented
        private void SignOnBtnConfigPanel_Click(object sender, EventArgs e)
        {
            deleteAllItemsOrdered();
        }

        //top layer btn's currently unused icl
        private void tableBtn_Click(object sender, EventArgs e)
        {

        }

        private void nameBtn_Click(object sender, EventArgs e)
        {
            NameForm nameForm = new NameForm();
            nameForm.ShowDialog();
            if (nameForm.staffSelected == null) nameBtn.Text = "name";
            else
            {
                currentStaff = nameForm.staffSelected;
                nameBtn.Text = currentStaff.Name.ToUpper();
            }
        }

        private void PricingBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("basically this is a thing but is honestly never used" +
                "\n in practice i have never ever pressed this button" +
                "\n and it is something you would only ever change on the tills anyways sooo" +
                "\n return not implemented lol");
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            deleteAllItemsOrdered();
            allPannelsBlank();
            addCatagory();
        }
    }


    

}
