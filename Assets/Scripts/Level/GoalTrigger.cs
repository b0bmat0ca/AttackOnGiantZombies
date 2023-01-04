#define DEBUG
#undef DEBUG

using UnityEngine;

using static GameStateData;

public class GoalTrigger : MonoBehaviour
{
    [Header("シーン用ゲームマネージャーを指定"), SerializeField] private GameManager controller;
    [Header("次のシーンを指定"), SerializeField] private StageScene nextScene;

    private void OnTriggerEnter(Collider other)
    {
#if DEBUG
        Debug.Log($"GoalTrigger.OnTriggerEnter: {other.gameObject.tag} {(int)nextScene}");
#endif
        if (other.gameObject.CompareTag("Player"))
        {
            controller.destinationGuide.SetActive(false);
            StartCoroutine(controller.TransitionScene((int)nextScene));
        }
    }
}
