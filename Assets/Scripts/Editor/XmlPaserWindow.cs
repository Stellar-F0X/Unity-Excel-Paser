using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

using Object = UnityEngine.Object;
using System.Linq;
using System.IO;

public class XmlPaserWindow : EditorWindow
{
    [MenuItem("Tools/Xml Paser")]
    public static void CreatePaserWindow()
    {
        XmlPaserWindow xmlPaser = (XmlPaserWindow)GetWindow(typeof(XmlPaserWindow));
        xmlPaser.Show();
    }

    private const string XML_EXTENSION = ".xlsx";

    private Rect _dropArea;
    private Vector2 _inAreaOffset;
    private Lazy<List<string>> _xmlPathList = new Lazy<List<string>>(() => new());



    private void OnEnable()
    {
        position = new Rect(position.x, position.y, 290, 400);

        this.minSize = new Vector2(290, 400);
        this.maxSize = new Vector2(290, 400);

        _dropArea = new Rect(0, position.height - 70f, position.width, 70f);
    }


    private void OnGUI()
    {
        this.DrawEditorGUIElments();
        this.GenerateDragDropBoxField();
        this.ReceiveXmlAssets();
    }


    private void ReceiveXmlAssets()
    {
        if (Event.current.type == EventType.DragUpdated && _dropArea.Contains(Event.current.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }

        if (Event.current.type == EventType.DragPerform && DragAndDrop.paths.Length > 0)
        {
            if (_xmlPathList.Value.Count < 12)
            {
                this.GetXmlFilePath(DragAndDrop.paths);
            }
            else
            {
                //추후 핑 콘솔 가르키는 기능 추가
                Debug.Log("한번에 12개만 가능");
            }
        }
    }


    private void GetXmlFilePath(string[] xmlPathes)
    {
        foreach (var xmlPath in xmlPathes)
        {
            if (File.Exists(xmlPath) && Path.GetExtension(xmlPath).ToLower() == XML_EXTENSION)
            {
                _xmlPathList.Value.Add(xmlPath);
            }
            else
            {
                Debug.Log("경로가 잘못되었거나, 지원하지 않는 파일 유형");
            }
        }
    }


    private void DrawEditorGUIElments()
    {
        GUILayout.Label("Parsing XML", EditorStyles.objectFieldThumb);
        float y = EditorGUIUtility.singleLineHeight * 1.5f;

        var areaRect = new Rect(5, y, position.width - 10, position.height - y - 80f);
        using (var scope = new GUILayout.AreaScope(areaRect, string.Empty, GUI.skin.window))
        {
            _inAreaOffset = new Vector2(0, 5);

            float startPosition = position.width / 1.25f;
            float endPosition = position.width / 1.3f;

            this.DrawXmlPathList(startPosition, endPosition);

            Rect buttonSize = new Rect(_inAreaOffset.x + 5, position.height - 130, position.width - 20, EditorGUIUtility.singleLineHeight);

            if (GUI.Button(buttonSize, "Parsing XML", EditorStyles.miniButton) && _xmlPathList.IsValueCreated && _xmlPathList.Value.Count > 0)
            {
                XmlPaser.ReadXmlData(_xmlPathList.Value.ToArray());
            }
        }

        _inAreaOffset = Vector2.zero;
    }



    private void DrawXmlPathList(float startX, float endX)
    {
        for (int i = 0; i < _xmlPathList.Value.Count; ++i)
        {
            var asset = _xmlPathList.Value[i];

            if (asset == null)
            {
                continue;
            }

            var textFieldRect = new Rect(_inAreaOffset, new Vector2(endX, EditorGUIUtility.singleLineHeight));
            var buttonRect = new Rect(new Vector2(startX, _inAreaOffset.y), new Vector2(position.width - startX - 15, textFieldRect.height));

            if (GUI.Button(buttonRect, "Del", EditorStyles.miniButton))
            {
                _xmlPathList.Value.Remove(asset);
            }
            else
            {
                _inAreaOffset.y += EditorGUIUtility.singleLineHeight * 1.2f;

                using (var scope = new EditorGUI.DisabledGroupScope(true))
                {
                    GUI.TextField(textFieldRect, asset);
                }
            }
        }
    }



    private void GenerateDragDropBoxField()
    {
        //var boxStyle = EditorStyles.helpBox;
        //boxStyle.fontStyle = FontStyle.Bold;

        Color32 originalColor = GUI.backgroundColor;

        GUI.backgroundColor = new Color32(50, 50, 50, 255);
        GUI.Box(_dropArea, "Drag & Drop Field");
        GUI.backgroundColor = originalColor;
    }
}