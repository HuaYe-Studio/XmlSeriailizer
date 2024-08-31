using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class Serializer : MonoBehaviour
{
    //本类实现xml的存储类,类对象的写入请在其他类中实现
    void Start()
    {
        MusicList musicList = new MusicList();
        
        //这里放List信息的初始化
        NoteList noteList = new NoteList();
        noteList.TimeLimit = 6.6f;
        noteList.SingleNoteList = new List<char>() {'a','b' };
        NoteList noteList2 = new();
        noteList2.SingleNoteList = new List<char>() {'c','d','a','a','b','b','c','d'};
        musicList.NoteLists = new List<NoteList>(){noteList,noteList2};
        
        string path = Application.persistentDataPath + "/MusicList1.xml";
        print(path);
        using (StreamWriter stream = new StreamWriter(path))
        {
            XmlSerializer s = new XmlSerializer(typeof(MusicList));
            s.Serialize(stream,musicList);
        }
    }

}
