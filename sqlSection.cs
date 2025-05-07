using System;
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.ApplicationServices;
using System.Text.Json;
namespace WorkCloneCS
{
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

    class SQL
    {
        private static void print(string a)
        {
            System.Diagnostics.Debug.WriteLine("\n\nmsg: {0}\n", a);
        }

        public static void tmep()
        {
            Logger logger = new Logger();
            print("here");
            string connectionString = "Server=localhost\\SQLEXPRESS;" +
                "Database=testDatabase;" +
                "Trusted_Connection=True;" +
                "Encrypt=False;";

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
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex.Message);
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            } 
            catch (SqlException ex)
            {
                Logger.Log(ex.Message);
                print("SQL Exception: " + ex.Message);
            }
            


        }
    }
}
