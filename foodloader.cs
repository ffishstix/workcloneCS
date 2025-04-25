using System;
using System.Collections.Generic;
using System.IO;
namespace WorkCloneCS
{
    public class FoodLoader
    {

        private static string filePathReturn(string filename)
        {
            return @$"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\workclonecs\{filename}.txt";
        }
        public static string[] LoadCatagorys(string c)
        {
            string filePath = filePathReturn(c);
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The {c}.txt file was not found at: " + filePath);

            return File.ReadAllLines(filePath);
            
        }
        public static List<(string Name, decimal Price, string extra)> LoadFoodItems(string c)
        {
            string filePath = filePathReturn(c);

            List<(string, decimal, string)> foodItems = [];

            if (!File.Exists(filePath)) return null;


            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                string[] parts = trimmed.Split(',');
                switch (parts.Length)
                {
                    case 1:
                        foodItems.Add((parts[0], 0, ""));
                        break;
                    case 2:
                        foodItems.Add((parts[0].Trim(), Convert.ToDecimal(parts[1])/100, ""));
                        break;
                    case 3:
                        foodItems.Add((parts[0].Trim(), Convert.ToDecimal(parts[1])/100, parts[2].Trim()));
                        break;
                    default:
                        continue;
                }
            }

            return foodItems;
        }
    }

}
