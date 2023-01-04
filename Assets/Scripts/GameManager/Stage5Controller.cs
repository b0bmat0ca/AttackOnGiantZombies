#define DEBUG
#undef DEBUG

using UnityEngine;

using static GameStateData;

public class Stage5Controller : GameManager
{
    // Character
    [Header("巨大ゾンビを指定"), SerializeField] private GameObject largeZombie;

    // Item
    [Header("赤い石を指定"), SerializeField] private GameObject redStone;

    // Level
    [Header("赤い石の台座を指定"), SerializeField] private GameObject goalTrigger;
    private GameObject step1Trigger;
    private GameObject step2Trigger;


    // UI

    // System


    protected override void Awake()
    {
        base.Awake();
        currentStage = StageScene.Stage5;
        InitStage();
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

        if (state != currentState)
        {
            // 各状態における初期化処理
            switch (state)
            {
                case StageSceneState.Step1:
                    step1Trigger.SetActive(false);
                    largeZombie.SetActive(true);
                    break;
                case StageSceneState.Step2:
                    if (largeZombie.activeSelf)
                    {
                        largeZombie.GetComponent<Stage5ZombieController>().destination = step2Trigger;
                    }
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
            case StageSceneState.Step2:
                // 経過時間を取得
                gameTime += Time.deltaTime;

                // 赤い石を持っているか否かで、目標地点を変更
                if (redStone.GetComponent<RedStoneGrabbable>().isGrabbable)
                {
                    destinationGuide.GetComponent<DestinationGuide>().destination = goalTrigger.transform;
                }
                else
                {
                    destinationGuide.GetComponent<DestinationGuide>().destination = redStone.transform;
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
        base.InitStage();

        step1Trigger = GameObject.Find("Step1Trigger");
        step2Trigger = GameObject.Find("Step2Trigger");

        largeZombie.SetActive(false);
    }
}
