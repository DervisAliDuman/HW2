using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;
using Random = UnityEngine.Random;

public class readFile : MonoBehaviour
{

    public Mesh objectToCreate1;
    public Mesh objectToCreate2;

    public Material Material1;
    public Material Material2;
    public Material Material3;

    private ArrayList objects1 = new ArrayList();
    private ArrayList objects2 = new ArrayList();

    public int point_count = 0;
    private bool ransaced = false;


    void ReadAndCreateObjects(string path, int objectNum )
    {
        
        
        StreamReader reader = new StreamReader(path);
        string readed = reader.ReadLine();

        if (readed != null)
            point_count = int.Parse(readed);

        double x = 0, y = 0, z = 0;
        Debug.Log(point_count);

        char[] delimiterChars = { ' ' };
        int k = 0;

        for (int i = 1; i < point_count + 1; i++)
        {
            readed = reader.ReadLine();
            var gameObject = new GameObject("OBJECT " + objectNum+ " : " + i);
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();
            if(objectNum == 1)
            {
                meshFilter.sharedMesh = objectToCreate1;
                gameObject.GetComponent<MeshRenderer>().material = Material1;

            }
            else if(objectNum == 2)
            {
                meshFilter.sharedMesh = objectToCreate2;
                gameObject.GetComponent<MeshRenderer>().material = Material2;
                
            }

            string[] points = readed.Split(delimiterChars);
            k = 0;
            foreach (string point in points)
            {
                //Debug.Log(point);
                if (k == 0)
                {
                    x = Convert.ToDouble(point);
                }
                else if (k == 1)
                {
                    y = Convert.ToDouble(point);
                }
                else if (k == 2)
                {
                    z = Convert.ToDouble(point);
                }
                k++;
            }
            Vector3 temp = new Vector3((float)x, (float)y, (float)z);
            gameObject.transform.position += temp;
            gameObject.transform.localScale += new Vector3(-0.75f, -0.75f, -0.75f);

            if (objectNum == 1)
            {
                objects1.Add(gameObject);
            }
            else if (objectNum == 2)
            {
                objects2.Add(gameObject);
            }

        }

        reader.Close();
    }

    Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        //find cross
        Vector3 side1 = b - a;
        Vector3 side2 = c - a;

