namespace BeyondLucky
{
    internal class RollType
    {
        public int MaximumRoll { get; }
        public int MinimumRoll { get; }
        public string Formula { get; }

        public int NumberOfRolls { get; private set; }
        public int TotalAccumulated { get; private set; }
        public int NumberOfCrits { get; private set; }
        public int NumberOfFumbles { get; private set; }

        public RollType(int minimumRoll, int maximumRoll, string formula)
        {
            MaximumRoll = maximumRoll;
            MinimumRoll = minimumRoll;
            NumberOfRolls = 0;
            TotalAccumulated = 0;
            NumberOfCrits = 0;
            NumberOfFumbles = 0;
            Formula = formula;
        }

        public void AddResult(int result)
        {
            NumberOfRolls++;
            TotalAccumulated += result;
            if (result == MaximumRoll)
            {
                NumberOfCrits++;
            }
            else if (result == MinimumRoll)
            {
                NumberOfFumbles++;
            }
        }
    }
}
