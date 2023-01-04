#define DEBUG
#undef DEBUG

using UnityEngine;

using static GameStateData;

public class BaseTrigger : MonoBehaviour
{
    [Header("シーン用ゲームマネージャーを指定"), SerializeField] private GameManager controller;
    [Header("次のシーンを指定"), SerializeField] private StageScene nextScene;
    [Header("台座のキャンバスを指定"), SerializeField] private GameObject baseCanvas;

    private AudioSource audioSource;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }


    private void OnCollisionEnter(Collision collision)
    {
#if DEBUG
        Debug.Log($"BaseTrigger.OnCollisionEnter: {collision.gameObject.tag} {(int)nextScene}");
#endif

        if (collision.gameObject.name == "RedStone")
        {
            baseCanvas.SetActive(false);
            audioSource.Play();
            collision.gameObject.transform.position =  collision.contacts[0].point;
            collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;

            StartCoroutine(controller.TransitionScene((int)nextScene));
        }
    }
}
