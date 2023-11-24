using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDefiner : MonoBehaviour
{
    float maxRoomSize = 100;
    bool waitingForClick = false;
    float boxSize = 1;
    private void Start()
    {
        ScanWorld();
    }
    private void Update()
    {
        if(waitingForClick)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Vector3 pos = HelperMethods.CursorToWorldCoord();
                Vector3 posOnGround = HelperMethods.ConvertPosToBeOnGround(pos, 1, new Vector2(0.05f,0.05f));
                if(pos != posOnGround)
                {
                    TextLog.AddLog("scan started", Color.blue);
                    ScanBuilding(posOnGround);
                }
                else
                {
                    TextLog.AddLog("pos not in house", Color.blue);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            waitingForClick = !waitingForClick;
            if (waitingForClick)
            {
                TextLog.AddLog("Click on an area within the building", Color.blue);
            }
            else
            {
                TextLog.AddLog("Pathmaking cancelled", Color.blue);
            }
        }
    }

    void ScanWorld()
    {
        GameObject[] walls = HelperMethods.FindGameObjectsWithLayer(6);
        GameObject[] ladders = HelperMethods.FindGameObjectsWithLayer(7);
        if (walls.Length > 0)
        {
            Bounds houseBounds = new Bounds(Vector3.zero, Vector3.zero);
            foreach (GameObject wall in walls)
            {
                houseBounds.Encapsulate(wall.GetComponent<MeshRenderer>().bounds);
            }
            
        }
    }

    /*GameObject GetMostExtremePosInDir(GameObject[] allObjects,Vector3 dir)
    {
        Vector3 extremePos = Vector3.zero;
        foreach(GameObject obj in allObjects)
        {
            if (extremePos == Vector3.zero)
            {
                extremePos = obj.GetComponent<Mesh>().bounds.
            }
        }
    }*/

    void ScanBuilding(Vector3 startPos)
    {
        ScanRoom(startPos);
    }

    void ScanRoom(Vector3 startPos)
    {
        RaycastHit hit;
        if (CustomBoxCast(startPos, Vector3.left, out hit))
        {
            print("left wall at:" + hit.point);
        }
        else
        {
            print("no wall!");
        }
    }

    bool CustomBoxCast(Vector3 startPos, Vector3 dir, out RaycastHit hit)
    {
        return Physics.BoxCast(startPos, new Vector3(boxSize / 2, boxSize / 2, boxSize / 2), dir, out hit, Quaternion.identity, maxRoomSize, 1 << 6);
    }
}
