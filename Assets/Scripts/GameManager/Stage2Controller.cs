#define DEBUG
#undef DEBUG

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

using static GameStateData;

public class Stage2Controller : GameManager
{
    // Character
    private GameObject enemyGroup;
    private GameObject enemyGroup2;
    [SerializeField] private GameObject zombieLook;

    // Item

    // Level
    private GameObject step1Trigger;

    // UI

    // System
    [Header("効果音用Timelineを指定"), SerializeField] private PlayableDirector director;


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
        currentStage = StageScene.Stage2;
        InitStage();
    }

    // Start is called before the first frame update
    void Start()
    {
        director.Play();
        StartCoroutine(nameof(SetEvent));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (state != currentState)
        {
            // 各状態における初期化処理
            switch (state)
            {
                case StageSceneState.Step1:
                    step1Trigger.SetActive(false);
                    ZombieGroupStart(enemyGroup);
                    ZombieGroupStart(enemyGroup2);
                    zombieLook.GetComponent<Stage2LookZombieController>().destination = player;
                    zombieLook.GetComponent<Stage2LookZombieController>().SetIdle();
                    break;

                default:
                    break;
            }
            currentState = state;
        }

        switch (state)
        {
            // 状態進行に伴う処理
            case StageSceneState.Start:
            case StageSceneState.Step1:
                // 経過時間を取得
                gameTime += Time.deltaTime;
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Stageの初期化
    /// </summary>
    protected override void InitStage()
    {
        base.InitStage();

        step1Trigger = GameObject.Find("Step1Trigger");

        enemyGroup = GameObject.Find("EnemyGroup");
        enemyGroup2 = GameObject.Find("EnemyGroup2");

        destinationCursor.SetActive(true);
    }

    /// <summary>
    /// ゾンビにプレイヤーを狙わせる
    /// </summary>
    private void ZombieGroupStart(GameObject eGroup)
    {
        ZombieController zctl;
        for (int i = 0; i < eGroup.transform.childCount; i++)
        {
            zctl = eGroup.transform.GetChild(i).gameObject.GetComponent<ZombieController>();
            zctl.destination = player;
        }
    }

    private IEnumerator SetEvent()
    {
#if DEBUG
        Debug.Log("Stage2Controller.SetEvent");
#endif

        yield return new WaitForSeconds(3f);
        if (state != StageSceneState.Step1)
        {
            ZombieGroupStart(enemyGroup);
        }

        yield return new WaitForSeconds(10f);
        if (state != StageSceneState.Step1)
        {
            zombieLook.GetComponent<Stage2LookZombieController>().destination = player;
            zombieLook.GetComponent<Stage2LookZombieController>().SetIdle();
        }
    }

    /// <summary>
    /// ディレクタ停止時のイベント処理
    /// </summary>
    /// <param name="aDirector"></param>
    void OnPlayableDirectorStopped(PlayableDirector pDirector)
    {
        pDirector.Play();
    }

    /// <summary>
    /// GameOver処理
    /// </summary>
    protected override IEnumerator GameOverSence()
    {
        OnDisable();
        director.Stop();
        bgm.PlayOneShot(gameOver);
        while (vignette.intensity.value < 1.0f)
        {
            vignette.intensity.value += 0.1f;
            yield return new WaitForSeconds(0.3f);
        }
        StartCoroutine(TransitionScene(SceneManager.GetActiveScene().buildIndex));
    }
}
