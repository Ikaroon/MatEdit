using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

public class DocumentationGenerator : EditorWindow {

    string methodData = "";

    string generated = "";

    string testString = "";
    string testStringConverted = "";

    [MenuItem("Window/MatEdit/CreateDoc")]
    public static void Init()
    {
        DocumentationGenerator gen = CreateInstance<DocumentationGenerator>();
        gen.ShowUtility();
    }

    GUIStyle richTextBox;

    private void OnGUI()
    {
        if (richTextBox == null)
        {
            richTextBox = new GUIStyle(EditorStyles.label);
            richTextBox.richText = true;
        }

        methodData = EditorGUILayout.TextArea(methodData);

        if (GUILayout.Button("Generate"))
        {
            Generate();
        }

        if (GUILayout.Button("Autofill"))
        {
            AutoFill();
        }

        EditorGUILayout.TextArea(generated);

        EditorGUI.BeginChangeCheck();
        testString = EditorGUILayout.TextField(testString);
        EditorGUILayout.LabelField(testStringConverted, richTextBox);
        if (EditorGUI.EndChangeCheck())
        {
            testStringConverted = FormatMethodHead(testString);
        }
    }

    void AutoFill()
    {
        string summary = Tagged(methodData, "summary");
        string returns = Tagged(methodData, "returns");
        string allParams = TaggedAll(methodData, "param");

        testString = Regex.Replace(Regex.Match(methodData, @"\n(.+?)$").Value, @"(?:\n(.+?)[^a-zA-Z0-9]*)", "");

        Dictionary<string, string> paramDesc = new Dictionary<string, string>();
        string[] paramDescSplits = allParams.Split('\n');

        for (int pd = 0; pd < paramDescSplits.Length; pd++)
        {
            string key = Regex.Match(paramDescSplits[pd], '"' + "(.+?)" + '"').Value.Replace("name=" + '"', "").Replace('"' + "", "");
            string desc = Regex.Match(paramDescSplits[pd], ">(.+?)$").Value.Replace(">", "").Replace("<", "");
            if (key != "" && desc != "")
            {
                paramDesc.Add(key, desc);
            }
        }
    }

    string FormatMethodHead(string input)
    {
        string methodLine = input;
        Match methodMatch = Regex.Match(methodLine, @" (.+?)(\(|<(.+?)>\()", RegexOptions.RightToLeft);
        string methodName = Regex.Replace((methodMatch != null ? methodMatch.Value : "").Replace("(", ""), "<(.+?)>", "");

        Match paramMatch = Regex.Match(methodLine, @"\((.*)$");
        string methodParams = (paramMatch != null ? paramMatch.Value : "");
        methodLine = (methodParams != "" ? methodLine.Replace(methodParams, "") : methodLine);

        methodLine = (methodName != "" ? methodLine.Replace(methodName, "<b>" + methodName + "</b>") : methodLine);

        string[] paramters = methodParams.Split(new string[] { ", ", "," }, System.StringSplitOptions.None);
        string resultParams = "";
        for (int p = 0; p < paramters.Length; p++)
        {
            string param = paramters[p];
            string[] defaulted = param.Split('=');
            param = "";
            if (defaulted.Length >= 1)
            {
                string cutString = "";
                if (Regex.IsMatch(defaulted[0], @"\);$"))
                {
                    defaulted[0] = Regex.Replace(defaulted[0], @"\);$", "");
                    cutString = ");";
                }

                if (Regex.IsMatch(defaulted[0], @"\)$"))
                {
                    defaulted[0] = Regex.Replace(defaulted[0], @"\)$", "");
                    cutString = ")";
                }

                defaulted[0] = Regex.Replace(defaulted[0], " (.+?)$", SouroundB, RegexOptions.RightToLeft);

                param += defaulted[0] + cutString;
            }
            if (defaulted.Length >= 2)
            {
                string cutString = "";
                if (Regex.IsMatch(defaulted[1], @"\);$"))
                {
                    defaulted[1] = Regex.Replace(defaulted[1], @"\);$", "");
                    cutString = ");";
                }

                if (Regex.IsMatch(defaulted[1], @"\)$"))
                {
                    defaulted[1] = Regex.Replace(defaulted[1], @"\)$", "");
                    cutString = ")";
                }

                defaulted[1] = Regex.Replace(defaulted[1].Replace(")", ""), "^(.+?)$", SouroundI, RegexOptions.RightToLeft);
                param += "=" + defaulted[1] + cutString;
            }

            resultParams += param;
            if (p < paramters.Length - 1)
            {
                resultParams += ", ";
            }
        }
        methodLine += resultParams;
        return methodLine;
    }

