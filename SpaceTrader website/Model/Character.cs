using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SpaceTrader.Model
{
    public class Character
    {
        public string Name { get; }
        public IEnumerable<string> BioLines { get; }

        public Uri ImageURI { get; }

        private Character(string name, IEnumerable<string> bio, Uri imageUri)
        {
            Name = name;
            BioLines = bio;
            ImageURI = imageUri;
        }

        public static IEnumerable<Character> ReadCharacters(Uri rootUri, string charactersFile)
        {
            using (var reader = File.OpenText(charactersFile))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var characters = (JObject)JToken.ReadFrom(jsonReader);

                return characters.Properties().Select(prop =>
                {
                    var name = prop.Name;
                    var imageUrl = new Uri(rootUri, name + ".png");

                    var charInfo = prop.Value as JObject;

                    var bioArray = (charInfo?["bio"] as JArray)?.Values() ?? Enumerable.Empty<JToken>();
                    var bioLines = bioArray.Select(line => line.ToString());

                    return new Character(name, bioLines, imageUrl);
                });
            }
        }
    }
}