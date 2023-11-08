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

    List<Pathpoint> FindPath(Vector2 startPos, Vector2 goalPos)
    {
        //Setup
        openList.Clear();
        closedList.Clear();
        Pathpoint goalPoint = FindClosestPoint(goalPos);
        Pathpoint startPoint = FindClosestPoint(startPos);
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
                return path;
            }
            for (int i = 0; i < currentPoint.connections.Length; i++)
            {
                Pathpoint p = currentPoint.connections[i];
                if (!p.isLocked)
                {
                    float distFromStart = currentPoint.distFromStart + Vector3.Distance(p.transform.position, currentPoint.transform.position);
                    if (openList.Contains(p) || closedList.Contains(p))
                    {
                        if (distFromStart < p.distFromStart)
                        {
                            System.Console.WriteLine("SHOULD UPDATE NODE AND ALL CHILD NODES");
                        }
                    }
                    else
                    {
                        p.distFromStart = distFromStart;
                        p.estDistToEnd = Vector3.Distance(p.transform.position, goalPos);
                        p.total = p.distFromStart + p.estDistToEnd;
                        p.discoveryPoint = currentPoint;
                        openList.Add(p);
                    }
                }
            }
            closedList.Add(currentPoint);
            openList.Remove(currentPoint);
        }
        return null;
    }

    public Pathpoint sp = null;
    public Pathpoint ep = null;
    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            sp = FindClosestPoint(mousePos);
            print($"startpos: {sp.transform.position}");
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            ep = FindClosestPoint(mousePos);
            print($"endpos: {ep.transform.position}");
        }
        if(Input.GetKeyDown(KeyCode.Space) && sp != null && ep != null)
        {
            List<Pathpoint> path =  FindPath(sp.transform.position, ep.transform.position);
            print("PATH:");
            for (int i = path.Count - 1; i >= 0; i--)
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
            if (openList[i].estDistToEnd < bestPoint.estDistToEnd)
            {
                bestPoint = openList[i];
            }
        }

        return bestPoint;
    }

    void WalkPath(Pathpoint[] path)
    {

    }
}
