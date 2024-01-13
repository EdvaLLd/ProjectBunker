using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Kvar att fixa: 
 * om man klickar på en punkt innan man har en karaktär markerad så går snubben dit direkt efter man markerar denne
 */
public class Pathfinding : MonoBehaviour
{
    static Pathpoint[] allPoints;
    static List<Pathpoint> openList = new List<Pathpoint>();
    static List<Pathpoint> closedList = new List<Pathpoint>();


    //hur stor skillnaden kan vara i y-led för att karaktären ska kunna
    //skippa pathpointsen och ta sig direkt till målet
    public static float acceptablePathCutYAxisValue = 1;
    public static float zMoveValue = 0.5f;

    private void Start()
    {
        allPoints = FindObjectsOfType(typeof(Pathpoint)) as Pathpoint[];
    }

    public static List<Vector3> FindPath(Vector3 startPos, Vector3 goalPos, float characterHeight, BoxCollider acceptableCharacterEndPlacement = null)
    {
        startPos = HelperMethods.ConvertPosToBeOnGround(startPos, characterHeight);
        goalPos = HelperMethods.ConvertPosToBeOnGround(goalPos, characterHeight);
        //Setup
        openList.Clear();
        closedList.Clear();
        Pathpoint startPoint = FindClosestPoint(startPos, characterHeight);
        if(HelperMethods.WallBetweenPointsOnGround(startPoint.transform.position, startPos, characterHeight))
        {
            Debug.Log("no entrypoint");
            return new List<Vector3>();
        }
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
            if (!HelperMethods.WallBetweenPointsOnGround(currentPoint.transform.position, goalPos, characterHeight))
            {
                return MakePath(startPos, goalPos, currentPoint, startPoint, characterHeight, acceptableCharacterEndPlacement);
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
        Debug.Log("no path");
        return new List<Vector3>();
    }

    static List<Vector3> MakePath(Vector3 startPos, Vector3 goalPos, Pathpoint currentPoint, Pathpoint startPoint, float characterHeight, BoxCollider acceptableCharacterEndPlacement)
    {
        List<Pathpoint> pathWithPoints = new List<Pathpoint>();
        while (currentPoint != startPoint)
        {
            pathWithPoints.Add(currentPoint);
            currentPoint = currentPoint.discoveryPoint;
        }


        if(pathWithPoints.Count == 0)
        {
            if(acceptableCharacterEndPlacement != null)
            {
                return new List<Vector3>() { acceptableCharacterEndPlacement.ClosestPointOnBounds(startPos) };
            }
            else
            {
                return new List<Vector3>() { goalPos };
            }
        }
        //Om avståndet mellan startpositionen och första pathnoden är större än första noden och startpointen
        if (Vector3.Distance(startPos, pathWithPoints[pathWithPoints.Count - 1].transform.position) >
            Vector3.Distance(pathWithPoints[pathWithPoints.Count - 1].transform.position, startPoint.transform.position)
            ||
            HelperMethods.WallBetweenPointsOnGround(pathWithPoints[pathWithPoints.Count - 1].transform.position, startPos, characterHeight))
        {
            pathWithPoints.Add(startPoint);
        }
        pathWithPoints.Reverse();

        Vector3 distToGoalPos = goalPos - startPos;

        if (Vector3.Distance(startPos, goalPos) < Vector3.Distance(startPos, pathWithPoints[0].transform.position) && distToGoalPos.y < acceptablePathCutYAxisValue)
        {
            if (!HelperMethods.WallBetweenPointsOnGround(startPos, goalPos, characterHeight))
            {
                if (acceptableCharacterEndPlacement != null)
                {
                    return new List<Vector3>() { acceptableCharacterEndPlacement.ClosestPointOnBounds(startPos) };
                }
                else
                {
                    return new List<Vector3>() { goalPos };
                }
            }
        }
        return FixPath(pathWithPoints, startPos, goalPos, characterHeight, acceptableCharacterEndPlacement);
    }

    static List<Vector3> FixPath(List<Pathpoint> pathToFix, Vector3 startPos,Vector3 goalPos, float characterHeight, BoxCollider acceptableCharacterEndPlacement)
    {
        List<Vector3> result = new List<Vector3>();
        for (int i = 0; i < pathToFix.Count; i++)
        {
            if (pathToFix[i].isLadder)
            {
                if(i == 0 || i == pathToFix.Count-1 || 
                    !pathToFix[i - 1].isLadder || !pathToFix[i + 1].isLadder ||
                    !HelperMethods.IsCoordinatesOnAVerticalLine(pathToFix[i - 1].transform.position, pathToFix[i].transform.position) ||
                    !HelperMethods.IsCoordinatesOnAVerticalLine(pathToFix[i].transform.position, pathToFix[i + 1].transform.position))
                {
                    result.Add(HelperMethods.ConvertPosToBeOnGround(pathToFix[i].transform.position, characterHeight));
                }
            }
            else
            {
                result.Add(HelperMethods.ConvertPosToBeOnGround(pathToFix[i].transform.position, characterHeight));
            }
        }
        result.Add(goalPos);
        if(HelperMethods.WallBetweenPointsOnGround(startPos, result[0], characterHeight))
        {
            result.InsertRange(0, FixPathBetweenPoints(startPos, result[0], characterHeight));
        }
        if(result.Count > 1)
        {
            if (acceptableCharacterEndPlacement != null)
            {
                result[result.Count - 1] = acceptableCharacterEndPlacement.ClosestPoint(result[result.Count - 2]);
            }
            if (HelperMethods.WallBetweenPointsOnGround(result[result.Count - 2], result[result.Count - 1], characterHeight))
            {
                result.InsertRange(result.Count - 1, FixPathBetweenPoints(result[result.Count - 2], result[result.Count - 1], characterHeight));
                if (acceptableCharacterEndPlacement != null)
                {
                    result[result.Count - 1] = acceptableCharacterEndPlacement.ClosestPoint(result[result.Count - 2]);
                }
            }

            for (int i = 0; i < result.Count-1; i++)
            {
                if (!HelperMethods.WallBetweenPointsOnGround(result[i + 1], startPos, characterHeight))
                {
                    result.RemoveAt(i);
                    i--;
                }
                else
                {
                    break;
                }
            }
            for (int i = result.Count-2; i > 1; i--)
            {
                if (!HelperMethods.WallBetweenPointsOnGround(result[i - 1], goalPos, characterHeight))
                {
                    result.RemoveAt(i);
                    i--;
                }
                else
                {
                    break;
                }
            }
        }
        return result;
    }

    static List<Vector3> FixPathBetweenPoints(Vector3 p1, Vector3 p2, float height)
    {
        List<Vector3> result = new List<Vector3>();
        Vector3 t = p1;
        t.z = zMoveValue;
        t = HelperMethods.ConvertPosToBeOnGround(t, height);
        result.Add(t);

        t = p2;
        t.z = zMoveValue;
        t = HelperMethods.ConvertPosToBeOnGround(t, height);
        result.Add(t);
        return result;
    }

    static void UpdatePointValues(Pathpoint pointToUpdate, Pathpoint parentPoint, Vector3 endPos)
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
            if (closedList.Contains(pointToUpdate))
            {
                UpdateAllChildPoints(pointToUpdate, endPos);
            }
        }
    }

    static void UpdateAllChildPoints(Pathpoint point, Vector3 endPos)
    {
        for (int i = 0; i < point.connections.Length; i++)
        {
            UpdatePointValues(point.connections[i], point, endPos);
        }
    }

    static Pathpoint FindClosestPoint(Vector3 currentPos, float characterHeight)
    {
        Pathpoint closestPoint = allPoints[0];
        for (int i = 1; i < allPoints.Length; i++)
        {
            if (!allPoints[i].isLocked)
            {
                if (!HelperMethods.WallBetweenPointsOnGround(allPoints[i].transform.position, currentPos, characterHeight))
                {
                    if(HelperMethods.WallBetweenPointsOnGround(closestPoint.transform.position, currentPos, characterHeight))
                    {
                        closestPoint = allPoints[i];
                    }
                    else
                    {
                        if (Vector3.Distance(closestPoint.transform.position, currentPos) > Vector3.Distance(allPoints[i].transform.position, currentPos))
                        {
                            closestPoint = allPoints[i];
                        }
                    }
                }
            }
        }
        return closestPoint;
    }

    static Pathpoint PointWithBestEstimation()
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








    /*static Pathpoint[] allPoints;
    static List<Pathpoint> openList = new List<Pathpoint>();
    static List<Pathpoint> closedList = new List<Pathpoint>();


    //hur stor skillnaden kan vara i y-led för att karaktären ska kunna
    //skippa pathpointsen och ta sig direkt till målet
    public static float acceptablePathCutYAxisValue = 1;
    public static float zMoveValue = 0.5f;

    private void Start()
    {
        allPoints = FindObjectsOfType(typeof(Pathpoint)) as Pathpoint[];
    }

    public static List<Vector3> FindPath(Vector3 startPos, Vector3 goalPos)
    {
        if (Mathf.Abs(startPos.y - goalPos.y) < acceptablePathCutYAxisValue)
        {
            TextLog.AddLog("Pathing on the same floor");
            return new List<Vector3>() { goalPos };
        }
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

    static List<Vector3> MakePath(Vector3 startPos, Vector3 goalPos, Pathpoint currentPoint, Pathpoint startPoint)
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

    static void UpdatePointValues(Pathpoint pointToUpdate, Pathpoint parentPoint, Vector3 endPos)
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

    static void UpdateAllChildPoints(Pathpoint point, Vector3 endPos)
    {
        for (int i = 0; i < point.connections.Length; i++)
        {
            UpdatePointValues(point.connections[i], point, endPos);
        }
    }

    static Vector3 startPos = Vector3.zero;
    static public Vector3 endPos = Vector3.zero;

    static Pathpoint FindClosestPoint(Vector3 currentPos)
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

    static Pathpoint PointWithBestEstimation()
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
    }*/
    }
