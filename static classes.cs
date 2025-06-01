using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace WorkCloneCS
{
    class Logger
    {
        private static readonly string logFilePath = @$"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\workclonecs\log.txt";
        private static int logCount = 0;
        public static void Here()
        {
            Log($"here{logCount}");
        }


        public static void Log(string message)
        {
            try
            {
                if (!File.Exists(logFilePath)) File.Create(logFilePath);
                logCount++;
                string number = logCount.ToString();
                string formattedCount = number.PadLeft(4, '-');
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffff");
                string logEntry = $"{formattedCount}:{timestamp}: {message}";
                SQL.print(logEntry);
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Logging failed: " + ex.Message);
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
    
    class SQL
    {

        private static bool testFiles = false;
        private static List<catagory> catagoriesFromFile;
        private static string jsonDir;
        private static string jsonstaffDir;
        private static string connectionString;
        
        public static void initSQL()
        {
            
            string sql = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/workCloneCs/sql/";
            jsonDir = sql + "catagoryJson.txt";
            jsonstaffDir = sql + "staff.txt";
            catagoriesFromFile = pullCatFile();
            if (!testFiles)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(sql + "ConnectionStringsConfiguration.json")
                    .Build();
                connectionString = configuration.GetConnectionString("DefaultConnection");
            }
            else connectionString = "Data Source=localhost;Initial Catalog=workclonecs;Integrated Security=True";
        }
        
        public static void print(string a)
        {
            Console.WriteLine(a);
            System.Diagnostics.Debug.WriteLine("\n\nmsg: {0}\n", a);
        }

        public static (int, int) getRangeOfCatagoryID()
        {
            string query = "SELECT top 1 catagoryId " +
                "from catagories " +
                "order by catagoryId ";
            int min = 0;
            int max = 0;
            try
            {

                //main method
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(query + " desc ", connection);
                        Logger.Log(command.Parameters.ToString());
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                max = reader.GetInt32(0);
                                Logger.Log($"max: {min}");
                            }
                        }

                        command = new SqlCommand(query + "asc ", connection);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                min = reader.GetInt32(0);
                                Logger.Log($"max: {max}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return errorCallCI(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                return errorCallCI(ex);

            }

            return (min, max);
        }
        
        public static List<staff> getStaffData()
        {
            string query = "select * from staff order by id desc";
            List<staff> values= new List<staff>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        print("/n/n/n/nhere/n/n/n/n");
                        using (SqlCommand command = new SqlCommand(query, connection))
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            try
                            {
                                while (reader.Read())
                                {
                                    int id = reader.GetInt32(0);
                                    string name = reader.GetString(1);
                                    int access = reader.GetInt32(2);
                                    values.Add(new staff
                                    {
                                        Id = id,
                                        Access = access,
                                        Name = name
                                    });
                                    Logger.Log($"ID: {id}, Name: {name}, accessLevel: {access}");


                                }

                                try
                                {
                                    // logging it just incase it cannot pull it next time
                                    string jsonString = JsonSerializer.Serialize(values,
                                        new JsonSerializerOptions { WriteIndented = true });
                                    
                                    File.WriteAllText(jsonstaffDir, jsonString);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    Logger.Log(ex.Message);
                                }

                                return values;
                            }
                            catch (Exception ex)
                            {
                                return errorCallSD(ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return errorCallSD(ex);
                    }
                }
            } 
            catch (SqlException ex)
            {
                return errorCallSD(ex);
            }
            return null;


        }

        
        private static List<catagory> pullCatFile()
        {
            if (File.Exists(jsonDir))
            {
                try
                {
                    string json = File.ReadAllText(jsonDir); // Read file contents
                    List<catagory> fileJson = JsonSerializer.Deserialize<List<catagory>>(json); // Deserialize JSON text
                    Logger.Log("json was read");
                    return fileJson;
                }
                catch (Exception ex)
                {
                    Logger.Log($"pullCatFile() failed: {ex.Message}");
                    return null;
                }
            }
            Logger.Log(jsonDir + " doesn't exist");
            return null;
        }

        public static catagory getCatagory(int catagoryChosen)
        {
            catagory currentCatagory = new catagory();
            List<item> values = new List<item>();
            string query = 
            "SELECT cat.catagoryId, catName, itemName, price, " +
            "ISNULL(ai.extraInfo, '') AS extraInfo, ISNULL(cat.extraInfo, '') AS catExtraInfo, " +
            "isnull(ai.chosenColour, 'grey') as chosenColour " +
            "FROM allItems ai " +
            "JOIN [foodCatagory] foo ON ai.itemID = foo.itemId " +
            "JOIN [catagories] cat ON cat.catagoryId = foo.catagoryId " +
            "WHERE cat.catagoryId = @catagoryId " +
            "AND cat.catagoryId IS NOT NULL " +
            "AND catName IS NOT NULL " +
            "AND itemName IS NOT NULL " +
            "AND price IS NOT NULL " +
            "ORDER BY cat.catagoryId ";
            
            try
            {
                
                //main method
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@catagoryId", catagoryChosen);
                        print("/n/n/n/nhere/n/n/n/n");
                        using (command)
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                currentCatagory.catagoryId = reader.GetInt32(0);
                                currentCatagory.catName = reader.GetString(1);
                                currentCatagory.catagoryExtraInfo = reader.IsDBNull(4) ? null : reader.GetString(5);
                                values.Add(new item()
                                {
                                    itemName = reader.GetString(2),
                                    price = (decimal)reader.GetInt32(3)/100,
                                    extraInfo = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    chosenColour = reader.GetString(6),
                                });
                                Logger.Log($"got: item: {reader.GetString(2)}");
                            }
                            currentCatagory.items = values;
                            currentCatagory.connected = false;
                            try
                            {
                                List<catagory> fileJson = new List<catagory>();
                                
                                

                                if (File.Exists(jsonDir))
                                {
                                    Logger.Here();
                                    fileJson = pullCatFile();

                                    if (fileJson == null)
                                    {
                                        Logger.Log("file was null or weird so I'm starting again");
                                        fileJson = new List<catagory>();
                                    }
                                }
                                else
                                {
                                    Logger.Log($"{jsonDir} doesn't exist, creating a new one");
                                    fileJson = new List<catagory>();
                                }
                                bool ah = false;
                                foreach (catagory cat in fileJson)
                                {
                                    if (cat != null)
                                    {
                                        if ((cat.catName == currentCatagory.catName || cat.catagoryId == currentCatagory.catagoryId))
                                        {
                                            ah = true;
                                        }
                                    }
                                    
                                }
                                if (!ah) fileJson.Add(currentCatagory);
                                string jsonStrings = JsonSerializer.Serialize(fileJson, new JsonSerializerOptions { WriteIndented = true });
                                File.WriteAllText(jsonDir, jsonStrings);
                            }
                            catch (Exception ex)
                            {
                                Logger.Log($"error while logging items {ex.Message}");
                            }
                            currentCatagory.connected = true;
                            return currentCatagory;

                        }
                    }
                    catch (Exception ex) {
                        //backup method just incase server down
                        
                        return errorCallGC(ex, catagoryChosen);

                    }

                }
            }
            catch (Exception ex) {
                return errorCallGC(ex, catagoryChosen);

            }

        }
        
        
        // only used in getCatagory when catch is called
        private static catagory errorCallGC(Exception ex, int catagoryChosen)
        {
            Logger.Log("\ncouldn't connect so am resorting to backup\n");
            Logger.Log(ex.Message);
            Logger.Here();
            if (catagoriesFromFile != null)
            {
                Logger.Log("tbf i think it worked just have a quick look tbf");
                foreach (catagory cat in catagoriesFromFile) 
                {
                    Logger.Log($"catID {cat.catagoryId}, chosen cat: {catagoryChosen}");
                    if (cat.catagoryId == catagoryChosen) return cat;
                }
            }
                        
            Logger.Log("catagories file or cat doesnt exist so :(");
            return null;
        }
        
        //only using in getRangeOfCatagoryID when catch is called
        private static (int, int) errorCallCI(Exception ex)
        {
            int min = 0;
            int max = 0;
            Logger.Log(ex.Message);
            List<int> d = new List<int>();
            //couldnt connect or something so 
            if (catagoriesFromFile != null)
            {
                foreach (catagory cat in catagoriesFromFile)
                {
                    d.Add(cat.catagoryId);
                }

                min = d.Min();
                max = d.Max();    
            }
            // else basically means we are screwed icl
            else
            {
                min = 0;
                max = 0;
            }
            return (min, max);
        }
        
        //only used in getStaffData when catch is called
        private static List<staff> errorCallSD(Exception ex)
        {
            Logger.Log(ex.Message);
            Console.WriteLine("Error: " + ex.Message);
            string file = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/workclonecs/sql/staff.txt";
            if (File.Exists(file)) {
                List<staff> staff = JsonSerializer.Deserialize<List<staff>>(File.ReadAllText(file));
                return staff;

            }

            return null;
        }
    }
    
}
