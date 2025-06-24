using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{
    public static TimelineManager Instance { get; private set; }

    [SerializeField] PlayableDirector bossIntroTimeline;
    [SerializeField] PlayableDirector bossClearTimeline;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayBossIntro() => bossIntroTimeline.Play();
    public void PlayBossClear() => bossClearTimeline.Play();
}
