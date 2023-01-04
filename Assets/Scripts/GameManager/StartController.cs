#define DEBUG
#undef DEBUG

using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

using static GameStateData;

public class StartController : GameManager
{
    [Header("オープニング用Timelineを指定"), SerializeField] private PlayableDirector director;
    [Header("オープニング開始用テキストを指定"), SerializeField] private GameObject pressA;
    [Header("オープニング表示用キャンバスを指定"), SerializeField] private GameObject opCanvvas;


    protected override void Awake()
    {
        base.Awake();
        currentStage = StageScene.Start;
        InitStage();
    }

    private void OnEnable()
    {
        gameTime = 0f;
        // ディレクタ停止時のイベントハンドラを登録
        director.stopped += OnPlayableDirectorStopped;
    }

    private void OnDisable()
    {
        // ディレクタ停止時のイベントハンドラを削除
        director.stopped -= OnPlayableDirectorStopped;
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerDisabled();
        StartCoroutine(nameof(StartDirector));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            director.Stop();
        }
    }

    /// <summary>
    /// Stageの初期化
    /// </summary>
    protected override void InitStage()
    {
        base.InitStage();

        destinationGuide.SetActive(false);
        director.enabled = false;
    }

    /// <summary>
    /// ディレクタ停止時のイベント処理
    /// </summary>
    /// <param name="aDirector"></param>
    void OnPlayableDirectorStopped(PlayableDirector pDirector)
    {
        PlayerEnabled();
        destinationGuide.SetActive(true);
        pDirector.enabled = false;

        // 画面上の文字を消す
        opCanvvas.SetActive(false);
    }

    IEnumerator StartDirector()
    {
        // ボタンAを押すことで、オープニング開始
        yield return new WaitUntil(() => OVRInput.GetDown(OVRInput.RawButton.A));
        pressA.SetActive(false);
        director.enabled = true;
        director.Play();
    }
}
