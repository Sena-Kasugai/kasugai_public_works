using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class LightPropertyMixerBehaviour : PlayableBehaviour
{
    public TimelineClip[] Clips { get; set; }
    public PlayableDirector Director { get; set; }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var lightProperty = playerData as LightProperty;
        if (lightProperty == null) {
            return;
        }

        var time = Director.time; // Timeline全体の現在の時間
        var color = Color.clear;
        var intensity = 0.0f;
        bool isColor = false;
        bool isIntensity = false;

        for (int i = 0; i < Clips.Length; i++) {
            var clip = Clips[i];
            if(clip == null) continue;
            var clipProgress = (float)((time - clip.start) / clip.duration); // クリップの進行率
            if (clipProgress >= 0.0f && clipProgress <= 1.0f)
            {
                if(clip.asset is LightPropertyClip)
                {
                    isColor = true;
                    isIntensity = true;
                    LightPropertyClipProcess(playable, clip, time, i, ref color, ref intensity);
                }
                else if(clip.asset is LightPropertyGradientClip)
                {
                    isColor = true;
                    LightPropertyGradientClipProcess(playable, clip, time, i, ref color);
                }
                else if(clip.asset is LightPropertyIntensityClip)
                {
                    isIntensity = true;
                    LightPropertyIntensityClipProcess(playable, clip, time, i, ref intensity);
                }
            }
        }

        if(isColor) lightProperty.Color = color;
        if(isIntensity) lightProperty.Intensity = intensity;
    }

    private void LightPropertyClipProcess(Playable playable, TimelineClip clip, double time, int i, ref Color color, ref float intensity)
    {
            var clipAsset = clip.asset as LightPropertyClip; // クリップのアセット
            var behaviour = clipAsset.behaviour; // クリップが持つBehaviour
            var clipWeight = playable.GetInputWeight(i); // クリップのブレンドウェイト
            var clipProgress = (float)((time - clip.start) / clip.duration); // クリップの進行率

            color += behaviour.gradient.Evaluate(clipProgress) * clipWeight;
            intensity += behaviour.intensity * clipWeight;
    }

    private void LightPropertyGradientClipProcess(Playable playable, TimelineClip clip, double time, int i, ref Color color)
    {
            var clipAsset = clip.asset as LightPropertyGradientClip; // クリップのアセット
            var behaviour = clipAsset.behaviour; // クリップが持つBehaviour
            var clipWeight = playable.GetInputWeight(i); // クリップのブレンドウェイト
            var clipProgress = (float)((time - clip.start) / clip.duration); // クリップの進行率
            
            color += behaviour.gradient.Evaluate(clipProgress) * clipWeight;
    }

    private void LightPropertyIntensityClipProcess(Playable playable, TimelineClip clip, double time, int i, ref float intensity)
    {
            var clipAsset = clip.asset as LightPropertyIntensityClip; // クリップのアセット
            var behaviour = clipAsset.behaviour; // クリップが持つBehaviour
            var clipWeight = playable.GetInputWeight(i); // クリップのブレンドウェイト

            intensity += behaviour.intensity * clipWeight;
    }
}