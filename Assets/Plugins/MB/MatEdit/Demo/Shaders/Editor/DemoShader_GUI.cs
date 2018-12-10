// System
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;
using UnityEditor;

// MB
using MB.MatEdit;

public class DemoShader_GUI : ShaderGUI {

	// Displays the Material GUI
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        MatGUI.SetScope((Material)materialEditor.target);
             
        // Draw Material GUI
        MatGUI.Group(new GUIContent("Main"), GroupStyle.Main, delegate ()
        {
            MatGUI.TextureField(new GUIContent("Texture"), "_MainTex", TextureFieldType.Small);
            MatGUI.TextureDataField(GUIContent.none, "_MainTex_ST");
            MatGUI.GradientField(new GUIContent("Gradient"), "_ColorGradient", 128);
        });

        MatGUI.ToggleGroup(new GUIContent("Height Modification"), "HEIGHT_MOD", ToggleMode.Feature, GroupStyle.Main, delegate ()
        {
            MatGUI.AnimationCurveField(new GUIContent("X_UV Height"), "_CurveHeightX", 128);
            MatGUI.AnimationCurveField(new GUIContent("Y_UV Height"), "_CurveHeightY", 128);
            MatGUI.Group(new GUIContent("Sub Group"), GroupStyle.Sub, delegate ()
            {
                MatGUI.MinMaxSliderField(new GUIContent("Height Contraints"), "_MinHeight", "_MaxHeight", 0f, 5f, true);
            });
        });

        MatGUI.FoldGroup(new GUIContent("Extras"), "_Extras", GroupStyle.Main, delegate ()
        {
            MatGUI.SliderField(new GUIContent("Brightness"), "_Brightness", 0f, 2f);
        });
    }
}