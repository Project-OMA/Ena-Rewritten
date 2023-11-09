using System;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
sealed class CsvColumnAttribute : Attribute
{
    public string ColumnName { get; }

    public CsvColumnAttribute(string columnName)
    {
        ColumnName = columnName;
    }
}