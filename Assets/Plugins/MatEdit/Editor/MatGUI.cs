using UnityEngine;
using UnityEditor;

namespace MB.MatEdit
{
    public class MatGUI
    {
        #region Static Data

        private static Material SCOPE_MATERIAL;

        #endregion

        #region Static Methods

        public static void SetScope(Material material)
        {
            SCOPE_MATERIAL = material;
        }

        #endregion SettingsFunctions

        //-----------------------------------------------------------------------------------------

        #region Style Data

        private static GUIStyle[] groupStyles;
        private static GUIStyle optionsStyle;
        private static GUIStyle[] groupTitleStyles;

        #endregion

        #region Style Methods

        private static void InitStyles()
        {
            if (groupStyles != null)
            {
                return;
            }

            groupStyles = new GUIStyle[4];
            groupTitleStyles = new GUIStyle[4];

            // Main Group
            groupStyles[0] = new GUIStyle(GUI.skin.GetStyle("LargeButton"));
            groupStyles[0].padding = new RectOffset(10, 10, 5, 5);
            groupStyles[0].margin = new RectOffset(0, 0, 5, 5);

            groupTitleStyles[0] = new GUIStyle(GUI.skin.GetStyle("Label"));
            groupTitleStyles[0].alignment = TextAnchor.MiddleLeft;
            groupTitleStyles[0].margin = new RectOffset(0, 0, 0, 0);

            // Main Two Group
            groupStyles[1] = new GUIStyle(GUI.skin.GetStyle("GroupBox"));
            groupStyles[1].padding = new RectOffset(10, 10, 5, 5);
            groupStyles[1].margin = new RectOffset(0, 0, 5, 5);

            groupTitleStyles[1] = new GUIStyle(GUI.skin.GetStyle("Label"));
            groupTitleStyles[1].alignment = TextAnchor.MiddleLeft;
            groupTitleStyles[1].margin = new RectOffset(0, 0, 0, 0);

            // Sub Group
            groupStyles[2] = new GUIStyle(EditorStyles.helpBox);
            groupStyles[2].padding = new RectOffset(10, 10, 5, 5);
            groupStyles[2].margin = new RectOffset(0, 0, 5, 5);

            groupTitleStyles[2] = new GUIStyle(GUI.skin.GetStyle("Label"));
            groupTitleStyles[2].alignment = TextAnchor.MiddleLeft;
            groupTitleStyles[2].margin = new RectOffset(0, 0, 0, 0);

            // Rounded Group
            groupStyles[3] = new GUIStyle(GUI.skin.GetStyle("CN CountBadge"));
            groupStyles[3].fixedHeight = 0f;
            groupStyles[3].padding = new RectOffset(10, 10, 5, 5);
            groupStyles[3].margin = new RectOffset(0, 0, 5, 5);

            groupTitleStyles[3] = new GUIStyle(GUI.skin.GetStyle("Label"));
            groupTitleStyles[3].alignment = TextAnchor.MiddleLeft;
            groupTitleStyles[3].margin = new RectOffset(0, 0, 0, 0);

            // Group Options
            optionsStyle = new GUIStyle(GUI.skin.GetStyle("Icon.TrackOptions"));
            optionsStyle.alignment = TextAnchor.MiddleRight;
            optionsStyle.margin = new RectOffset(0, 0, 5, 5);
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Task Data

        private enum Task
        {
            None,

            PrepareReset,
            Reset
        }

        private static Task currentTask = Task.None;
        private static object taskObject;

        #endregion

        #region Task Methods

        private static void RegisterTask(Task task, object obj)
        {
            currentTask = task;
            taskObject = obj;
        }

        #endregion

        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //-----------------------------------< SIMPLE FIELDS >-----------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\

        #region Int Field

        public static void IntField(GUIContent content, string property)
        {
            IntField(content, property, SCOPE_MATERIAL);
        }

        public static void IntField(GUIContent content, string property, Material material)
        {
            InitStyles();

            if (currentTask == Task.Reset)
            {
                Material tempMat = new Material(material.shader);
                material.SetInt(property, tempMat.GetInt(property));
                Object.DestroyImmediate(tempMat);
            }

            material.SetInt(property, EditorGUILayout.IntField(content, material.GetInt(property)));
        }

        #endregion

        #region Popup

        public static int Popup(GUIContent content, string property, GUIContent[] options)
        {
            return Popup(content, property, options, SCOPE_MATERIAL);
        }

        public static int Popup(GUIContent content, string property, GUIContent[] options, Material material)
        {
            InitStyles();

            if (currentTask == Task.Reset)
            {
                Material tempMat = new Material(material.shader);
                material.SetInt(property, tempMat.GetInt(property));
                Object.Destroy(tempMat);
            }

            int lResult = EditorGUILayout.Popup(content, material.GetInt(property), options);
            material.SetInt(property, lResult);
            return lResult;
        }

        #endregion

        #region Float Field

        public static void FloatField(GUIContent content, string property)
        {
            FloatField(content, property, SCOPE_MATERIAL);
        }

        public static void FloatField(GUIContent content, string property, Material material)
        {
            InitStyles();

            if (currentTask == Task.Reset)
            {
                Material tempMat = new Material(material.shader);
                material.SetFloat(property, tempMat.GetFloat(property));
                Object.DestroyImmediate(tempMat);
            }

            material.SetFloat(property, EditorGUILayout.FloatField(content, material.GetFloat(property)));
        }

        #endregion

        #region Slider Field

        public static void SliderField(GUIContent content, string property, float min, float max, float snapSize = 0f)
        {
            SliderField(content, property, min, max, SCOPE_MATERIAL, snapSize);
        }

        public static void SliderField(GUIContent content, string property, float min, float max, Material material, float snapSize = 0f)
        {
            InitStyles();

            if (currentTask == Task.Reset)
            {
                Material tempMat = new Material(material.shader);
                material.SetFloat(property, tempMat.GetFloat(property));
                Object.DestroyImmediate(tempMat);
            }

            float lValue = EditorGUILayout.Slider(content, material.GetFloat(property), min, max);
            if (snapSize != 0f)
            {
                lValue = Mathf.Round(lValue / snapSize) * snapSize;
            }
            material.SetFloat(property, lValue);
        }

        #endregion

        #region Min Max Slider Field

        public static void MinMaxSliderField(GUIContent content, string startProperty, string endProperty, float min, float max, bool drawFloatFields = false)
        {
            MinMaxSliderField(content, startProperty, endProperty, min, max, SCOPE_MATERIAL, drawFloatFields);
        }

        public static void MinMaxSliderField(GUIContent content, string startProperty, string endProperty, float min, float max, Material material, bool drawFloatFields = false)
        {
            InitStyles();

            if (currentTask == Task.Reset)
            {
                Material tempMat = new Material(material.shader);
                material.SetFloat(startProperty, tempMat.GetFloat(startProperty));
                material.SetFloat(endProperty, tempMat.GetFloat(endProperty));
                Object.DestroyImmediate(tempMat);
            }

            float lMinValue = material.GetFloat(startProperty);
            float lMaxValue = material.GetFloat(endProperty);

            EditorGUILayout.MinMaxSlider(content, ref lMinValue, ref lMaxValue, min, max);

            if (drawFloatFields)
            {
                EditorGUILayout.BeginHorizontal();

                lMinValue = EditorGUILayout.FloatField(lMinValue);
                lMaxValue = EditorGUILayout.FloatField(lMaxValue);

                lMinValue = Mathf.Clamp(Mathf.Min(lMinValue, lMaxValue), min, max);
                lMaxValue = Mathf.Clamp(Mathf.Max(lMaxValue, lMinValue), min, max);

                EditorGUILayout.EndHorizontal();
            }

            material.SetFloat(startProperty, lMinValue);
            material.SetFloat(endProperty, lMaxValue);
        }

        #endregion

        #region Toggle Field

        public static void ToggleField(GUIContent content, string property, ToggleMode toggleMode)
        {
            ToggleField(content, property, toggleMode, SCOPE_MATERIAL);
        }

        public static void ToggleField(GUIContent content, string property, ToggleMode toggleMode, Material material)
        {
            InitStyles();
            
            switch (toggleMode)
            {
                case ToggleMode.Int:
                    {
                        if (currentTask == Task.Reset)
                        {
                            Material tempMat = new Material(material.shader);
                            material.SetInt(property, tempMat.GetInt(property));
                            Object.DestroyImmediate(tempMat);
                        }

                        EditorGUI.BeginChangeCheck();
                        bool tempOpen = (int)Mathf.Clamp01(material.GetInt(property)) == 1;
                        tempOpen = EditorGUILayout.Toggle(content, tempOpen);
                        if (EditorGUI.EndChangeCheck())
                        {
                            material.SetInt(property, tempOpen ? 1 : 0);
                        }
                    }
                    break;
                case ToggleMode.Feature:
                    {
                        if (currentTask == Task.Reset)
                        {
                            Material tempMat = new Material(material.shader);
                            bool tempReset = tempMat.IsKeywordEnabled(property);
                            if (tempReset)
                            {
                                material.EnableKeyword(property);
                            }
                            else
                            {
                                material.DisableKeyword(property);
                            }
                            Object.DestroyImmediate(tempMat);
                        }

                        EditorGUI.BeginChangeCheck();
                        bool tempOpen = material.IsKeywordEnabled(property);
                        tempOpen = EditorGUILayout.Toggle(content, tempOpen);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (tempOpen)
                            {
                                material.EnableKeyword(property);
                            }
                            else
                            {
                                material.DisableKeyword(property);
                            }
                        }
                    }
                    break;
                case ToggleMode.Pass:
                    {
                        if (currentTask == Task.Reset)
                        {
                            Material tempMat = new Material(material.shader);
                            bool tempReset = tempMat.GetShaderPassEnabled(property);
                            material.SetShaderPassEnabled(property, tempReset);
                            Object.DestroyImmediate(tempMat);
                        }

                        EditorGUI.BeginChangeCheck();
                        bool tempOpen = material.GetShaderPassEnabled(property);
                        Debug.Log(tempOpen);
                        tempOpen = EditorGUILayout.Toggle(content, tempOpen);
                        if (EditorGUI.EndChangeCheck())
                        {
                            material.SetShaderPassEnabled(property, tempOpen);
                        }

                        Debug.Log(tempOpen);
                    }
                    break;
            }
        }

