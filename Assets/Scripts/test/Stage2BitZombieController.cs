#define DEBUG
#undef DEBUG

using System.Collections;
using UnityEngine;

public class Stage2BitZombieController : ZombieController
{
    [Header("咀嚼音を指定"), SerializeField] private AudioClip bit;

    // Animatorの状態
    private readonly int zombieBitHash = Animator.StringToHash("ZombieBiting");
    // Animatorのパラメータ
    private readonly int bitHash = Animator.StringToHash("Bit");


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        StartCoroutine(BitMotion());
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
    private IEnumerator BitMotion()
    {
        while (destination == null)
        {
            animator.SetTrigger(bitHash);

            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(bit);
            }

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(zombieBitHash));
            yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(zombieBitHash));
        }
    }
}
