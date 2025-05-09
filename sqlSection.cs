using System;
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.ApplicationServices;
using System.Text.Json;
namespace WorkCloneCS
{
    // logger function completely made by ai i take no responsibility
    class Logger
    {
        private static readonly string logFilePath = @"C:\workclonecs\log.txt";

        public static void Log(string message)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string logEntry = $"{timestamp}: {message}";
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
        public int Access {  get; set; }
    }

    public class item
    {
        public int itemId { get; set; }
        public string itemName { get; set; }
        public string extraInfo { get; set; }
        public string catName { get; set; }
        public int catagoryId { get; set; }
        public string catagoryExtraInfo { get; set; }
        public decimal price { get; set; }

    }

    class SQL
    {
        private const string connectionString = "Server=localhost\\SQLEXPRESS;" +
                "Database=testDatabase;" +
                "Trusted_Connection=True;" +
                "Encrypt=False;";
        private static void print(string a)
        {
            System.Diagnostics.Debug.WriteLine("\n\nmsg: {0}\n", a);
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
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                int access = reader.GetInt32(2);
                                values.Add(new staff { 
                                    Id = id, 
                                    Access = access, 
                                    Name = name 
                                });
                                Logger.Log($"ID: {id}, Name: {name}, accessLevel: {access}");
                                
                                
                            }
                            try
                            {
                                string jsonString = JsonSerializer.Serialize(values, new JsonSerializerOptions { WriteIndented = true });
                                string dir = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/workclonecs/sql/staff.txt";
                                File.WriteAllText(dir, jsonString);
                            } catch (Exception ex) { 
                                Console.WriteLine(ex.Message);
                                Logger.Log(ex.Message);                            
                            }
                            return values;
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex.Message);
                        Console.WriteLine("Error: " + ex.Message);
                        string file = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/workclonecs/sql/staff.txt";
                        if (File.Exists(file)) {
                            List<staff> staff = JsonSerializer.Deserialize<List<staff>>(File.ReadAllText(file));
                            return staff;

                        }
                    }
                }
            } 
            catch (SqlException ex)
            {
                Logger.Log(ex.Message);
                print("SQL Exception: " + ex.Message);
            }
            return null;


        }
    
        public static List<item> getItemsFromCatagory(int catagoryChosen)
        {
            List<item> values = new List<item>();
            string query = 
    "SELECT cat.catagoryId, catName, itemName, price, " +
    "ISNULL(ai.extraInfo, '') AS extraInfo, ISNULL(cat.extraInfo, '') AS catExtraInfo " +
    "FROM allItems ai " +
    "JOIN [foodCatagory] foo ON ai.itemID = foo.itemId " +
    "JOIN [catagories] cat ON cat.catagoryId = foo.catagoryId " +
    "WHERE cat.catagoryId = @catagoryId " +
    "AND cat.catagoryId IS NOT NULL " +
    "AND catName IS NOT NULL " +
    "AND itemName IS NOT NULL " +
    "AND price IS NOT NULL " +
    "ORDER BY ai.itemId";
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
                                values.Add(new item()
                                {
                                    catagoryId = reader.GetInt32(0),
                                    catName = reader.GetString(1),
                                    itemName = reader.GetString(2),
                                    price = (decimal)reader.GetInt32(3)/100,
                                    extraInfo = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    catagoryExtraInfo = reader.IsDBNull(4) ? null : reader.GetString(5)
                                });
                                Logger.Log($"got: item: {reader.GetString(2)}");
                            }
                            return values;
                        }
                    }
                    catch (Exception ex) {
                        //backup method just incase server down
                        Logger.Log(ex.Message);
                        values = null;
                        List<rowPanelTag> file = FoodLoader.LoadFoodItems($"catagories{catagoryChosen}");
                        if (file != null)
                        {
                            foreach (rowPanelTag f in file)
                            {
                                values.Add(new item()
                                {
                                    itemName = f.Name,
                                    price = f.Price,
                                    catagoryId = catagoryChosen
                                });
                            }
                            return values;
                        }



                    }

                }
            }
            catch (Exception ex) {
                Logger.Log($"exception occured with item connection: {ex.Message}");
            }

            return null;
        }
    
    }
}
