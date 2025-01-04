using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;
using ExcelDataReader;
using UnityEditor;
using Codice.Client.BaseCommands;

public sealed class XmlPaser
{
    private const string _RELATION_PATH = "Assets/Resources/Parsed Data/";

    //메모이제이션용 컬렉션들
    private static Lazy<Dictionary<string, Dictionary<string, List<object>>>> _xmlData = new(() => new());
    private static Lazy<List<string>> _keyList                                         = new(() => new());

    private Dictionary<string, Dictionary<string, List<object>>> xmlData
    {
        get { return _xmlData.Value;}
    }

    private List<string> keyList
    {
        get { return _keyList.Value; }
    }


    public static void ReadXmlData(string[] paths)
    {
        var xmlPaser = new XmlPaser();

        xmlPaser.Initialize();
        xmlPaser.ParsingXmlData(paths);
    }


    private void Initialize()
    {
        xmlData.Clear();
        keyList.Clear();
    }

    private void ParsingXmlData(string[] paths)
    {
        foreach (var path in paths)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                continue;
            }

            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var readableDataSet = reader.AsDataSet();

                for (int i = 0; i < readableDataSet.Tables.Count; ++i)
                {
                    var table = readableDataSet.Tables[i] ?? throw new Exception("Sheet not exist");
                    var data = this.ReadSheet(table.Rows ?? throw new Exception("data not exist"));
                    xmlData.TryAdd(table.TableName, data);
                }
                this.CreateXmlDataContainer(Path.GetFileNameWithoutExtension(path));
            }
            this.Initialize();
            stream.Close();
        }
    }

    private Dictionary<string, List<object>> ReadSheet(DataRowCollection tables)
    {
        Dictionary<string, List<object>> subXmlData = new();

        for (int col = 0; col < tables.Count; ++col)
        {
            for (int row = 0; row < tables[col].ItemArray.Length; ++row)
            {
                if (col == 0)
                {
                    keyList.Add(tables[0][row].ToString());
                    subXmlData.Add(keyList[row], new List<object>());
                }
                else
                {
                    subXmlData[keyList[row]].Add(tables[col][row].ToString());
                }
            }
        }

        return subXmlData;
    }

    private void CreateXmlDataContainer(string tableName)
    {
        if (!Directory.Exists(_RELATION_PATH))
        {
            Directory.CreateDirectory(_RELATION_PATH);
        }

        string index = "";
        while (File.Exists($"{_RELATION_PATH}{tableName} {index}.asset"))
        {
            if (string.IsNullOrEmpty(index))
            {
                index = "1";
            }
            else
            {
                index = $"{int.Parse(index) + 1}";
            }
        }

        //추후 factory로 교체
        XmlDataCollection dataCollection = new DialogDataCollection();
        dataCollection.xmlDataCollection = xmlData;
        dataCollection.Formatting();

        AssetDatabase.CreateAsset(dataCollection, $"{_RELATION_PATH}{tableName} {index}.asset");
    }
}