using System.Collections.Generic;

namespace BeyondLucky
{
    class Program
    {
        static void Main(string[] args)
        {
            const string inputFileName = @"C:\Users\Sathalom\Downloads\ChatLog.html";
            const string outputFileName = @"C:\Users\Sathalom\Downloads\DiceStats.csv";
            HashSet<string> playerCharacterNames = new HashSet<string>
            {
                "Elli Dee",
                "Tibby Dee",
                "Sheesh Gimble",
                "Itzril Aysuair"
            };
            GameLogAnalyser rollStatsAnalyser = new GameLogAnalyser(playerCharacterNames);
            rollStatsAnalyser.Analyse(inputFileName);
            rollStatsAnalyser.ExportStats(outputFileName);
        }
    }
}
