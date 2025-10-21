using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Animation))]
public class AnimationSupport : MonoBehaviour
{
    Animation _animationComponent;
    public Animation animationComponent
    {
        get
        {
            if (_animationComponent == null) _animationComponent = GetComponent<Animation>();

            return _animationComponent;
        }
    }

    [Dropdown("animNames")]
    public string animName;

    public string[] animNames
    {
        get
        {
            string s = string.Empty;
            foreach (AnimationState state in animationComponent)
            {
                if (state.clip != null)
                {
                    if (string.IsNullOrEmpty(s))
                    {
                        s = state.clip.name;
                    }
                    else
                    {
                        s = string.Format("{0},{1}", s, state.clip.name);
                    }
                }
            }

            return s.Split(",");
        }
        private set { }
    }

    [Button]
    void SetupFirstFrameAnimation()
    {
        SetupFirstFrameAnimation(animName);
    }

    public void Play(string animName)
    {
        animationComponent[animName].speed = 1f;
        animationComponent.Play(animName);
    }

    public async UniTask PlayAsync(string animName)
    {
        var animationClip = animationComponent.GetClip(animName);
        var lenghClip = animationClip.length;

        animationComponent[animName].speed = 1f;
        animationComponent.Play(animName);
        await UniTask.Delay(Mathf.RoundToInt(lenghClip * 1000));
    }
    
    public async UniTask PlayAsync(string animName, float timePlay)
    {
        var animationClip = animationComponent.GetClip(animName);
        var lenghClip = animationClip.length;

        animationComponent[animName].speed = lenghClip / timePlay;
        animationComponent.Play(animName);
        await UniTask.Delay(Mathf.RoundToInt(lenghClip * 1000));
    }

    public void Play(string animName, float timePlay)
    {
        //Debug.LogFormat("Play animation {0} in {1}", animName, timePlay);

        var animationClip = animationComponent.GetClip(animName);
        var lenghClip = animationClip.length;

        animationComponent[animName].speed = lenghClip / timePlay;
        animationComponent.Play(animName);
    }

    public void Stop()
    {
        animationComponent.Stop();
    }

    public void SetupFirstFrameAnimation(string animName)
    {
        var animationClip = animationComponent.GetClip(animName);
        //Debug.LogFormat("SetupFirstFrameAnimation = {0} ({1})", animName, animationClip);
        if (animationClip == null) return;
        if (animationComponent.isPlaying) animationComponent.Stop();

        animationClip.SampleAnimation(gameObject, 0);
    }

    public void SetupLastFrameAnimation(string animName)
    {
        var animationClip = animationComponent.GetClip(animName);
        //Debug.LogFormat("SetupLastFrameAnimation = {0} ({1})", animName, animationClip);
        if (animationClip == null) return;
        if (animationComponent.isPlaying) animationComponent.Stop();

        animationClip.SampleAnimation(gameObject, 1);
    }
}
