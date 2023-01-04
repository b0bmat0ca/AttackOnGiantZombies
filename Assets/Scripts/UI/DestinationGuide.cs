#define DEBUG
#undef DEBUG

using UnityEngine;

public class DestinationGuide : MonoBehaviour
{
    [Header("目的地を指定")] public Transform destination;

    private Transform player;
    private float playerHeight;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHeight = player.gameObject.GetComponent<CharacterController>().height;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 displayPosition = player.position - new Vector3(0f, playerHeight, 0) + player.forward * 3;

        //Vector3 displayPosition = Camera.main.transform.position - new Vector3(0f, 1.5f, 0) + Camera.main.transform.forward * 2;
        transform.position = displayPosition;
        transform.rotation = player.rotation;

        transform.LookAt(destination);
    }
}
