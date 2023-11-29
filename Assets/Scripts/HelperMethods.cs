using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperMethods
{
    //blir dubbla det här värdet
    static float maxDistToGroundCheck = Mathf.Infinity;


    public static Vector3 CursorToWorldCoord()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float pos;
        Plane plane = new Plane(Vector3.forward, -Pathfinding.zMoveValue);
        if (plane.Raycast(ray, out pos))
        {
            return ray.GetPoint(pos);
            
        }
        return Vector3.zero;
    }

    public static bool WallBetweenPointsOnGround(Vector3 p1, Vector3 p2)
    {
        return WallBetweenPointsOnGround(p1, p2, 1);
    }

    public static bool WallBetweenPointsOnGround(Vector3 p1, Vector3 p2, float height)
    {
        p1 = ConvertPosToBeOnGround(p1, height);
        p2 = ConvertPosToBeOnGround(p2, height);
        return WallBetweenPoints(p1, p2);
    }

    public static bool WallBetweenPoints(Vector3 p1, Vector3 p2)
    {
        Vector3 dir = (p2 - p1).normalized;
        float length = Vector3.Distance(p1, p2);
        return Physics.Raycast(p1, dir, length, 1 << 6);
    }

    public static Vector3 ConvertPosToBeOnGround(Vector3 pos)
    {
        return ConvertPosToBeOnGround(pos, 1);
    }

    public static Vector3 ConvertPosToBeOnGround(Vector3 pos, float height)
    {
        return ConvertPosToBeOnGround(pos, height, new Vector2(0.5f, 0.5f));
    }

    public static Vector3 ConvertPosToBeOnGround(Vector3 pos, float height, Vector2 rayDimensions)
    {
        RaycastHit hit;
        if (Physics.BoxCast(pos, new Vector3(rayDimensions.x, .01f, rayDimensions.y), Vector3.down, out hit, Quaternion.identity, maxDistToGroundCheck, 1 << 6))
        {
            float groundPosY = hit.point.y;
            float characterHeight = height;
            pos = new Vector3(pos.x, groundPosY + (characterHeight / 2), pos.z);
        }
        return pos;
    }

    public static GameObject[] FindGameObjectsWithLayer(int layer)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        List<GameObject> objectsWithLayer = new List<GameObject>();
        foreach(GameObject o in allObjects)
        {
            if (o.layer == layer) objectsWithLayer.Add(o);
        }
        return objectsWithLayer.ToArray();
    }
}
