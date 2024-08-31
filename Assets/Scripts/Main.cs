using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public InputField inputMusicNameField;
    public InputField inputRootNameField;
    public InputField inputNodeContentField;
    
    private string _nodeContent;
    private string _nodeName;
    //下面这两个名字等到点下保存按钮才会真正保存
    public string musicName;
    public string rootName;
    
    
    
    private XmlElement _root;
    public Text rootNameText;//这里显示当前选中的节点名字
    void Start()
    {
        string path = Application.persistentDataPath + $"/{musicName}.xml";
        XmlDocument xmlDocument = new XmlDocument();
        XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", "");
        xmlDocument.AppendChild(xmlDeclaration);
        
        XmlElement root = xmlDocument.CreateElement(rootName);
        xmlDocument.AppendChild(root);
        
        
        
    }

    public void OnEndEdit()
    {
        musicName = inputMusicNameField.text;
    }

    public void OnEndRootNameEdit()
    {
        rootName = inputRootNameField.text;
    }
        
    public void OnEndNodeContentEdit()
    {
        _nodeContent = inputNodeContentField.text;
    }
}
/// <summary>
/// 该类旨在用一个类来管理xml文件的读写,省去看官方API的时间并支持自定义
/// </summary>
public class XmlManager
{
    private XmlDocument _xmlDocument;
    private XmlElement _root;
    private string _path;
    
    public XmlManager()
    {
        _path = Application.persistentDataPath + "/DefaultXml.xml";
        Initialize();   
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName">这里就写文件名，不要加后缀</param>
    public XmlManager(string fileName)
    {
        _path = Application.persistentDataPath + "/" + fileName + ".xml";
        Initialize();
    }
    
    public XmlManager(string fileName,string rootName)
    {
        _path = Application.persistentDataPath + "/" + fileName + ".xml";
        Initialize();
        InitializeRoot(rootName);
    }
    private void Initialize()
    {
        XmlDocument xmlDocument = new();
        _xmlDocument = xmlDocument;
        XmlDeclaration xmlDeclaration = _xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", "");
        _xmlDocument.AppendChild(xmlDeclaration);
    }
    /// <summary>
    /// 初始化根节点
    /// </summary>
    /// <param name="RootName">根节点名字</param>
    private void InitializeRoot(string RootName)
    {
        XmlElement root = _xmlDocument.CreateElement(RootName);
        _root = root;
        _xmlDocument.AppendChild(_root);
    }
    /// <summary>
    /// 为父节点添加子节点
    /// </summary>
    /// <param name="fatherNode">XmlElement类型的父节点</param>
    /// <param name="childNodeName">子节点名字</param>
    /// <param name="innerText">子节点内容</param>
    public void AppendChild(XmlElement fatherNode, string childNodeName, string innerText)
    {
        XmlElement childNode = _xmlDocument.CreateElement(childNodeName);
        childNode.InnerText = innerText;
        fatherNode.AppendChild(childNode);
    }
    /// <summary>
    /// 为父节点添加带有一个属性的子节点
    /// </summary>
    /// <param name="fatherNode">XmlElement类型的父节点</param>
    /// <param name="childNodeName">子节点名字</param>
    /// <param name="innerText">子节点内容</param>
    /// <param name="attributeName">子节点属性名字</param>
    /// <param name="attributeValue">子节点属性的内容</param>
    public void AppendChild(XmlElement fatherNode,string childNodeName,string innerText,string attributeName,string attributeValue)
    {
        XmlElement childNode = _xmlDocument.CreateElement(childNodeName);
        childNode.InnerText = innerText;
        AppendAttribute(childNode, attributeName, attributeValue);
        fatherNode.AppendChild(childNode);
    }
    /// <summary>
    /// 为节点添加属性
    /// </summary>
    /// <param name="node">XmlElement类型的节点</param>
    /// <param name="attributeName">属性名字</param>
    /// <param name="attributeValue">属性值</param>
    public void AppendAttribute(XmlElement node, string attributeName, string attributeValue)
    {
        node.SetAttribute(attributeName, attributeValue);
    }

    public void Save()
    {
        _xmlDocument.Save(_path);
    }
    
}