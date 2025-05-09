using System;
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.ApplicationServices;
using System.Text.Json;
namespace WorkCloneCS
{
    // logger function completely made by ai i take no responsibility
    

    class SQL
    {
        private static string jsonDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}" +
                "/workclonecs/sql/catagoryJson.txt";
        private const string connectionString = "Server=localhost\\SQLEXPRESS;" +
                "Database=testDatabase;" +
                "Trusted_Connection=True;" +
                "Encrypt=False;";
        private static void print(string a)
        {
            System.Diagnostics.Debug.WriteLine("\n\nmsg: {0}\n", a);
        }

        public static (int, int) getRangeOfCatagoryID()
        {
            string query = "SELECT catagoryId " +
                "from catagories " +
                "order by catagoryId ";
            string fetch = "offset 0 rows fetch next 1 rows only";
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
                        SqlCommand command = new SqlCommand(query + "desc " + fetch, connection);
                        Logger.Log(command.Parameters.ToString());
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                max = reader.GetInt32(0);
                                Logger.Log($"max: {min}");
                            }
                        }
                        command = new SqlCommand(query + "asc " + fetch, connection);
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
                        Logger.Log(ex.Message);
                    }
                }
                
            } catch (Exception ex) { Logger.Log(ex.Message); }
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
                                // logging it just incase it cannot pull it next time
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

        private static List<catagory> pullCatFile()
        {
            if (File.Exists(jsonDir))
            {
                try
                {
                    string json = File.ReadAllText(jsonDir); // Read file contents
                    List<catagory> fileJson = JsonSerializer.Deserialize<List<catagory>>(json); // Deserialize JSON text
                    return fileJson;
                }
                catch (Exception ex)
                {
                    Logger.Log($"pullCatFile() failed: {ex.Message}");
                    return null;
                }
            }
            else return null;
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
                                Logger.Log("\ncouldn't connect so am resorting to backup\n");

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
                                    Logger.Log("file doesn't exist, creating a new one");
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
                        Logger.Log(ex.Message);
                        Logger.Here();
                        List<catagory> x = pullCatFile();
                        if (x == null) return null;
                        else {
                            Logger.Log("tbf i think it worked just have a quick look tbf");
                            foreach (catagory cat in x) {
                                Logger.Log($"catID {cat.catagoryId}, chosen cat: {catagoryChosen}");
                                if (cat.catagoryId == catagoryChosen) return cat;
                            }
                            return x[catagoryChosen]; 
                        }
                        




                    }

                }
            }
            catch (Exception ex) {
                Logger.Log($"exception occured with item connection: {ex.Message}");
                Logger.Log("gonna try and return local files");
                return pullCatFile()[catagoryChosen];

                
            }

        }
    
    }
}
