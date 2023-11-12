using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    Pathpoint[] allPoints;
    List<Pathpoint> openList = new List<Pathpoint>();
    List<Pathpoint> closedList = new List<Pathpoint>();

    private void Start()
    {
        allPoints = FindObjectsOfType(typeof(Pathpoint)) as Pathpoint[];
    }

    public List<Pathpoint> FindPath(Vector2 startPos, Vector2 goalPos)
    {
        //Setup
        openList.Clear();
        closedList.Clear();
        Pathpoint goalPoint = FindClosestPoint(goalPos);
        Pathpoint startPoint = FindClosestPoint(startPos);
        for (int i = 0; i < allPoints.Length; i++)
        {
            allPoints[i].ResetPoint();
        }
        Pathpoint currentPoint = startPoint;
        currentPoint.distFromStart = 0;
        currentPoint.estDistToEnd = Vector3.Distance(currentPoint.transform.position, goalPos);
        currentPoint.total = currentPoint.estDistToEnd;
        openList.Add(currentPoint);

        while (openList.Count > 0)
        {
            //Loop
            currentPoint = PointWithBestEstimation();
            if(currentPoint == goalPoint)
            {
                List<Pathpoint> path = new List<Pathpoint>();
                while (currentPoint != startPoint)
                {
                    path.Add(currentPoint);
                    currentPoint = currentPoint.discoveryPoint;
                }
                
                path.Add(startPoint);

                if(Vector3.Distance(new Vector3(startPos.x,startPos.y, 0), new Vector3(path[path.Count-1].transform.position.x, path[path.Count - 1].transform.position.y,0)) > 
                    Vector3.Distance(new Vector3(path[path.Count - 1].transform.position.x, path[path.Count - 1].transform.position.y,0), new Vector3(startPoint.transform.position.x, startPoint.transform.position.y,0))) path.Add(startPoint);
                path.Reverse();
                return path;
            }
            for (int i = 0; i < currentPoint.connections.Length; i++)
            {
                Pathpoint p = currentPoint.connections[i];
                if (!p.isLocked)
                {
                    UpdatePointValues(p, currentPoint, goalPos);
                }
                
            }
            closedList.Add(currentPoint);
            openList.Remove(currentPoint);
        }
        return null;
    }

    void UpdatePointValues(Pathpoint pointToUpdate, Pathpoint parentPoint, Vector3 endPos)
    {
        float distFromStart = parentPoint.distFromStart + Vector3.Distance(pointToUpdate.transform.position, parentPoint.transform.position);

        if (distFromStart < pointToUpdate.distFromStart)
        {
            pointToUpdate.distFromStart = distFromStart;
            pointToUpdate.estDistToEnd = Vector3.Distance(pointToUpdate.transform.position, endPos);
            pointToUpdate.total = pointToUpdate.distFromStart + pointToUpdate.estDistToEnd;
            pointToUpdate.discoveryPoint = parentPoint;
            if (!openList.Contains(pointToUpdate) && !closedList.Contains(pointToUpdate))
            {
                openList.Add(pointToUpdate);
            }
            if(closedList.Contains(pointToUpdate))
            {
                UpdateAllChildPoints(pointToUpdate, endPos);
            }
        }
    }

    void UpdateAllChildPoints(Pathpoint point, Vector3 endPos)
    {
        for (int i = 0; i < point.connections.Length; i++)
        {
            UpdatePointValues(point.connections[i], point, endPos);
        }
    }

    Vector3 startPos = Vector3.zero;
    public Vector3 endPos = Vector3.zero;
    private void Update()
    {
;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            startPos = mousePos;
            print($"startpos: {startPos}");
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            endPos = mousePos;
            print($"endpos: {endPos}");
        }
        if(Input.GetKeyDown(KeyCode.Space) && startPos != Vector3.zero && endPos != Vector3.zero)
        {
            List<Pathpoint> path = FindPath(startPos, endPos);
            print("PATH:");
            for (int i = 0; i < path.Count; i++)
            {
                print(path[i]);
            }
        }
    }

    Pathpoint FindClosestPoint(Vector3 currentPos)
    {
        Pathpoint closestPoint = allPoints[0];
        for (int i = 1; i < allPoints.Length; i++)
        {
            if (!allPoints[i].isLocked) 
            {
                if (Vector3.Distance(closestPoint.transform.position, currentPos) > Vector3.Distance(allPoints[i].transform.position, currentPos))
                {
                    closestPoint = allPoints[i];
                }
            }
        }
        return closestPoint; //Den här kommer antagligen behöva pillas med
    }

    Pathpoint PointWithBestEstimation()
    {
        if (openList.Count < 0) return null;

        Pathpoint bestPoint = openList[0];
        for (int i = 1; i < openList.Count; i++)
        {
            if (openList[i].total < bestPoint.total)
            {
                bestPoint = openList[i];
            }
        }

        return bestPoint;
    }
}