        #endregion

        #region Color Field

        public static void ColorField(GUIContent content, string property)
        {
            ColorField(content, property, SCOPE_MATERIAL);
        }

        public static void ColorField(GUIContent content, string property, Material material)
        {
            InitStyles();

            if (!material.HasProperty(property))
            {
                return;
            }

            if (currentTask == Task.Reset)
            {
                Material tempMat = new Material(material.shader);
                material.SetColor(property, tempMat.GetColor(property));
                Object.DestroyImmediate(tempMat);
            }

            material.SetColor(property, EditorGUILayout.ColorField(content, material.GetColor(property)));
        }

        #endregion

        #region Float As Vector Field

        public static void FloatAsVectorField(GUIContent content, string property, PackagePart component)
        {
            FloatAsVectorField(content, property, component, SCOPE_MATERIAL);
        }

        public static void FloatAsVectorField(GUIContent content, string property, PackagePart component, Material material)
        {
            InitStyles();

            if (currentTask == Task.Reset)
            {
                Material tempMat = new Material(material.shader);
                material.SetVector(property, tempMat.GetVector(property));
                Object.DestroyImmediate(tempMat);
            }

            Vector4 tempVector = material.GetVector(property);
            tempVector[(int)component] = EditorGUILayout.FloatField(content, tempVector[(int)component]);
            material.SetVector(property, tempVector);
        }

