using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using DS.Game.Luna;
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
        int selected = 0;
        int newSelected = 0;
        string[] files;
        public static GameObject routeHolder;

        void OnGUI()
        {
            try
            {
                if (ToolsManager.Instance.uiEnabled)
                {
                    GUIStyle style = new GUIStyle(GUI.skin.button);
                    GUILayout.BeginArea(new Rect(Screen.width - 120, 0, 120, Screen.height ));
                    newSelected = GUILayout.SelectionGrid(selected, files, 1,style);
                    GUILayout.Label(currentRoute.GetCurrentSegment().name);
                    GUILayout.EndArea();
                    if(selected != newSelected)
                    {
                        if (newSelected == files.Length - 1)
                        {
                            int i = -1;
                            string filename;
                            do
                            {
                                i++;
                                filename = "myRoute_" + i.ToString() + ".json";
                            } while (File.Exists(Path.Combine(ToolsManager.Instance.RoutesFolder, filename)));
                            currentRoute = new Route("myRoute_" + i.ToString(), Color.cyan, "community");
                            StreamWriter sr = File.CreateText(Path.Combine(ToolsManager.Instance.RoutesFolder, filename));
                            sr.Close();
                            UpdateSavedRoutes();
                            for (int j = 0; j < files.Length; j++)
                            {
                                if (files[j].Equals("myRoute_" + i.ToString()))
                                {
                                    newSelected = j;
                                }
                            }
                        }
                        else
                        {
                            currentRoute = Route.LoadFromJson(files[newSelected]);
                        }
                        currentRoute.RefreshRoute();
                        selected = newSelected;
                    }
                }
            }catch(Exception ex)
            {
                Debugger.Log("On GUI : "+ex.Message);
            }
        }

        void OnEnable()
        {
            routeHolder.SetActive(true);
            UpdateSavedRoutes();
        }

        void OnDisable()
        {
            routeHolder.SetActive(false);
        }

        public void RetrieveServers()
        {
            try
            {
                this.hero = FindObjectOfType<Hero_Server>();
                currentRoute.segmentIndex = currentRoute.GetNearestSegment();
                currentRoute.RefreshRoute();
            }
            catch(Exception ex)
            {
                Debugger.Log("Retrieve servers " + ex.Message);
            }
        }

        void UpdateSavedRoutes()
        {
            string[] rawFiles = Directory.GetFiles(ToolsManager.Instance.RoutesFolder);
            files = new string[rawFiles.Length+1];
            for (int i = 0; i < rawFiles.Length; i++)
            {
                int fileNameStartIndex = rawFiles[i].LastIndexOf('\\') + 1;
                files[i] = rawFiles[i].Substring(fileNameStartIndex, rawFiles[i].LastIndexOf('.') - fileNameStartIndex);
            }
            files[rawFiles.Length] = "+";
        }
        

        // Use this for initialization
        void Start()
        {
            routeHolder = new GameObject("RouteHolder");
            routeHolder.transform.SetParent(gameObject.transform);
            UpdateSavedRoutes();
            currentRoute = Route.LoadFromJson(files[newSelected]);
            currentRoute.RefreshRoute();
            RetrieveServers();
            SBNetworkManager.Instance.Server_HeroesSpawned += this.RetrieveServers;
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
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
            {
                try
                {
                    currentRoute.SaveToJson();
                }catch(Exception ex)
                {
                    Debugger.Log(ex.Message);
                }
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
        public int segmentIndex;
        LineRenderer lr;

        public Route(string name, Color color, string author)
        {
            segmentIndex = 0;
            segments = new List<Segment>();
            this.name = name;
            this.color = color;
            this.author = author;
            SetupLineRenderer();
        }

        public void SetupLineRenderer()
        {
            try
            {
                lr = RouteManager.routeHolder.GetComponent<LineRenderer>();
                if (lr == null)
                {
                    lr = RouteManager.routeHolder.AddComponent<LineRenderer>();
                }
                lr.sortingOrder = 32000;
                lr.alignment = LineAlignment.View;
                lr.startWidth = 0.07f;
                lr.endWidth = 0.07f;
                lr.material.shader = ToolsManager.Instance.shader;
                lr.material.color = color;
            }catch(Exception ex)
            {
                Debugger.Log(ex.Message);
            }
        }

        public void SaveToJson()
        {
            try
            {
                string filePath = Path.Combine(ToolsManager.Instance.RoutesFolder, this.name + ".json");
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    var ser = new DataContractJsonSerializer(typeof(Route));
                    ser.WriteObject(fs, this);
                }
            }catch(Exception ex)
            {
                Debugger.Log("Saving json :" + ex.Message);
            }
        }
        
        public static Route LoadFromJson(string name)
        {
            try
            {
                string filePath = Path.Combine(ToolsManager.Instance.RoutesFolder, name + ".json");
                if (File.Exists(filePath))
                {
                    Route importedRoute;
                    using (FileStream fs = new FileStream(filePath, FileMode.Open))
                    {
                        var ser = new DataContractJsonSerializer(typeof(Route));
                        importedRoute = (Route)ser.ReadObject(fs);
                    }
                    importedRoute.SetupLineRenderer();
                    importedRoute.segmentIndex = 0;
                    importedRoute.segments.ForEach(e =>
                    {
                        e.refresh = importedRoute.RefreshRoute;
                    });
                    importedRoute.segmentIndex = importedRoute.GetNearestSegment();
                    return importedRoute;
                }
                else
                {
                    return new Route(name, Color.cyan, "community");
                }
            }catch(Exception ex)
            {
                Debugger.Log("Failed to load route : "+ex.Message);
                return new Route(name, Color.cyan, "community");
            }
            
        }

        public int GetNearestSegment()
        {
            try
            {
                int upDistance = 0;
                int upIndex = segmentIndex;
                bool upFound = false;
                int downDistance = 0;
                int downIndex = segmentIndex;
                bool downFound = false;

                if (segments.Count <= 1)
                    return segmentIndex;

                for (int i = segmentIndex; i < segments.Count; i++)
                {
                    if(SceneManager.GetActiveScene().name.Equals(segments[i].name))
                    {
                        upDistance = Math.Abs(i-segmentIndex);
                        upIndex = i;
                        upFound = true;
                        break;
                    }
                }
                for(int i = segmentIndex; i >= 0; i--)
                {
                    if (SceneManager.GetActiveScene().name.Equals(segments[i].name))
                    {
                        downDistance = Math.Abs(segmentIndex - i);
                        downIndex = i;
                        downFound = true;
                        break;
                    }
                }

                if (upFound && !downFound)
                    return upIndex;
                else if (downFound && !upFound)
                    return downIndex;
                else if(downFound && upFound)
                    if(upDistance <= downDistance)
                        return upIndex;
                    else
                        return downIndex;
                return segmentIndex;

            }catch(Exception ex)
            {
                Debugger.Log(ex.Message);
                return segmentIndex;

            }
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
        int pointIndex = -1;
        public Action refresh { get; set; }
        static GameObject sphere;


        public Segment(string name, Action RefreshRoute) 
        {
            this.name = name;
            pointIndex = -1;
            refresh = RefreshRoute;
            points = new List<Vector2>();
        }

        public void AddPoint(Vector2 coord)
        {
            points.Insert(pointIndex+1, coord);
            NextPoint();
            refresh();
        }

        public void RemovePoint()
        {
            points.RemoveAt(pointIndex);
            LastPoint();
            refresh();
        }

        public void NextPoint()
        {
            if(pointIndex< points.Count-1)
                pointIndex++;

            RefreshPoint();
        }

        public void LastPoint()
        {
            if (pointIndex >= 0)
                pointIndex--;

            RefreshPoint();
        }

        void RefreshPoint()
        {
            
            try
            {
                if (points.Count == 0 || pointIndex == -1)
                {
                    GameObject.Destroy(sphere);
                    return;
                }
                if (sphere == null)
                {

                    sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.localScale = new Vector3(.7f, .7f, .7f);
                    sphere.GetComponent<MeshRenderer>().material.shader = ToolsManager.Instance.shader;
                    sphere.GetComponent<MeshRenderer>().material.color = Color.blue;
                }

                sphere.transform.position = points[pointIndex];
            }catch(Exception ex)
            {
                Debugger.Log(ex.Message);
            }
        }
    }   
}