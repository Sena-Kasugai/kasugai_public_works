using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class LightPropertyGradientClip : PlayableAsset, ITimelineClipAsset
{
    // 必ずpublic（レコードボタンが表示されない）でBehaviourを持たせる
    public LightPropertyGradientPlayableBehaviour behaviour = new LightPropertyGradientPlayableBehaviour();

    // このクリップの特徴を定義
    public ClipCaps clipCaps {
        get {
            // ブレンドに対応、タイムスケール変更に対応
            return ClipCaps.Blending | ClipCaps.SpeedMultiplier;
        }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return ScriptPlayable<LightPropertyGradientPlayableBehaviour>.Create(graph);
    }

}