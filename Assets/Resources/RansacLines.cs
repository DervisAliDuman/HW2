using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RansacLines : MonoBehaviour
{
    void Start()
    {
        // get transform child count
        int amount = transform.childCount;
        Debug.Log("amount : " + amount);
        // collect child positions into array
        var points = new Vector3[amount];
        int index = 0;
        foreach (Transform t in transform)
        {
            points[index++] = t.position;
        }


        // maximum distance to the line, to be considered as an inlier point
        float threshold = 0.25f;
        float bestScore = Mathf.Infinity;

        // results array (all the points within threshold distance to line)
        Vector3[] bestInliers = new Vector3[0];
        Vector3 bestPointA = Vector3.zero;
        Vector3 bestPointB = Vector3.zero;

        // how many search iterations we should do
        int iterations = 100;
        for (int i = 0; i < iterations; i++)
        {
            // take 2 points randomly selected from dataset
            int indexA = Random.Range(0, amount);
            int indexB = Random.Range(0, amount);
            var pointA = points[indexA];
            var pointB = points[indexB];

            // reset score and list for this round of iteration
            float currentScore = 0;
            // temporary list for points found in one search
            List<Vector3> currentInliers = new List<Vector3>();

            // loop all points in the dataset
            for (int n = 0; n < amount; n++)
            {
                // take one point form all points
                var p = points[n];
                // get distance to line, NOTE using editor only helper method
                var currentError = HandleUtility.DistancePointToLine(p, pointA, pointB);

                // distance is within threshold, add to current inliers point list
                if (currentError < threshold)
                {
                    currentScore += currentError;
                    currentInliers.Add(p);
                }
                else // outliers
                {
                    currentScore += threshold;
                }
            } // for-all points

            // check score for the best line found
            if (currentScore < bestScore)
            {
                bestScore = currentScore;
                bestInliers = currentInliers.ToArray();
                bestPointA = pointA;
                bestPointB = pointB;
            }
        } // for-iterations


        // show results

        // draw the searched line
        Debug.DrawRay(bestPointA, (bestPointA - bestPointB).normalized * 999, Color.yellow, 99);
        Debug.DrawRay(bestPointA, (bestPointB - bestPointA).normalized * 999, Color.yellow, 99);

        for (int i = 0, length = bestInliers.Length; i < length; i++)
        {
            // draw cross for all points within line
            DrawDebug(bestInliers[i], Color.green, 0.5f);
        }

    } // Start


    void DrawDebug(Vector2 pos, Color color, float scale = 0.05f)
    {
        Debug.DrawRay(pos, Vector2.up * scale, color, 99,true);
        Debug.DrawRay(pos, -Vector2.up * scale, color, 99,true);
        Debug.DrawRay(pos, Vector2.right * scale, color, 99,true);
        Debug.DrawRay(pos, -Vector2.right * scale, color, 99,true);
    }

}