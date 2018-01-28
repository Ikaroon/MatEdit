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

        #endregion

        #region Style Methods

        private static void InitStyles()
        {
            if (groupStyles != null)
            {
                return;
            }

            groupStyles = new GUIStyle[] { GUI.skin.GetStyle("LargeButton"), GUI.skin.GetStyle("GroupBox"), EditorStyles.helpBox };
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Data

        public enum PackagePart { x = 0, y = 1, z = 2, w = 3, r = 0, g = 1, b = 2, a = 3};
        public enum GroupStyle { Main = 0, MainTwo = 1, Sub = 2 };
        public enum TextureFieldType { Small = 16, Medium = 32, Large = 64 };

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

        public static void ToggleField(GUIContent content, string property)
        {
            ToggleField(content, property, SCOPE_MATERIAL);
        }

        public static void ToggleField(GUIContent content, string property, Material material)
        {
            material.SetInt(property, EditorGUILayout.Toggle(content, material.GetInt(property) == 1 ? true : false) ? 1 : 0);
        }

        #endregion

        #region Color Field

        public static void ColorField(GUIContent content, string property)
        {
            ColorField(content, property, SCOPE_MATERIAL);
        }

        public static void ColorField(GUIContent content, string property, Material material)
        {
            if (!material.HasProperty(property) || material.GetColor(property) == null)
            {
                return;
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
            Vector4 tempVector = material.GetVector(property);
            tempVector[(int)component] = EditorGUILayout.FloatField(content, tempVector[(int)component]);
            material.SetFloat(property, tempVector[(int)component]);
        }

        #endregion

        #region Vector Field

        public static void VectorField(GUIContent content, string property, params PackagePart[] part)
        {
            VectorField(content, property, SCOPE_MATERIAL, part);
        }

        public static void VectorField(GUIContent content, string property, Material material, params PackagePart[] part)
        {
            if (!material.HasProperty(property) || material.GetVector(property) == null)
            {
                return;
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
    }
}