        return Vector3.Cross(side1, side2).normalized;
    }

    Vector3 GetCross(Vector3 a, Vector3 b, Vector3 c)
    {
        //find cross
        Vector3 side1 = b - a;
        Vector3 side2 = c - a;

        return Vector3.Cross(side1, side2);
    }

    Vector3 GetCrossNormalized(Vector3 a, Vector3 b, Vector3 c)
    {
        //find cross
        Vector3 x = b - a;
        Vector3 y = c - a;
        Vector3 z = Vector3.Cross(x.normalized, y.normalized);
        Vector3 xt = new Vector3(x.x, y.x, z.x);
        Vector3 yt = new Vector3( x.y, y.y, z.y );
        Vector3 zt = new Vector3(x.z, y.z, z.z);

        Matrix4x4 m = new Matrix4x4(x,y,z,x);
        //double[,] array2D = new double[,] {  { x.x, y.x, z.x }, { x.y, y.y, z.y }, { x.z, y.z, z.z } };
        

        return z;
    }

    bool AreAligned(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 e, Vector3 f)
    {
        if (GetNormal(a, b, c).magnitude - GetNormal(d, e, f).magnitude == 0)
        {
            Debug.Log(a + " "+ b + " "+ c + " ");
            Debug.Log(d + " " + e + " " + f + " ");
            return true;
        }
        return false;
    }

    void ransac()
    {
        Vector3 shift = ((GameObject)objects1[0]).transform.position - ((GameObject)objects2[0]).transform.position;

        for (int k = 0; k < 3; k++)
        {

            //Debug.Log(((GameObject)points1[k]).transform.position);
            ((GameObject)objects1[k]).GetComponent<MeshRenderer>().material = Material3;
            ((GameObject)objects2[k]).GetComponent<MeshRenderer>().material = Material3;
        }

        Debug.Log("SHIFTING " + shift);
        for (int k = 0; k < objects2.Count; k++)
        {
            ((GameObject)objects2[k]).transform.position += shift;

        }


    }

    void rotateX()
    {
        if (ransaced)
            for (int k = 0; k < objects2.Count; k++)
            {
                Vector3 pos = ((GameObject)objects2[k]).transform.position;
                Vector3 shift = new Vector3(pos.x   , (float)(pos.y * Math.Cos(Math.PI / 2) - pos.z * Math.Sin(Math.PI / 2))  , (float)(pos.y * Math.Sin(Math.PI / 2) - pos.z * Math.Cos(Math.PI / 2)) ) - pos;
                ((GameObject)objects2[k]).transform.position += shift;
                ((GameObject)objects1[k]).transform.position += shift;

            }
    }

    void rotateY()
    {
        if (ransaced)
            for (int k = 0; k < objects2.Count; k++)
            {
                Vector3 pos = ((GameObject)objects2[k]).transform.position;
                Vector3 shift = new Vector3(  (float)(pos.x * Math.Cos(Math.PI / 2) + pos.z * Math.Sin(Math.PI / 2))    , pos.y   , (float)(-pos.x * Math.Sin(Math.PI / 2) - pos.z * Math.Cos(Math.PI / 2))) - pos;
                ((GameObject)objects2[k]).transform.position += shift;
                ((GameObject)objects1[k]).transform.position += shift;
            }
    }

    void rotateZ()
    {
        if (ransaced)
            for (int k = 0; k < objects2.Count; k++)
            {
                Vector3 pos = ((GameObject)objects2[k]).transform.position;
                Vector3 shift = new Vector3((float)(pos.x * Math.Cos(Math.PI / 2) - pos.y * Math.Sin(Math.PI / 2))   , (float)(pos.x * Math.Sin(Math.PI / 2) - pos.y * Math.Cos(Math.PI / 2)), pos.z) - pos;
                ((GameObject)objects2[k]).transform.position += shift;
                ((GameObject)objects1[k]).transform.position += shift;
            }
    }

    void ScaleUp()
    {
        if (ransaced)
            for (int k = 0; k < objects2.Count; k++)
            {
                Vector3 pos = ((GameObject)objects2[k]).transform.position;
                Vector3 shift = new Vector3(pos.x*2, pos.y * 2, pos.z) - pos;
                ((GameObject)objects2[k]).transform.position += shift;
                ((GameObject)objects1[k]).transform.position += shift;
            }
    }

    void ScaleDown()
    {
        if (ransaced)
            for (int k = 0; k < objects2.Count; k++)
            {
                Vector3 pos = ((GameObject)objects2[k]).transform.position;
                Vector3 shift = new Vector3(pos.x / 2, pos.y / 2, pos.z) - pos;
                ((GameObject)objects2[k]).transform.position += shift;
                ((GameObject)objects1[k]).transform.position += shift;
            }
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z) && !ransaced)
        {
            ransac();
            ransaced = true;
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            rotateX();
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            rotateY();
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            rotateZ();
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            ScaleUp();
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            ScaleDown();
        }


    }

    /*
    void ransac()
    {
        int max_iteration = 999;
        double bestErr = 9999999, thisErr = 999999;

        //ArrayList selected = new ArrayList();
        double treshold = 0.001;
        bool found = false;
        ArrayList points1 = new ArrayList();
        ArrayList points2 = new ArrayList();

        int randInt = 0;
        ArrayList check = new ArrayList();

        Debug.Log("aaaaaaaaa");

        for (int i = 0; i < max_iteration && !found; i++)
        {

            for (int k = 0; k < 3; k++)
            {

                randInt = Random.Range(0, point_count);
                if (check.Contains(randInt))
                {
                    k--;
                }
                else
                {
                    check.Add(randInt);
                    Debug.Log(randInt);
                    points1.Add(objects1[randInt]);
                }

            }
            check.Clear();

            for (int k = 0; k < 3; k++)
            {

                randInt = Random.Range(0, point_count);
                if (check.Contains(randInt))
                {
                    k--;
                }
                else
                {
                    check.Add(randInt);
                    Debug.Log(randInt);
                    points2.Add(objects2[randInt]);
                }

            }
            check.Clear();
            
            if(AreAligned(((GameObject)points1[0]).transform.position, ((GameObject)points1[1]).transform.position, ((GameObject)points1[2]).transform.position, ((GameObject)points2[0]).transform.position, ((GameObject)points2[1]).transform.position, ((GameObject)points2[2]).transform.position))
            {
                found = true;
                Debug.Log("FOUNDED " );
            }
            

            

            if (found)
            {
                for (int k = 0; k < 3; k++)
                {

                    //Debug.Log(((GameObject)points1[k]).transform.position);
                    ((GameObject)points1[k]).GetComponent<MeshRenderer>().material = Material3;
                    ((GameObject)points2[k]).GetComponent<MeshRenderer>().material = Material3;
                }

                //Vector3 shift = GetCross(((GameObject)points1[0]).transform.position, ((GameObject)points1[1]).transform.position, ((GameObject)points1[2]).transform.position) - GetCross(((GameObject)points2[0]).transform.position, ((GameObject)points2[1]).transform.position, ((GameObject)points2[2]).transform.position);
                Vector3 shift = ((GameObject)points1[0]).transform.position - ((GameObject)points2[0]).transform.position;
                Debug.Log("SHIFTING "+ shift);
                for (int k = 0; k < points2.Count; k++)
                {
                    ((GameObject)points2[k]).transform.position += shift;
                    
                }
            }
            

            points1.Clear();
            points2.Clear();
                                                          //for now

        }


    }*/

    /*
    void ransac()
    {
        
        int i = 0, j = 0;
        double bestFit = 99999999999, bestErr = 9999999, thisErr = 999999;
        int max_iteration = 99999;
        Random r = new Random();
        bool found = false;
        point_count = 10; //number                                                      //parameter

        ArrayList selected = new ArrayList();
        double treshold = 0.001;

        ArrayList maybeInliners = new ArrayList();
        ArrayList maybeModel = new ArrayList();
        ArrayList alsoInliners = new ArrayList();

        ArrayList check = new ArrayList();

        for (i = 0; i < max_iteration && !found ; i++)
        {
            //int randInt = r.Next(0, point_count);               //farkli noktalar al

            for(int k = 0; k < 3; k++)
            {

                randInt = r.Next(0, point_count);
                if (check.Contains(randInt))
                {
                    k--;
                }
                else
                {
                    check.Add(randInt);
                    maybeInliners.Add(objects1[randInt]);
                }
                
            }
            check.Clear();
            
            //maybeInliners -> noktalari depola
            //maybeModel -> fit olan noktalari burada depoluyoruz
            //alsoInliners -> suan bos set
            /*
            for (j = 0; j < 3; j++)
            {
                if ( treashold >  matchRate )
                {
                    //add point in alsoInliners
                }
            }

            if ( alsoInliners elemanlari 3 olduysa gir ) //burda noktalari test ediyoruz 
            {
                
                betterModel:= model parameters fitted to all points in maybeInliers and alsoInliers
                thisErr := a measure of how well betterModel fits these points
                

                if (thisErr == 0)
                {
                    found = true;
                }

                if (thisErr < bestErr)
                {

                    //bestFit := betterModel
                    bestErr = thisErr;
                }
            }

            
        }*/
    

    void Start()
    {
        string path1 = "Assets/Resources/txt1.txt";
        string path2 = "Assets/Resources/txt2.txt";
        ReadAndCreateObjects(path1,1 );
        ReadAndCreateObjects(path2, 2);
        //ransac();
    }

  
}