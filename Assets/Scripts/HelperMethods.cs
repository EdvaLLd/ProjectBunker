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
        if (Physics.BoxCast(pos + new Vector3(0,0.05f,0), new Vector3(rayDimensions.x, .01f, rayDimensions.y), Vector3.down, out hit, Quaternion.identity, maxDistToGroundCheck, 1 << 6))
        {
            float groundPosY = hit.point.y;
            float characterHeight = height;
            pos = new Vector3(pos.x, groundPosY + (characterHeight / 2), pos.z);
        }
        return pos;
    }

    public static bool IsCoordinatesOnAVerticalLine(Vector3 pos1, Vector3 pos2)
    {
        float margin = 0.05f;
        if(Mathf.Abs(pos1.x - pos2.x) < margin)
        {
            if (Mathf.Abs(pos1.z - pos2.z) < margin)
            {
                return true;
            }
        }
        return false;
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

    public static void ClearChilds(Transform parent)
    {
        foreach (Transform child in parent.transform)
        {
            if(child.gameObject == ItemHoverDesc.windowHovering)
            {
                child.GetComponent<ItemHoverDesc>().OnPointerExit(null); //kanske en skev lösning?
            }
            Object.Destroy(child.gameObject);
        }
    }

    public static List<Character> GetCharactersInSameRoom(Character ch)
    {
        List<Character> allCharacters = UnitController.GetCharacters();
        List<Character> allCharactersInRoom = new List<Character>();
        foreach (Character c in allCharacters)
        {
            if(!HelperMethods.WallBetweenPoints(ch.transform.position, c.transform.position) &&
                c != ch)
            {
                allCharactersInRoom.Add(c);
            }
        }
        return allCharactersInRoom;
    }

    //kopierad, antar att den funkar?
    public static List<T> ScrambleList<T>(List<T> list)
    {
        list.Sort((a, b) => 1 - 2 * Random.Range(0, list.Count));
        return list;
    }
}
