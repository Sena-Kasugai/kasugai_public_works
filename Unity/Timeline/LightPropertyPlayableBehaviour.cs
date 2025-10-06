using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using StageLightManeuver;

// #if UNITY_EDITOR
// using UnityEditor;
// #endif

[Serializable]
public class LightPropertyBasePlayableBehaviour : PlayableBehaviour
{
    private PlayableDirector playableDirector;
    public LightProperty lightProperty { get; set; }

    public override void OnPlayableCreate(Playable playable)
    {
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;
    }

    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
    }
}

[Serializable]
public class LightPropertyPlayableBehaviour : LightPropertyBasePlayableBehaviour
{
    public Gradient gradient;
    public float intensity;

    private Color? origColor = null;

    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if(origColor == null && lightProperty != null) origColor = lightProperty.Color;
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if(lightProperty == null) return;
        if(origColor != null){
            lightProperty.Color = (Color)origColor;
            origColor = null;
        }
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        if(lightProperty == null) return;
        var progress = (float)(playable.GetTime() / playable.GetDuration());
        lightProperty.Color = gradient.Evaluate(progress);
        lightProperty.Intensity = intensity;
    }
}


[Serializable]
public class LightPropertyIntensityPlayableBehaviour : LightPropertyBasePlayableBehaviour
{
    public float intensity;
    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        if(lightProperty == null) return;
        lightProperty.Intensity = intensity;
    }

}


[Serializable]
public class LightPropertyGradientPlayableBehaviour : LightPropertyBasePlayableBehaviour
{
    public Gradient gradient;
    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        if(lightProperty == null) return;
        var progress = (float)(playable.GetTime() / playable.GetDuration());
        lightProperty.Color = gradient.Evaluate(progress);
    }

}


// #if UNITY_EDITOR

// [CustomPropertyDrawer(typeof(LightPropertyPlayableBehaviour))]
// public class LightPropertyPlayableBehaviourDrawer : PropertyDrawer
// {
//     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//     {
//         return EditorGUIUtility.singleLineHeight * 2;
//     }

//     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//     {
//         var isColorProp = property.FindPropertyRelative("isColor");
//         var gradientProp = property.FindPropertyRelative("gradient");
//         var isIntensityProp = property.FindPropertyRelative("isIntensity");
//         var intensityProp = property.FindPropertyRelative("intensity");

//         var fieldRect = position;
//         fieldRect.height = EditorGUIUtility.singleLineHeight;

//         EditorGUI.PropertyField(fieldRect, isColorProp);
//         fieldRect.y += EditorGUIUtility.singleLineHeight;
//         if(isColorProp.boolValue)
//         {
//             EditorGUI.PropertyField(fieldRect, gradientProp);
//             fieldRect.y += EditorGUIUtility.singleLineHeight;
//         }
//         EditorGUI.PropertyField(fieldRect, isIntensityProp);
//         fieldRect.y += EditorGUIUtility.singleLineHeight;

//         if(isIntensityProp.boolValue)
//         {
//             EditorGUI.PropertyField(fieldRect, intensityProp,);
//         }
//     }
// }

// #endif