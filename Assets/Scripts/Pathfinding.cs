using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Kvar att fixa: 
 * så man går i djupled till maskiner
 * så man går på marken
 * om man är närmre ett slutpunkten än objektet, men objektet är närmre näst sista punkten än sista så går man fel
 * om man klickar på en punkt innan man har en karaktär markerad så går snubben dit direkt efter man markerar denne
 */
public class Pathfinding : MonoBehaviour
{
    Pathpoint[] allPoints;
    List<Pathpoint> openList = new List<Pathpoint>();
    List<Pathpoint> closedList = new List<Pathpoint>();


    //hur stor skillnaden kan vara i y-led för att karaktären ska kunna
    //skippa pathpointsen och ta sig direkt till målet
    float acceptablePathCutYAxisValue = 1; 

    private void Start()
    {
        allPoints = FindObjectsOfType(typeof(Pathpoint)) as Pathpoint[];
    }

    public List<Vector3> FindPath(Vector3 startPos, Vector3 goalPos)
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
                
                return MakePath(startPos, goalPos, currentPoint, startPoint);
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
        return new List<Vector3>();
    }

    List<Vector3> MakePath(Vector3 startPos, Vector3 goalPos, Pathpoint currentPoint, Pathpoint startPoint)
    {
        List<Vector3> path = new List<Vector3>();

        path.Add(goalPos);
        while (currentPoint != startPoint)
        {
            path.Add(currentPoint.transform.position);
            currentPoint = currentPoint.discoveryPoint;
        }


        //Om avståndet mellan startpositionen och första pathnoden är större än första noden och startpointen
        if (Vector3.Distance(startPos, path[path.Count - 1]) >
            Vector3.Distance(path[path.Count - 1], startPoint.transform.position))
        { 
            path.Add(startPoint.transform.position);
        }
        path.Reverse();

        Vector3 distToGoalPos = goalPos - startPos;

        if (Vector3.Distance(startPos, goalPos) < Vector3.Distance(startPos, path[0]) && distToGoalPos.y < acceptablePathCutYAxisValue)
        {
            return new List<Vector3>() { goalPos };
        }
        return path;
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
    /*private void Update()
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
    }*/

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