        #endregion

        #region Vector Field

        public static void VectorField(GUIContent content, string property, params PackagePart[] part)
        {
            VectorField(content, property, SCOPE_MATERIAL, part);
        }

        public static void VectorField(GUIContent content, string property, Material material, params PackagePart[] part)
        {
            InitStyles();

            if (!material.HasProperty(property))
            {
                return;
            }

            if (currentTask == Task.Reset)
            {
                Material tempMat = new Material(material.shader);
                material.SetVector(property, tempMat.GetVector(property));
                Object.DestroyImmediate(tempMat);
            }

            Vector4 lOriginal = material.GetVector(property);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(content);
            for (int p = 0; p < part.Length; p++)
            {
                lOriginal[(int)part[p]] = EditorGUILayout.FloatField(lOriginal[(int)part[p]]);
            }
            EditorGUILayout.EndHorizontal();
            material.SetVector(property, lOriginal);
        }

        #endregion

        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //------------------------------------< GROUP FIELDS >-----------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\

        #region Group Data

        private static int groupLevel = 0;

        #endregion

        #region Static Group

        public static void Group(GUIContent title, GroupStyle style, System.Action content)
        {
            Group(title, style, null, content);
        }

        public static void Group(GUIContent title, GroupStyle style, System.Action<GenericMenu> context, System.Action content)
        {
            InitStyles();

            GUILayout.BeginVertical(groupStyles[(int)style]);

            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(title, groupTitleStyles[(int)style]);

            GroupContext(context, content, groupLevel + 1);

            GUILayout.EndHorizontal();

            GroupContent(content);

            GUILayout.EndVertical();
        }

