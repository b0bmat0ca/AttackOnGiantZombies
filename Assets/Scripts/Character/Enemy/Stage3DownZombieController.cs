#define DEBUG
#undef DEBUG

using UnityEngine;
using UnityEngine.AI;

public class Stage3DownZombieController : ZombieController
{
    [Header("EnemyLayerのトリガーを指定"), SerializeField] protected BoxCollider enemyTriger;
    [Header("WeaknessLayerのトリガーを指定"), SerializeField] protected SphereCollider weaknessTriger;

    // Animatorのパラメータ
    private readonly int dead1Hash = Animator.StringToHash("Dead1");
    private readonly int dead2Hash = Animator.StringToHash("Dead2");
    private readonly int IdleHash = Animator.StringToHash("Idle");


    // Update is called once per frame
    protected override void Update()
    {
        if (!isHit && !isDead && destination != null)
        {
            Move();
        }
    }

    /// <summary>
    /// 倒れた(死亡)状態1に遷移
    /// </summary>
    public void SetDead1()
    {
#if DEBUG
        Debug.Log("Stage3DownZombieController.SetDead1");
#endif
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        enemyTriger.enabled = false;
        weaknessTriger.enabled = false;
        animator.applyRootMotion = true;
        animator.SetTrigger(dead1Hash);
    }

    /// <summary>
    /// 倒れた(死亡)状態2に遷移
    /// </summary>
    public void SetDead2()
    {
#if DEBUG
        Debug.Log("Stage3DownZombieController.SetDead2");
#endif
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        enemyTriger.enabled = false;
        weaknessTriger.enabled = false;
        animator.applyRootMotion = true;
        animator.SetTrigger(dead2Hash);
    }

    /// <summary>
    /// アイドル状態に遷移
    /// </summary>
    public void SetIdle()
    {
#if DEBUG
        Debug.Log("Stage3DownZombieController.SetIdle");
#endif
        animator.SetTrigger(IdleHash);
        animator.applyRootMotion = false;
        animator.gameObject.transform.localPosition = Vector3.zero;
        animator.gameObject.transform.localRotation = Quaternion.identity;
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;
        enemyTriger.enabled = true;
        weaknessTriger.enabled = true;
    }
}
