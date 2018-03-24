using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Mutatio
{
    public class PackagesFileParser
    {
        public IEnumerable<(string name, string version)> GetPackages(string filePath)
        {
            var doc = XDocument.Load(filePath);
            return doc.Elements("packages").Descendants().Select(p => (p.Attribute("id").Value, p.Attribute("version").Value));
        }
    }
}
