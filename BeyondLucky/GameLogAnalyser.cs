using CsvHelper;
using CsvHelper.Configuration;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace BeyondLucky
{
    internal class GameLogAnalyser
    {
        private const string ClassAttribute = "class";
        private const string TitleAttribute = "title";
        private const string DivNode = "div";
        private const string RollTemplateClassName = "sheet-rolltemplate-default";

        private Dictionary<string, Character> CharacterRollsTotals = new Dictionary<string, Character>();

        public void Analyse(string chatLogFilePath)
        {
            String rawChatLog = File.ReadAllText(chatLogFilePath);
            rawChatLog = WebUtility.HtmlDecode(rawChatLog);
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(rawChatLog);
            List<HtmlNode> diceRollNodes = htmlDocument.DocumentNode.Descendants().Where(x => (x.Name == DivNode && x.Attributes[ClassAttribute] != null && x.Attributes[ClassAttribute].Value.Contains(RollTemplateClassName))).ToList();
            foreach (HtmlNode rollNode in diceRollNodes)
            {
                ParseDiceRoll(rollNode);
            }
        }

        private void ParseDiceRoll(HtmlNode divNode)
        {
            HtmlNode tableNode = divNode.ChildNodes.First(x => x.Name == "table");
            HtmlNode captionNode = tableNode.ChildNodes.First(x => x.Name == "caption");
            string characterName = captionNode.InnerText;
            HtmlNode bodyNode = tableNode.ChildNodes.First(x => x.Name == "tbody");
            HtmlNode tableRowNode = bodyNode.ChildNodes.First(x => x.Name == "tr");
            HtmlNode tableDataNode0 = tableRowNode.ChildNodes.First(x => x.Name == "td");
            string rollName = tableDataNode0.InnerText;

            if (rollName == "Source") // Spells that show their descriptions either don't matter or are harder to handle
            {
                return;
            }

            if (rollName == "Save") // TODO: handle attacks/abilities that have a saving throw
            {
                return;
            }

            HtmlNode tableDataNode1 = tableRowNode.ChildNodes.First(x => x.Name == "td" && x != tableDataNode0);
            HtmlNode spanNode = tableDataNode1.ChildNodes.First(x => x.Name == "span");
            string diceRollText = spanNode.Attributes[TitleAttribute].Value;


            if (characterName == "Ellli Dee")// Ugly hack...
            {
                characterName = "Elli Dee";
            }

            Character character;
            if (!CharacterRollsTotals.TryGetValue(characterName, out character))
            {
                character = new Character(characterName);
                CharacterRollsTotals[characterName] = character;
            }

            character.AddRoll(rollName, diceRollText); // TODO: Fill data structure!
        }

        private void ExportWithCsvWriter(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            CsvConfiguration config = new CsvConfiguration(CultureInfo.CurrentCulture);
            config.Delimiter = ",";
            config.HasHeaderRecord = true;

            using (FileStream fileStream = File.Create(filePath))
            using (StreamWriter writer = new StreamWriter(fileStream))
            using (CsvWriter csvWriter = new CsvWriter(writer, config))
            {
                csvWriter.WriteHeader(typeof(Character));
                csvWriter.WriteRecords(CharacterRollsTotals.Values);

                writer.Flush();
            }
        }

        public void ExportStats(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            
            using (StreamWriter streamWriter = new StreamWriter(filePath, true))
            {
                foreach (DataTable dataTable in ConvertToDataSet().Tables)
                {
                    WriteDataTable(streamWriter, dataTable);
                }
            }
        }

        private DataSet ConvertToDataSet()
        {
            DataSet dataSet = new DataSet("DiceStats");
            foreach (Character character in CharacterRollsTotals.Values)
            {
                DataTable dataTable = new DataTable(character.Name);

                dataTable.Columns.Add(character.Name, typeof(string));
                dataTable.Columns.Add("Average", typeof(float));
                dataTable.Columns.Add("Expected Average", typeof(float));
                dataTable.Columns.Add("Crits", typeof(int));
                dataTable.Columns.Add("Fumbles", typeof(int));
                dataTable.Columns.Add("RollsMade", typeof(int));

                foreach (KeyValuePair<string, RollType> rollType in character.RollsTotals)
                {
                    DataRow row = dataTable.NewRow();
                    row.ItemArray = new object[]
                    {
                        rollType.Key, // Rolls
                        (float)rollType.Value.TotalAccumulated / rollType.Value.NumberOfRolls, // Average
                        ((float)rollType.Value.MaximumRoll + rollType.Value.MinimumRoll) / 2, // Expected Average
                        rollType.Value.NumberOfCrits, // Crits
                        rollType.Value.NumberOfFumbles, // Fumbles
                        rollType.Value.NumberOfRolls, // RollsMade
                    };

                    dataTable.Rows.Add(row);
                }

                dataSet.Tables.Add(dataTable);
            }

            return dataSet;
        }

        private void WriteDataTable(StreamWriter streamWriter, DataTable dataTable)
        {
            //headers    
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                streamWriter.Write(dataTable.Columns[i]);
                if (i < dataTable.Columns.Count - 1)
                {
                    streamWriter.Write(",");
                }
            }
            streamWriter.Write(streamWriter.NewLine);
            foreach (DataRow dr in dataTable.Rows)
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            streamWriter.Write(value);
                        }
                        else
                        {
                            streamWriter.Write(dr[i].ToString());
                        }
                    }
                    if (i < dataTable.Columns.Count - 1)
                    {
                        streamWriter.Write(",");
                    }
                }
                streamWriter.Write(streamWriter.NewLine);
            }
        }
    }
}
