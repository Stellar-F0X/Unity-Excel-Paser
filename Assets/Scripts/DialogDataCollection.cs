using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DialogDataCollection : XmlDataCollection
{
    public List<DialogueDataTable> _dialogueDatatables = new List<DialogueDataTable>();
  

    private Dictionary<string, Type> _bindingTargets = new () {
        { "Script",  typeof(DialogueDataTable)  }
      
    };


    protected override Dictionary<string, Type> bindingTargets
    {
        get { return _bindingTargets; }
    }


    protected override void BindToDataTable(string keyName, object item)
    {
        switch (keyName)
        {
            case "Script": _dialogueDatatables.Add(item as DialogueDataTable); break;
            default: throw new NotSupportedException();
        }
    }
}



[Serializable]
public class DialogueDataTable
{
    public string ID;
    public string QuestionText;
    public string ResponseText;
    public string Evidence;
}


