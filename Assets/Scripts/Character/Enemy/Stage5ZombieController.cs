#define DEBUG
#undef DEBUG


public class Stage5ZombieController : ZombieController
{
    private float controllerFrequency;    // コントローラーの振幅
    private float controllerAmplitude;     // コントローラーの振動数


    protected override void Awake()
    {
        base.Awake();
        controllerFrequency = 0;
        controllerAmplitude = 0;
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    protected override void Move()
    {
        agent.SetDestination(destination.transform.position);

        /*
         * こいつを入れると、巨大ゾンビの追随がうまく行かない
        if (Vector3.Distance(agent.steeringTarget, transform.position) < 1.0f)
        {
            agent.speed = 1.0f;
        }
        else
        {
            agent.speed = moveSpeed;
        }
        */
        
        animator.SetFloat(speedHash, agent.velocity.sqrMagnitude);

        if (animator.GetFloat(speedHash) < 0.01)
        {
            // コントローラーの振動を止める
            controllerAmplitude = 0;
            controllerFrequency = 0;
            OVRInput.SetControllerVibration(controllerFrequency, controllerAmplitude, OVRInput.Controller.RTouch);

            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
        else if (animator.GetFloat(speedHash) < walkToRunSpeed)
        {
            // コントローラーを振動させる
            controllerAmplitude = 0.5f;
            controllerFrequency = 0.5f;
            OVRInput.SetControllerVibration(controllerFrequency, controllerAmplitude, OVRInput.Controller.RTouch);

            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(walk);
            }
        }
        else
        {
            // コントローラーを振動させる
            controllerAmplitude = 1.0f;
            controllerFrequency = 1.0f;
            OVRInput.SetControllerVibration(controllerFrequency, controllerAmplitude, OVRInput.Controller.RTouch);

            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(run);
            }
        }
    }
}
