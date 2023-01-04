#define DEBUG
#undef DEBUG

using UnityEngine;

using static GameStateData;

public class StepTrigger : MonoBehaviour
{
    [Header("シーン用ゲームマネージャーを指定"), SerializeField] private GameManager controller;
    [Header("次のシーン状態を指定"), SerializeField] private StageSceneState nextState;

    private void OnTriggerEnter(Collider other)
    {
#if DEBUG
        Debug.Log($"StepTrigger.OnTriggerEnter: {other.gameObject.name}");
#endif
        if (other.gameObject.CompareTag("Player"))
        {
            controller.state = nextState;
        }
    }

    private void OnTriggerStay(Collider other)
    {
#if DEBUG
        Debug.Log($"StepTrigger.OnTriggerStay: {other.gameObject.name}");
#endif
        if (other.gameObject.CompareTag("Player"))
        {
            controller.state = nextState;
        }
    }
}
