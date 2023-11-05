using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class CsvWriter
{
    public static void WriteToCsv<T>(string filePath, IEnumerable<T> data)
    {
        using (var writer = new StreamWriter(filePath))
        {
            var header = typeof(T).GetProperties().Select(prop => prop.Name);
            writer.WriteLine(string.Join(",", header));

            Debug.Log($"Stopping: {data.Count()}");
            foreach (var item in data)
            {
                writer.WriteLine(string.Join(",", GetCsvFields(item)));
            }
        }
    }

    private static IEnumerable<string> GetCsvFields<T>(T item)
    {
        foreach (var property in typeof(T).GetProperties())
        {
            yield return property.GetValue(item).ToString();
        }
    }
}
