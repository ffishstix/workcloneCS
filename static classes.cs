using System;
using System.Collections.Generic;

namespace WorkCloneCS
{
    class Logger
    {
        private static readonly string logFilePath = @"C:\workclonecs\log.html";
        private static int logCount = 0;
        public static void Here()
        {
            Log($"here{logCount}");
        }


        /*
        public static void Log(string message)
        {
            try
            {
                logCount++;
                string number = logCount.ToString();
                string formattedCount = number.PadLeft(4, '-');
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffff");
                string logEntry = $"{formattedCount}:{timestamp}: {message}";
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Logging failed: " + ex.Message);
            }
        }
        // Log function with severity handling
        
        */
        public static void Log(string message, int severity = 0)
        {
            try
            {
                logCount++;
                string number = logCount.ToString();
                string formattedCount = number.PadLeft(4, '-');
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffff");

                // Default to normal (no color)
                string color = "black"; // Black color for severity 0 (normal)

                // Apply colors based on severity level
                switch (severity)
                {
                    case 1:
                        color = "blue"; // Blue for info
                        break;
                    case 2:
                        color = "green"; // Green for success
                        break;
                    case 3:
                        color = "orange"; // Orange for warning
                        break;
                    case 4:
                        color = "red"; // Red for error
                        break;
                    default:
                        color = "black"; // Black (normal) for severity 0
                        break;
                }

                // Build the HTML log entry
                string logEntry = $"<p style='color:{color};'><b>{formattedCount}</b>: {timestamp}: {message}</p>";

                // Check if the log file exists
                if (!File.Exists(logFilePath))
                {
                    // If the file doesn't exist, write the HTML header
                    string htmlHeader = @"<!DOCTYPE html><html><head><title>Log File</title></head><body>";
                    File.WriteAllText(logFilePath, htmlHeader);
                }

                // Append the log entry to the file
                File.AppendAllText(logFilePath, logEntry);

                // Optionally close the HTML body and document if it's the last log entry
                // This could be done when closing the application or after a certain condition is met
            }
            catch (Exception ex)
            {
                Console.WriteLine("Logging failed: " + ex.Message);
            }
        }

        // Optional: Call this method when you're done logging to close the HTML document properly
        public static void CloseLog()
        {
            try
            {
                string htmlFooter = "</body></html>";
                if (File.Exists(logFilePath))
                {
                    File.AppendAllText(logFilePath, htmlFooter);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to close log file: " + ex.Message);
            }
        }
    }

        public class staff
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Access { get; set; }
    }

    class catagory
    {
        // this is purely for purposes such as how in the locally saved files 
        // there might not be certain properties from the database so i am just putting this here so i can 
        public bool connected { get; set; }
        public string catName { get; set; }
        public int catagoryId { get; set; }
        public string catagoryExtraInfo { get; set; }

        public List<item> items { get; set; }
    }

    class item
    {
        public int itemId { get; set; }
        public string itemName { get; set; }
        public string extraInfo { get; set; }

        public decimal price { get; set; }
        public string chosenColour { get; set; }

    }

    public class rowPanelTag
    {
        private string name;
        private int count;
        private decimal price;
        private int itemCount;
        public rowPanelTag()
        {
            name = "changeMe";
            count = 1;
            price = 0;
            itemCount = 1;
        }
        public string Name { get { return name; } set { name = value; } }
        public int Count { get { return count; } set { count = value; } }
        public decimal Price { get { return price; } set { price = value; } }
        public int ItemCount { get { return itemCount; } set { itemCount += value; } }
        public decimal TotalPrice
        {
            get { return itemCount * Price; }
        }

    }

    public class rowOfItem
    {
        private string foodName;
        private decimal price = 0;
        private int itemCount = 1;
        private int indexCount = 0;
        private int rowHeight = 40;

        private Label left, middle, right;

        private rowPanelTag Tag;

        public int maxWidth = 850;
        public FlowLayoutPanel rowPannel;
        public string FoodName { get { return foodName; } set { foodName = value; } }
        public decimal Price { get { return price; } set { price = value; } }
        public int ItemCount { get { return itemCount; } set { itemCount = value; } }
        public int IndexCount { get { return indexCount; } }
        public decimal TotalPrice { get { return itemCount * price; } }

        public Label Left { get { return left; } set { left = value; } }
        public Label Right { get { return right; } set { right = value; } }
        public Label Middle { get { return middle; } set { middle = value; } }
        public void IncreaseIndexCount()
        {
            indexCount++;
        }
        public rowOfItem()
        {
            foodName = "litterallly anythingelse";
            int countLabelWidth = 30;
            int priceLabelWidth = 60;
            Tag = new rowPanelTag()
            {
                ItemCount = itemCount,
                Name = FoodName,
                Count = indexCount,
                Price = price

            };


            rowPannel = new()
            {
                Height = rowHeight,
                Tag = Tag,
                Padding = new Padding(0),
                Margin = new Padding(0),
                Width = maxWidth - SystemInformation.VerticalScrollBarWidth,
                AutoSize = false,
                AutoScroll = false,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Green
            };

            left = new Label
            {
                Text = itemCount.ToString(),
                Width = countLabelWidth,
                Height = rowHeight,
                AutoSize = false,
                Padding = new Padding(0),
                Margin = new Padding(0),
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Yellow

            };

            middle = new Label
            {
                Text = foodName,
                Height = rowHeight,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Padding = new Padding(10, 0, 0, 0),
                Tag = Tag,
                Width = maxWidth - countLabelWidth - priceLabelWidth - SystemInformation.VerticalScrollBarWidth - 9
            };

            right = new Label
            {
                Text = price.ToString("c"),
                Tag = Tag,
                Width = priceLabelWidth,
                Height = rowHeight,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Padding = new Padding(0),
                BackColor = Color.Red,

            };

            rowPannel.Controls.Add(left);
            rowPannel.Controls.Add(middle);
            rowPannel.Controls.Add(right);

        }
        public void SetHeight(int height)
        {
            rowHeight = height;
            left.Height = height;
            middle.Height = height;
            right.Height = height;
            rowPannel.Height = height;



        }


        public void updateText()
        {
            left.Text = ItemCount.ToString();
            middle.Text = foodName;
            right.Text = TotalPrice.ToString("c");
        }

        public void Dispose()
        {
            foreach (Control control in rowPannel.Controls)
            {
                if (control == Left || control == Right || control == middle)
                {
                    foreach (Control control2 in control.Controls)
                    {
                        control.Controls.Remove(control2);
                        control2.Dispose();
                    }
                }
                rowPannel.Controls.Remove(control);
                control.Dispose();
            }
        }

    }
}
