using System.Collections.Generic;
using System.Diagnostics;

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
            // Expected format for rollResult:
            // "Rolling Result [XdY + Z] = (number) + Z = Result"

            string formula = ExtractFormula(rollResult);
            int numberOfDice = int.Parse(formula.Remove(formula.IndexOf('d')));
            int sizeOfDice = ExtractSizeOfDice(formula);
            int modifier = ExtractModifier(formula);
            
            int maximum = numberOfDice * sizeOfDice + modifier;
            int minimum = numberOfDice + modifier;
            int result = ExtractResult(rollResult);

            Debug.Assert(minimum <= result && result <= maximum);

            RollType roll;
            if (!RollsTotals.TryGetValue(rollName, out roll))
            {
                roll = new RollType(minimum, maximum, formula);
                RollsTotals[rollName] = roll;
            }
            
            roll.AddResult(result);
        }

        private string ExtractFormula(string rollResult)
        {
            int startIndex = rollResult.IndexOf('[') + 1;
            int endIndex = rollResult.IndexOf(']');
            return rollResult.Substring(startIndex, endIndex - startIndex);
        }

        private int ExtractResult(string rollResult)
        {
            string rollResultWithoutFirstWord = rollResult.Remove(0, "Rolling ".Length);
            string resultString = rollResultWithoutFirstWord.Remove(rollResultWithoutFirstWord.IndexOf(' '));
            return int.Parse(resultString);
        }

        private int ExtractSizeOfDice(string formula)
        {
            int modifierIndex = formula.IndexOf(' ');
            string formulaWithoutModifier = formula;
            if (modifierIndex != -1) // make sure there is a modifier
            {
                formulaWithoutModifier = formula.Remove(formula.IndexOf(' '));
            }
            
            string formulaWithoutNumberOfDice = formulaWithoutModifier.Substring(formula.IndexOf('d') + 1);
            return int.Parse(formulaWithoutNumberOfDice);
        }

        private int ExtractModifier(string formula)
        {
            int startIndex = formula.IndexOf('-');
            if (startIndex == -1)
            {
                startIndex = formula.IndexOf('+');

                if (startIndex == -1)
                {
                    return 0;
                }

                startIndex++; // avoid parsing the '+'
            }
            string formulaModifer = formula.Substring(startIndex).Replace(" ", "");
            return int.Parse(formulaModifer);
        }
    }
}
