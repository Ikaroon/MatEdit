using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.CodeDom.Compiler;
using System.CodeDom;
using Microsoft.CSharp;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;

namespace MB.MatEdit {
    public class DocumentationEditor : EditorWindow {

        private MonoScript script;
        private System.Type type;
        private MethodInfo[] methodInfos;

        private MethodInfo[] method;
        private string methodName;
        private string methodHead;
        private string methodSummary;
        private string methodReturns;
        Dictionary<string, string> methodProperties = new Dictionary<string, string>();

        [MenuItem("Tools/Test")]
        static void Init()
        {
            DocumentationEditor docEditor = CreateInstance<DocumentationEditor>();
            docEditor.ShowUtility();
        }

        private void OnEnable()
        {
            minSize = new Vector2(1024f, 800f);
            titleContent = new GUIContent("Documentation Editor");
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10f, 10f, position.width - 20f, position.height - 40f));
            GUILayout.BeginHorizontal();
            script = (MonoScript)EditorGUILayout.ObjectField(script, typeof(MonoScript), false);
            if (GUILayout.Button("Fetch from Method"))
            {
                if (script == null)
                {
                    return;
                }

                type = script.GetClass();
                
                methodInfos = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

                GenericMenu menu = new GenericMenu();
                for (int m = 0; m < methodInfos.Length; m++)
                {
                    string methodHead = Regex.Replace(InfoToHead(methodInfos[m]), @"\(.*?\);", "");
                    menu.AddItem(new GUIContent(methodHead), false, CreateEditor, FindMethodSiblings(methodInfos[m]));
                }
                menu.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            methodName = EditorGUILayout.TextField("Method", methodName);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Heads");
            methodHead = EditorGUILayout.TextArea(methodHead);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Summary");
            methodSummary = EditorGUILayout.TextArea(methodSummary);
            EditorGUILayout.Space();
            methodReturns = EditorGUILayout.TextField("Returns", methodReturns);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Properties");
            List<string> keys = new List<string>(methodProperties.Keys);
            for (int p = 0; p < keys.Count; p++)
            {
                methodProperties[keys[p]] = EditorGUILayout.TextField(keys[p], methodProperties[keys[p]]);
            }

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10f, position.height - 28f, position.width - 20f, 20f));
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Preview"))
            {
                ConvertToHTML();
                // TODO: Generate Html and open it
            }

            if (GUILayout.Button("Copy Code"))
            {
                // TODO: Copy generated code to the clipboard
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        void ConvertToHTML()
        {
            Debug.Log(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));
            Debug.Log(Path.Combine(Directory.GetParent(Application.dataPath).FullName, AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this))));
            Debug.Log(Directory.GetParent(Path.Combine(Directory.GetParent(Application.dataPath).FullName, AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)))).FullName);
            string path = Directory.GetParent(Path.Combine(Directory.GetParent(Application.dataPath).FullName,AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)))).FullName;
            string templatePath = Path.Combine(path, "preview_template.txt");
            string previewPath = Path.Combine(path, "preview_template.html");
            string htmlTemplate = FileOperator.ReadStringFromFile(templatePath);

            htmlTemplate = htmlTemplate.Replace("#MethodName#", PrepareHTML(methodName));
            htmlTemplate = htmlTemplate.Replace("#MethodExtra#", method[0].IsStatic ? "static" : "");
            htmlTemplate = htmlTemplate.Replace("#MethodNamespace#", PrepareHTML(method[0].DeclaringType.FullName));

            if (methodProperties.Count <= 0)
            {
                htmlTemplate = Regex.Replace(htmlTemplate, "<methodProperties>(.|\n)*?</methodProperties>", "");
            }
            else
            {
                string props = "";
                foreach (KeyValuePair<string, string> prop in methodProperties)
                {
                    props += HTMLRow(prop.Key, prop.Value);
                }
                htmlTemplate = htmlTemplate.Replace("#Properties#", props);
            }

            if (string.IsNullOrEmpty(methodReturns))
            {
                htmlTemplate = Regex.Replace(htmlTemplate, "<methodReturn>(.|\n)*?</methodReturn>", "");
            }
            else
            {
                htmlTemplate = PrepareHTML(htmlTemplate.Replace("#ReturnValue#", "<b>" + FormatToCompiler(method[0].ReturnType) + "</b> - " + methodReturns));
            }

            if (string.IsNullOrEmpty(methodSummary))
            {
                htmlTemplate = Regex.Replace(htmlTemplate, "<methodDescription>(.|\n)*?</methodDescription>", "");
            }
            else
            {
                htmlTemplate = PrepareHTML(htmlTemplate.Replace("#DescriptionValue#", methodSummary));
            }
            
            string modifiedHeads = "";
            for (int m = 0; m < method.Length; m++)
            {
                modifiedHeads += PrepareHTML(InfoToHead(method[m], delegate (CodeType t)
                {
                    switch (t)
                    {
                        case CodeType.Method: return "b";
                        case CodeType.MethodParamter: return "b";
                        case CodeType.ParameterDefault: return "i";
                    }
                    return "";
                })) + (m < method.Length - 1 ? "<br/>\n" : "\n");
            }
            // TODO: format modified header

            htmlTemplate = htmlTemplate.Replace("#MethodHead#", modifiedHeads);
            
            Debug.Log(previewPath);
            Debug.Log(htmlTemplate);

            FileOperator.WriteStringToFile(htmlTemplate, previewPath);
            Application.OpenURL("file:///" + previewPath);
        }

        string PrepareHTML(string input)
        {
            return input;// input.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        string PrepareHTMLBraces(string input)
        {
            return input.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        string HTMLRow(string key, string value)
        {
            return "<tr><th>" + PrepareHTML(key) + "</th><th>" + PrepareHTML(value) + "</th></tr>\n";
        }

        void CreateEditor(object methodInfo)
        {
            methodName = "";
            methodHead = "";
            methodSummary = "";
            methodReturns = "";

            MethodInfo[] info = (MethodInfo[])methodInfo;
            methodName = info[0].Name;
            method = info;

            string[] unformatedHeads = new string[info.Length];
            string[] formatedHeads = new string[info.Length];

            for (int i = 0; i < info.Length; i++)
            {
                unformatedHeads[i] = InfoToHead(info[i]);
                formatedHeads[i] = InfoToHead(info[i], delegate (CodeType t)
                {
                    switch (t)
                    {
                        case CodeType.Method: return "b";
                        case CodeType.MethodParamter: return "b";
                        case CodeType.ParameterDefault: return "i";
                    }
                    return "";
                });

                methodHead += unformatedHeads[i] + (i < info.Length - 1 ? "\n" : "");
                Debug.Log(formatedHeads[i]);
            }

            string[] scriptDatas = Regex.Split(script.text, @"\n(\s)*?\n");
            string xmls = "";

            for (int i = 0; i < info.Length; i++)
            {
                string searchQuery = @"\/\/\/ <summary>(.|\n)*?" + unformatedHeads[i].Replace(";", "").Replace("(", @"\(").Replace(")", @"\)").Replace("[", @"\[").Replace("]", @"\]").Replace(" ", @"(.*?)");
                foreach (string scriptData in scriptDatas)
                {
                    Match xml = Regex.Match(scriptData, searchQuery, RegexOptions.RightToLeft | RegexOptions.IgnoreCase);
                    if (xml != null)
                    {
                        xmls += xml.Value;
                    }
                }
            }

            string[] summaries = new string[info.Length];
            string[] returnVariants = new string[info.Length];
            Dictionary<string, string>[] descriptions = new Dictionary<string, string>[info.Length];

            for (int i = 0; i < info.Length; i++)
            {
                summaries[i] = MethodSummary(info[i], xmls);
                returnVariants[i] = MethodReturn(info[i], xmls);
                descriptions[i] = PropertyDescriptions(info[i], xmls);
            }

            string summary = "";
            string returns = "";
            Dictionary<string, string> finalDescriptions = new Dictionary<string, string>();

            for (int i = 0; i < info.Length; i++)
            {
                if (summaries[i].Length > summary.Length)
                {
                    summary = summaries[i];
                }
                if (returnVariants[i].Length > returns.Length)
                {
                    returns = returnVariants[i];
                }
                foreach (KeyValuePair<string, string> description in descriptions[i])
                {
                    if (!finalDescriptions.ContainsKey(description.Key))
                    {
                        finalDescriptions.Add(description.Key, description.Value);
                    }
                }
            }

            Debug.Log("<b>Summary:</b> " + summary);
            methodSummary = summary;
            foreach (KeyValuePair<string, string> description in finalDescriptions)
            {
                Debug.Log("<b>" + description.Key + ":</b> " + description.Value);
            }
            methodProperties = finalDescriptions;
            Debug.Log("<b>Returns:</b> " + returns);
            methodReturns = returns;
        }

        string MethodReturn(MethodInfo info, string xml)
        {
            string returns = Regex.Replace(Regex.Replace(Regex.Match(xml, "<returns>(.|\n)*?" + "</returns>").Value, "<(.*?)returns>", ""), @"\n(.*?)\/\/\/", "");
            return returns;
        }

        string MethodSummary(MethodInfo info, string xml)
        {
            string summary = Regex.Replace(Regex.Replace(Regex.Match(xml, "<summary>(.|\n)*?" + "</summary>").Value, "<(.*?)summary>", ""), @"\n(.*?)\/\/\/", "");
            return summary;
        }

        Dictionary<string, string> PropertyDescriptions(MethodInfo info, string xml)
        {
            ParameterInfo[] parameters = info.GetParameters();
            Dictionary<string, string> descriptions = new Dictionary<string, string>();

            for (int p = 0; p < parameters.Length; p++)
            {
                string description = Regex.Replace(Regex.Replace(Regex.Match(xml, "<param(.*?)" + '"' + parameters[p].Name + '"' + "(.*?)>(.|\n)*?" + "</param>").Value, "<param(.*?)>|</param>", ""), @"\n(.*?)\/\/\/", "");
                descriptions.Add(parameters[p].Name, description);
            }

            return descriptions;
        }

        MethodInfo[] FindMethodSiblings(MethodInfo info)
        {
            List<MethodInfo> namedMethodInfos = new List<MethodInfo>(type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance));
            for (int n = namedMethodInfos.Count - 1; n >= 0; n--)
            {
                if (namedMethodInfos[n].Name != info.Name)
                {
                    namedMethodInfos.RemoveAt(n);
                }
            }
            return namedMethodInfos.ToArray();
        }

        string InfoToHead(MethodInfo info, System.Func<CodeType, string> format = null)
        {
            string methodHead = "";

            methodHead += info.IsPublic ? "public " : "";
            methodHead += info.IsPrivate ? "private " : "";
            methodHead += info.IsStatic ? FormatCode("static ", CodeType.Static, format) : "";
            methodHead += info.IsAbstract ? FormatCode("abstract ", CodeType.Abstract, format) : "";
            methodHead += info.IsVirtual ? FormatCode("virtual ", CodeType.Virtual, format) : "";
            methodHead += FormatCode(PrepareHTMLBraces(FormatToCompiler(info.ReturnType)), CodeType.PrimitveType, format) + " ";
            methodHead = FormatCode(methodHead, CodeType.Keyword, format);
            methodHead += FormatCode(info.Name, CodeType.Method, format) + "(";

            ParameterInfo[] parameters = info.GetParameters();

            for (int p = 0; p < parameters.Length; p++)
            {
                string paramterType = PrepareHTMLBraces(FormatToCompiler(parameters[p].ParameterType));
                if (parameters[p].ParameterType.IsClass)
                {
                    paramterType = FormatCode(paramterType, CodeType.Class, format).Replace("UnityEngine.", "").Replace("UnityEditor.", "");
                }
                else
                {
                    paramterType = FormatCode(paramterType, CodeType.PrimitveType, format);
                }
                methodHead += paramterType + " ";
                methodHead += FormatCode(parameters[p].Name, CodeType.MethodParamter, format);
                if (parameters[p].DefaultValue == null)
                {
                    methodHead += " = " + FormatCode(FormatCode("null", CodeType.PrimitveType, format), CodeType.ParameterDefault, format);
                }
                else if (parameters[p].DefaultValue.GetType() != typeof(System.DBNull))
                {
                    string defaultValue = parameters[p].DefaultValue + "";
                    if (parameters[p].DefaultValue.GetType().IsClass)
                    {
                        defaultValue = FormatCode(defaultValue, CodeType.Class, format);
                    }
                    else
                    {
                        defaultValue = FormatCode(ValueFitType(defaultValue, parameters[p].DefaultValue.GetType()), CodeType.PrimitveType, format);
                    }
                    methodHead += " = " + FormatCode(defaultValue, CodeType.ParameterDefault, format);
                }

                if (p < parameters.Length - 1)
                {
                    methodHead += ", ";
                }
            }

            methodHead += ");";

            return methodHead;
        }

        string ValueFitType(string value, System.Type type)
        {
            if (type == typeof(float))
            {
                return value + "f";
            }
            return value;
        }

        string FormatToCompiler(System.Type input)
        {
            CSharpCodeProvider compiler = new CSharpCodeProvider();
            string result = compiler.GetTypeOutput(new CodeTypeReference(input)).Replace(input.Namespace + ".", "");
            compiler = null;
            return result;
        }

        string FormatCode(string input, CodeType type, System.Func<CodeType, string> format)
        {
            if (format == null)
            {
                return input;
            }
            else
            {
                string formatTag = format.Invoke(type);
                if (string.IsNullOrEmpty(formatTag))
                {
                    return input;
                }
                return "<" + formatTag + ">" + input + "</" + formatTag + ">";
            }
        }

        enum CodeType
        {
            Class,
            Constructor,
            Deconstructor,
            Delegate,
            Enum,
            Event,
            Field,
            Interface,
            Keyword,
            Label,
            Method,
            MethodParamter,
            NamedParamter,
            ParameterDefault,
            Namespace,
            Number,
            Operator,
            Preprocessor,
            PrimitveType,
            Property,
            String,
            Struct,
            StructConstructor,
            TypeParameter,
            Variable,
            Constant,
            Static,
            Abstract,
            Virtual
        }
    }
}