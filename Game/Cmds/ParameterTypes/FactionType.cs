using Game.Factions;
using SampSharp.GameMode.SAMP.Commands.ParameterTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Game.Cmds.ParameterTypes
{
    class FactionType : ICommandParameterType
    {
        #region Implementation of ICommandParameterType

        public bool Parse(ref string commandText, out object output)
        {
            output = null;

            if (string.IsNullOrWhiteSpace(commandText))
                return false;

            var word = commandText.TrimStart().Split(' ').First();

            Console.WriteLine("parse " + word);

            // find a faction with a matching id.
            int id;
            if (int.TryParse(word, NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
            {
                var faction = Faction.Find(id);
                if (faction != null)
                {
                    output = faction;
                    commandText = commandText.Substring(word.Length).TrimStart(' ');
                    return true;
                }
            }

            var lowerWord = word.ToLower();

            // find all candiates containing the input word, case insensitive.
            var candidates = Faction.All.Where(f => f.Name.ToLower().Contains(lowerWord))
                .ToList();

            // in case of ambiguities find all candiates containing the input word, case sensitive.
            if (candidates.Count > 1)
                candidates = candidates.Where(f => f.Name.Contains(word)).ToList();

            // in case of ambiguities find all candiates matching exactly the input word, case insensitive.
            if (candidates.Count > 1)
                candidates = candidates.Where(f => f.Name.ToLower() == lowerWord).ToList();

            // in case of ambiguities find all candiates matching exactly the input word, case sensitive.
            if (candidates.Count > 1)
                candidates = candidates.Where(f => f.Name == word).ToList();

            if (candidates.Count == 1)
            {
                output = candidates.First();

                commandText = word.Length == commandText.Length
                    ? string.Empty
                    : commandText.Substring(word.Length).TrimStart(' ');
                return true;
            }

            return false;
        }

        #endregion
    }
}
