#define DEBUG
#undef DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using static GameStateData;

public class TextMessageChanger : MonoBehaviour
{
    [Header("シーン上の次の状態を指定"), SerializeField] private StageSceneState nextState;

    [Header("文字を表示するUIを指定"), SerializeField] private TextMeshProUGUI message;
    [Header("列送りボタンUIを指定"), SerializeField] private Image pressA;
    [Header("表示する文字リストを指定"), SerializeField] private List<string> messageText;
    [Header("表示する文字送り秒数を指定"), SerializeField] private float messageSpeed;

    private GameManager controller;
    protected int lineCount;
    protected int currentLine = 0;


    // Start is called before the first frame update
    void Start()
    {
        lineCount = messageText.Count;
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        StartCoroutine(nameof(NextMessageText));
    }

    // Update is called once per frame
    void Update()
    {
        if (currentLine > lineCount)
        {
            controller.state = nextState;
        }
    }

    protected virtual IEnumerator NextMessageText()
    {
        for (currentLine = 0; currentLine <= lineCount; currentLine++)
        {
            // ボタンAを押すことで、次の行を表示
            yield return new WaitUntil(() => OVRInput.GetDown(OVRInput.RawButton.A));

            pressA.enabled = false;

            if (currentLine == 0)
            {
                // 表示位置を左上に変更
                message.alignment = TextAlignmentOptions.TopLeft;
            }

            if (currentLine != lineCount)
            {
                message.text = string.Empty;
                for (int i = 0; i < messageText[currentLine].Length; i++)
                {
                    message.text += messageText[currentLine][i];
                    yield return new WaitForSeconds(messageSpeed);
                }
            }
            pressA.enabled = true;
        }
    }
}
