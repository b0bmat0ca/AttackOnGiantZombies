#define DEBUG
#undef DEBUG

using System.Collections;
using UnityEngine;


public class Stage2AttackZombieController : ZombieController
{
    // Animatorの状態
    private readonly int zombieAttack2Hash = Animator.StringToHash("ZombieAttack2");
    // Animatorのパラメータ
    private readonly int attack2kHash = Animator.StringToHash("Attack2");


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        StartCoroutine(AttackMotion());
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
    private IEnumerator AttackMotion()
    {
        while (destination == null)
        {
            animator.SetTrigger(attack2kHash);
            yield return new WaitForSeconds(1.2f);
            audioSource.PlayOneShot(down);

            yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(zombieAttack2Hash));
        }
    }
}
