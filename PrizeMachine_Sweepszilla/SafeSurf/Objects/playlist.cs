//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.1.4322.573
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=1.1.4322.573.
// 
using System.Xml.Serialization;


/// <remarks/>
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public class PlaylistItems
{
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("PlaylistItem", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public PlaylistItem[] Items;
}

/// <remarks/>
public class PlaylistItem
{
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string PlaylistName;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string MediaSource;
}