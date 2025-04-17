using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
namespace WorkCloneCS
{

    class rowPanelTag
    {
        private string name = "changeMe";
        private int count = 1;
        private decimal price = 0;
        private int itemCount = 1;
        private decimal totalPrice = 0;

        public string Name { get { return name; } set { name = value; } }
        public int Count { get { return count; } set { count = value; } }
        public decimal Price { get { return price; } set { price = value; } }
        public int ItemCount { get { return itemCount; } set { itemCount += value; } }
        public decimal TotalPrice 
        {
            get { return totalPrice; }
            set { totalPrice += value; } 
        }

    }
    public partial class Form1 : Form
    {
        int globalCount = 0;
        private string[] catagories = FoodLoader.LoadCatagorys("catagories");
        public Form1()
        {
            InitializeComponent();
            InitFoodList();
            addCatagory(catagories);
        }

        private void deleteChildbox() { if (catPan != null) catPan.Controls.Clear(); }

        private void addCatagory(string[] lis)
        {
            deleteChildbox();


            foreach (string catagory in lis)
            {
                Label item = new Label
                {
                    Text = catagory,
                    AutoSize = false,
                    BackColor = Color.Gray,
                    Width = (catPan.Width / 8) - 2,
                    Height = 50,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 12, FontStyle.Regular),
                    Margin = new Padding(1),
                    Tag = catagory
                };

                item.Click += catClick;

                catPan.Controls.Add(item);
            }
        }

        private void catClick(object sender, EventArgs e)
        {
            deleteChildbox();
            InitItemList((string)((Control)sender).Tag);
        }

        private void addItem(decimal Price, string Name, int rowHeight)
        {
            priceTotal += Price;
            globalCount++;
            int countLabelWidth = 30;
            int priceLabelWidth = 60;
            rowPanelTag tag = new rowPanelTag { 
                Name = Name,
                Price = Price,
                Count = globalCount,
                TotalPrice = Price
            };
            
            FlowLayoutPanel rowPannel = new()
            {
                Height = rowHeight,
                Tag = tag,
                Padding = new Padding(0),
                Margin = new Padding(0),
                Name = $"rowPannel{globalCount}",
                Width = scrollPanel.Width - SystemInformation.VerticalScrollBarWidth,
                AutoSize = false,
                AutoScroll = false,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Green
            };

            Label countLabel = new Label
            {
                Text = "1",
                Name = $"countLabel{globalCount}",
                Width = countLabelWidth,
                Height = rowHeight,
                AutoSize = false,
                Padding = new Padding(0),
                Margin = new Padding(0),
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Yellow

            };

            Label foodLabel = new Label
            {
                Text = Name,
                Height = rowHeight,
                AutoSize = false,
                Name = $"foodLabel{globalCount}",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Padding = new Padding(10, 0, 0, 0),
                Tag = (Name, Price),
                Width = scrollPanel.Width - countLabelWidth - priceLabelWidth - SystemInformation.VerticalScrollBarWidth - 9
            };
            EnableSwipeToDelete(foodLabel);

            Label priceLabel = new Label
            {
                Text = Price.ToString("c"),
                Name = $"priceLabel{globalCount}",
                Tag = Price.ToString(),
                Width = priceLabelWidth,
                Height = rowHeight,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Padding = new Padding(0),
                BackColor = Color.Red,

            };

            rowPannel.Controls.Add(countLabel);
            rowPannel.Controls.Add(foodLabel);
            rowPannel.Controls.Add(priceLabel);
            scrollPanel.Controls.Add(rowPannel);

            scrollPanel.VerticalScroll.Value = scrollPanel.VerticalScroll.Maximum;
            scrollPanel.PerformLayout();
        }


