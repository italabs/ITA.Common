using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace ITA.Common.WCF.RestHelp.Data
{
    internal sealed class AssemblyDocumentation
    {
        private RootComment _rootDoc;

        public AssemblyDocumentation(Assembly assembly)
        {
            Load(assembly);
        }       

        public string AssemblyName { get; private set; }

        public MemberComment GetMemeberDoc(string key)
        {
            return _rootDoc != null ? _rootDoc.Members.FirstOrDefault(m => m.Name == key) : null;
        }

        private void Load(Assembly assembly)
        {
            var dllPath = assembly.Location;
            var xmlPath = Path.ChangeExtension(dllPath, "xml");
            if (!File.Exists(xmlPath))
            {
                return;
            }

            AssemblyName = assembly.FullName;

            var rootElement = XDocument.Load(xmlPath).Root;
            var assemblyElement = rootElement.Descendants("assembly").FirstOrDefault();
            var membersElement = rootElement.Descendants("member");
            _rootDoc = new RootComment
            {
                Assembly = new AssemblyComment
                {
                    Name = assemblyElement.Element("name").Value
                }
            };

            foreach (var element in membersElement)
            {
                var name = GetAttributeValue(element, "name");
                var summary = GetElementValue(element, "summary");
                var example = GetElementValue(element, "example");
                var returns = GetElementValue(element, "returns");
                var parameters = element.Descendants("param");
                _rootDoc.Members.Add(new MemberComment
                {
                    Name = name,
                    Summary = summary,
                    Example = example,
                    Returns = returns,
                    Parameters = parameters.Select(p => new MethodParamComment
                    {
                        Name = GetAttributeValue(p, "name"),
                        Description = string.Concat(p.Nodes())
                    }).ToList()
                });
            }
        }

        private string GetElementValue(XElement element, string name)
        {
            if (element == null || !element.HasElements)
            {
                return null;
            }

            var sub = element.Element(name);
            return sub == null ? null : string.Concat(sub.Nodes());
        }

        private string GetAttributeValue(XElement element, string name)
        {
            if (element == null)
            {
                return null;
            }

            var attr = element.Attribute(name);
            return attr == null ? null : attr.Value;
        }        
    }

    internal class RootComment
    {
        public RootComment()
        {
            Members = new List<MemberComment>();
        }

        public AssemblyComment Assembly { get; set; }

        public List<MemberComment> Members { get; set; }
    }

    internal class AssemblyComment
    {
        public string Name { get; set; }
    }

    internal class MemberComment
    {
        public string Name { get; set; }

        public string Summary { get; set; }

        public string Example { get; set; }

        public List<MethodParamComment> Parameters { get; set; }

        public string Returns { get; set; }
    }

    internal class MethodParamComment
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}