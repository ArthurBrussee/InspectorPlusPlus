#if UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class InspectorPlusSummary
{
    Dictionary<string, string> summaries = new Dictionary<string, string>();
    List<string> lines = new List<string>();

    string FindVarName(int index)
    {
        int i = index + 1;

        if (lines[i].EndsWith(";"))
        {
            string[] words = lines[i].Split(' ');

            if (!lines[i].Contains("="))
                return words[words.Length - 1].Replace(";", "");
            else
            {
                for (int j = words.Length - 1; j >= 0; j -= 1)
                {
                    if (words[j].Contains("="))
                        return words[j - 1];
                }
            }
        }
        return "";
    }

    public void ReadSummaries(string file)
    {
        if (file == "" || !File.Exists(file))
            return; 

        StreamReader reader = new StreamReader(file);
        string contents = reader.ReadToEnd();
        string[] linesRaw = contents.Split('\n');

        //format lines.
        foreach (string l in linesRaw)
        {
            string addString = l;
            addString = addString.Trim();

            if (addString.Length < 1)
                continue;

            lines.Add(addString);
        }

        //format summaries
        bool summary = false;
        string curSum = "";
        int count = 0;

        for (int i = 0; i < lines.Count; i += 1)
        {
            if (summary)
                curSum += lines[i].Replace("///", "").Trim();

            if (lines[i].Contains("<summary>"))
            {
                summary = true;
            }

            if (lines[i].Contains("</summary>") && summary)
            {
                summary = false;
                count += 1;

                string name = FindVarName(i);
                if (name != "")
                    summaries.Add(name, curSum.Replace("</summary>", ""));

                curSum = "";
            }
        }

        reader.Close();
    }

    public string GetSummary(string name)
    {
        if (summaries.ContainsKey(name))
            return summaries[name];

        return "";
    }
}

#endif