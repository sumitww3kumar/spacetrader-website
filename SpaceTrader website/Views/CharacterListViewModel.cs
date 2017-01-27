using SpaceTrader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpaceTrader.Views
{
    public class CharacterListViewModel
    {
        public string Name { get; }
        public IEnumerable<string> BioLines { get; }
        public Uri ImageURI { get; }

        public CharacterListViewModel(Character character)
        {
            Name = character.Name;
            BioLines = character.BioLines.ToList();
            ImageURI = character.ImageURI;
        }
    }
}