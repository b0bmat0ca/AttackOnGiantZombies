#define DEBUG
#undef DEBUG

using System.Collections;
using UnityEngine;

using static GameStateData;

public class Stage1Controller : GameManager
{
    // Character
    [Header("チュートリアル用ゾンビを指定"), SerializeField] private GameObject tutorialZombie;
    private GameObject enemyGroup;
    [Header("大型ゾンビを指定"), SerializeField] private GameObject largeZombie;

    // Item
    [SerializeField] private GameObject gun;

    // Level
    private GameObject goalTrigger;
    private GameObject step3Trigger;

    // UI
    [Header("StartCanvasを指定"), SerializeField] private GameObject startCanvas;

    // System
    [Header("サウンドエフェクト用AudioSourceを指定"), SerializeField] AudioSource se;
    [Header("雷の音を指定"), SerializeField] private AudioClip thunder;
    [Header("ゾンビのうめき声を指定"), SerializeField] private AudioClip groan;
    [Header("カラスの鳴き声を指定"), SerializeField] private AudioClip crow;


    protected override void Awake()
    {
        base.Awake();
        currentStage = StageScene.Stage1;
        InitStage();
    }

    // Start is called before the first frame update
    void Start()
    {
        bgm.Play();
        StartCoroutine(nameof(SoundEffects));
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
                    PlayerEnabled();
                    destinationGuide.SetActive(true);
                    tutorialZombie.GetComponent<ZombieController>().destination = player;
                    break;
                case StageSceneState.Step2:
                    for (int i = 0; i < enemyGroup.transform.childCount; i++)
                    {
                        enemyGroup.transform.GetChild(i).gameObject.GetComponent<ZombieController>().destination = player;
                    }
                    destinationCursor.SetActive(true);
                    destinationGuide.GetComponent<DestinationGuide>().destination = destinationCursor.transform;
                    goalTrigger.SetActive(true);
                    break;
                case StageSceneState.Step3:
                    step3Trigger.SetActive(false);
                    for (int i = 0; i < enemyGroup.transform.childCount; i++)
                    {
                        enemyGroup.transform.GetChild(i).gameObject.GetComponent<ZombieController>().destination = player;
                    }
                    largeZombie.GetComponent<ZombieController>().destination = player;
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
            case StageSceneState.Step2:
            case StageSceneState.Step3:
                // 経過時間を取得
                gameTime += Time.deltaTime;

                if (!tutorialZombie.activeSelf)
                {
                    gun.GetComponent<GunShootStage1>().enabled = false;
                    gun.GetComponent<GunShoot>().enabled = true;
                    state = StageSceneState.Step2;
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

        destinationGuide.SetActive(false);
        PlayerDisabled();

        goalTrigger = GameObject.Find("GoalTrigger");
        goalTrigger.SetActive(false);
        step3Trigger = GameObject.Find("Step3Trigger");

        enemyGroup = GameObject.Find("EnemyGroup");
    }    

    private IEnumerator SoundEffects()
    {
        // 効果音の再生
        se.PlayOneShot(thunder);

        yield return new WaitForSeconds(4.0f);

        se.volume = 0.5f;
        se.PlayOneShot(groan);

        yield return new WaitForSeconds(2.0f);

        se.volume = 0.5f;
        se.PlayOneShot(crow);
    }
    
}
