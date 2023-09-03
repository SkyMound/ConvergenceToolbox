﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using DS.Game.Luna;
using DS.Tech.App;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using UnityEngine.SceneManagement;

namespace Tools
{


    public class RouteManager : MonoBehaviour
    {
        Hero_Server hero;
        Route currentRoute;
        public bool isEnabled;
        public bool isActive;
        int selected = 0;
        int newSelected = 0;
        string[] files;

        void OnGUI()
        {
            newSelected = GUILayout.SelectionGrid(selected, files, 1);
            if(selected != newSelected)
            {
                currentRoute = Route.LoadFromJson(files[newSelected], gameObject);
                currentRoute.FindNearestSegment();
                selected = newSelected;
            }
        }

        void OnEnable()
        {
            currentRoute = Route.LoadFromJson(files[newSelected], gameObject);
            currentRoute.FindNearestSegment();
        }

        public void RetrieveServers()
        {
            this.hero = FindObjectOfType<Hero_Server>();
            try
            {
                currentRoute.FindNearestSegment();
            }
            catch(Exception ex)
            {
                Debugger.Log(ex.Message);
            }
            
        }

        

        // Use this for initialization
        void Start()
        {
            try
            {

                files = Directory.GetFiles(ToolsManager.Instance.RoutesFolder);
                for(int i = 0; i< files.Length; i++)
                {
                    int fileNameStartIndex = files[i].LastIndexOf('\\')+1;
                    Debugger.Log(files[i]+" - "+fileNameStartIndex.ToString());
                    files[i] = files[i].Substring(fileNameStartIndex, files[i].LastIndexOf('.') - fileNameStartIndex);
                }
                currentRoute = new Route(gameObject,"Standard route", Color.cyan, "me");
                RetrieveServers();
                SBNetworkManager.Instance.Server_HeroesSpawned += this.RetrieveServers;
            }catch(Exception ex)
            {
                Debugger.Log(ex.Message);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentRoute.LastSegment();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentRoute.NextSegment();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentRoute.GetCurrentSegment().NextPoint();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentRoute.GetCurrentSegment().LastPoint();
            }
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                currentRoute.GetCurrentSegment().AddPoint(hero.transform.position + new Vector3(0,1,0));
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                currentRoute.GetCurrentSegment().RemovePoint();
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                currentRoute.SaveToJson();
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                try
                {
                    currentRoute = Route.LoadFromJson("Standard route", gameObject);
                    currentRoute.FindNearestSegment();
                }
                catch (Exception ex)
                {
                    Debugger.Log(ex.Message);
                }
                Debugger.Log("Imported : " + currentRoute.name);
            }
        }
    }

    [DataContract]
    public class Route
    {
        [DataMember]
        public string name;
        [DataMember]
        public Color color;
        [DataMember]
        public string author;
        [DataMember]
        List<Segment> segments;
        int segmentIndex;
        LineRenderer lr;

        public Route(GameObject routeHolder, string name, Color color, string author)
        {
            try
            {
                SetupLineRenderer(routeHolder);
                segmentIndex = 0;
                segments = new List<Segment>();
                this.name = name;
                this.color = color;
                this.author = author;
            }catch(Exception ex)
            {
                Debugger.Log(ex.Message);
            }
        }

        public void SetupLineRenderer(GameObject routeHolder)
        { 
            lr = routeHolder.GetComponent<LineRenderer>();
            if (lr == null)
            {
                lr = routeHolder.AddComponent<LineRenderer>();
            }
            lr.sortingOrder = 32000;
            lr.alignment = LineAlignment.View;
            lr.startWidth = 0.07f;
            lr.endWidth = 0.07f;
            lr.material.shader = ToolsManager.Instance.shader;
            lr.material.color = color;
        }

        public void SaveToJson()
        {
            string filePath = Path.Combine(ToolsManager.Instance.RoutesFolder, this.name + ".json");
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                var ser = new DataContractJsonSerializer(typeof(Route));
                ser.WriteObject(fs, this);
            }
        }
        
        public static Route LoadFromJson(string name, GameObject routeHolder)
        {
            
            string filePath = Path.Combine(ToolsManager.Instance.RoutesFolder, name+".json");
            if (File.Exists(filePath))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    var ser = new DataContractJsonSerializer(typeof(Route));
                    Route importedRoute = (Route)ser.ReadObject(fs);
                    importedRoute.SetupLineRenderer(routeHolder);
                    importedRoute.segmentIndex = 0;
                    importedRoute.segments.ForEach(e =>
                    {
                        e.refresh = importedRoute.RefreshRoute;
                    });

                    return importedRoute;
                    // Now, 'importedRoute' contains the deserialized Route object from the JSON file.
                }
            }
            else
            {
                return new Route(routeHolder, "default", Color.blue, "nobody");
            }
        }

        public void FindNearestSegment()
        {
            int upDistance = 0;
            int upIndex = segmentIndex;
            bool upFound = false;
            int downDistance = 0;
            int downIndex = segmentIndex;
            bool downFound = false;

            for (int i = segmentIndex; i < segments.Count; i++)
            {
                if(SceneManager.GetActiveScene().name.Equals(segments[i].name))
                {
                    upDistance = Math.Abs(i-segmentIndex);
                    upIndex = i;
                    upFound = true; 
                }
            }
            for(int i = segmentIndex; i >= 0; i--)
            {
                if (SceneManager.GetActiveScene().name.Equals(segments[i].name))
                {
                    downDistance = Math.Abs(segmentIndex - i);
                    downIndex = i;
                    downFound = true;

                }
            }

            if (upFound && !downFound)
                segmentIndex = upIndex;
            else if (downFound && !upFound)
                segmentIndex = downIndex;
            else if(downFound && upFound)
                if(upDistance <= downDistance)
                    segmentIndex = upIndex;
                else
                    segmentIndex = downIndex;
            
            RefreshRoute();
        }


        public Segment GetCurrentSegment()
        {
            if (segmentIndex >= segments.Count)
                segments.Add(new Segment(SceneManager.GetActiveScene().name, RefreshRoute));
            return segments[segmentIndex];
        }

        public void NextSegment()
        {
            segmentIndex++;
            if (segmentIndex == segments.Count)
                segments.Add(new Segment(SceneManager.GetActiveScene().name, RefreshRoute));

            RefreshRoute();
        }

        public void LastSegment()
        {
            if (segmentIndex > 0)
                segmentIndex--;

            RefreshRoute();
        }

        public void RefreshRoute()
        {
            List<Vector2> points = GetCurrentSegment().points;
            lr.positionCount = points.Count;
            Vector3[] vector3Array = new Vector3[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                vector3Array[i] = new Vector3(points[i].x, points[i].y, 0);
            }
            lr.SetPositions(vector3Array);
        }

    }

    [DataContract]
    public class Segment
    {
        [DataMember]
        public string name;
        [DataMember]
        public List<Vector2> points;
        int pointIndex;
        public Action refresh { get; set; }
        static GameObject sphere;


        public Segment(string name, Action RefreshRoute) 
        {
            this.name = name;
            pointIndex = 0;
            refresh = RefreshRoute;
            points = new List<Vector2>();
        }

        public void AddPoint(Vector2 coord)
        {
            points.Insert(pointIndex, coord);
            NextPoint();
            refresh();
        }

        public void RemovePoint()
        {
            LastPoint();
            points.RemoveAt(pointIndex);
            refresh();
        }

        public void NextPoint()
        {
            if(pointIndex< points.Count)
                pointIndex++;

            RefreshPoint();
        }

        public void LastPoint()
        {
            if (pointIndex > 0)
                pointIndex--;

            RefreshPoint();
        }

        void RefreshPoint()
        {
            try
            {

                if (sphere == null)
                {

                    sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.localScale = new Vector3(.7f, .7f, .7f);
                    sphere.GetComponent<MeshRenderer>().material.shader = ToolsManager.Instance.shader;
                    sphere.GetComponent<MeshRenderer>().material.color = Color.blue;
                }

                sphere.transform.position = points[pointIndex-1];
            }catch(Exception ex)
            {
                Debugger.Log(ex.Message);
            }
        }
    }   
}