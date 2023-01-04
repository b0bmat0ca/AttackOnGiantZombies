#define DEBUG
#undef DEBUG

using UnityEngine;
using UnityEngine.UI;

public class GunShoot : MonoBehaviour
{
    [Header("シーン用ゲームマネージャーを指定"), SerializeField] protected GameManager controller;

    [Header("鉄砲の照準キャンバスを指定"), SerializeField] protected GameObject scouter;
    [Header("鉄砲の照準を指定"), SerializeField] protected Image crossHair;
    [Header("照準レーダーを指定"), SerializeField] protected LineRenderer lineRenderer;
    [Header("弾丸到達距離を指定"), SerializeField] protected float shootRange;

    [Header("敵のレイヤーを指定"), SerializeField] protected LayerMask enemyLayer;
    [Header("敵の弱点レイヤーを指定"), SerializeField] protected LayerMask weaknessLayer;

    protected Animator trigger;
    protected AudioSource shootSE;

    protected OVRGrabbable grabbable;
    //protected Rigidbody rbody;

    protected bool isShooting;
    protected Vector3 targetPoint;
    protected Color defaltCrossHairColor;


    protected virtual void OnDisable()
    {
        controller.deadCounter.enabled = false;
        lineRenderer.enabled = false;
        scouter.SetActive(false);
    }

    protected virtual void Awake()
    {
        trigger = GetComponent<Animator>();
        shootSE = GetComponent<AudioSource>();

        grabbable = GetComponent<OVRGrabbable>();
        //rbody = GetComponent<Rigidbody>();

        defaltCrossHairColor = crossHair.color;
        lineRenderer.enabled = false;
        scouter.SetActive(false);
        isShooting = false;
    }

    // Start is called before the first frame update
    protected void Start()
    {

    }

    // Update is called once per frame
    protected void Update()
    {
        if (grabbable.isGrabbed)
        {
            // 掴んでいる手を取得
            OVRGrabber grabber = grabbable.grabbedBy;

            // 右手のみ掴むことができる
            if (grabber.name != "CustomHandRight")
            {
                grabber.ForceRelease(grabbable);
                controller.deadCounter.enabled = false;
                lineRenderer.enabled = false;
                scouter.SetActive(false);
                return;
            }
            
            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
            {
                trigger.SetTrigger("GunTriggerOn");
                Shoot();
            }

            if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
            {
                trigger.SetTrigger("GunTriggerOff");
            }
        }
        else
        {
            controller.deadCounter.enabled = false;
            lineRenderer.enabled = false;
            scouter.SetActive(false);
            return;
        }

        scouter.SetActive(true);
        controller.deadCounter.enabled = true;

        
        if (!isShooting)
        {
            Vector3 direction = scouter.transform.forward;
            Ray ray = new(scouter.transform.position, direction);
            RaycastHit hit;
            ZombieHitController hitController;

            if (Physics.Raycast(ray, out hit, shootRange, weaknessLayer))
            {
                hitController = hit.collider.transform.gameObject.GetComponent<ZombieHitController>();
                targetPoint = hit.point;
                lineRenderer.enabled = true;
                crossHair.color = Color.red;
                controller.deadCounter.text = hitController.controller.currentDeadCount.ToString();
            }
            else if (Physics.Raycast(ray, out hit, shootRange, enemyLayer))
            {
                hitController = hit.collider.transform.gameObject.GetComponent<ZombieHitController>();
                targetPoint = hit.point;
                lineRenderer.enabled = true;
                crossHair.color = Color.cyan;
                controller.deadCounter.text = hitController.controller.currentDeadCount.ToString();
            }
            else
            {
                lineRenderer.enabled = false;
                crossHair.color = defaltCrossHairColor;
                controller.deadCounter.text = string.Empty;
            }
        }
        else
        {
            lineRenderer.enabled = false;
            crossHair.color = defaltCrossHairColor;
            controller.deadCounter.text = string.Empty;
        }
    }

    /// <summary>
    /// LineRenderの描画
    /// </summary>
    protected virtual void LateUpdate()
    {
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(0, targetPoint);
            lineRenderer.SetPosition(1, scouter.transform.position);
        }
    }

    /// <summary>
    /// 発砲
    /// </summary>
    protected void Shoot()
    {
        isShooting = true;

        if (!shootSE.isPlaying)
        {
            // 発砲音を再生
            shootSE.Play();

            Vector3 direction = scouter.transform.forward;
            Ray ray = new(scouter.transform.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, shootRange, weaknessLayer))
            {
                hit.collider.transform.gameObject.GetComponent<ZombieHitController>().controller.GunCricicalHit(direction);
            }
            else if (Physics.Raycast(ray, out hit, shootRange, enemyLayer))
            {
                hit.collider.transform.gameObject.GetComponent<ZombieHitController>().controller.GunHit(direction);
            }
        }

        isShooting = false;
    }


}
