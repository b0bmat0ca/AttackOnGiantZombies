#define DEBUG
#undef DEBUG

using UnityEngine;

public class RedStoneGrabbable : MonoBehaviour
{
    public bool isGrabbable = false;
    [Header("拾った時の音声を指定"), SerializeField] private AudioClip pickup;
    [Header("落とした時の音声を指定"), SerializeField] private AudioClip drop;

    private AudioSource audioSource;
    private OVRGrabbable grabbable;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        grabbable = GetComponent<OVRGrabbable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbable.isGrabbed)
        {
            // 掴んでいる手を取得
            OVRGrabber grabber = grabbable.grabbedBy;

            // 左手のみ掴むことができる
            if (grabber.name != "CustomHandLeft")
            {
                if (isGrabbable)
                {
                    audioSource.PlayOneShot(drop);
                }
                grabber.ForceRelease(grabbable);
                isGrabbable = false;
                return;
            }

            if (!isGrabbable)
            {
                audioSource.PlayOneShot(pickup);
            }
            isGrabbable = true;
        }
        else
        {
            if (isGrabbable)
            {
                audioSource.PlayOneShot(drop);
            }
            isGrabbable = false;
        }
    }
}
