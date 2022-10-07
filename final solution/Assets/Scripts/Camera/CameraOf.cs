using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraOf : MonoBehaviour
{
    public Transform playerTransform;
    public Transform ptr;

    private CinemachineVirtualCamera vrc;

    // Start is called before the first frame update
    void Start()
    {
        vrc = GetComponent<CinemachineVirtualCamera>();
        ptr.position = new Vector3(playerTransform.position.x, playerTransform.position.y + 20, playerTransform.position.z);
        vrc.Follow = ptr;
    }

    // Update is called once per frame
    void Update()
    {
        ptr.position = new Vector3(playerTransform.position.x, playerTransform.position.y + 5, playerTransform.position.z);
    }
}
