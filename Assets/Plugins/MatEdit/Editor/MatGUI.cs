using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace MB.MatEdit
{
    public sealed class MatGUI
    {
        #region Static Data

        /// <summary>
        /// The material which is used for all methods when no material is passed
        /// </summary>
        private static Material SCOPE_MATERIAL;

        #endregion

        #region Static Methods

        /// <summary>
        /// Set the material which is used for all upcoming fields which do not use a material parameter
        /// </summary>
        /// <param name="material">The desired scope material</param>
        public static void SetScope(Material material)
        {
            SCOPE_MATERIAL = material;
        }

        #endregion SettingsFunctions

        //-----------------------------------------------------------------------------------------

        #region Style Data
        
        /// <summary>
        /// The styles used for the different groups
        /// </summary>
        private static GUIStyle[] groupStyles;

        /// <summary>
        /// The styles used for the titles of the groups (prepared for possibly different title styles)
        /// </summary>
        private static GUIStyle[] groupTitleStyles;

        /// <summary>
        /// The style for the drop-down menu icon
        /// </summary>
        private static GUIStyle optionsStyle;

        #endregion

        #region Style Methods

        /// <summary>
        /// Initializes the Styles for the MatGUI system
        /// </summary>
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

        /// <summary>
        /// The tasks possible for the internal use
        /// </summary>
        private enum Task
        {
            None,

            PrepareReset,
            Reset,

            PrepareCopy,
            Copy,
            
            Paste
        }

        /// <summary>
        /// The currently used task
        /// </summary>
        private static Task currentTask = Task.None;

        /// <summary>
        /// The object used for the current task
        /// </summary>
        private static object taskObject;

        #endregion

        #region Task Methods

        /// <summary>
        /// Register a task with an object
        /// </summary>
        /// <param name="task">The type of task which should be applied</param>
        /// <param name="obj">The object which should be registered for this task</param>
        private static void RegisterTask(Task task, object obj)
        {
            currentTask = task;
            taskObject = obj;
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Helper Classes

        /// <summary>
        /// A package which stores data to recognize the calling group for the reset
        /// </summary>
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


        /// <summary>
        /// A package of CopyData for copying data from a group
        /// </summary>
        [System.Serializable]
        private class CopyPackage
        {
            [SerializeField]
            public List<CopyData> copyData = new System.Collections.Generic.List<CopyData>();
        }

        /// <summary>
        /// The field based copy data - this is specific for each kind of field
        /// </summary>
        [System.Serializable]
        private class CopyData
        {
            [SerializeField]
            FieldType type;

            [SerializeField]
            float floatContent;

            [SerializeField]
            int intContent;

            [SerializeField]
            Vector4 vectorContent;

            [SerializeField]
            Color colorContent;

            [SerializeField]
            FloatVector floatVectorContent;

            [SerializeField]
            bool boolContent;

            [SerializeField]
            string pathContent;

            [SerializeField]
            string property;

            // Constructors

            public CopyData(FieldType t, string p, float fc = 0f, int ic = 0, bool bc = false, string pc = "")
            {
                type = t;
                property = p;

                floatContent = fc;
                intContent = ic;
                boolContent = bc;
                pathContent = pc;
            }

            public CopyData(FieldType t, string p, Color cc)
            {
                type = t;
                property = p;

                colorContent = cc;
            }

            public CopyData(FieldType t, string p, Vector4 vc)
            {
                type = t;
                property = p;

                vectorContent = vc;
            }

            public CopyData(FieldType t, string p, FloatVector fvc)
            {
                type = t;
                property = p;

                floatVectorContent = fvc;
            }
            
            /// <summary>
            /// Applies the saved data - typacilly used for a paste progress
            /// </summary>
            /// <param name="mat">The material on which the data should be applied</param>
            public void Apply(Material mat)
            {
                switch (type)
                {
                    case FieldType.Int:
                        mat.SetInt(property, intContent);
                        break;
                    case FieldType.Float:
                        mat.SetFloat(property, floatContent);
                        break;
                    case FieldType.Vector:
                        mat.SetVector(property, vectorContent);
                        break;
                    case FieldType.Color:
                        mat.SetColor(property, colorContent);
                        break;
                    case FieldType.FloatVector:
                        Vector4 tempVector = mat.GetVector(property);
                        tempVector[(int)floatVectorContent.part] = floatVectorContent.value;
                        mat.SetVector(property, tempVector);
                        break;
                    case FieldType.Keyword:
                        if (boolContent)
                        {
                            mat.EnableKeyword(property);
                        }
                        else
                        {
                            mat.DisableKeyword(property);
                        }
                        break;
                    case FieldType.Pass:
                        mat.SetShaderPassEnabled(property, boolContent);
                        break;
                    case FieldType.Texture:
                        mat.SetTexture(property, AssetDatabase.LoadAssetAtPath<Texture>(pathContent));
                        break;
                }
            }
        }

        /// <summary>
        /// A field to save the modification of a vector in one component
        /// </summary>
        [System.Serializable]
        private class FloatVector
        {
            [SerializeField]
            public float value;

            [SerializeField]
            public PackagePart part;

            public FloatVector(float v, PackagePart p)
            {
                value = v;
                part = p;
            }
        }

        #endregion

        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //-----------------------------------< SIMPLE FIELDS >-----------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\

        #region Int Field
        
        /// <summary>
        /// A Field to change an int property
        /// </summary>
        /// <param name="content">The title for the field</param>
        /// <param name="property">The int property in the material</param>
        /// <param name="material">The material to use for the field - default is the scope material</param>
        /// <returns>The value of the int property</returns>
        public static int IntField(GUIContent content, string property, Material material = null)
        {
            if (material == null)
            {
                material = SCOPE_MATERIAL;
            }

            InitStyles();

            // Process Tasks
            if (currentTask == Task.Reset)
            {
                Material tempMat = new Material(material.shader);
                material.SetInt(property, tempMat.GetInt(property));
                Object.DestroyImmediate(tempMat);
            }
            else if (currentTask == Task.Copy)
            {
                CopyPackage tempS = (CopyPackage)taskObject;
                tempS.copyData.Add(new CopyData(FieldType.Int, property, ic: material.GetInt(property)));
            }

            // Draw Editor Field
            material.SetInt(property, EditorGUILayout.IntField(content, material.GetInt(property)));
            return material.GetInt(property);
        }

        #endregion

        #region Popup

        /// <summary>
        /// A Field to change an int property in form of an popup
        /// </summary>
        /// <param name="content">The title for the field</param>
        /// <param name="property">The int property in the material</param>
        /// <param name="options">An array of dropdown options</param>
        /// <param name="material">The material to use for the field - default is the scope material</param>
        /// <returns>The value of the int property</returns>
        public static int Popup(GUIContent content, string property, GUIContent[] options, Material material = null)
        {
            if (material == null)
            {
                material = SCOPE_MATERIAL;
            }

            InitStyles();

            // Process Tasks
            if (currentTask == Task.Reset)
            {
                Material tempMat = new Material(material.shader);
                material.SetInt(property, tempMat.GetInt(property));
                Object.Destroy(tempMat);
            }
            else if (currentTask == Task.Copy)
            {
                CopyPackage tempS = (CopyPackage)taskObject;
                tempS.copyData.Add(new CopyData(FieldType.Int, property, ic: material.GetInt(property)));
            }

            // Draw Editor Field
            int lResult = EditorGUILayout.Popup(content, material.GetInt(property), options);
            material.SetInt(property, lResult);
            return lResult;
        }

        #endregion

        #region Float Field

        /// <summary>
        /// A Field to change a float property
        /// </summary>
        /// <param name="content">The title for the field</param>
        /// <param name="property">The float property in the material</param>
        /// <param name="material">The material to use for the field - default is the scope material</param>
        /// <returns>The value of the float property</returns>
        public static float FloatField(GUIContent content, string property, Material material = null)
        {
            if (material == null)
            {
                material = SCOPE_MATERIAL;
            }

            InitStyles();

            // Process Tasks
            if (currentTask == Task.Reset)
            {
                Material tempMat = new Material(material.shader);
                material.SetFloat(property, tempMat.GetFloat(property));
                Object.DestroyImmediate(tempMat);
            }
            else if (currentTask == Task.Copy)
            {
                CopyPackage tempS = (CopyPackage)taskObject;
                tempS.copyData.Add(new CopyData(FieldType.Float, property, fc: material.GetFloat(property)));
            }

            // Draw Editor Fields
            material.SetFloat(property, EditorGUILayout.FloatField(content, material.GetFloat(property)));
            return material.GetFloat(property);
        }

        #endregion

        #region Slider Field

        /// <summary>
        /// A Field to change a float property in form of a slider
        /// </summary>
        /// <param name="content">The title for the field</param>
        /// <param name="property">The float property in the material</param>
        /// <param name="min">The min border for the slider</param>
        /// <param name="max">The max border for the slider</param>
        /// <param name="snapSize">The step size for the snapping</param>
        /// <param name="material">The material to use for the field - default is the scope material</param>
        /// <returns>The value of the float property</returns>
        public static float SliderField(GUIContent content, string property, float min, float max, float snapSize = 0f, Material material = null)
        {
            if (material == null)
            {
                material = SCOPE_MATERIAL;
            }

            InitStyles();

            // Process Tasks
            if (currentTask == Task.Reset)
            {
                Material tempMat = new Material(material.shader);
                material.SetFloat(property, tempMat.GetFloat(property));
                Object.DestroyImmediate(tempMat);
            }
            else if (currentTask == Task.Copy)
            {
                CopyPackage tempS = (CopyPackage)taskObject;
                tempS.copyData.Add(new CopyData(FieldType.Float, property, fc: material.GetFloat(property)));
            }

            // Draw Editor Fields
            float lValue = EditorGUILayout.Slider(content, material.GetFloat(property), min, max);
            if (snapSize != 0f)
            {
                lValue = Mathf.Round(lValue / snapSize) * snapSize;
            }
            material.SetFloat(property, lValue);
            return lValue;
        }

        #endregion

        #region Min Max Slider Field

        /// <summary>
        /// A Field to change two float properties in form of a min max slider
        /// </summary>
        /// <param name="content">The title for the field</param>
        /// <param name="startProperty">The lower float property</param>
        /// <param name="endProperty">The higher float property</param>
        /// <param name="min">The min border for the slider</param>
        /// <param name="max">The max border for the slider</param>
        /// <param name="drawFloatFields">Should this field draw two float fields indicating the min and the max values</param>
        /// <param name="material">The material to use for the field - default is the scope material</param>
        /// <returns>The value of the min and the max property</returns>
        public static Vector2 MinMaxSliderField(GUIContent content, string startProperty, string endProperty, float min, float max, bool drawFloatFields = false, Material material = null)
        {
            if (material == null)
            {
                material = SCOPE_MATERIAL;
            }

            InitStyles();

            // Process Tasks
            if (currentTask == Task.Reset)
            {
                Material tempMat = new Material(material.shader);
                material.SetFloat(startProperty, tempMat.GetFloat(startProperty));
                material.SetFloat(endProperty, tempMat.GetFloat(endProperty));
                Object.DestroyImmediate(tempMat);
            }
            else if (currentTask == Task.Copy)
            {
                CopyPackage tempS = (CopyPackage)taskObject;
                tempS.copyData.Add(new CopyData(FieldType.Float, startProperty, fc: material.GetFloat(startProperty)));
                tempS.copyData.Add(new CopyData(FieldType.Float, endProperty, fc: material.GetFloat(endProperty)));
            }

            // Draw Editor Field
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

            return new Vector2(lMinValue, lMaxValue);
        }

        #endregion

        #region Toggle Field

        /// <summary>
        /// A Field to show a toggle which changes a value in the material depending on the toggleMode
        /// </summary>
        /// <param name="content">The title for the field</param>
        /// <param name="property">The property, feature or pass name in the material</param>
        /// <param name="toggleMode">The type of toggle (Int = property, Feature = keyword, Pass = shader pass)</param>
        /// <param name="material">The material to use for the field - default is the scope material</param>
        /// <returns>Is the toggle enabled or disabled</returns>
        public static bool ToggleField(GUIContent content, string property, ToggleMode toggleMode, Material material = null)
        {
            if (material == null)
            {
                material = SCOPE_MATERIAL;
            }

            InitStyles();

            // Process Toggle Mode
            switch (toggleMode)
            {
                case ToggleMode.Int:
                    {
                        // Process Tasks
                        if (currentTask == Task.Reset)
                        {
                            Material tempMat = new Material(material.shader);
                            material.SetInt(property, tempMat.GetInt(property));
                            Object.DestroyImmediate(tempMat);
                        }
                        else if (currentTask == Task.Copy)
                        {
                            CopyPackage tempS = (CopyPackage)taskObject;
                            tempS.copyData.Add(new CopyData(FieldType.Int, property, ic: material.GetInt(property)));
                        }

                        // Draw Editor Field
                        EditorGUI.BeginChangeCheck();
                        bool tempOpen = (int)Mathf.Clamp01(material.GetInt(property)) == 1;
                        tempOpen = EditorGUILayout.Toggle(content, tempOpen);
                        if (EditorGUI.EndChangeCheck())
                        {
                            material.SetInt(property, tempOpen ? 1 : 0);
                        }
                        return tempOpen;
                    }
                case ToggleMode.Feature:
                    {
                        // Process Tasks
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
                        else if (currentTask == Task.Copy)
                        {
                            CopyPackage tempS = (CopyPackage)taskObject;
                            tempS.copyData.Add(new CopyData(FieldType.Keyword, property, bc: material.IsKeywordEnabled(property)));
                        }

                        // Draw Editor Field
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
                        return tempOpen;
                    }
                case ToggleMode.Pass:
                    {
                        // Process Tasks
                        if (currentTask == Task.Reset)
                        {
                            Material tempMat = new Material(material.shader);
                            bool tempReset = tempMat.GetShaderPassEnabled(property);
                            material.SetShaderPassEnabled(property, tempReset);
                            Object.DestroyImmediate(tempMat);
                        }
                        else if (currentTask == Task.Copy)
                        {
                            CopyPackage tempS = (CopyPackage)taskObject;
                            tempS.copyData.Add(new CopyData(FieldType.Pass, property, bc: material.GetShaderPassEnabled(property)));
                        }

                        // Draw Editor Field
                        EditorGUI.BeginChangeCheck();
                        bool tempOpen = material.GetShaderPassEnabled(property);
                        Debug.Log(tempOpen);
                        tempOpen = EditorGUILayout.Toggle(content, tempOpen);
                        if (EditorGUI.EndChangeCheck())
                        {
                            material.SetShaderPassEnabled(property, tempOpen);
                        }
                        return tempOpen;
                    }
            }
            return false;
        }

        #endregion

        #region Color Field

        /// <summary>
        /// A Field to change a color property
        /// </summary>
        /// <param name="content">The title for the field</param>
        /// <param name="property">The color property in the material</param>
        /// <param name="material">The material to use for the field - default is the scope material</param>
        /// <returns>The value of the color property</returns>
        public static Color ColorField(GUIContent content, string property, Material material = null)
        {
            if (material == null)
            {
                material = SCOPE_MATERIAL;
            }

            InitStyles();

            // Check if property exists
            if (!material.HasProperty(property))
            {
                return Color.black;
            }

            // Process Tasks
            if (currentTask == Task.Reset)
            {
                Material tempMat = new Material(material.shader);
                material.SetColor(property, tempMat.GetColor(property));
                Object.DestroyImmediate(tempMat);
            }
            else if (currentTask == Task.Copy)
            {
                CopyPackage tempS = (CopyPackage)taskObject;
                tempS.copyData.Add(new CopyData(FieldType.Color, property, material.GetColor(property)));
            }

            // Draw Editor Field
            material.SetColor(property, EditorGUILayout.ColorField(content, material.GetColor(property)));
            return material.GetColor(property);
        }

        #endregion

        #region Float As Vector Field

        /// <summary>
        /// A Field to change a component in a vector property in form of a float field
        /// </summary>
        /// <param name="content">The title for the field</param>
        /// <param name="property">The vector property in the material</param>
        /// <param name="component">The component of the vector</param>
        /// <param name="material">The material to use for the field - default is the scope material</param>
        /// <returns>The value at the component index in the vector property</returns>
        public static float FloatAsVectorField(GUIContent content, string property, PackagePart component, Material material = null)
        {
            if (material == null)
            {
                material = SCOPE_MATERIAL;
            }

            InitStyles();

            // Process Tasks
            if (currentTask == Task.Reset)
            {
                Material tempMat = new Material(material.shader);
                material.SetVector(property, tempMat.GetVector(property));
                Object.DestroyImmediate(tempMat);
            }
            else if (currentTask == Task.Copy)
            {
                CopyPackage tempS = (CopyPackage)taskObject;
                tempS.copyData.Add(new CopyData(FieldType.FloatVector, property, new FloatVector(material.GetVector(property)[(int)component], component)));
            }

            // Draw Editor Field
            Vector4 tempVector = material.GetVector(property);
            tempVector[(int)component] = EditorGUILayout.FloatField(content, tempVector[(int)component]);
            material.SetVector(property, tempVector);

            return tempVector[(int)component];
        }

        #endregion

        #region Vector Field

        /// <summary>
        /// A Field to change a vector property
        /// </summary>
        /// <param name="content">The title for the field</param>
        /// <param name="property">The vector property in the material</param>
        /// <param name="part">The components of the material and the order</param>
        /// <returns>The value of the Vector property</returns>
        public static Vector4 VectorField(GUIContent content, string property, params PackagePart[] part)
        {
            return VectorField(content, property, SCOPE_MATERIAL, part);
        }

        /// <summary>
        /// A Field to change a vector property
        /// </summary>
        /// <param name="content">The title for the field</param>
        /// <param name="property">The vector property in the material</param>
        /// <param name="material">The material to use for the field - default is the scope material</param>
        /// <param name="part">The components of the material and the order</param>
        /// <returns>The value of the Vector property</returns>
        public static Vector4 VectorField(GUIContent content, string property, Material material, params PackagePart[] part)
        {
            InitStyles();

            // Check if property exists
            if (!material.HasProperty(property))
            {
                return Vector4.zero;
            }

            // Process Tasks
            if (currentTask == Task.Reset)
            {
                Material tempMat = new Material(material.shader);
                material.SetVector(property, tempMat.GetVector(property));
                Object.DestroyImmediate(tempMat);
            }
            else if (currentTask == Task.Copy)
            {
                CopyPackage tempS = (CopyPackage)taskObject;
                tempS.copyData.Add(new CopyData(FieldType.Vector, property, material.GetVector(property)));
            }

            // Draw Editor Field
            Vector4 lOriginal = material.GetVector(property);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(content);
            for (int p = 0; p < part.Length; p++)
            {
                lOriginal[(int)part[p]] = EditorGUILayout.FloatField(lOriginal[(int)part[p]]);
            }
            EditorGUILayout.EndHorizontal();
            material.SetVector(property, lOriginal);
            return lOriginal;
        }

        #endregion

        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //------------------------------------< GROUP FIELDS >-----------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\

        #region Group Data

        /// <summary>
        /// Saves which level of a group is reached (e.g.: 1 is most upper level, 2 is the first sub level, ...)
        /// </summary>
        private static int groupLevel = 0;

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Static Group
        
        /// <summary>
        /// Displays a group containing all fields which are used in the content.
        /// </summary>
        /// <param name="title">The title shown in the head of the group</param>
        /// <param name="style">The style used for the group border</param>
        /// <param name="content">The content of the group: anonymous function possible (goes onto the heap) - delegate() {} </param>
        /// <param name="material">The material to use for the group commands</param>
        public static void Group(GUIContent title, GroupStyle style, System.Action content, Material material = null)
        {
            Group(title, style, null, content, material);
        }

        /// <summary>
        /// Displays a group containing all fields which are used in the content.
        /// </summary>
        /// <param name="title">The title shown in the head of the group</param>
        /// <param name="style">The style used for the group border</param>
        /// <param name="context">The context menu of the group (menu, groupContent, groupLevel): anonymous function possible (goes onto the heap) - delegate() {} </param>
        /// <param name="content">The content of the group: anonymous function possible (goes onto the heap) - delegate() {} </param>
        /// <param name="material">The material to use for the group commands</param>
        public static void Group(GUIContent title, GroupStyle style, System.Action<GenericMenu, System.Action, int> context, System.Action content, Material material = null)
        {
            if (material == null)
            {
                material = SCOPE_MATERIAL;
            }

            InitStyles();

            GUILayout.BeginVertical(groupStyles[(int)style]);

            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(title, groupTitleStyles[(int)style]);

            GroupContext(context, content, material, groupLevel + 1);

            GUILayout.EndHorizontal();

            GroupContent(content);

            GUILayout.EndVertical();
        }

        #endregion

        #region Fold Group

        /// <summary>
        /// Displays a group containing all fields which are used in the content.
        /// </summary>
        /// <param name="title">The title shown in the head of the group</param>
        /// <param name="property">The property in the MatGUI_DATA which contains if this group is open or closed</param>
        /// <param name="style">The style used for the group border</param>
        /// <param name="content">The content of the group: anonymous function possible (goes onto the heap) - delegate() {} </param>
        /// <param name="material">The material to use for the group commands</param>
        /// <returns>If the group is open</returns>
        public static bool FoldGroup(GUIContent title, string property, GroupStyle style, System.Action content, Material material = null)
        {
            return FoldGroup(title, property, style, null, content, material);
        }

        /// <summary>
        /// Displays a group containing all fields which are used in the content.
        /// </summary>
        /// <param name="title">The title shown in the head of the group</param>
        /// <param name="property">The property in the MatGUI_DATA which contains if this group is open or closed</param>
        /// <param name="style">The style used for the group border</param>
        /// <param name="context">The context menu of the group (menu, groupContent, groupLevel): anonymous function possible (goes onto the heap) - delegate() {} </param>
        /// <param name="content">The content of the group: anonymous function possible (goes onto the heap) - delegate() {} </param>
        /// <param name="material">The material to use for the group commands</param>
        /// <returns>If the group is open</returns>
        public static bool FoldGroup(GUIContent title, string property, GroupStyle style, System.Action<GenericMenu, System.Action, int> context, System.Action content, Material material = null)
        {
            if (material == null)
            {
                material = SCOPE_MATERIAL;
            }

            InitStyles();

            GUILayout.BeginVertical(groupStyles[(int)style]);

            GUILayout.BeginHorizontal();

            bool foldOpen = MatGUI_DATA_Editor.GetMaterialSubToggle(property, material);
            if (GUILayout.Button(title, groupTitleStyles[(int)style]))
            {
                foldOpen = !foldOpen;
                MatGUI_DATA_Editor.SetMaterialSubToggle(property, foldOpen, material);
            }

            GroupContext(context, content, material, groupLevel + 1);

            GUILayout.EndHorizontal();

            if (foldOpen)
            {
                GroupContent(content);
            }

            GUILayout.EndVertical();

            return foldOpen;
        }

        #endregion

        #region Toggle Group

        /// <summary>
        /// Displays a group containing all fields which are used in the content.
        /// </summary>
        /// <param name="title">The title shown in the head of the group</param>
        /// <param name="property">The property in the material which contains if this group is open or closed</param>
        /// <param name="toggleMode">The mode how to save the open/close state and what to manipulate</param>
        /// <param name="style">The style used for the group border</param>
        /// <param name="content">The content of the group: anonymous function possible (goes onto the heap) - delegate() {} </param>
        /// <param name="material">The material to use for the group commands</param>
        /// <returns>If the group is open</returns>
        public static bool ToggleGroup(GUIContent title, string property, ToggleMode toggleMode, GroupStyle style, System.Action content, Material material = null)
        {
            return ToggleGroup(title, property, toggleMode, style, null, content, material);
        }

        /// <summary>
        /// Displays a group containing all fields which are used in the content.
        /// </summary>
        /// <param name="title">The title shown in the head of the group</param>
        /// <param name="property">The property in the material which contains if this group is open or closed</param>
        /// <param name="toggleMode">The mode how to save the open/close state and what to manipulate</param>
        /// <param name="style">The style used for the group border</param>
        /// <param name="context">The context menu of the group (menu, groupContent, groupLevel): anonymous function possible (goes onto the heap) - delegate() {} </param>
        /// <param name="content">The content of the group: anonymous function possible (goes onto the heap) - delegate() {} </param>
        /// <param name="material">The material to use for the group commands</param>
        /// <returns>If the group is open</returns>
        public static bool ToggleGroup(GUIContent title, string property, ToggleMode toggleMode, GroupStyle style, System.Action<GenericMenu, System.Action, int> context, System.Action content, Material material = null)
        {
            if (material == null)
            {
                material = SCOPE_MATERIAL;
            }

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

            GroupContext(context, content, material, groupLevel + 1);

            GUILayout.EndHorizontal();

            if (tempOpen)
            {
                GroupContent(content);
            }

            GUILayout.EndVertical();

            return tempOpen;
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Group Content
        
        /// <summary>
        /// Draws the group content
        /// </summary>
        /// <param name="content">The content to draw inside the group</param>
        private static void GroupContent(System.Action content)
        {
            groupLevel++;

            // Process Tasks
            ResetPackage taskConvert = null;
            if (currentTask != Task.Copy)
            {
                taskConvert = ((ResetPackage)taskObject);
                if (currentTask == Task.PrepareReset)
                {
                    if (taskConvert != null && taskConvert.groupLevel == groupLevel && taskConvert.resetContent.Method == content.Method)
                    {
                        RegisterTask(Task.Reset, null);
                    }
                }
                else if (currentTask == Task.PrepareCopy)
                {
                    if (taskConvert != null && taskConvert.groupLevel == groupLevel && taskConvert.resetContent.Method == content.Method)
                    {
                        RegisterTask(Task.Copy, new CopyPackage());
                    }
                }
            }

            // Make space if content is available and draw it
            if (content != null)
            {
                GUILayout.Space(5f);
                content.Invoke();
            }

            //  Apply the tasks
            if (taskConvert != null && groupLevel == taskConvert.groupLevel && (currentTask == Task.Reset || currentTask == Task.Copy))
            {
                if (currentTask == Task.Copy)
                {
                    CopyPackage package = (CopyPackage)taskObject;
                    Debug.Log(package.copyData.Count);

                    string copy = JsonUtility.ToJson(package, true);
                    EditorGUIUtility.systemCopyBuffer = copy;
                }
                RegisterTask(Task.None, null);
            }

            groupLevel--;
        }

        #endregion

        #region Group Context

        /// <summary>
        /// Draws the group context menu
        /// </summary>
        /// <param name="context">The context extension given by the user</param>
        /// <param name="content">The content to identify the group</param>
        /// <param name="material">The material to apply commands on</param>
        /// <param name="level">The group level to identify the group</param>
        private static void GroupContext(System.Action<GenericMenu, System.Action, int> context, System.Action content, Material material, int level)
        {
            if (GUILayout.Button("", optionsStyle))
            {
                GenericMenu menu = new GenericMenu();
                if (content != null)
                {
                    menu.AddItem(new GUIContent("Reset", "Resets all elements in the group"), false, ResetGroup, new ResetPackage(content, level));
                    menu.AddItem(new GUIContent("Copy", "Copies the content inside of the group"), false, CopyGroup, new ResetPackage(content, level));
                    menu.AddItem(new GUIContent("Paste", "Pastes the content copied"), false, PasteGroup, material);
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Reset", "Resets all elements in the group"));
                    menu.AddDisabledItem(new GUIContent("Copy", "Copies the content inside of the group"));
                    menu.AddDisabledItem(new GUIContent("Paste", "Pastes the content copied"));
                }

                if (context != null)
                {
                    menu.AddSeparator("");
                    context.Invoke(menu, content, level);
                }

                menu.ShowAsContext();
            }
        }

        #endregion

        #region Context Methods

        /// <summary>
        /// Registers the repare reset task
        /// </summary>
        /// <param name="obj">ResetPackage to identify the group to reset</param>
        private static void ResetGroup(object obj)
        {
            RegisterTask(Task.PrepareReset, obj);
        }

        /// <summary>
        /// Registers the copy group task
        /// </summary>
        /// <param name="obj">ResetPackage to identify the group to reset</param>
        private static void CopyGroup(object obj)
        {
            RegisterTask(Task.PrepareCopy, obj);
        }

        /// <summary>
        /// Applies the copied changes onto the given material
        /// </summary>
        /// <param name="obj">The material to apply the changes on</param>
        private static void PasteGroup(object obj)
        {
            Material mat = (Material)obj;
            string content = EditorGUIUtility.systemCopyBuffer;
            CopyPackage package = JsonUtility.FromJson<CopyPackage>(content);

            if (package != null)
            {
                for (int d = 0; d < package.copyData.Count; d++)
                {
                    package.copyData[d].Apply(mat);
                }
            }
        }

        #endregion

    }
}