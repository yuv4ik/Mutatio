using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Mutatio
{
    public class PackagesFileParser
    {
        public IEnumerable<Tuple<string, string>> GetPackages(string filePath)
        {
            var doc = XDocument.Load(filePath);
            return doc.Elements("packages").Descendants().Select(p => new Tuple<string, string>(p.Attribute("id").Value, p.Attribute("version").Value));
        }
    }
}
