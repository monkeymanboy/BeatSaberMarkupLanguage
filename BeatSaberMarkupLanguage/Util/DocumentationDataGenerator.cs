#if DEBUG
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage.Macros;
using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.TypeHandlers;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BeatSaberMarkupLanguage.Util
{
    internal class DocumentationDataGenerator
    {
        private readonly IReadOnlyList<Type> typeHandlerTypes;

        internal DocumentationDataGenerator(IEnumerable<Type> typeHandlers)
        {
            this.typeHandlerTypes = typeHandlers.Select(t => t.GetCustomAttribute<ComponentHandler>(true).Type).OrderBy(t => t.Name).ToList();
        }

        internal void Generate()
        {
            IDeserializer deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            ISerializer serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

            GenerateTags(serializer);
            GenerateMacros(deserializer, serializer);
            GenerateTypeHandlers(deserializer, serializer);
        }

        private void GenerateTags(ISerializer serializer)
        {
            GameObject dummy = new();
            List<TagDefinition> tags = new();

            foreach (BSMLTag tag in Utilities.GetInstancesOfDescendants<BSMLTag>())
            {
                tag.Initialize();

                GameObject currentNode = tag.CreateObject(dummy.transform);

                tags.Add(new TagDefinition
                {
                    Type = tag.GetType().Name,
                    Aliases = tag.Aliases,
                    Components = typeHandlerTypes.Where(type => BSMLParser.GetExternalComponent(currentNode, type) != null).Select(t => t.Name).ToList(),
                });
            }

            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "Tags.yml"), serializer.Serialize(tags.OrderBy(t => t.Type)));
            UnityEngine.Object.Destroy(dummy);
        }

        private void GenerateMacros(IDeserializer deserializer, ISerializer serializer)
        {
            GameObject dummy = new();
            List<MacroDefinition> macros = deserializer.Deserialize<List<MacroDefinition>>(File.ReadAllText("Macros.yml"));

            foreach (BSMLMacro macro in Utilities.GetInstancesOfDescendants<BSMLMacro>())
            {
                string typeName = macro.GetType().Name;
                MacroDefinition def = macros.FirstOrDefault(m => m.Type == typeName);

                if (def == null)
                {
                    def = new MacroDefinition() { Type = typeName };
                    macros.Add(def);
                }

                Dictionary<string, string[]> props = macro.Props.Where(kvp => kvp.Key is not "id").ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                def.Aliases = macro.Aliases.Select(a => BSMLParser.MacroPrefix + a).ToList();
                def.Properties = def.Properties?.Where(p => props.ContainsKey(p.Name)).ToList() ?? new List<Property>();

                foreach (KeyValuePair<string, string[]> pair in props)
                {
                    Property property = def.Properties.FirstOrDefault(p => p.Name == pair.Key);

                    if (property == null)
                    {
                        property = new Property() { Name = pair.Key };
                        def.Properties.Add(property);
                    }

                    property.Aliases = pair.Value;
                }

                def.Properties = def.Properties.OrderBy(p => p.Name).ToList();
            }

            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "Macros.yml"), serializer.Serialize(macros.OrderBy(m => m.Type)));
            UnityEngine.Object.Destroy(dummy);
        }

        private void GenerateTypeHandlers(IDeserializer deserializer, ISerializer serializer)
        {
            GameObject dummy = new();
            List<TypeHandlerDefinition> typeHandlers = deserializer.Deserialize<List<TypeHandlerDefinition>>(File.ReadAllText("TypeHandlers.yml"));
            typeHandlers = typeHandlers.Where(thd => typeHandlerTypes.Any(th => thd.Type == th.Name)).ToList();

            foreach (TypeHandler typeHandler in Utilities.GetInstancesOfDescendants<TypeHandler>().ToList())
            {
                string typeName = typeHandler.GetType().GetCustomAttribute<ComponentHandler>().Type.Name;
                TypeHandlerDefinition def = typeHandlers.FirstOrDefault(thd => thd.Type == typeName);

                if (def == null)
                {
                    def = new TypeHandlerDefinition() { Type = typeName };
                    typeHandlers.Add(def);
                }

                Dictionary<string, string[]> props = typeHandler.Props.Where(kvp => kvp.Key is not "id" and not "cellTemplate").ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                def.Properties = def.Properties?.Where(p => props.ContainsKey(p.Name)).ToList() ?? new List<Property>();

                foreach (KeyValuePair<string, string[]> pair in props)
                {
                    Property property = def.Properties.FirstOrDefault(p => p.Name == pair.Key);

                    if (property == null)
                    {
                        property = new Property() { Name = pair.Key };
                        def.Properties.Add(property);
                    }

                    property.Aliases = pair.Value;
                }

                def.Properties = def.Properties.OrderBy(p => p.Name).ToList();
            }

            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "TypeHandlers.yml"), serializer.Serialize(typeHandlers.OrderBy(th => th.Type)));
            UnityEngine.Object.Destroy(dummy);
        }

        private record TagDefinition
        {
            public string Type { get; set; }

            public IList<string> Aliases { get; set; }

            public IList<string> Components { get; set; }
        }

        private record MacroDefinition
        {
            public string Type { get; set; }

            public string Description { get; set; }

            public IList<string> Aliases { get; set; }

            public IList<Property> Properties { get; set; }
        }

        private record TypeHandlerDefinition
        {
            public string Type { get; set; }

            public List<Property> Properties { get; set; }
        }

        private record Property
        {
            public string Name { get; set; }

            public string Type { get; set; } = "TODO";

            public string Description { get; set; } = "TODO";

            public IList<string> Aliases { get; set; }
        }
    }
}
#endif
