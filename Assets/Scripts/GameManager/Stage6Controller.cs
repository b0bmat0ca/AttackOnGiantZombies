#define DEBUG
#undef DEBUG

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

using static GameStateData;

public class Stage6Controller : GameManager
{
    // Character

    // Item

    // Level

    // UI
    [Header("EndCanvasを指定")] public GameObject endCanvas;
    [Header("クレジットタイトルを指定"), SerializeField] private GameObject creditTitle;
    [Header("プレイ時間を指定"), SerializeField] private GameObject playTime;

    // System
    [Header("エンディング用Timelineを指定"), SerializeField] private PlayableDirector director;


    private void OnEnable()
    {
        // ディレクタ停止時のイベントハンドラを登録
        director.stopped += OnPlayableDirectorStopped;
    }

    void OnDisable()
    {
        // ディレクタ停止時のイベントハンドラを削除
        director.stopped -= OnPlayableDirectorStopped;
    }

    protected override void Awake()
    {
        base.Awake();
        currentStage = StageScene.Stage6;
        InitStage();
    }

    /// <summary>
    /// Stageの初期化
    /// </summary>
    protected override void InitStage()
    {
        base.InitStage();

        destinationGuide.SetActive(false);
        endCanvas.SetActive(false);
    }

    /// <summary>
    /// ディレクタ停止時のイベント処理
    /// </summary>
    /// <param name="pDirector"></param>
    private void OnPlayableDirectorStopped(PlayableDirector pDirector)
    {
        StartCoroutine(nameof(EndGame));
    }

    private IEnumerator EndGame()
    {
        TextMeshProUGUI playTimeText = playTime.GetComponent<TextMeshProUGUI>();
        playTimeText.text = playTimeText.text.Replace(" TIME ", $" {GetGameTime()} ");

        creditTitle.SetActive(true);
        endCanvas.SetActive(true);

        yield return new WaitForSeconds(5f);

        creditTitle.SetActive(false);
        playTime.SetActive(true);

        yield return new WaitForSeconds(10f);

        StartCoroutine(TransitionScene((int)StageScene.Start));
    }

    private string GetGameTime()
    {
        TimeSpan span = new TimeSpan(0, 0, (int)gameTime);
#if DEBUG
        Debug.Log($"Stage6Controller.GetGameTime: {span.ToString(@"mm\分ss\秒")}");
#endif
        return span.ToString(@"mm\分ss\秒");
    }
}
