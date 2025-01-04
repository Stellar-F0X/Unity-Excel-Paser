using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


//자동으로 할당해주는 에디터 만들어볼까
public abstract class XmlDataCollection : ScriptableObject
{
    public Dictionary<string, Dictionary<string, List<object>>> xmlDataCollection;

    protected abstract Dictionary<string, Type> bindingTargets
    {
        get;
    }


    public virtual void Formatting()
    {
        var keys = xmlDataCollection.Keys.ToArray();

        for (int i = 0; i < keys.Length; i++)
        {
            var tables = xmlDataCollection[keys[i]].Values.ToList();

            for (int row = 0; row < tables[0].Count; ++row)
            {
                var target = Activator.CreateInstance(bindingTargets[keys[i]]);
                var fields = bindingTargets[keys[i]].GetFields();

                for (int colum = 0; colum < tables.Count; ++colum)
                {
                    object convertedValue = Convert.ChangeType(tables[colum][row], fields[colum].FieldType);
                    fields[colum].SetValue(target, convertedValue);
                }

                this.BindToDataTable(keys[i], target);
            }
        }
    }


    protected abstract void BindToDataTable(string key, object target);
}