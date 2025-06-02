using Communication;
using DataCommon;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

string xmlSerialized = "";
string xmlPath = "C:\\Users\\pawel\\Desktop\\Programowanie\\Programowanie-urzadzen-mobilnych\\XMLSchema\\";

static bool ValidateXml(string xmlMessage, string schemaPath)
{
    var validationErrors = "";
    XmlSchemaSet schemas = new XmlSchemaSet();
    schemas.Add(null, schemaPath);

    XmlReaderSettings settings = new XmlReaderSettings();
    settings.Schemas = schemas;
    settings.ValidationType = ValidationType.Schema;
    settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

    settings.ValidationEventHandler += (sender, e) =>
    {
        validationErrors += $"{e.Severity}: {e.Message}\n";
    };

    using (var stringReader = new StringReader(xmlMessage))
    using (var xmlReader = XmlReader.Create(stringReader, settings))
    {
        try
        {
            while (xmlReader.Read()) { }
        }
        catch (XmlException ex)
        {
            validationErrors += $"XmlException: {ex.Message}\n";
            return false;
        }
    }

    return string.IsNullOrEmpty(validationErrors);
}

XmlSchemas schemas = new XmlSchemas();

Request request = new Request("Name", "Args");
var requestSerializer = new XmlSerializer(typeof(Request));
using (var stringWriter = new StringWriter())
{
    requestSerializer.Serialize(stringWriter, request);
    xmlSerialized = stringWriter.ToString();
}
File.WriteAllText(xmlPath + "Request.xml", xmlSerialized);
XmlTypeMapping mapping = new XmlReflectionImporter().ImportTypeMapping(typeof(Request));
XmlSchemaExporter exporter = new XmlSchemaExporter(schemas);
exporter.ExportTypeMapping(mapping);
if (schemas.Count > 0)
{
    using (var writer = XmlWriter.Create(xmlPath + "Request.xsd", new XmlWriterSettings { Indent = true }))
    {
        schemas[0].Write(writer);
    }
}
schemas.Clear();

DelClientRequest dcrequest = new DelClientRequest();
var dcrequestSerializer = new XmlSerializer(typeof(DelClientRequest));
using (var stringWriter = new StringWriter())
{
    dcrequestSerializer.Serialize(stringWriter, dcrequest);
    xmlSerialized = stringWriter.ToString();
}
File.WriteAllText(xmlPath + "DelClientRequest.xml", xmlSerialized);
XmlTypeMapping dcmapping = new XmlReflectionImporter().ImportTypeMapping(typeof(DelClientRequest));
XmlSchemaExporter dcexporter = new XmlSchemaExporter(schemas);
dcexporter.ExportTypeMapping(dcmapping);
if (schemas.Count > 0)
{
    using (var writer = XmlWriter.Create(xmlPath + "DelClientRequest.xsd", new XmlWriterSettings { Indent = true }))
    {
        schemas[0].Write(writer);
    }
}
schemas.Clear();


LoadRequest lrequest = new LoadRequest();
lrequest.Books.Add(new Book());
lrequest.Books.Add(new Book());
lrequest.Books.Add(new Book());
var lrequestSerializer = new XmlSerializer(typeof(LoadRequest));
using (var stringWriter = new StringWriter())
{
    lrequestSerializer.Serialize(stringWriter, lrequest);
    xmlSerialized = stringWriter.ToString();
}
File.WriteAllText(xmlPath + "LoadRequest.xml", xmlSerialized);
XmlTypeMapping lmapping = new XmlReflectionImporter().ImportTypeMapping(typeof(LoadRequest));
XmlSchemaExporter lexporter = new XmlSchemaExporter(schemas);
lexporter.ExportTypeMapping(lmapping);
if (schemas.Count > 0)
{
    using (var writer = XmlWriter.Create(xmlPath + "LoadRequest.xsd", new XmlWriterSettings { Indent = true }))
    {
        schemas[0].Write(writer);
    }
}
schemas.Clear();


