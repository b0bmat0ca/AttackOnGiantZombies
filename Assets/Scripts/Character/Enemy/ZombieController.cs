#define DEBUG
#undef DEBUG

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    public bool isDead; // 死亡状態
    public bool isHit;  // 攻撃処理中
    public int currentDeadCount;   // 現在の死亡までのHit回数(ヒットポイント)

    [Header("シーン用ゲームマネージャーを指定")] public GameManager controller;
    [Header("初期に向かせる方向を指定"), SerializeField] protected GameObject firstLookAt;
    [Header("目標地点を指定"), SerializeField] public GameObject destination;

    [Header("ナビゲーションエージェントを指定"), SerializeField] protected NavMeshAgent agent;
    [Header("死亡までのHit回数を指定"), SerializeField] protected int zombieDeadCount;
    [Header("プレイヤーに気づく距離を指定"), SerializeField] protected float autoTargetDistance;


    // 音声
    [SerializeField] protected AudioSource audioSource;
    [Header("歩く音源を指定"), SerializeField] protected AudioClip walk;
    [Header("走る音源を指定"), SerializeField] protected AudioClip run;
    [Header("嗚咽音源を指定"), SerializeField] protected AudioClip wail;
    [Header("倒れる音源を指定"), SerializeField] protected AudioClip down;

    // アニメーション
    [SerializeField] protected Animator animator;
    [Header("WalkからRunアニメーションに変わるSpeedを指定"), SerializeField] protected float walkToRunSpeed;
    [Header("Deadアニメーション内の倒れるタイミングを指定"), SerializeField] protected float downTime;
    [Header("死亡後にオブジェクトを消すタイミングを指定"), SerializeField] protected float deadTime;

    // Animatorの状態
    protected readonly int zombieHitHash = Animator.StringToHash("ZombieHit");
    protected readonly int zombieAttackHash = Animator.StringToHash("ZombieAttack");
    // Animatorのパラメータ
    protected readonly int speedHash = Animator.StringToHash("Speed");
    protected readonly int attackHash = Animator.StringToHash("Attack");
    protected readonly int deadHash = Animator.StringToHash("Dead");
    protected readonly int hitHash = Animator.StringToHash("Hit");

    protected float moveSpeed;
    

    protected virtual void Awake()
    {
        isDead = false;
        isHit = false;
        currentDeadCount = zombieDeadCount;
        moveSpeed = agent.speed;
        autoTargetDistance = 40f;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        if (destination != null)
        {
            transform.LookAt(destination.transform);
            StartCoroutine(nameof(SearchPath));
        }
        else
        {
            transform.LookAt(firstLookAt.transform);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (destination == null)
        {
            transform.LookAt(firstLookAt.transform);

            // 近づくと感知する
            if (Vector3.Distance(controller.player.transform.position, transform.position) < autoTargetDistance)
            {
                destination = controller.player.gameObject;
            }
        }

        if (!isHit && !isDead && destination != null)
        {
            Move();
        }
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    protected virtual void Move()
    {
        agent.SetDestination(destination.transform.position);
        
        if (Vector3.Distance(agent.steeringTarget, transform.position) < 1.0f)
        {
            agent.speed = 1.0f;
        }
        else
        {
            agent.speed = moveSpeed;
        }
        
        animator.SetFloat(speedHash, agent.velocity.sqrMagnitude);

        if (animator.GetFloat(speedHash) < 0.01)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
        else if (animator.GetFloat(speedHash) < walkToRunSpeed)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(walk);
            }
        }
        else
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(run);
            }
        }
    }

    /// <summary>
    /// 移動アニメーションを止める
    /// </summary>
    protected void Stop()
    {
        audioSource.Stop();
        agent.velocity = Vector3.zero;
#if DEBUG
        Debug.Log($"ZombieController.Stop: {agent.velocity.sqrMagnitude}");
#endif
        animator.SetFloat(speedHash, agent.velocity.sqrMagnitude);
    }

    /// <summary>
    /// 攻撃状態に遷移
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator SetAttack()
    {
#if DEBUG
        Debug.Log("ZombieController.Attack");
#endif
        animator.applyRootMotion = true;
        animator.SetTrigger(attackHash);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(zombieAttackHash));
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(zombieAttackHash));

        controller.GameOver();
    }

    /// <summary>
    /// WeaknessLayerへの攻撃
    /// </summary>
    /// <param name="direction"></param>
    public virtual void GunCricicalHit(Vector3 direction)
    {
#if DEBUG
        Debug.Log("ZombieController.GunCricicalHit");
#endif

        currentDeadCount--;

        if (currentDeadCount <= 0)
        {
            StartCoroutine(nameof(Dead));
        }
        else
        {
            GunHit(direction);
        }
    }

    /// <summary>
    /// EnemyLayerへの攻撃
    /// </summary>
    /// <param name="direction"></param>
    public virtual void GunHit(Vector3 direction)
    {
#if DEBUG
        Debug.Log("ZombieController.GunHit");
#endif
        if (isHit)
        {
            return;
        }

        StartCoroutine(nameof(ForceStop));
    }

    /// <summary>
    /// 攻撃による強制停止処理
    /// </summary>
    protected virtual IEnumerator ForceStop()
    {
#if DEBUG
        Debug.Log("ZombieController.ForceStop");
#endif
        isHit = true;
        agent.isStopped = true;
        Stop();

        animator.SetTrigger(hitHash);

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(zombieHitHash));
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(zombieHitHash));

        agent.isStopped = false;
        isHit = false;
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Dead()
    {
#if DEBUG
        Debug.Log("ZombieController.Dead");
#endif
        isDead = true;
        agent.isStopped = true;
        Stop();
        animator.applyRootMotion = true;

        animator.SetTrigger(deadHash);
        audioSource.PlayOneShot(wail);

        yield return new WaitForSeconds(downTime);
        audioSource.PlayOneShot(down);

        yield return new WaitForSeconds(deadTime);

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 経路探索待機処理
    /// </summary>
    /// <returns></returns>
    protected IEnumerator SearchPath()
    {
#if DEBUG
        Debug.Log("ZombieController.SearchPath");
#endif
        yield return new WaitUntil(() => !agent.pathPending);
    }

    public void EventStop()
    {
        agent.isStopped = true;
        Stop();
    }

    public void EventRestart()
    {
        agent.isStopped = false;
    }
}
