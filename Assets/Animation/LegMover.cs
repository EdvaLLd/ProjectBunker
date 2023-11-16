using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegMover : MonoBehaviour
{
    public Transform limbSolverTarget;
    public float moveDistance;
    public LayerMask groundLayer;
    public float offset;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
        if(Vector2.Distance(limbSolverTarget.position, transform.position) > moveDistance)
        {
            limbSolverTarget.position = transform.position + new Vector3(0, offset, 0);
        }
    }

    public void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, Vector3.down, 5, groundLayer);
        if(hit.collider != null)
        {
            //The point where the leg hit something
            Vector3 point = hit.point;
            point.y += 0.1f;
            transform.position = point;
        }
    }
}
