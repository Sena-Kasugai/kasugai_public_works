using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System.Collections.Generic;

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//	アニメーションクリップファイルのパスの文字列を置換する処理
//	oldReplacePath: 置換対象の文字列
//	newReplacePath: 置換後の文字列
////////////////////////////////////////////////////////////////////////////////////////////////////////////

public class AnimationPathReplace : EditorWindow
{
    [SerializeField]
    private AnimationClip[] animClips;

    private string oldReplacePath;
    private string newReplacePath;

    [MenuItem("Tools/AnimationPathReplace")]
    private static void Init()
    {
        var window = GetWindow<AnimationPathReplace>();
        window.Show();
    }

    private void OnGUI()
    {
        var so = new SerializedObject(this);
        so.Update();

        EditorGUILayout.PropertyField(so.FindProperty("animClips"), true);

        so.ApplyModifiedProperties();

        oldReplacePath = EditorGUILayout.TextField("置換したい名前", oldReplacePath);
        newReplacePath = EditorGUILayout.TextField("置換後の名前", newReplacePath);
        if(GUILayout.Button("リネーム"))
        {
            if(animClips.Length <= 0) return;
            if(oldReplacePath == "") return;
            if(newReplacePath == "") return;
            ReplacePath(animClips, oldReplacePath, newReplacePath);
        }
    }

    private static void ReplacePath(AnimationClip[] clips, string oldPath, string newPath)
    {
        Undo.RecordObjects(clips, "replace animationclip paths");
        foreach(var clip in clips)
        {
            var bindings = AnimationUtility.GetCurveBindings(clip);
            var removeBindings = bindings.Where(c => c.path.Contains(oldPath));

            foreach(var binding in removeBindings)
            {
                var curve = AnimationUtility.GetEditorCurve(clip, binding);
                var newBinding = binding;
                newBinding.path = newBinding.path.Replace(oldPath, newPath);
                AnimationUtility.SetEditorCurve(clip, binding, null);
                AnimationUtility.SetEditorCurve(clip, newBinding, curve);
            }
        }
    }
}