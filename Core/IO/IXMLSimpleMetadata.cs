
namespace AirSuperiority.Core.IO
{
    /// <summary>
    /// Base interface for simple XML data.
    /// </summary>
    public interface IXMLSimpleMetadata
    {
        XMLSimpleMetadata ParseAttributes(XMLAttributesCollection col);
    }
}
