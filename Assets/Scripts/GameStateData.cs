using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Create GameStateData")]
public class GameStateData : ScriptableObject
{
    public enum StageSceneState
    {
        Start,
        Step1,
        Step2,
        Step3,
        Step4,
        End
    }

    public enum StageScene
    {
        Start,
        Stage1,
        Stage2,
        Stage3,
        Stage4,
        Stage5,
        Stage6
    }
}
