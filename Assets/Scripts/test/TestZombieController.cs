#define DEBUG
#undef DEBUG

using System.Collections;
using UnityEngine;

public class TestZombieController : ZombieController
{
    /// <summary>
    /// 移動処理
    /// </summary>
    protected override void Move()
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
    /// 攻撃処理
    /// </summary>
    /// <returns></returns>
    public override IEnumerator SetAttack()
    {
        animator.SetTrigger(attackHash);

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(zombieAttackHash));

        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(zombieAttackHash));
    }
}
