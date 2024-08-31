using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    public Text fileName;
    public InputField fileNameInputField;
    public Button findFileButton;//这个默认不能UnEnable
    public Text objectCount;
    public Text operateIndexDisplay2;
    
    public Button thePreviousOneButton;
    public Button theNextOneButton;
    public Button addNewListButton;
    public Button saveListChangesButton;
    public Button saveAllChangesButton;
    
    public Text timeLimitDisplay;
    public InputField[] elementInputFields;
    public Text saveSuccessfulTip;
    public Text saveAllChangesSuccessfulTip;
    public InputField timeLimitInputField;
    public Text saveFailedTip;

    private MusicList _operatingObject;
    private string _path;//存储路径
    
    private string _fileName;
    private int _indexOfLists;
    private int IndexOfLists//当前正在操作的列表索引
    {
        get
        {
            return _indexOfLists;
        }
        set
        {
            if (value < 0) { }
            else if (value >= _operatingObject.NoteLists.Count-1)
            {
                _indexOfLists = _operatingObject.NoteLists.Count-1;
            }
            else
            {
                _indexOfLists = value;
            }
        }
    }
    void Start()
    {
        _indexOfLists = 0;
        UnEnableAllButtons();
        HideSaveSuccessText();
        HideSaveAllChangesSuccessText();
        HideFailedText();
    }

    public void OnClickFindFileButton()
    {
        _fileName = fileNameInputField.text;
        string path = Application.persistentDataPath + $"/{_fileName}.xml";
        _path = path;
        if (!File.Exists(path))
        {
            fileName.text = "文件不存在！";
            return;
        }

        using (StreamReader reader = new StreamReader(path))
        {
            XmlSerializer s = new XmlSerializer(typeof(MusicList));
            _operatingObject = s.Deserialize(reader) as MusicList;
        }

        fileName.text = fileNameInputField.text;
        ListSafetyCheck();
        DisplayObjectCounts();
        EnableAllButtons();
        DisplayTimeLimit();
        DisPlayListContent();
    }

    public void OnClickCreateFileButton()
    {
        _fileName = fileNameInputField.text;
        string path = Application.persistentDataPath + $"/{_fileName}.xml";
        _path = path;
        if (File.Exists(path))
        {
            fileName.text = "文件已存在,请不要重复创建同名文件";
            return;
        }

        MusicList musicList = new MusicList();
        NoteList noteList = new NoteList();
        noteList.TimeLimit = 0f;
        List<char> list = new List<char>();
        ListSerialize(list);
        noteList.SingleNoteList = list;
        musicList.NoteLists = new List<NoteList>(){noteList};
        
        using (StreamWriter writer = new StreamWriter(path))
        {
            XmlSerializer s = new XmlSerializer(typeof(MusicList));
            s.Serialize(writer,musicList);
        }
        
        using (StreamReader reader = new StreamReader(path))
        {
            XmlSerializer s = new XmlSerializer(typeof(MusicList));
            _operatingObject = s.Deserialize(reader) as MusicList;
        }

        fileName.text = fileNameInputField.text;
        ListSafetyCheck();
        DisplayObjectCounts();
        EnableAllButtons();
        DisplayTimeLimit();
        DisPlayListContent();
    }
    private void ListSafetyCheck()
    {
        foreach (var noteList in _operatingObject.NoteLists)
        {
            ListSerialize(noteList.SingleNoteList);
        }
    }

    private void ListSerialize(List<char> list)
    {
        while (list.Count<8)
        {
            list.Add('\0');
        }
    }

    private void EnableAllButtons()
    {
        thePreviousOneButton.interactable = true;
        theNextOneButton.interactable = true;
        addNewListButton.interactable = true;
        saveListChangesButton.interactable = true;
        saveAllChangesButton.interactable = true;
    }

    private void UnEnableAllButtons()
    {
        thePreviousOneButton.interactable = false;
        theNextOneButton.interactable = false;
        addNewListButton.interactable = false;
        saveListChangesButton.interactable = false;
        saveAllChangesButton.interactable = false;
    }

    private void DisplayObjectCounts()
    {
        objectCount.text = $"文件中共有 {_operatingObject.NoteLists.Count} 个类对象";
        operateIndexDisplay2.text = $"第 {IndexOfLists} / {_operatingObject.NoteLists.Count-1} 个列表";
        // Debug.Log($"此时_operatingObject.NoteList.Count的值为:{_operatingObject.NoteLists.Count}");
    }

    public void OnClickThePreviousOneButton()
    {
        IndexOfLists -= 1; 
        DisplayObjectCounts();
        DisplayTimeLimit();
        DisPlayListContent();
    }

    public void OnClickTheNextOneButton()
    {
        IndexOfLists += 1;
        DisplayObjectCounts();
        DisplayTimeLimit();
        DisPlayListContent();
    }

    public void OnClickAddNewListButton()
    {
        NoteList noteList = new NoteList();
        noteList.TimeLimit = 2.4f;
        ListSerialize(noteList.SingleNoteList);
        _operatingObject.NoteLists.Add(noteList);
        DisplayObjectCounts();
        DisplayTimeLimit();
    }

    private void DisplayTimeLimit()
    {
        timeLimitInputField.text = $"{_operatingObject.NoteLists[IndexOfLists].TimeLimit}";
    }
    
    //刷新列表内输入框显示的内容
    private void DisPlayListContent()
    {
        int i = 0;
        foreach (var note in _operatingObject.NoteLists[IndexOfLists].SingleNoteList)
        {
            elementInputFields[i].text =$"{note}";
            i++;
        }
    }
    
    //将输入框的内容覆写原列表
    public void OnClickSaveListChangesButton()
    {
        try
        {
            _operatingObject.NoteLists[IndexOfLists].TimeLimit = float.Parse(timeLimitInputField.text);
        }
        catch (FormatException e)
        {
            ShowFailedText();
            Invoke(nameof(HideFailedText),1.5f);
            return;
        }
        int i = 0;
        foreach (var inputField in elementInputFields)
        {
            if (inputField.text.Equals(null))
            {
                _operatingObject.NoteLists[IndexOfLists].SingleNoteList[i] = '\0';
            }
            else if(inputField.text.Equals("a")||inputField.text.Equals("A"))
            {
                _operatingObject.NoteLists[IndexOfLists].SingleNoteList[i] ='a';
            }
            else if(inputField.text.Equals("w")||inputField.text.Equals("W"))
            {
                _operatingObject.NoteLists[IndexOfLists].SingleNoteList[i] ='w';
            }
            else if(inputField.text.Equals("s")||inputField.text.Equals("S"))
            {
                _operatingObject.NoteLists[IndexOfLists].SingleNoteList[i] ='s';
            }
            else if(inputField.text.Equals("d")||inputField.text.Equals("D"))
            {
                _operatingObject.NoteLists[IndexOfLists].SingleNoteList[i] ='d';
            }
            else
            {
                _operatingObject.NoteLists[IndexOfLists].SingleNoteList[i] = '\0';
            }
            i++;
        }
        
        ShowSaveSuccessText();
        Invoke(nameof(HideSaveSuccessText),1.5f);
    }

    public void OnClickSaveAllChangesButton()
    {
        using StreamWriter writer = new StreamWriter(_path);
        XmlSerializer s = new XmlSerializer(typeof(MusicList));
        s.Serialize(writer,_operatingObject);
        ShowSaveAllChangesSuccessText();
        Invoke(nameof(HideSaveAllChangesSuccessText),1.5f);
    }

    private void ShowSaveAllChangesSuccessText()
    {
        saveAllChangesSuccessfulTip.enabled = true;
    }

    private void HideSaveAllChangesSuccessText()
    {
        saveAllChangesSuccessfulTip.enabled = false;
    }
    private void ShowSaveSuccessText()
    {
        saveSuccessfulTip.enabled = true;
    }

    private void HideSaveSuccessText()
    {
        saveSuccessfulTip.enabled = false;
    }

    private void ShowFailedText()
    {
        saveFailedTip.enabled = true;
    }

    private void HideFailedText()
    {
        saveFailedTip.enabled = false;
    }
}
