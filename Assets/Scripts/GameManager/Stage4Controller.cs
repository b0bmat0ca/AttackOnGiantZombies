#define DEBUG
#undef DEBUG

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

using static GameStateData;

public class Stage4Controller : GameManager
{
    // Character
    private GameObject enemyGroup;
    private GameObject enemyGroup2;

    // Item

    // Level
    private GameObject stageBorder;

    // UI
    [Header("StartCanvasを指定"), SerializeField] private GameObject startCanvas;

    // System


    protected override void Awake()
    {
        base.Awake();
        currentStage = StageScene.Stage4;
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
                    startCanvas.SetActive(false);
                    ZombieGroupStart(enemyGroup);
                    ZombieGroupStart(enemyGroup2);
                    PlayerEnabled();
                    destinationCursor.SetActive(true);
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
            case StageSceneState.Step1:
                // 経過時間を取得
                gameTime += Time.deltaTime;

                if (isTransitionScene && stageBorder.activeSelf)
                {
                    stageBorder.SetActive(false);
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

        stageBorder = GameObject.Find("StageBorder");

        destinationGuide.SetActive(false);
        PlayerDisabled();

        enemyGroup = GameObject.Find("EnemyGroup");
        enemyGroup2 = GameObject.Find("EnemyGroup2");
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
}
