﻿using System;
using Audio;
using Interact;
using Manager;
using ScreenEffect;
using UI;
using UnityEngine;

namespace Load.Scenes
{
  public class SceneStarter : MonoBehaviour
  {
    public virtual void OnStart()
    {
    }

    private void Awake()
    {
      if (FindObjectOfType<GameManager>() is null)
      {
        var manager = Resources.Load<GameObject>("Managers");
        Instantiate(manager).name = "Managers";
      }

      if (this is IScreenClickable screenClickable)
      {
        ScreenClick.Instance.SetActive(true);
        ScreenClick.Instance.onPointClick += screenClickable.OnScreenClick;
      }
      else
        ScreenClick.Instance.SetActive(false);
    }

    private void Start()
    {
      OnStart();
      FindObjectOfType<InteractUI>().GetComponent<CanvasGroup>().alpha = 0;
    }

    protected static void StartScreenEffect(EffectOption effectOption, bool force = false)
    {
      if (force || !SceneLoader.Instance.isLoading)
        ScreenEffectManager.Instance.Play(effectOption);
    }

    protected static void PlayBGM(string bgmName) => AudioManager.Instance.PlayBGM(bgmName);

    protected static void StopBGM() => AudioManager.Instance.StopBGM();

    protected static void ChangeScene(string sceneName) => SceneLoader.Instance.Load(sceneName);

    protected static void ChangeScene(string sceneName, EffectOption beforeEffect, EffectOption afterEffect)
      => SceneLoader.Instance.Load(sceneName, beforeEffect, afterEffect);

    private void OnDestroy()
    {
      if (this is IScreenClickable screenClickable)
        ScreenClick.Instance.onPointClick -= screenClickable.OnScreenClick;
    }
  }
}