        private void swipeToTheLeftLogic(Label label, FlowLayoutPanel parent)
        {
            if (parent.Tag is rowPanelTag tag && tag != null)
            {
                foreach (Control ctrl in parent.Controls)
                {
                    if (ctrl is Label countLabel && countLabel.Name == $"countLabel{tag.Count}") // or check Name, Tag, etc.
                    {
                        countLabel.BackColor = Color.White;
                        try
                        {
                            int itemCount = tag.ItemCount;
                            if (tag.ItemCount > 1)
                            {
                                tag.ItemCount--;
                                countLabel.Text = (tag.ItemCount).ToString();
                                tag.TotalPrice = -tag.Price;

                            }
                            else
                            {
                                parent.Controls.Remove(label);
                                label.Dispose();
                            }
                            break;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(
                            $"An error occurred:\n{ex.Message}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                            );
                        }
                    }
                }
                updateTotalPrice(-tag.Price);
                updateTotalItems(-1);
            }
            
        }

        private void swipeToTheRightLogic(Label label, FlowLayoutPanel parent)
        {
            if (parent.Tag is rowPanelTag tag)
            {
            label.BackColor = Color.Blue;

                // Search for the countLabel inside the same rowPanel
                foreach (Control ctrl in parent.Controls)
                {
                    if (ctrl is Label countLabel && countLabel.Name == $"countLabel{tag.Count}") // or check Name, Tag, etc.
                    {
                        countLabel.BackColor = Color.White;
                        try
                        {

                            
                            tag.ItemCount++;
                            tag.TotalPrice = tag.Price;
                            countLabel.Text = $"{tag.ItemCount}";
                            MessageBox.Show($"count: {tag.ItemCount}, price {tag.Price} so should be: {tag.TotalPrice}");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(
                                $"An error occurred:\n{ex.Message}",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                        }
                        break;
                    }
                }
                updateTotalItems(1);
                updateTotalPrice(tag.Price);
                

                //CURENTLY BROKEN,
                //must add fix for having multiple of the same products and then swiping right
                // also remove one from the price on the left

            }
        }
        private void EnableSwipeToDelete(Label label)
        {
           
            Point mouseDownLocation = Point.Empty;
            bool isDragging = false;
            FlowLayoutPanel rowPannel = (FlowLayoutPanel)label.Parent;
            label.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    mouseDownLocation = e.Location;
                    isDragging = true;
                }
            };

            label.MouseMove += (s, e) =>
            {
                if (!isDragging) return;

                int deltaX = e.X - mouseDownLocation.X;

                if (Math.Abs(deltaX) > 5)
                    label.BackColor = Color.LightCoral;
            };

            label.MouseUp += (s, e) =>
            {
                if (!isDragging) return;
                isDragging = false;

                int deltaX = e.X - mouseDownLocation.X;

                if (deltaX < -100 && rowPannel != null) // Swipe left threshold
                {
                    swipeToTheLeftLogic(label, rowPannel);
                }
                else if (deltaX > 100 && rowPannel != null)
                {
                    swipeToTheRightLogic(label, rowPannel);
                }
                else
                {
                    MessageBox.Show($"it is null here is the thing innit: {rowPannel.Controls}");
                }
                label.BackColor = SystemColors.Control;
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
            List<(string Name, decimal Price)> foodItems = null;
            int rowHeight = 40;
            int padding = 5;
            priceTotal = 0m;
            int count = 0;
            if (foodItems != null)
            {
                foreach (var item in foodItems) addItem(item.Price, item.Name, rowHeight);
                count = foodItems.Count;
            }
            leftLabel = new Label
            {
                Text = $"Items: {count}",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Left,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Width = scrollPanel.Width / 2 - SystemInformation.VerticalScrollBarWidth / 2,
                Padding = new Padding(10, 0, 0, 0),
                Tag = (int)(count)

            };
            rightLabel = new Label
            {
                Text = $"Price: {priceTotal.ToString("c")}",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Right,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Width = scrollPanel.Width / 2 - SystemInformation.VerticalScrollBarWidth / 2,
                Padding = new Padding(10, 0, 0, 0),
                Tag = priceTotal
            };
            panel2.Controls.Add(leftLabel);
            panel2.Controls.Add(rightLabel);
        }


        private void deleteAllItemsOrdered()
        {
            scrollPanel.Controls.Clear();
        }

        //
        private void generalItem_Click(object sender, EventArgs e)
        {
            var item = (Tuple<string, decimal>)((Control)sender).Tag;
            string name = item.Item1;
            decimal price = item.Item2;

            addItem(price, name, 40);
            leftLabel.Tag = (int)leftLabel.Tag + 1;
            leftLabel.Text = $"Items: {leftLabel.Tag}";
            updateTotalPrice(price);
        }


        //top layer btn's currently unused icl
        private void tableBtn_Click(object sender, EventArgs e)
        {

        }

        private void nameBtn_Click(object sender, EventArgs e)
        {

        }

        private void PricingBtn_Click(object sender, EventArgs e)
        {

        }


        //in the bottom right
        private void backBtn_Click(object sender, EventArgs e)
        {
            deleteChildbox();
            addCatagory(catagories);
            allPannelsBlank();
        }


        //this happens when shit is open
        private void Form1_Load(object sender, EventArgs e)
        {

        }


        //called when catagory clicked on, gets items from file called "{catagoryName}".txt -
        //should probs change for an api call icl but you never know yk 
        // then just goes through each and adds each value and what not
        private void InitItemList(string e)
        {
            List<(string Name, decimal Price, string extra)> foodItems = FoodLoader.LoadFoodItems(e);
            if (foodItems != null)
            {
                foreach (var foodItem in foodItems)
                {
                    addLabel(foodItem.Name, foodItem.Price, foodItem.extra);
                }
            }
            else
            {
                addLabel("In Development", 0m, "");
            }
        }


        //add all of the items made clickeable that all take them to the gereralItem_Click()
        private void addLabel(string name, decimal price, string extra)
        {
            List<(string name, decimal Price, string extra)> slice = new List<(string name, decimal Price, string extra)>();
            slice.Insert(0, (name, price, extra));
            Label item = new Label
            {
                Text = name,
                Tag = Tuple.Create(name, price),
                AutoSize = false,
                BackColor = Color.Gray,
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
        }

        //this is the bottom right button to bring up the right menu ;)
        private void ConfigBtn_Click(object sender, EventArgs e)
        {
            bool temp = !ConfigPannel.Visible;
            allPannelsBlank();
            ConfigPannel.Visible = temp;

        }

        private void SignOnBtnConfigPanel_Click(object sender, EventArgs e)
        {
            deleteAllItemsOrdered();
        }
    }
}