        #endregion

        #region Fold Group

        public static void FoldGroup(GUIContent title, string property, GroupStyle style, System.Action content, Material material = null)
        {
            FoldGroup(title, property, style, null, content, material);
        }

        public static void FoldGroup(GUIContent title, string property, GroupStyle style, System.Action<GenericMenu> context, System.Action content, Material material = null)
        {
            if (material == null)
            {
                material = SCOPE_MATERIAL;
            }

            InitStyles();

            GUILayout.BeginVertical(groupStyles[(int)style]);

            GUILayout.BeginHorizontal();

            bool foldOpen = GetMaterialSubProperty(property, material);
            if (GUILayout.Button(title, groupTitleStyles[(int)style]))
            {
                foldOpen = !foldOpen;
                SetMaterialSubProperty(property, foldOpen, material);
            }

            GroupContext(context, content, groupLevel + 1);

            GUILayout.EndHorizontal();

            if (foldOpen)
            {
                GroupContent(content);
            }

            GUILayout.EndVertical();
        }

        #endregion

        #region Toggle Group

        public static void ToggleGroup(GUIContent title, string property, ToggleMode toggleMode, GroupStyle style, System.Action content)
        {
            ToggleGroup(title, property, toggleMode, style, SCOPE_MATERIAL, null, content);
        }
        
        public static void ToggleGroup(GUIContent title, string property, ToggleMode toggleMode, GroupStyle style, Material material, System.Action<GenericMenu> context, System.Action content)
        {

            InitStyles();

            GUILayout.BeginVertical(groupStyles[(int)style]);

            GUILayout.BeginHorizontal();

            bool tempOpen = false;

            switch (toggleMode)
            {
                case ToggleMode.Int:
                    {
                        EditorGUI.BeginChangeCheck();
                        tempOpen = (int)Mathf.Clamp01(material.GetInt(property)) == 1;
                        tempOpen = EditorGUILayout.Toggle(tempOpen, GUILayout.Width(20f));
                        if (EditorGUI.EndChangeCheck())
                        {
                            material.SetInt(property, tempOpen ? 1 : 0);
                        }
                    }
                    break;
                case ToggleMode.Feature:
                    {
                        EditorGUI.BeginChangeCheck();
                        tempOpen = material.IsKeywordEnabled(property);
                        tempOpen = EditorGUILayout.Toggle(tempOpen, GUILayout.Width(20f));
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (tempOpen)
                            {
                                material.EnableKeyword(property);
                            }
                            else
                            {
                                material.DisableKeyword(property);
                            }
                        }
                    }
                    break;
                case ToggleMode.Pass:
                    {
                        EditorGUI.BeginChangeCheck();
                        tempOpen = material.GetShaderPassEnabled(property);
                        Debug.Log(tempOpen);
                        tempOpen = EditorGUILayout.Toggle(tempOpen, GUILayout.Width(20f));
                        if (EditorGUI.EndChangeCheck())
                        {
                            material.SetShaderPassEnabled(property, tempOpen);
                        }

                        Debug.Log(tempOpen);
                    }
                    break;
            }
            
