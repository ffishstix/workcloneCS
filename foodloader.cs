using System;
using System.Collections.Generic;
using System.IO;
namespace WorkCloneCS
{
    public class FoodLoader
    {

        public static string filePathReturn(string filename)
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
        public static List<rowPanelTag> LoadFoodItems(string c)
        {
            string filePath = filePathReturn(c);

            if (!File.Exists(filePath)) return null;

            string[] lines = File.ReadAllLines(filePath);
            List<rowPanelTag> temp = new List<rowPanelTag>();

            for (int i = 0; i < lines.Length; i++)
            {
                string trimmed = lines[i].Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                string[] parts = trimmed.Split(',');
                rowPanelTag item = new rowPanelTag();

                switch (parts.Length)
                {
                    case 1:
                        item.Name = parts[0];
                        item.Price = 0;
                        break;
                    case 2:
                        item.Name = parts[0];
                        item.Price = (decimal)Convert.ToInt32(parts[1]) / 100;
                        break;
                    default:
                        continue;
                }

                temp.Add(item);
            }

            return temp;
        }

    }

}