NewClientRequest ncrequest = new NewClientRequest(Guid.NewGuid());
var ncrequestSerializer = new XmlSerializer(typeof(NewClientRequest));
using (var stringWriter = new StringWriter())
{
    ncrequestSerializer.Serialize(stringWriter, ncrequest);
    xmlSerialized = stringWriter.ToString();
}
File.WriteAllText(xmlPath + "NewClientRequest.xml", xmlSerialized);
XmlTypeMapping ncmapping = new XmlReflectionImporter().ImportTypeMapping(typeof(NewClientRequest));
XmlSchemaExporter ncexporter = new XmlSchemaExporter(schemas);
ncexporter.ExportTypeMapping(ncmapping);
if (schemas.Count > 0)
{
    using (var writer = XmlWriter.Create(xmlPath + "NewClientRequest.xsd", new XmlWriterSettings { Indent = true }))
    {
        schemas[0].Write(writer);
    }
}
schemas.Clear();


ReturnBorrowRequest rbrrequest = new ReturnBorrowRequest();
var rbrrequestSerializer = new XmlSerializer(typeof(ReturnBorrowRequest));
using (var stringWriter = new StringWriter())
{
    rbrrequestSerializer.Serialize(stringWriter, rbrrequest);
    xmlSerialized = stringWriter.ToString();
}
File.WriteAllText(xmlPath + "ReturnBorrowRequest.xml", xmlSerialized);
XmlTypeMapping rbrmapping = new XmlReflectionImporter().ImportTypeMapping(typeof(ReturnBorrowRequest));
XmlSchemaExporter rbrexporter = new XmlSchemaExporter(schemas);
rbrexporter.ExportTypeMapping(rbrmapping);
if (schemas.Count > 0)
{
    using (var writer = XmlWriter.Create(xmlPath + "ReturnBorrowRequest.xsd", new XmlWriterSettings { Indent = true }))
    {
        schemas[0].Write(writer);
    }
}
schemas.Clear();


ReturnBorrowResponseRequest rbrrrequest = new ReturnBorrowResponseRequest();
var rbrrrequestSerializer = new XmlSerializer(typeof(ReturnBorrowResponseRequest));
using (var stringWriter = new StringWriter())
{
    rbrrrequestSerializer.Serialize(stringWriter, rbrrrequest);
    xmlSerialized = stringWriter.ToString();
}
File.WriteAllText(xmlPath + "ReturnBorrowResposneRequest.xml", xmlSerialized);
XmlTypeMapping rbrrmapping = new XmlReflectionImporter().ImportTypeMapping(typeof(ReturnBorrowResponseRequest));
XmlSchemaExporter rbrrexporter = new XmlSchemaExporter(schemas);
rbrrexporter.ExportTypeMapping(rbrrmapping);
if (schemas.Count > 0)
{
    using (var writer = XmlWriter.Create(xmlPath + "ReturnBorrowResposneRequest.xsd", new XmlWriterSettings { Indent = true }))
    {
        schemas[0].Write(writer);
    }
}
schemas.Clear();


SubRequest srequest = new SubRequest();
var srequestSerializer = new XmlSerializer(typeof(SubRequest));
using (var stringWriter = new StringWriter())
{
    srequestSerializer.Serialize(stringWriter, srequest);
    xmlSerialized = stringWriter.ToString();
}
File.WriteAllText(xmlPath + "SubRequest.xml", xmlSerialized);
XmlTypeMapping submapping = new XmlReflectionImporter().ImportTypeMapping(typeof(SubRequest));
XmlSchemaExporter subexporter = new XmlSchemaExporter(schemas);
subexporter.ExportTypeMapping(submapping);
if (schemas.Count > 0)
{
    using (var writer = XmlWriter.Create(xmlPath + "SubRequest.xsd", new XmlWriterSettings { Indent = true }))
    {
        schemas[0].Write(writer);
    }
}
schemas.Clear();

