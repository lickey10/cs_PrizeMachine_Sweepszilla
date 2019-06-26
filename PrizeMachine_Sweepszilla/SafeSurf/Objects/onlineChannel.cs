using System.Xml.Serialization;


/// <remarks/>
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public class OnlineChannels {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("OnlineChannel", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public OnlineChannel[] Items;
}

/// <remarks/>
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public class Archive
{
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("URL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string URL;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("text", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string text;
}

/// <remarks/>
public class OnlineChannel {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ShowTitle", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ShowTitle;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("URL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string URL;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ChannelTitle", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ChannelTitle;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Archives", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public Archive[] Archives;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("PopUpURL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string PopUpURL;
}
