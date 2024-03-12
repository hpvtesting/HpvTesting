using RazorEngine.Templating;
using System;
using System.IO;
using System.Reflection;

namespace HPVTesting.Mail
{
    internal class EmbeddedTemplateManager : ITemplateManager
    {
        private readonly string _ns;

        public EmbeddedTemplateManager(string @namespace)
        {
            _ns = @namespace;
        }

        public ITemplateSource Resolve(ITemplateKey key)
        {
            var resourceName = $"{_ns}.{key.Name}.cshtml";
            string content;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (var streamReader = new StreamReader(stream ?? throw new InvalidOperationException()))
            {
                content = streamReader.ReadToEnd();
            }

            return new LoadedTemplateSource(content);
        }

        public ITemplateKey GetKey(string name, ResolveType resolveType, ITemplateKey context)
        {
            return new NameOnlyTemplateKey(name, resolveType, context);
        }

        public void AddDynamic(ITemplateKey key, ITemplateSource source)
        {
            throw new NotImplementedException("");
        }
    }
}