using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Newtonsoft.Json;
using System.IO;
using System;
using DS.Game.Luna;
using DS.Tech.App;

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
            SBNetworkManager.Instance.Server_HeroesSpawned += this.RetrieveServers;

            currentRoute = new Route(gameObject,"Standard route", Color.cyan, "me");
            /*
            try
            {

                Route r = new Route("Standard route", Color.cyan, "me");
                //SaveRouteToJson(r);
            }catch(Exception ex)
            {
                Debugger.Log(ex.Message);
            }
            */
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

    public class Route
    {

        public string name;
        public Color color;
        public string author;
        List<Segment> segments;
        int segmentIndex;
        LineRenderer lr;

        public Route(GameObject routeHolder, string name, Color color, string author)
        {
            try
            {

                lr = routeHolder.AddComponent<LineRenderer>();
                lr.sortingOrder = 32000;
                lr.alignment = LineAlignment.View;
                lr.startWidth = 0.07f;
                lr.endWidth = 0.07f;
                lr.material.shader = ToolsManager.Instance.shader;
                lr.material.color = color;
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

        public Segment GetCurrentSegment()
        {
            if (segmentIndex >= segments.Count)
                segments.Add(new Segment(RefreshRoute));
            return segments[segmentIndex];
        }

        public void NextSegment()
        {
            segmentIndex++;
            if (segmentIndex == segments.Count)
                segments.Add(new Segment(RefreshRoute));

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
            lr.positionCount = GetCurrentSegment().Count;
            lr.SetPositions(GetCurrentSegment().ToArray());
        }

    }

    public class Segment : List<Vector3>
    {
        public string name;
        int pointIndex;
        readonly Action refresh;
        GameObject sphere;

        public Segment(Action RefreshRoute) : base()
        {
            pointIndex = 0;
            refresh = RefreshRoute;
        }

        public void AddPoint(Vector3 coord)
        {
            Insert(pointIndex, coord);
            NextPoint();
            refresh();
        }

        public void RemovePoint()
        {
            LastPoint();
            RemoveAt(pointIndex);
            refresh();
        }

        public void NextPoint()
        {
            if(pointIndex<Count)
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

                sphere.transform.position = this[pointIndex-1];
                Debugger.Log("Sphere to " + this[pointIndex - 1].ToString());
            }catch(Exception ex)
            {
                Debugger.Log(ex.Message);
            }
        }
    }   
}