#define DEBUG
#undef DEBUG

using UnityEngine;

public class DestinationCursor : MonoBehaviour
{
    private Transform player;


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player);
    }
}
