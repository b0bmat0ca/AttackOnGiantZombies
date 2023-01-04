#define DEBUG
#undef DEBUG

using UnityEngine;

using static GameStateData;

public class DoorTrigger : MonoBehaviour
{
    [Header("シーン用ゲームマネージャーを指定"), SerializeField] private GameManager controller;
    [Header("次のシーンを指定"), SerializeField] private StageScene nextScene;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            audioSource.Play();
            StartCoroutine(controller.TransitionScene((int)nextScene));
        }
    }
}
