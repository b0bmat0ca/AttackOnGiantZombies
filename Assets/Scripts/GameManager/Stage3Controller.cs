#define DEBUG
#undef DEBUG

using UnityEngine;

using static GameStateData;

public class Stage3Controller : GameManager
{
    // Character
    private GameObject enemyGroup;
    private GameObject enemyGroup2;

    // Item
    [Header("赤い石を指定"), SerializeField] private GameObject redStone;

    // Level
    private GameObject stageBorder;
    private GameObject goalTrigger;
    private GameObject step1Trigger;
    private GameObject step2Trigger;
    [Header("赤い石のカーソルを指定"), SerializeField] private GameObject redStoneDestinationCursor;

    // UI
    [Header("ReadStoneCanvasを指定"), SerializeField] private GameObject redStoneCanvas;

    // System


    protected override void Awake()
    {
        base.Awake();
        currentStage = StageScene.Stage3;
        InitStage();
    }

    private void OnDisable()
    {
        redStone.SetActive(false);
    }
    
    // Start is called before the first frame update
    void Start()
    {   
        bgm.Play();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
#if DEBUG
        Debug.Log($"Stage3Controller.Update: state: {state}");
#endif
        if (state != currentState)
        {
            // 各状態における初期化処理
            switch (state)
            {
                case StageSceneState.Step1:
                    step1Trigger.SetActive(false);
                    step2Trigger.SetActive(false);
                    DeadZombieWakeUp();
                    break;
                case StageSceneState.Step2:
                    step1Trigger.SetActive(false);
                    step2Trigger.SetActive(false);
                    ZombieController zctl;
                    for (int i = 0; i < enemyGroup2.transform.childCount; i++)
                    {
                        zctl = enemyGroup2.transform.GetChild(i).gameObject.GetComponent<ZombieController>();
                        zctl.destination = player;
                    }
                    DeadZombieWakeUp();
                    break;
                case StageSceneState.Step3:
                    break;
                case StageSceneState.Step4:
                    ZombieStopAll();
                    destinationGuide.SetActive(false);
                    PlayerDisabled();
                    Vector3 canvasPos = player.transform.position + mainCamera.transform.forward * 2;
                    redStoneCanvas.transform.position = canvasPos;
                    redStoneCanvas.SetActive(true);
                    PlayerController pctl = player.GetComponent<PlayerController>();
                    pctl.gunTutorial.enabled = false;
                    pctl.redStoneTutorial.enabled = true;
                    pctl.manual.SetActive(true);
                    break;
                case StageSceneState.End:
                    redStoneCanvas.SetActive(false);
                    ZombieReStartAll();
                    PlayerEnabled();
                    destinationGuide.SetActive(true);
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
                // 経過時間を取得
                gameTime += Time.deltaTime;
                break;
            case StageSceneState.Step1:
            case StageSceneState.Step2:
                // 経過時間を取得
                gameTime += Time.deltaTime;

                // 赤い石出現
                redStone.SetActive(true);
                redStoneDestinationCursor.SetActive(true);
                destinationGuide.GetComponent<DestinationGuide>().destination = redStone.transform;

                state = StageSceneState.Step3;
                break;
            case StageSceneState.Step3:
                // 経過時間を取得
                gameTime += Time.deltaTime;

                if (Vector3.Distance(redStone.transform.position, player.transform.position) < 4.0f)
                {
                    redStoneDestinationCursor.SetActive(false);
                    state = StageSceneState.Step4;
                }
                break;
            case StageSceneState.End:
                // 経過時間を取得
                gameTime += Time.deltaTime;

                if (isTransitionScene)
                {
                    stageBorder.SetActive(false);
                }
                else
                {
                    // 赤い石を持っているか否かで、目標地点を変更
                    if (redStone.GetComponent<RedStoneGrabbable>().isGrabbable)
                    {
                        goalTrigger.SetActive(true);
                        destinationCursor.SetActive(true);
                        destinationGuide.GetComponent<DestinationGuide>().destination = destinationCursor.transform;
                    }
                    else
                    {
                        goalTrigger.SetActive(false);
                        destinationCursor.SetActive(false);
                        destinationGuide.GetComponent<DestinationGuide>().destination = redStone.transform;
                    }
                }

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
#if DEBUG
        Debug.Log("Stage3Controller.InitStage");
#endif
        base.InitStage();

        stageBorder = GameObject.Find("StageBorder");
        goalTrigger = GameObject.Find("GoalTrigger");
        goalTrigger.SetActive(false);
        step1Trigger = GameObject.Find("Step1Trigger");
        step2Trigger = GameObject.Find("Step2Trigger");

        redStoneDestinationCursor.SetActive(false);

        enemyGroup = GameObject.Find("EnemyGroup");
        enemyGroup2 = GameObject.Find("EnemyGroup2");

        // 初期状態は、地面に倒れている
        Stage3DownZombieController zctl;
        for (int i = 0; i < enemyGroup.transform.childCount; i++)
        {
            zctl = enemyGroup.transform.GetChild(i).gameObject.GetComponent<Stage3DownZombieController>();
            if (Random.Range(1, 10) > 5)
            {
                zctl.SetDead2();
            }
            else
            {
                zctl.SetDead1();
            }
        }

        redStoneCanvas.SetActive(false);
        redStone.SetActive(false);
    }

    /// <summary>
    /// 倒れているゾンビを起こす
    /// </summary>
    private void DeadZombieWakeUp()
    {
        Stage3DownZombieController zctl;
        for (int i = 0; i < enemyGroup.transform.childCount; i++)
        {
            zctl = enemyGroup.transform.GetChild(i).gameObject.GetComponent<Stage3DownZombieController>();
            zctl.SetIdle();
            zctl.destination = player;
        }
    }

    /// <summary>
    /// フィールド上の全ゾンビの動きを止める
    /// </summary>
    private void ZombieStopAll()
    {
        ZombieController zctl;
        for (int i = 0; i < enemyGroup.transform.childCount; i++)
        {
            zctl = enemyGroup.transform.GetChild(i).gameObject.GetComponent<ZombieController>();
            zctl.EventStop();
        }
        for (int i = 0; i < enemyGroup2.transform.childCount; i++)
        {
            zctl = enemyGroup2.transform.GetChild(i).gameObject.GetComponent<ZombieController>();
            zctl.EventStop();
        }
    }

    /// <summary>
    /// フィールド上の全ゾンビの動きを再開する
    /// </summary>
    private void ZombieReStartAll()
    {
        ZombieController zctl;
        for (int i = 0; i < enemyGroup.transform.childCount; i++)
        {
            zctl = enemyGroup.transform.GetChild(i).gameObject.GetComponent<ZombieController>();
            zctl.EventRestart();
        }
        for (int i = 0; i < enemyGroup2.transform.childCount; i++)
        {
            zctl = enemyGroup2.transform.GetChild(i).gameObject.GetComponent<ZombieController>();
            zctl.EventRestart();
        }
    }
}
