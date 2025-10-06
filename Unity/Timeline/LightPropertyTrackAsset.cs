using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[TrackBindingType(typeof(LightProperty))] // コントロールする対象の型
[TrackColor(1, 0, 0)] // トラックの色
[TrackClipType(typeof(LightPropertyClip))] // 設定できるクリップの型（複数指定可能）
[TrackClipType(typeof(LightPropertyIntensityClip))]
[TrackClipType(typeof(LightPropertyGradientClip))]
public class LightPropertyTrackAsset : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        // Mixerを作って返す
        var mixer = ScriptPlayable<LightPropertyMixerBehaviour>.Create(graph, inputCount);
        mixer.GetBehaviour().Clips = GetClips().ToArray();
        mixer.GetBehaviour().Director = go.GetComponent<PlayableDirector>();
        return mixer;
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        // Timelineから外したときに値を戻したい場合はこのように書く
#if UNITY_EDITOR
        LightProperty trackBinding = director.GetGenericBinding(this) as LightProperty;
        if (trackBinding == null)
            return;
        driver.AddFromName<LightProperty>(trackBinding.gameObject, "color");
#endif
    }
}