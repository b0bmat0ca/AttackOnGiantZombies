#define DEBUG
#undef DEBUG

using UnityEngine;


public class GunShootStage1 : GunShoot
{
    [Header("鉄砲チュートリアルを指定"), SerializeField] private GameObject GunTutorial;


    protected override void OnDisable()
    {
        base.OnDisable();
        GunTutorial.SetActive(false);
    }

    protected override void Awake()
    {
        base.Awake();
        GunTutorial.SetActive(false);
    }

    /// <summary>
    /// LineRenderの描画
    /// </summary>
    protected override void LateUpdate()
    {
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(0, targetPoint);
            lineRenderer.SetPosition(1, scouter.transform.position);
            GunTutorial.SetActive(true);
        }
        else
        {
            GunTutorial.SetActive(false);
        }
    }
}
