using UnityEngine;


public class GrappleVFX : MonoBehaviour
{
    [Header("シーン用ゲームマネージャーを指定"), SerializeField] private GameManager controller;
    [Header("VFX表示距離を指定"), SerializeField] private float maxDistance;

    private ParticleSystem grappleVFX;


    private void Awake()
    {
        grappleVFX = GetComponent<ParticleSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Transform playerParent = controller.player.transform.parent;

        // 自分がよじ登っているオブジェクトの場合はVFXを非表示
        if (playerParent != null && (playerParent.transform.parent.gameObject == gameObject.transform.parent.gameObject))
        {
            if (grappleVFX.isPlaying)
            {
                grappleVFX.Stop();
            }
        }
        // 表示距離まで近づいていない場合はVFXを非表示
        else if (Vector3.Distance(controller.player.transform.position, gameObject.transform.position) > maxDistance)
        {
            if (grappleVFX.isPlaying)
            {
                grappleVFX.Stop();
            }
        }
        else
        { 
            if (!grappleVFX.isPlaying)
            {
                grappleVFX.Play();
            }
        }
    }
}
