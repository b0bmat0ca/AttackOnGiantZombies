#define DEBUG
#undef DEBUG

using System.Collections;
using UnityEngine;

public class Stage2LookZombieController : ZombieController
{
    // Animatorの状態
    private readonly int zombieLookHash = Animator.StringToHash("ZombieLook");
    // Animatorのパラメータ
    private readonly int lookkHash = Animator.StringToHash("Look");
    private readonly int IdleHash = Animator.StringToHash("Idle");


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        StartCoroutine(LookkMotion());
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (destination == null)
        {
            transform.LookAt(firstLookAt.transform);
        }

        if (!isHit && !isDead && destination != null)
        {
            Move();
        }
    }

    /// <summary>
    /// 登場時の演出用
    /// </summary>
    /// <returns></returns>
    private IEnumerator LookkMotion()
    {
        while (destination == null)
        {
            animator.SetTrigger(lookkHash);

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(zombieLookHash));
            yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(zombieLookHash));
        }
    }

    /// <summary>
    /// アイドル状態に遷移
    /// </summary>
    public void SetIdle()
    {
        animator.SetTrigger(IdleHash);
        animator.gameObject.transform.localPosition = Vector3.zero;
        animator.gameObject.transform.localRotation = Quaternion.identity;
    }
}
