using UnityEngine;
using System.Linq;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using Unity.Profiling;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//	UnityのSceneウインドウに対してGUIを追加する処理
//	この処理ではhierarchy上で選択されたパーティクルオブジェクトのパーティクル数と、
//	Meshパーティクルの場合にはポリゴン数の表示をする
//	パーティクルオブジェクトの全選択ボタンを押すとhierarchy上のパーティクルオブジェクトを全選択できる
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
[InitializeOnLoad]
public class DebugEditor : Editor
{
    private static Color textColor = Color.white;
    private static float alphaValue = 2.0f;
    private static bool isShowParticleCount = false;
    private static bool isShowParticlePolygonCount = false;
    static DebugEditor()
    {
        SceneView.duringSceneGui += OnGui;
    }

    private static void OnGui(SceneView sceneView)
    {
        // value save
        int totalParticleCount = 0;
        int totalParticlePolygonCount = 0;
        if (isShowParticleCount)
        {
            ParticleSystem[] _particles = FindObjectsOfType<ParticleSystem>();
            if (_particles != null)
            {
                foreach (ParticleSystem particle in _particles)
                {
                    if (particle == null || !particle.gameObject.activeSelf || !particle.gameObject.GetComponent<ParticleSystemRenderer>().isVisible) continue;
                    totalParticleCount += particle.particleCount;
                    if (isShowParticlePolygonCount)
                    {
                        if (particle.gameObject.GetComponent<ParticleSystemRenderer>().renderMode == ParticleSystemRenderMode.Mesh)
                        {
                            totalParticlePolygonCount += particle.gameObject.GetComponent<ParticleSystemRenderer>().mesh.triangles.Length / 3 * particle.particleCount;
                        }
                    }
                }
            }
        }

        Handles.BeginGUI();
        var color = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0,0,0, alphaValue);
        using(new EditorGUILayout.VerticalScope("Box", GUILayout.Width(300)))
        {
            GUI.backgroundColor = color;
            GUIStyle style = new GUIStyle(EditorStyles.label);
            GUIStyleState styleState = new GUIStyleState();
            styleState.textColor = textColor;
            style.normal = styleState;
            style.fontSize = 20;

            GUILayout.BeginHorizontal(GUILayout.Width(200));
            GUILayout.Label("背景アルファ");
            alphaValue = EditorGUILayout.Slider(alphaValue, 0.0f, 3.0f);
            GUILayout.EndHorizontal();
            isShowParticleCount = GUILayout.Toggle(isShowParticleCount, new GUIContent("パーティクル数の表示"));
            if(isShowParticleCount)
            {
                if(GUILayout.Button(new GUIContent("パーティクルオブジェクトをすべて選択"), GUILayout.Width(300)))
                {
                    ParticleSystem[] _particles = FindObjectsOfType<ParticleSystem>().Where(particle => particle != null && particle.gameObject.activeSelf && particle.gameObject.GetComponent<ParticleSystemRenderer>().isVisible).ToArray();
                    List<GameObject> gameObjects = new List<GameObject>();
                    foreach(ParticleSystem particle in _particles) gameObjects.Add(particle.gameObject);
                    Selection.objects = gameObjects.ToArray();
                }
                isShowParticlePolygonCount = GUILayout.Toggle(isShowParticlePolygonCount, new GUIContent("パーティクルのポリゴン数の表示"));
                GUILayout.Label("TotalParticleCount: " + totalParticleCount.ToString(), style);
                if(isShowParticlePolygonCount) GUILayout.Label("TotalParticlePolygonCount: " + totalParticlePolygonCount.ToString(), style);
            }

        }
        Handles.EndGUI();
    }


}
#endif