    void Generate()
    {
        string summary = Tagged(methodData, "summary");
        string returns = Tagged(methodData, "returns");
        Debug.Log(summary);
        Debug.Log(returns);
        string allParams = TaggedAll(methodData, "param");
        Debug.Log(allParams);

        string methodLine = Regex.Replace(Regex.Match(methodData, @"\n(.+?)$").Value, @"(?:\n(.+?)[^a-zA-Z0-9]*)", "");
        Debug.Log(methodLine);
        methodLine = FormatMethodHead(methodLine);

        Dictionary<string, string> paramDesc = new Dictionary<string, string>();
        string[] paramDescSplits = allParams.Split('\n');

        for (int pd = 0; pd < paramDescSplits.Length; pd++)
        {
            string key = Regex.Match(paramDescSplits[pd], '"' + "(.+?)" + '"').Value.Replace("name=" + '"', "").Replace('"' + "", "");
            string desc = Regex.Match(paramDescSplits[pd], ">(.+?)$").Value.Replace(">","").Replace("<","");
            if (key != "" && desc != "")
            {
                paramDesc.Add(key, desc);
            }
        }
        

        string finalDoc = "";

        finalDoc += methodLine + "\n";

        if (allParams != "")
        {
            finalDoc += "<h1>Properties</h1>\n";
            finalDoc += "<table class=" + '"' + "prop-table" + '"' + "><tbody>\n";
            foreach(KeyValuePair<string, string> paramdesc in paramDesc)
            {
                finalDoc += "<tr><th>" + paramdesc.Key + "</th><th>" + paramdesc.Value + "</th></tr>\n";
            }
            finalDoc += "</tbody></table>\n";
        }

        if (returns != "")
        {
            finalDoc += "<h1>Returns</h1>\n";
            finalDoc += returns + "\n";
        }

        if (summary != "")
        {
            finalDoc += "<h1>Description</h1>\n";
            finalDoc += summary + "\n";
        }

        Debug.Log(finalDoc);
        generated = finalDoc;
    }

    static string SouroundB(Match m)
    {
        return "<b>" + m.Value + "</b>";
    }

    static string SouroundI(Match m)
    {
        return "<i>" + m.Value + "</i>";
    }

    string Tagged(string input, string tag)
    {
        input = input.Replace("\n", "&nl&");
        return Regex.Replace(Regex.Match(input, @"<" + tag + ">(.+?)</" + tag + ">").Value, @"(?:///(.*?)[^a-zA-Z0-9])|(?:&nl&)|(?:<(.*?)" + tag + ">)", "");
    }

    string TaggedAll(string input, string tag)
    {
        input = input.Replace("\n", "&nl&");
        MatchCollection got = Regex.Matches(input, @"<" + tag + "(.+?)</" + tag + ">");
        string found = "";
        for (int f = 0; f < got.Count; f++)
        {
            found += Regex.Replace(Regex.Replace(Regex.Replace(got[f].Value, @"(?:///(.*?)[^a-zA-Z0-9])|(?:&nl&)", ""), @"</" + tag + ">", "\n"), @"\n(.+?)<" + tag, "\n<" + tag);
        }
        return found;
    }

    string TaggedGreedy(string input, string tag)
    {
        input = input.Replace("\n", "&nl&");
        return Regex.Replace(Regex.Replace(Regex.Replace(Regex.Match(input, @"<" + tag + "(.+)</" + tag + ">").Value, @"(?:///(.*?)[^a-zA-Z0-9])|(?:&nl&)", ""), @"</" + tag + ">", "\n"), @"\n(.+?)<" + tag, "\n<" + tag);
    }

    /// <summary>
    /// A Field to change a float property
    /// </summary>
    /// <param name="content">The title for the field</param>
    /// <param name="property">The float property in the material</param>
    /// <param name="material">The material to use for the field - default is the scope material</param>
    /// <returns>The value of the float property</returns>
}
