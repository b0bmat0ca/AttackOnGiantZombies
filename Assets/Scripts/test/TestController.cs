using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TestController : GameManager
{
    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        base.InitStage();
        bgm.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
