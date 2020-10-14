using System;
using System.Globalization;
using System.Xml;

public class OSMBase
{
    private NumberFormatInfo nfi = new NumberFormatInfo();
    protected T GetAttribute<T>(string attributeName, XmlAttributeCollection attributes)
    {
        nfi.NumberDecimalSeparator = ".";
        string attributeValue = attributes[attributeName].Value;
        T returnedValue = (T) Convert.ChangeType(attributeValue, typeof(T), nfi);
        return returnedValue;
    }
}
