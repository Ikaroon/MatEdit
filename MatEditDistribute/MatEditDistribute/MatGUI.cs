// System
using System.Collections.Generic;

// Unity
using UnityEngine;
using UnityEditor;

namespace MB.MatEditDistribute
{
    /// <summary>
    /// A class to visualize a Material UI easily and with extra features
    /// </summary>
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
            int idContent;

            [SerializeField]
            List<SerializableKeyframe> curveContent = new List<SerializableKeyframe>();

            [SerializeField]
            List<SerializableColorKey> colorKeyContent = new List<SerializableColorKey>();

            [SerializeField]
            List<SerializableAlphaKey> alphaKeyContent = new List<SerializableAlphaKey>();

            [SerializeField]
            string property;

            // Constructors

            public CopyData(FieldType t, string p, float fc = 0f, int ic = 0, bool bc = false, int guidc = 0, AnimationCurve cc = null, Gradient gc = null)
            {
                type = t;
                property = p;

                floatContent = fc;
                intContent = ic;
                boolContent = bc;
                idContent = guidc;
                if (cc != null)
                {
                    Keyframe[] frames = cc.keys;
                    for (int f = 0; f < frames.Length; f++)
                    {
                        curveContent.Add(new SerializableKeyframe(frames[f]));
                    }
                }
                if (gc != null)
                {
                    GradientColorKey[] colorkeys = gc.colorKeys;
                    for (int c = 0; c < colorkeys.Length; c++)
                    {
                        colorKeyContent.Add(new SerializableColorKey(colorkeys[c]));
                    }
                    GradientAlphaKey[] alphakeys = gc.alphaKeys;
                    for (int a = 0; a < alphakeys.Length; a++)
                    {
                        alphaKeyContent.Add(new SerializableAlphaKey(alphakeys[a]));
                    }
                    idContent = (int)gc.mode;
                }
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
                if (!mat.HasProperty(property))
                {
                    return;
                }

                MaterialData data = MaterialData.Of(mat);

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
                        mat.SetTexture(property, (Texture)EditorUtility.InstanceIDToObject(idContent));
                        break;
                    case FieldType.Curve:
                        List<Keyframe> frames = new List<Keyframe>();
                        for (int f = 0; f < curveContent.Count; f++)
                        {
                            frames.Add(curveContent[f].GetKeyframe());
                        }

                        if (data.curves.ContainsKey(property))
                        {
                            data.curves[property] = new AnimationCurve(frames.ToArray());
                        }
                        else
                        {
                            data.curves.Add(property, new AnimationCurve(frames.ToArray()));
                        }

                        Texture2D mainTexture = AnimationCurveToTexture(data.curves[property], intContent);

                        if (data.unsavedTextures.ContainsKey(property))
                        {
                            Object.DestroyImmediate(data.unsavedTextures[property]);
                            data.unsavedTextures[property] = mainTexture;
                        }
                        else
                        {
                            data.unsavedTextures.Add(property, mainTexture);
                        }

                        mat.SetTexture(property, mainTexture);

                        MarkForSave(mat);

                        break;
                    case FieldType.Gradient:
                        List<GradientColorKey> colorkeys = new List<GradientColorKey>();
                        for (int c = 0; c < colorKeyContent.Count; c++)
                        {
                            colorkeys.Add(colorKeyContent[c].GetColorKey());
                        }

                        List<GradientAlphaKey> alphakeys = new List<GradientAlphaKey>();
                        for (int a = 0; a < alphaKeyContent.Count; a++)
                        {
                            alphakeys.Add(alphaKeyContent[a].GetAlphaKey());
                        }

                        Gradient tempGradient = new Gradient();
                        tempGradient.colorKeys = colorkeys.ToArray();
                        tempGradient.alphaKeys = alphakeys.ToArray();
                        tempGradient.mode = (GradientMode)idContent;
                        
                        if (data.gradients.ContainsKey(property))
                        {
                            data.gradients[property] = tempGradient;
                        }
                        else
                        {
                            data.gradients.Add(property, tempGradient);
                        }

                        EditorUtility.SetDirty(data);

                        Texture2D gradientTexture = GradientToTexture(tempGradient, intContent);

                        if (data.unsavedTextures.ContainsKey(property))
                        {
                            Object.DestroyImmediate(data.unsavedTextures[property]);
                            data.unsavedTextures[property] = gradientTexture;
                        }
                        else
                        {
                            data.unsavedTextures.Add(property, gradientTexture);
                        }

                        mat.SetTexture(property, gradientTexture);

                        MarkForSave(mat);

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

        /// <summary>
        /// A class to save the values inside of a Keyframe in a JSON
        /// </summary>
        [System.Serializable]
        private class SerializableKeyframe
        {
            [SerializeField]
            public float inTan;

            [SerializeField]
            public float outTan;

            [SerializeField]
            public int tanMode;

            [SerializeField]
            public float time;

            [SerializeField]
            public float value;

            public SerializableKeyframe(float it, float ot, int tm, float t, float v)
            {
                inTan = it;
                outTan = ot;
                tanMode = tm;
                time = t;
                value = v;
            }

            public SerializableKeyframe(Keyframe keyframe)
            {
                inTan = keyframe.inTangent;
                outTan = keyframe.outTangent;
                tanMode = keyframe.tangentMode;
                time = keyframe.time;
                value = keyframe.value;
            }

            public Keyframe GetKeyframe()
            {
                Keyframe kf = new Keyframe(time, value, inTan, outTan);
                kf.tangentMode = tanMode;
                return kf;
            }
        }

        /// <summary>
        /// A class to save the values inside of a GradientColorKey in a JSON
        /// </summary>
        [System.Serializable]
        private class SerializableColorKey
        {
            [SerializeField]
            Color value;

            [SerializeField]
            float time;

            public SerializableColorKey(float t, Color v)
            {
                time = t;
                value = v;
            }

            public SerializableColorKey(GradientColorKey colorkey)
            {
                value = colorkey.color;
                time = colorkey.time;
            }

            public GradientColorKey GetColorKey()
            {
                GradientColorKey ck = new GradientColorKey(value, time);
                return ck;
            }
        }

        /// <summary>
        /// A class to save the values inside of a GradientAlphaKey in a JSON
        /// </summary>
        [System.Serializable]
        private class SerializableAlphaKey
        {
            [SerializeField]
            float value;

            [SerializeField]
            float time;

            public SerializableAlphaKey(float t, float v)
            {
                time = t;
                value = v;
            }

            public SerializableAlphaKey(GradientAlphaKey alphakey)
            {
                value = alphakey.alpha;
                time = alphakey.time;
            }

            public GradientAlphaKey GetAlphaKey()
            {
                GradientAlphaKey ak = new GradientAlphaKey(value, time);
                return ak;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Converts an animation curve into a texture
        /// </summary>
        /// <param name="curve">The curve which should be converted</param>
        /// <param name="steps">The step count which determindes how detailed this curve is converted</param>
        /// <returns>The texture containing the animation curve data</returns>
        internal static Texture2D AnimationCurveToTexture(AnimationCurve curve, int steps)
        {
            Texture2D lResult = new Texture2D(steps, 1);

            Vector4 borders = CurveToBorders(curve);

            Color[] lPixels = new Color[steps];
            float length = steps;
            for (int p = 0; p < steps; p++)
            {
                float point = p;
                float x = Mathf.InverseLerp(borders.x, borders.z, point / length);
                float lVal = Mathf.InverseLerp(borders.y, borders.w, curve.Evaluate(x)) * 4f;
                lPixels[p] = new Color(lVal, (lVal - 1f), (lVal - 2f), (lVal - 3f));
            }

            lResult.SetPixels(lPixels);
            lResult.Apply();

            return lResult;
        }

        /// <summary>
        /// Generates a Vector4 to define the borders of an AnimationCurve
        /// </summary>
        /// <param name="curve">The AnimationCurve to convert</param>
        /// <returns>The borders of the AnimationCurve</returns>
        internal static Vector4 CurveToBorders(AnimationCurve curve)
        {
            float xStart = curve.keys[0].time;
            float yStart = curve.keys[0].value;
            float xEnd = curve.keys[curve.keys.Length - 1].time;
            float yEnd = curve.keys[curve.keys.Length - 1].value;

            for (int k = 0; k < curve.keys.Length - 1; k++)
            {
                float startTime = curve.keys[k].time;
                float endTime = curve.keys[k + 1].time;
                int iterations = Mathf.CeilToInt(Mathf.Abs(endTime - startTime) * 3f);
                for (int p = 0; p <= iterations; p++)
                {
                    float value = curve.Evaluate(Mathf.Lerp(startTime, endTime, p / iterations));
                    if (value > yEnd)
                    {
                        yEnd = value;
                    }
                    if (value < yStart)
                    {
                        yStart = value;
                    }
                }
            }

            return new Vector4(xStart, yStart, xEnd, yEnd);
        }

        /// <summary>
        /// Converts a gradient into a texture
        /// </summary>
        /// <param name="gradiant">The gradient which should be converted</param>
        /// <param name="steps">The steo count which determinds how detailed this curve is converted</param>
        /// <returns>The texture containing the gradient data</returns>
        internal static Texture2D GradientToTexture(Gradient gradiant, int steps)
        {
            Texture2D lResult = new Texture2D(steps, 1);

            Color[] lPixels = new Color[steps];
            float length = steps;
            for (int p = 0; p < steps; p++)
            {
                float point = p;
                lPixels[p] = gradiant.Evaluate(point / length);
            }

            lResult.SetPixels(lPixels);
            lResult.Apply();
            
            return lResult;
        }

        #endregion

        #region Save Methods

        private static Material SAVE_MATERIAL = null;

        private static void CheckToSave()
        {
            Object[] objs = Selection.objects;
            Selection.activeObject = SAVE_MATERIAL;

            MaterialData lData = MaterialData.Of(SAVE_MATERIAL);

            if (lData == null || lData.unsavedTextures == null)
            {
                SAVE_MATERIAL = null;
                Selection.selectionChanged -= CheckToSave;
                Selection.objects = objs;
                Selection.selectionChanged.Invoke();
                return;
            }

            foreach (KeyValuePair<string, Texture2D> tex in lData.unsavedTextures)
            {
                if (lData.savedTextures.ContainsKey(tex.Key))
                {
                    if (lData.savedTextures[tex.Key] != null)
                    {
                        Texture2D oldTexture = lData.savedTextures[tex.Key];
                        oldTexture.SetPixels(tex.Value.GetPixels());
                        oldTexture.Apply();
                        EditorUtility.SetDirty(oldTexture);

                        Object.DestroyImmediate(tex.Value, true);
                    } else
                    {
                        lData.savedTextures[tex.Key] = tex.Value;
                    }
                }
                else
                {
                    tex.Value.name = tex.Key;
                    tex.Value.hideFlags = HideFlags.HideInHierarchy;
                    lData.savedTextures.Add(tex.Key, tex.Value);
                    AssetDatabase.AddObjectToAsset(tex.Value, SAVE_MATERIAL);
                }
                SAVE_MATERIAL.SetTexture(tex.Key, lData.savedTextures[tex.Key]);

                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(SAVE_MATERIAL));
            }

            lData.unsavedTextures.Clear();
            EditorUtility.SetDirty(lData);

            SAVE_MATERIAL = null;
            Selection.selectionChanged -= CheckToSave;
            Selection.objects = objs;
            Selection.selectionChanged.Invoke();
        }

        private static void MarkForSave(Material material)
        {
            if (SAVE_MATERIAL == null)
            {
                Selection.selectionChanged += CheckToSave;
                SAVE_MATERIAL = material;
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
                Vector4 lTempNew = tempMat.GetVector(property);
                Vector4 lTempOriginal = tempMat.GetVector(property);
                for (int p = 0; p < part.Length; p++)
                {
                    lTempOriginal[(int)part[p]] = lTempNew[(int)part[p]];
                }
                material.SetVector(property, lTempOriginal);
                Object.DestroyImmediate(tempMat);
            }
            else if (currentTask == Task.Copy)
            {
                CopyPackage tempS = (CopyPackage)taskObject;
                for (int p = 0; p < part.Length; p++)
                {
                    tempS.copyData.Add(new CopyData(FieldType.FloatVector, property, new FloatVector(material.GetVector(property)[(int)part[p]], part[p])));
                }
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
        //----------------------------------< TEXTURE FIELDS >-----------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\

        #region Texture Field

        /// <summary>
        /// A Field to change a standard texture property
        /// </summary>
        /// <param name="content">The title for the field</param>
        /// <param name="property">The texture property in the material</param>
        /// <param name="size">The size for the texture field - default is small (one line)</param>
        /// <param name="material">The material to use for the field - default is the scope material</param>
        /// <returns>The value of the texture property</returns>
        public static Texture TextureField(GUIContent content, string property, TextureFieldType size = TextureFieldType.Small, Material material = null)
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
                material.SetTexture(property, tempMat.GetTexture(property));
                Object.DestroyImmediate(tempMat);
            }
            else if (currentTask == Task.Copy)
            {
                CopyPackage tempS = (CopyPackage)taskObject;
                tempS.copyData.Add(new CopyData(FieldType.Texture, property, guidc: material.GetTexture(property).GetInstanceID()));
            }

            // Draw Editor Field
            Texture2D mainTexture = (Texture2D)EditorGUILayout.ObjectField(content, material.GetTexture(property), typeof(Texture2D), false, GUILayout.Height((float)size));
            material.SetTexture(property, mainTexture);

            return mainTexture;
        }

        #endregion

        #region Normal Map Field

        /// <summary>
        /// A Field to change a normal map property
        /// </summary>
        /// <param name="content">The title for the field</param>
        /// <param name="property">The texture property in the material</param>
        /// <param name="size">The size for the texture field - default is small (one line)</param>
        /// <param name="material">The material to use for the field - default is the scope material</param>
        /// <returns>The value of the normal map property</returns>
        public static Texture NormalTextureField(GUIContent content, string property, TextureFieldType size = TextureFieldType.Small, Material material = null)
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
                material.SetTexture(property, tempMat.GetTexture(property));
                Object.DestroyImmediate(tempMat);
            }
            else if (currentTask == Task.Copy)
            {
                CopyPackage tempS = (CopyPackage)taskObject;
                tempS.copyData.Add(new CopyData(FieldType.Texture, property, guidc: material.GetTexture(property).GetInstanceID()));
            }

            // Draw Editor Field
            Texture2D normalTexture = (Texture2D)EditorGUILayout.ObjectField(content, material.GetTexture(property), typeof(Texture), false, GUILayout.Height((float)size));
            if (normalTexture != null)
            {
                TextureImporter lImporter = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(normalTexture.GetInstanceID()));
                // Draw notification if the texture is no normal map
                if (lImporter.textureType != TextureImporterType.NormalMap)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField("Texture is no normal map!");
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Fix now"))
                    {
                        lImporter.textureType = TextureImporterType.NormalMap;
                        lImporter.convertToNormalmap = true;
                    }
                    if (GUILayout.Button("To Settings"))
                    {
                        Selection.activeObject = lImporter;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }
            }
            material.SetTexture(property, normalTexture);
            return normalTexture;
        }

        #endregion

        #region Tiling Field

        /// <summary>
        /// A Field to change the tiling in the Texture_ST property
        /// </summary>
        /// <param name="content">The title for the field</param>
        /// <param name="property">The texture_ST property in the material</param>
        /// <param name="material">The material to use for the field - default is the scope material</param>
        /// <returns>The tiling of the texture_ST property</returns>
        public static Vector2 TextureTilingField(GUIContent content, string property, Material material = null)
        {
            if (material == null)
            {
                material = SCOPE_MATERIAL;
            }

            InitStyles();

            // Draw Editor Field
            if (content.text != "")
            {
                EditorGUILayout.LabelField(content);
            }

            Vector4 tiling = MatGUI.VectorField(new GUIContent("Tiling", ""), property, PackagePart.x, PackagePart.y);
            material.SetTextureScale(property.Substring(0, property.Length - 3), new Vector2(tiling.x, tiling.y));
            return new Vector2(tiling.x, tiling.y);
        }

        #endregion

        #region Offset Field

        /// <summary>
        /// A Field to change the offset in the Texture_ST property
        /// </summary>
        /// <param name="content">The title for the field</param>
        /// <param name="property">The texture_ST property in the material</param>
        /// <param name="material">The material to use for the field - default is the scope material</param>
        /// <returns>The offset of the texture_ST property</returns>
        public static Vector2 TextureOffsetField(GUIContent content, string property, Material material = null)
        {
            if (material == null)
            {
                material = SCOPE_MATERIAL;
            }

            InitStyles();

            // Draw Editor Field
            if (content.text != "")
            {
                EditorGUILayout.LabelField(content);
            }

            Vector4 tiling = MatGUI.VectorField(new GUIContent("Offset", ""), property, PackagePart.z, PackagePart.w);
            material.SetTextureOffset(property.Substring(0, property.Length - 3), new Vector2(tiling.z, tiling.w));
            return new Vector2(tiling.z, tiling.w);
        }

        #endregion

        #region Texture Data Field

        /// <summary>
        /// A Field to change the Texture_ST property
        /// </summary>
        /// <param name="content">The title for the field</param>
        /// <param name="property">The texture_ST property in the material</param>
        /// <param name="material">The material to use for the field - default is the scope material</param>
        /// <returns>The value of the texture_ST property</returns>
        public static Vector4 TextureDataField(GUIContent content, string property, Material material = null)
        {
            if (material == null)
            {
                material = SCOPE_MATERIAL;
            }

            InitStyles();

            // Draw Editor Field
            if (content.text != "")
            {
                EditorGUILayout.LabelField(content);
            }

            MatGUI.VectorField(new GUIContent("Tiling", ""), property, PackagePart.x, PackagePart.y);
            Vector4 tiling = MatGUI.VectorField(new GUIContent("Offset", ""), property, PackagePart.z, PackagePart.w);
            
            material.SetTextureScale(property.Substring(0, property.Length - 3), new Vector2(tiling.x, tiling.y));
            material.SetTextureOffset(property.Substring(0, property.Length - 3), new Vector2(tiling.z, tiling.w));

            return tiling;
        }

        #endregion

        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //-----------------------------------< SPECIAL FIELDS >----------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\

        #region Animation Curve Field

        /// <summary>
        /// A Field to change a texture property in form of an animation curve
        /// </summary>
        /// <param name="content">The title for the field</param>
        /// <param name="property">The texture property in the material</param>
        /// <param name="quality">The amount of pixels in the width</param>
        /// <param name="material">The material to use for the field - default is the scope material</param>
        /// <returns>The value of the texture property</returns>
        public static Texture AnimationCurveField(GUIContent content, string property, int quality, Material material = null)
        {
            if (material == null)
            {
                material = SCOPE_MATERIAL;
            }

            InitStyles();

            // Get the mat data to process the field
            MaterialData data = MaterialData.Of(material);
            if (data == null)
            {
                EditorGUILayout.CurveField(content, new AnimationCurve());
                return null;
            }

            // Process Tasks
            if (currentTask == Task.Reset)
            {
                AnimationCurve resetCurve = new AnimationCurve();
                data.curves[property] = resetCurve;
                if (data.unsavedTextures.ContainsKey(property))
                {
                    Object.DestroyImmediate(data.unsavedTextures[property]);
                }
                Texture2D resetTexture = AnimationCurveToTexture(resetCurve, quality);
                data.unsavedTextures[property] = resetTexture;
                material.SetTexture(property, resetTexture);
                Vector4 st = CurveToBorders(resetCurve);
                material.SetTextureOffset(property, new Vector2(st.z, st.w));
                material.SetTextureScale(property, new Vector2(st.x, st.y));
            }
            else if (currentTask == Task.Copy)
            {
                CopyPackage tempS = (CopyPackage)taskObject;
                tempS.copyData.Add(new CopyData(FieldType.Curve, property, cc: data.curves[property], ic: quality));
            }

            // Draw Editor Field
            AnimationCurve curve = new AnimationCurve();
            if (data.curves.ContainsKey(property))
            {
                curve = data.curves[property];
            }
            else
            {
                data.curves.Add(property, curve);
            }

            EditorGUI.BeginChangeCheck();
            curve = EditorGUILayout.CurveField(content, curve);
            bool lEdited = EditorGUI.EndChangeCheck();

            data.curves[property] = curve;
            EditorUtility.SetDirty(data);

            if (lEdited)
            {
                Texture2D mainTexture = AnimationCurveToTexture(curve, quality);

                if (data.unsavedTextures.ContainsKey(property))
                {
                    Object.DestroyImmediate(data.unsavedTextures[property]);
                    data.unsavedTextures[property] = mainTexture;
                }
                else
                {
                    data.unsavedTextures.Add(property, mainTexture);
                }

                material.SetTexture(property, mainTexture);
                Vector4 st = CurveToBorders(curve);
                material.SetTextureOffset(property, new Vector2(st.z, st.w));
                material.SetTextureScale(property, new Vector2(st.x, st.y));
            }


            MarkForSave(material);
            return material.GetTexture(property);
        }

        #endregion

        #region Gradient Field

        /// <summary>
        /// A Field to change a texture property in form of a gradient
        /// </summary>
        /// <param name="content">The title for the field</param>
        /// <param name="property">The texture property in the material</param>
        /// <param name="quality">The amount of pixels in the width</param>
        /// <param name="material">The material to use for the field - default is the scope material</param>
        /// <returns>The value of the texture property</returns>
        public static Texture GradientField(GUIContent content, string property, int quality, Material material = null)
        {
            if (material == null)
            {
                material = SCOPE_MATERIAL;
            }

            InitStyles();

            // Get the mat data to process the field
            MaterialData data = MaterialData.Of(material);
            if (data == null)
            {
                EditorGUILayout.CurveField(content, new AnimationCurve());
                return null;
            }

            // Process Tasks
            if (currentTask == Task.Reset)
            {
                Gradient resetGradient = new Gradient();
                data.gradients[property] = resetGradient;
                if (data.unsavedTextures.ContainsKey(property))
                {
                    Object.DestroyImmediate(data.unsavedTextures[property]);
                }
                Texture2D resetTexture = GradientToTexture(resetGradient, quality);
                data.unsavedTextures[property] = resetTexture;
                material.SetTexture(property, resetTexture);
            }
            else if (currentTask == Task.Copy)
            {
                CopyPackage tempS = (CopyPackage)taskObject;
                tempS.copyData.Add(new CopyData(FieldType.Gradient, property, gc: data.gradients[property], ic: quality));
            }

            // Draw Editor Field
            Gradient gradient = new Gradient();
            if (data.gradients.ContainsKey(property))
            {
                gradient = data.gradients[property];
            }
            else
            {
                data.gradients.Add(property, gradient);
            }

            EditorGUI.BeginChangeCheck();
            System.Reflection.MethodInfo method = typeof(EditorGUILayout).GetMethod("GradientField",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic,
                null, new System.Type[] { typeof(GUIContent), typeof(Gradient), typeof(GUILayoutOption[]) }, null);
            if (method != null)
            {
                gradient = (Gradient)method.Invoke(null, new object[] { content, gradient, new GUILayoutOption[] { } });
            }
            bool lEdited = EditorGUI.EndChangeCheck();

            if (lEdited)
            {
                data.gradients[property] = gradient;
                EditorUtility.SetDirty(data);

                Texture2D mainTexture = GradientToTexture(gradient, quality);

                if (data.unsavedTextures.ContainsKey(property))
                {
                    Object.DestroyImmediate(data.unsavedTextures[property]);
                    data.unsavedTextures[property] = mainTexture;
                }
                else
                {
                    data.unsavedTextures.Add(property, mainTexture);
                }

                material.SetTexture(property, mainTexture);
            }

            MarkForSave(material);
            return material.GetTexture(property);
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
        public static void Group(GUIContent title, GroupStyle style, System.Action<GenericMenu> context, System.Action content, Material material = null)
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
        public static bool FoldGroup(GUIContent title, string property, GroupStyle style, System.Action<GenericMenu> context, System.Action content, Material material = null)
        {
            if (material == null)
            {
                material = SCOPE_MATERIAL;
            }

            InitStyles();

            GUILayout.BeginVertical(groupStyles[(int)style]);

            GUILayout.BeginHorizontal();

            bool foldOpen = MaterialData.GetToggleOf(material, property);
            if (GUILayout.Button(title, groupTitleStyles[(int)style]))
            {
                foldOpen = !foldOpen;
                MaterialData.SetToggleOf(material, property, foldOpen);
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
        public static bool ToggleGroup(GUIContent title, string property, ToggleMode toggleMode, GroupStyle style, System.Action<GenericMenu> context, System.Action content, Material material = null)
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
        private static void GroupContext(System.Action<GenericMenu> context, System.Action content, Material material, int level)
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
                    context.Invoke(menu);
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