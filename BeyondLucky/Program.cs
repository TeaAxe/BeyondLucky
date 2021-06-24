using System;

namespace BeyondLucky
{
    class Program
    {
        static void Main(string[] args)
        {
            const string inputFileName = @"C:\Users\Sathalom\Downloads\ChatLog.html";
            const string outputFileName = @"C:\Users\Sathalom\Downloads\DiceStats.csv";
            GameLogAnalyser rollStatsAnalyser = new GameLogAnalyser();
            rollStatsAnalyser.Analyse(inputFileName);
            rollStatsAnalyser.ExportStats(outputFileName);
        }
    }
}
