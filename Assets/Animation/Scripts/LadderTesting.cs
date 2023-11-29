using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderTesting : MonoBehaviour
{
    public Transform parent;
    private Vector3 originalPos;
    public Transform footTarget;
    public Transform handTarget;
    private Transform[] IKs;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
        transform.SetParent(null);

        IKs = GetComponentsInChildren<Transform>();
        Debug.Log(IKs);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
        }

        IKs[0].position = footTarget.position;
        IKs[1].position = footTarget.position;
        IKs[2].position = handTarget.position;
        IKs[3].position = handTarget.position;
    }
}
