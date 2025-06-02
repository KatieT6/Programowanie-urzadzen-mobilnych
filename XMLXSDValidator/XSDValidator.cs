using System.Xml.Schema;
using System.Xml;

namespace XMLXSDValidator
{
    public class XSDValidator
    {
        private object lockObject = new object();

        Dictionary<Type, XmlSchemaSet> mapping;



        public XSDValidator()
        {
            mapping = new Dictionary<Type, XmlSchemaSet>();

            var delClientSchemaSet = new XmlSchemaSet();
            delClientSchemaSet.Add(null, "DelClientRequest.xsd");
            mapping.Add(typeof(Communication.DelClientRequest), delClientSchemaSet);

            var loadRequestSchemaSet = new XmlSchemaSet();
            loadRequestSchemaSet.Add(null, "LoadRequest.xsd");
            mapping.Add(typeof(Communication.LoadRequest), loadRequestSchemaSet);

            var newClientSchemaSet = new XmlSchemaSet();
            newClientSchemaSet.Add(null, "NewClientRequest.xsd");
            mapping.Add(typeof(Communication.NewClientRequest), newClientSchemaSet);

            var requestSchemaSet = new XmlSchemaSet();
            requestSchemaSet.Add(null, "Request.xsd");
            mapping.Add(typeof(Communication.Request), requestSchemaSet);

            var returnBorrowSchemaSet = new XmlSchemaSet();
            returnBorrowSchemaSet.Add(null, "ReturnBorrowRequest.xsd");
            mapping.Add(typeof(Communication.ReturnBorrowRequest), returnBorrowSchemaSet);

            var returnBorrowResponseSchemaSet = new XmlSchemaSet();
            returnBorrowResponseSchemaSet.Add(null, "ReturnBorrowResponseRequest.xsd");
            mapping.Add(typeof(Communication.ReturnBorrowResponseRequest), returnBorrowResponseSchemaSet);

            var subRequestSchemaSet = new XmlSchemaSet();
            subRequestSchemaSet.Add(null, "SubRequest.xsd");
            mapping.Add(typeof(Communication.SubRequest), subRequestSchemaSet);
        }

        public bool Validate(string message, Type type)
        {
            var validationErrors = "";

            
            lock (lockObject)
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas = mapping[type];
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

                settings.ValidationEventHandler += (sender, e) =>
                {
                    validationErrors += $"{e.Severity}: {e.Message}\n";
                };

                using (var stringReader = new StringReader(message))
                using (var xmlReader = XmlReader.Create(stringReader, settings))
                {
                    try
                    {
                        while (xmlReader.Read()) { }
                    }
                    catch (XmlException ex)
                    {
                        Console.WriteLine($"validation exception {ex.Message}");
                        validationErrors += $"XmlException: {ex.Message}\n";
                        return false;
                    }
                }
            }
            return string.IsNullOrEmpty(validationErrors);
        }
    }
}
