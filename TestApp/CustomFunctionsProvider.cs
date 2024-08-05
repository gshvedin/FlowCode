using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WorkflowEngine.Core.Dependencies.CustomFunctions;

namespace TestApp
{
    public class CustomFunctionsProvider : ICustomFunctionProvider
    {
   
        public string ContextData { get; set; }

        public string Execute(string functionName, string args)
        {
            var request = new TInfoSearch();
            request.Settings = new TSettings();
            request.Settings.Identifier = new List<TIdentifier>();
            request.Settings.Identifier.Add(new TIdentifier() { Name = "order_id", OrderId = "1", Text = "text" });

            request.Settings.Identifier.Add(new TIdentifier() { Name = "name", OrderId = "2", Text = "text" });
            request.Settings.OperationId = 1234;
            request.Settings.Service = 1456;


            switch (functionName)
            {
                case "getMessageLang":
                    return getMessageLang(args);
                // Add more cases for other functions
                case "getCurrencyName":
                    return getCurrencyName(args);
                // Add more cases for other functions
                default:
                    throw new InvalidOperationException($"Function '{functionName}' not found.");
            }
        }

        private string getMessageLang(string args)
        {
            string key = args.Split('|')[0];
            string scope = args.Split('|')[1];
            string locale = args.Split('|')[2];
            // Your logic to get the locale value
            // get locale from db
              key = $"{locale}:localized_value";
            //TODO rewrap with context
            return key;
        }

        private string getCurrencyName(string args)
        {
            string currTerminalCurrency = "";// get from session
            var result = "UAH";
            return result;
        }

        // Add more functions as needed
    }



    public class TInfoSearch

    {

        [JsonProperty("sale_point_id"), XmlElement(ElementName = "sale_point_id")]

        public int SalePointId { get; set; }

        [JsonProperty("service_id"), XmlElement(ElementName = "service_id")]

        public int ServiceId { get; set; }

        [JsonProperty("language"), XmlElement(ElementName = "language")]

        public string Language { get; set; }

        [JsonProperty("settings"), XmlElement(ElementName = "settings")]

        public TSettings Settings { get; set; }

        [JsonProperty("guid"), XmlIgnore]

        public string GUID { get; set; }    //9b1deb4d-3b7d-4bad-9bdd-2b0d7b3dcb6d

    }


    [XmlRoot(ElementName = "settings")]

    public class TSettings

    {

        [JsonProperty("identifier"), XmlElement(ElementName = "identifier")]

        public List<TIdentifier> Identifier { get; set; }

        [JsonProperty("service"), XmlElement(ElementName = "service")]

        public int? Service { get; set; }

        [JsonProperty("operation_id"), XmlElement(ElementName = "operation_id")]

        public long? OperationId { get; set; }

    }


    public class TIdentifier

    {

        [JsonProperty("-order_id"), XmlAttribute(AttributeName = "order_id")]

        public string OrderId { get; set; }

        [JsonProperty("-name"), XmlAttribute(AttributeName = "name")]

        public string Name { get; set; }

        [JsonProperty("#text"), XmlText]

        public string Text { get; set; }

        public override string ToString()

        {

            return $"OrderId: {OrderId}, Name: {Name}, Text: {Text}";

        }

    }

}
