#define DEBUG
#undef DEBUG

using UnityEngine;
using UnityEngine.UI;

using static GameStateData;

public class PlayerController : MonoBehaviour
{
    [Header("マニュアルUIを指定")] public GameObject manual;
    [Header("GunTutorialを指定")] public Image gunTutorial;
    [Header("RedSoneTutorialを指定")] public Image redStoneTutorial;
    [Header("GrapplingHookTutorialを指定")] public Image hookTutorial;


    // Start is called before the first frame update
    void Start()
    {
        switch (GameManager.currentStage)
        {
            case StageScene.Start:
                gunTutorial.enabled = false;
                redStoneTutorial.enabled = false;
                hookTutorial.enabled = false;
                break;
            case StageScene.Stage1:
                redStoneTutorial.enabled = false;
                hookTutorial.enabled = false;
                manual.SetActive(true);
                break;
            case StageScene.Stage2:
            case StageScene.Stage3:
                redStoneTutorial.enabled = false;
                hookTutorial.enabled = false;
                break;
            case StageScene.Stage4:
                gunTutorial.enabled = false;
                redStoneTutorial.enabled = false;
                manual.SetActive(true);
                break;
            case StageScene.Stage5:
                gunTutorial.enabled = false;
                redStoneTutorial.enabled = false;
                break;

            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // マニュアル表示・非表示
        if (OVRInput.GetDown(OVRInput.RawButton.Start))
        {
            manual.SetActive(!manual.activeSelf);
        }
    }
}
