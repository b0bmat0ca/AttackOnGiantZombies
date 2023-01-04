#define DEBUG
#undef DEBUG

using UnityEngine;

public class ZombieHitController : MonoBehaviour
{
    [Header("ゾンビコントローラーを指定")] public ZombieController controller;

    /// <summary>
    /// Playerと衝突した場合の処理
    /// </summary>
    /// <param name="other"></param>
    protected virtual void OnTriggerEnter(Collider other)
    {
#if DEBUG
        Debug.Log($"ZombieHitController.OnTriggerEnter: {other.gameObject.name}");
#endif
        if (other.gameObject.CompareTag("Player"))
        {
            // シーン遷移中以外
            if (!controller.controller.isTransitionScene)
            {
                StartCoroutine(controller.SetAttack());
            }
        }
    }
}
