#define DEBUG
#undef DEBUG

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using TMPro;

using static GameStateData;

public class GameManager : MonoBehaviour
{
    protected static float gameTime = 0;
    public static StageScene currentStage;

    // 状態管理
    [NonSerialized] public StageSceneState state;
    [NonSerialized] public StageSceneState currentState;
    [NonSerialized] public bool isTransitionScene;
    [NonSerialized] protected bool isGameOver;

    // Character
    [Header("プレイヤーを指定")] public GameObject player;

    // UI
    [Header("目的地カーソルを指定"), SerializeField] protected GameObject destinationCursor;
    [Header("目的地ガイドを指定")] public GameObject destinationGuide;
    [Header("GuideUIキャンバスを指定")] public Canvas guideUICanvas;
    [Header("敵のダメージ数値UIを指定")] public TextMeshProUGUI deadCounter;
    [Header("ClosedUIを指定")] public GameObject closed;
    [Header("NoEntryUIを指定")] public GameObject noEntry;

    // System
    [Header("メインカメラを指定")] public GameObject mainCamera;
    [Header("Post Process Volumeを指定"), SerializeField] protected Volume postProcessVolume;
    [Header("BGM用AudioSouceを指定"), SerializeField] protected AudioSource bgm;
    [Header("GameOver用AudioClipを指定"), SerializeField] protected AudioClip gameOver;

    protected OVRScreenFade screenFade; // フェードイン・フェードアウト効果
    public Vignette vignette;   // ビネット効果


    protected virtual void Awake()
    {
        state = currentState = StageSceneState.Start;
        isTransitionScene = false;
        isGameOver = false;

        screenFade = mainCamera.GetComponent<OVRScreenFade>();

        postProcessVolume.profile.TryGet(out vignette);
        if (vignette == null)
        {
            Debug.Log("GameManager.Awake: vignette = null");
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // 致命的なバグで、ステージから落ちる可能性があるための簡易対応
        if (player.transform.position.y < -100)
        {
            GameOver();
        }
    }

    /// <summary>
    /// Stageの初期化
    /// </summary>
    protected virtual void InitStage()
    {
        if (destinationCursor != null)
        {
            destinationCursor.SetActive(false);
        }
        if (deadCounter != null)
        {
            deadCounter.enabled = false;
        }
        if (noEntry != null)
        {
            noEntry.SetActive(false);
        }
        if (closed != null)
        {
            closed.SetActive(false);
        }
    }

    /// <summary>
    /// プレイヤーの無効化
    /// </summary>
    protected void PlayerDisabled()
    {
        player.GetComponent<OVRPlayerController>().EnableLinearMovement = false;
        player.GetComponent<OVRPlayerController>().EnableRotation = false;
        
    }

    /// <summary>
    /// プレイヤーの有効化
    /// </summary>
    protected void PlayerEnabled()
    {
        player.GetComponent<OVRPlayerController>().EnableLinearMovement = true;
        player.GetComponent<OVRPlayerController>().EnableRotation = true;
    }

    /// <summary>
    /// GameOver処理
    /// </summary>
    public virtual void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            StartCoroutine(nameof(GameOverSence));
        }
    }

    protected virtual IEnumerator GameOverSence()
    {
        bgm.Stop();
        bgm.PlayOneShot(gameOver);

        // ビネット効果適用
        while (vignette.intensity.value < 1.0f)
        {
            vignette.intensity.value += 0.1f;
            yield return new WaitForSeconds(0.3f);
        }
        StartCoroutine(TransitionScene(SceneManager.GetActiveScene().buildIndex));
    }

    /// <summary>
    /// シーン遷移
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public IEnumerator TransitionScene(int sceneNo)
    {
        isTransitionScene = true;
        destinationGuide.SetActive(false);
        screenFade.FadeOut();
        yield return new WaitForSeconds(screenFade.fadeTime);

        SceneManager.LoadSceneAsync(sceneNo);
    }

    
}
