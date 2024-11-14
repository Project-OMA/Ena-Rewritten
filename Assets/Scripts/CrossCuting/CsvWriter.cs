using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class CsvWriter
{
    public static void WriteToCsv<T>(IEnumerable<T> data, string fileName)
    {
        string filePath = Application.persistentDataPath + fileName;
        
        try
        {
            
            string csvContent = string.Empty;

           
            var properties = typeof(T).GetProperties();

            
            var header = properties
                .Where(prop => Attribute.IsDefined(prop, typeof(CsvColumnAttribute)))
                .Select(prop => GetCsvColumnName(prop));

            
            bool fileExists = File.Exists(filePath);

            if(fileName == "/playerlogs.csv"){

                if(fileExists){
                    File.Delete(filePath);
                    csvContent += string.Join(";", header) + Environment.NewLine;  
                }

                

            }else{

                if(!fileExists){
                    csvContent += string.Join(";", header) + Environment.NewLine; 
                }


            }

        

           
            foreach (var item in data)
            {
                var fields = properties
                    .Where(prop => Attribute.IsDefined(prop, typeof(CsvColumnAttribute)))
                    .Select(prop => GetCsvFieldValue(prop, item));

                csvContent += string.Join(";", fields) + Environment.NewLine;
            }

            File.AppendAllText(filePath, csvContent);

            Debug.Log("CSV File saved to: " + filePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error while writing CSV: " + ex.Message);
        }
    }

    
    private static string GetCsvColumnName(PropertyInfo property)
    {
        var attribute = (CsvColumnAttribute)Attribute.GetCustomAttribute(property, typeof(CsvColumnAttribute));
        return attribute.ColumnName;
    }

    private static string GetCsvFieldValue(PropertyInfo property, object item)
    {
        var value = property.GetValue(item);
        return value != null ? value.ToString() : string.Empty; // Handle null values
    }
}
