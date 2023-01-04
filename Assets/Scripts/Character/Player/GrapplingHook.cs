#define DEBUG
#undef DEBUG

using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [Header("シーン用ゲームマネージャーを指定")] public GameManager controller;

    [Header("鉤縄の照準用LineRendererを指定"), SerializeField] private LineRenderer lineRenderer;
    [Header("鉤縄のロープ用LineRendererを指定"), SerializeField] private LineRenderer ropeRenderer;
    [Header("鉤縄を指定"), SerializeField] private Transform grapplingHook;
    [Header("手の位置を指定"), SerializeField] private Transform handPos;
    [Header("プレイヤーの位置を指定"), SerializeField] private Transform player;
    [Header("鉤縄を引っ掛けるレイヤーを指定"), SerializeField] private LayerMask grappleLayer;
    [Header("鉤縄の到達距離を指定"), SerializeField] private float maxGrappleDistance;
    [Header("鉤縄のスピードを指定"), SerializeField] private float hookSpeed;
    [Header("鉤縄とロープの誤差を指定"), SerializeField] private Vector3 offset;

    private bool isShooting;      // 鉤縄を放っている
    private bool isGrappling;     // 鉤縄に引っ張られている
    private Vector3 hookPoint;
    private GameObject hookObject;
    private float controllerFrequency;    // コントローラーの振幅
    private float controllerAmplitude;     // コントローラーの振動数


    private void Awake()
    {
        isShooting = false;
        isGrappling = false;
        lineRenderer.enabled = false;
        ropeRenderer.enabled = false;
        controllerFrequency = 0;
        controllerAmplitude = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
#if DEBUG
        Debug.Log($"GrapplingHook.Update isShooting: {isShooting} isGrappling: {isGrappling} hookObject: {hookObject}");
#endif

        if (!isShooting && !isGrappling)
        {
            if (Physics.Raycast(handPos.position, handPos.forward, out RaycastHit hit, maxGrappleDistance, grappleLayer))
            {
                hookPoint = hit.point;
                if (hookObject == null || hit.collider.gameObject.transform.parent.gameObject != hookObject)
                {
                    lineRenderer.enabled = true;
                }
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }

        if (grapplingHook.parent == handPos)
        {
            // 鉤縄オブジェクトのローカル座標
            grapplingHook.localPosition = new Vector3(0.01f, 0, -0.25f);
            grapplingHook.localRotation = Quaternion.Euler(Vector3.zero);
        }

        // 鉤縄を放つ
        if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
        {
            ShootHook();
        }

        // 鉤縄を放ったオブジェクトに引っ掛かる状態を解除
        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            if (hookObject != null)
            {
                ReleaseHook();
            }
        }

        // 鉤縄に引っ張られる
        if (isGrappling)
        {
            // コントローラーを振動させる
            controllerAmplitude += 1.0f * Time.deltaTime;
            controllerFrequency += 1.0f * Time.deltaTime;
            OVRInput.SetControllerVibration(controllerFrequency, controllerAmplitude, OVRInput.Controller.LTouch);

            // 鉤縄を対象オブジェクトに移動
            grapplingHook.position = Vector3.Lerp(grapplingHook.position, hookPoint, hookSpeed * Time.deltaTime);
            if (Vector3.Distance(grapplingHook.position, hookPoint) < 0.5f)
            {
                // プレイヤーを対象オブジェクトに移動
                player.parent = null;
                player.GetComponent<CharacterController>().enabled = false;
                player.position = Vector3.Lerp(player.position, hookPoint - offset, hookSpeed * Time.deltaTime);

                // ビネット効果適用
                controller.vignette.intensity.value = 1.0f;
            }

            // プレイヤーが鉤縄に到達
            if (Vector3.Distance(player.position, hookPoint - offset) < 0.5f)
            {
                isGrappling = false;
                grapplingHook.SetParent(handPos);
                ropeRenderer.enabled = false;
                player.SetParent(hookObject.transform);

                // コントローラーの振動を止める
                controllerAmplitude = 0;
                controllerFrequency = 0;
                OVRInput.SetControllerVibration(controllerFrequency, controllerAmplitude, OVRInput.Controller.LTouch);

                // ビネット効果解除
                controller.vignette.intensity.value = 0f;
            }
        }
    }

    private void LateUpdate()
    {
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(0, hookPoint);
            lineRenderer.SetPosition(1, handPos.position);
        }
        else if (ropeRenderer.enabled)
        {
            ropeRenderer.SetPosition(0, grapplingHook.position);
            ropeRenderer.SetPosition(1, handPos.position);
        }
    }

    /// <summary>
    /// 鉤縄を放つ
    /// </summary>
    private void ShootHook()
    {
        if (isShooting || isGrappling)
        {
            return;
        }

        isShooting = true;

        if (Physics.Raycast(handPos.position, handPos.forward, out RaycastHit hit, maxGrappleDistance, grappleLayer))
        {
            if (hookObject == null || hit.collider.gameObject.transform.parent.gameObject != hookObject)
            {
                hookPoint = hit.point;
                hookObject = hit.collider.gameObject.transform.parent.gameObject;
                isGrappling = true;
                grapplingHook.parent = null;
                grapplingHook.LookAt(hookPoint);
                ropeRenderer.enabled = true;
            }
        }

        isShooting = false;
    }

    /// <summary>
    /// 鉤縄を外す
    /// </summary>
    private void ReleaseHook()
    {
        player.GetComponent<CharacterController>().enabled = true;
        player.parent = null;
        hookObject = null;
    }
}
