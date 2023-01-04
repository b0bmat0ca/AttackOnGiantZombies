#define DEBUG
#undef DEBUG

using UnityEngine;

public class GuideUIActive : MonoBehaviour
{
    [Header("Guideオブジェクトを指定"), SerializeField] private GameObject guideUI;


    private void OnTriggerEnter(Collider other)
    {
#if DEBUG
        Debug.Log($"GuideUIActive.OnTriggerEnter");
#endif

        // 前方から当たったか
        if (other.gameObject.CompareTag("Player") && Physics.Raycast(other.transform.position, other.transform.forward, out RaycastHit hit, 2.0f, LayerMask.GetMask("Stage")))
        {
#if DEBUG
            Debug.Log($"GuideUIActive.OnTriggerEnter: {gameObject.name}");
#endif
            if (hit.transform.gameObject == gameObject)
            {
                guideUI.SetActive(true);
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            guideUI.SetActive(false);
        }
    }
}
