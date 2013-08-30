using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace avt.ActionForm.Core.Actions
{
    public class Currency
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public Currency()
        {
        }

        public Currency(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public static IList<Currency> LoadFromConfiguration(string configFile)
        {
            List<Currency> currencies = new List<Currency>();

            XmlDocument xmlCurrencies = new XmlDocument();
            xmlCurrencies.Load(configFile);
            foreach (XmlElement xmlCur in xmlCurrencies.DocumentElement.SelectNodes("Currency")) {
                currencies.Add(new Currency(xmlCur["Code"].InnerText, xmlCur["Name"].InnerText));
            }

            return currencies;
        }
    }
}
