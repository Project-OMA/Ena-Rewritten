using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class CsvWriter
{
    public static void WriteToCsv<T>(string filePath, IEnumerable<T> data)
    {
        using (var writer = new StreamWriter(filePath))
        {
            var properties = typeof(T).GetProperties();
            var header = properties
                .Where(prop => Attribute.IsDefined(prop, typeof(CsvColumnAttribute)))
                .Select(prop => GetCsvColumnName(prop));

            writer.WriteLine(string.Join(",", header));

            Debug.Log($"Stopping: {data.Count()}");
            foreach (var item in data)
            {
                var fields = properties
                    .Where(prop => Attribute.IsDefined(prop, typeof(CsvColumnAttribute)))
                    .Select(prop => GetCsvFieldValue(prop, item));

                writer.WriteLine(string.Join(",", fields));
            }
        }
    }

    private static string GetCsvColumnName(PropertyInfo property)
    {
        var attribute = (CsvColumnAttribute)Attribute.GetCustomAttribute(property, typeof(CsvColumnAttribute));
        return attribute.ColumnName;
    }

    private static string GetCsvFieldValue(PropertyInfo property, object item)
    {
        return property.GetValue(item).ToString();
    }
}