            EditorGUILayout.LabelField(title, groupTitleStyles[(int)style]);

            GroupContext(context, content, groupLevel + 1);

            GUILayout.EndHorizontal();

            if (tempOpen)
            {
                GroupContent(content);
            }

            GUILayout.EndVertical();
        }

        #endregion

        #region Group Methods

        private static bool GetMaterialSubProperty(string property, Material material)
        {
            string path = AssetDatabase.GetAssetPath(material);
            if (path == "")
            {
                return false;
            }

            Object[] obj = AssetDatabase.LoadAllAssetsAtPath(path);
            MatGUI_DATA data = null;
            int o = 0;
            while (o < obj.Length && data == null)
            {
                if (obj[o].GetType() == typeof(MatGUI_DATA))
                {
                    data = (MatGUI_DATA)obj[o];
                }
                o++;
            }
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<MatGUI_DATA>();
                data.hideFlags = HideFlags.HideInHierarchy;
                data.name = "MatGUI_DATA";
                AssetDatabase.AddObjectToAsset(data, material);
                AssetDatabase.ImportAsset(path);
            }

            if (data.properties.ContainsKey(property))
            {
                return data.properties[property];
            }
            else
            {
                data.properties.Add(property, true);
                EditorUtility.SetDirty(data);
                return data.properties[property];
            }
        }

        private static void SetMaterialSubProperty(string property, bool value, Material material)
        {
            string path = AssetDatabase.GetAssetPath(material);
            if (path == "")
            {
                return;
            }

            Object[] obj = AssetDatabase.LoadAllAssetsAtPath(path);
            MatGUI_DATA data = null;
            int o = 0;
            while (o < obj.Length && data == null)
            {
                if (obj[o].GetType() == typeof(MatGUI_DATA))
                {
                    data = (MatGUI_DATA)obj[o];
                }
                o++;
            }
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<MatGUI_DATA>();
                data.hideFlags = HideFlags.HideInHierarchy;
                data.name = "MatGUI_DATA";
                AssetDatabase.AddObjectToAsset(data, material);
                AssetDatabase.ImportAsset(path);
            }

            if (data.properties.ContainsKey(property))
            {
                data.properties[property] = value;
                EditorUtility.SetDirty(data);
            }
            else
            {
                data.properties.Add(property, value);
                EditorUtility.SetDirty(data);
            }
        }

        private static void GroupContext(System.Action<GenericMenu> context, System.Action content, int level)
        {
            if (GUILayout.Button("", optionsStyle))
            {
                GenericMenu menu = new GenericMenu();
                if (content != null)
                {
                    menu.AddItem(new GUIContent("Reset", "Resets all elements in the group"), false, ResetGroup, new ResetPackage(content, level));
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Reset", "Resets all elements in the group"));
                }

                if (context != null)
                {
                    menu.AddSeparator("");
                    context.Invoke(menu);
                }

                menu.ShowAsContext();
            }
        }

        private static void GroupContent(System.Action content)
        {
            groupLevel++;

            ResetPackage taskConvert = ((ResetPackage)taskObject);
            if (currentTask == Task.PrepareReset)
            {
                if (taskConvert != null && taskConvert.groupLevel == groupLevel && taskConvert.resetContent.Method == content.Method)
                {
                    RegisterTask(Task.Reset, null);
                }
            }

            if (content != null)
            {
                GUILayout.Space(5f);
                content.Invoke();
            }

            if (taskConvert != null && groupLevel == taskConvert.groupLevel && currentTask == Task.Reset)
            {
                RegisterTask(Task.None, null);
            }

            groupLevel--;
        }

        private static void ResetGroup(object obj)
        {
            RegisterTask(Task.PrepareReset, obj);
        }

        private class ResetPackage
        {
            public System.Action resetContent;
            public int groupLevel;

            public ResetPackage(System.Action content, int level)
            {
                resetContent = content;
                groupLevel = level;
            }
        }

        #endregion
    }
}