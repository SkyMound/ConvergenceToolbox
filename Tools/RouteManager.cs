using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using DS.Game.Luna;
using DS.Tech.App;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Tools
{


    public class RouteManager : MonoBehaviour
    {
        Hero_Server hero;
        Route currentRoute;
        public bool isEnabled;
        public bool isActive;

        public void RetrieveServers()
        {
            this.hero = FindObjectOfType<Hero_Server>();
        }

        // Use this for initialization
        void Start()
        {
            RetrieveServers();
            SBNetworkManager.Instance.Server_HeroesSpawned += this.RetrieveServers;

            //currentRoute = new Route(gameObject,"Standard route", Color.cyan, "me");
            try
            {
                currentRoute = Route.LoadFromJson("Standard route", gameObject);
                currentRoute.RefreshRoute();
            }
            catch (Exception ex)
            {
                Debugger.Log(ex.Message);
            }
            Debugger.Log("Imported : " + currentRoute.name);

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
                    currentRoute.RefreshRoute();
                }catch(Exception ex)
                {
                    Debugger.Log(ex.Message);
                }
                Debugger.Log("Imported : " + currentRoute.name);
            }
        }
        /*
        public void SaveRouteToJson(Route route)
        {
            string json = JsonConvert.SerializeObject(route, Formatting.Indented);
            string filePath = Path.Combine(ToolsManager.RoutesFolder,route.name);
            File.WriteAllText(filePath, json);
        }

        public Route LoadRouteFromJson(string name)
        {
            string filePath = Path.Combine(ToolsManager.RoutesFolder,name);
            string json = File.ReadAllText(filePath);
            Route route = JsonConvert.DeserializeObject<Route>(json);
            return route;
        }
        */
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
            lr = routeHolder.AddComponent<LineRenderer>();
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
            //string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            //Debugger.Log(json);
            //File.WriteAllText(filePath, json);

        }
        
        public static Route LoadFromJson(string name, GameObject routeHolder)
        {
            
            string filePath = Path.Combine(ToolsManager.Instance.RoutesFolder, name+".json");
            //string json = File.ReadAllText(filePath);
            //Route route = JsonConvert.DeserializeObject<Route>(json);
            //return route;
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
        

        public Segment GetCurrentSegment()
        {
            if (segmentIndex >= segments.Count)
                segments.Add(new Segment(segmentIndex.ToString(),RefreshRoute));
            return segments[segmentIndex];
        }

        public void NextSegment()
        {
            segmentIndex++;
            if (segmentIndex == segments.Count)
                segments.Add(new Segment(segmentIndex.ToString(),RefreshRoute));

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
            lr.positionCount = GetCurrentSegment().points.Count;
            lr.SetPositions(GetCurrentSegment().points.ToArray());
        }

    }

    [DataContract]
    public class Segment
    {
        [DataMember]
        public string name;
        [DataMember]
        public List<Vector3> points;
        int pointIndex;
        public Action refresh { get; set; }
        GameObject sphere;


        public Segment(string name, Action RefreshRoute) 
        {
            this.name = name;
            pointIndex = 0;
            refresh = RefreshRoute;
            points = new List<Vector3>();
        }

        public void AddPoint(Vector3 coord)
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