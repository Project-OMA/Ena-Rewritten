using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class CsvWriter
{
    


    public static int WriteToCsv<T>(IEnumerable<T> data, string fileName, int lastLogPos)
    {
        string filePath = Application.persistentDataPath + fileName;
        int current = 0;
        try
        {
            
            string csvContent = string.Empty;

           
            var properties = typeof(T).GetProperties();

            
            var header = properties
                .Where(prop => Attribute.IsDefined(prop, typeof(CsvColumnAttribute)))
                .Select(prop => GetCsvColumnName(prop));

            
            bool fileExists = File.Exists(filePath);

   

            if(!fileExists){
                csvContent += string.Join(";", header) + Environment.NewLine; 
            }

            foreach (var item in data)
            {

                current +=1;

                if(current>lastLogPos){

                    var fields = properties
                    .Where(prop => Attribute.IsDefined(prop, typeof(CsvColumnAttribute)))
                    .Select(prop => GetCsvFieldValue(prop, item));

                    csvContent += string.Join(";", fields) + Environment.NewLine;
    
                }
            }

            File.AppendAllText(filePath, csvContent);

            Debug.Log("CSV File saved to: " + filePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error while writing CSV: " + ex.Message);
        }

        return current;
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
