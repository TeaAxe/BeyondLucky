using System.Collections.Generic;

namespace BeyondLucky
{
    internal class Character
    {
        public string Name { get; }

        public Dictionary<string, RollType> RollsTotals { get; }

        public Character(string name)
        {
            Name = name;
            RollsTotals = new Dictionary<string, RollType>();
        }

        public void AddRoll(string rollName, string rollResult)
        {
            // TODO: parse the roll result...
            RollType roll;
            if (!RollsTotals.TryGetValue(rollName, out roll))
            {
                roll = new RollType(0, 2);
                RollsTotals[rollName] = roll;
            }
            
            roll.AddResult(1);
        }
    }
}
