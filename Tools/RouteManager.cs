using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Newtonsoft.Json;
using System.IO;
using System;
using DS.Game.Luna;

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
                currentRoute.NextPoint();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentRoute.LastPoint();
            }
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                currentRoute.AddPoint(hero.transform.position + new Vector3(0,1,0));
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {

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
            lr = routeHolder.AddComponent<LineRenderer>();
            lr.sortingOrder = 32000;
            lr.alignment = LineAlignment.View;
            lr.startWidth = 0.07f;
            lr.endWidth = 0.07f;
            segmentIndex = 0;
            segments = new List<Segment>();
            this.name = name;
            this.color = color;
            this.author = author;
        }

        public Segment GetCurrentSegment()
        {
            if (segmentIndex >= segments.Count)
            {
                Segment newSeg = new Segment(lr);
                segments.Add(newSeg);
                return newSeg;
            }
            else
            {
                return segments[segmentIndex];
            }
        }

        public void NextSegment()
        {
            segmentIndex++;
            if (segmentIndex == segments.Count)
            { 
                Segment newSeg = new Segment(lr);
                segments.Add(newSeg);
            }
        }

        public void LastSegment()
        {
            if (segmentIndex > 0)
                segmentIndex--;
            lr.positionCount = segments[segmentIndex].Count;
        }


    }

    public class Segment : List<Vector3>
    {
        public string name;
        int pointIndex;
        LineRenderer lr;

        public Segment(LineRenderer lineRenderer) : base()
        {
            pointIndex = 0;
            lr = lineRenderer;
        }

        public void AddPoint(Vector3 coord)
        {
            Insert(pointIndex, coord);
            pointIndex++;
            lr.positionCount = Count;
            lr.SetPositions(ToArray());
        }

        public void NextPoint()
        {
            if(pointIndex<Count)
                pointIndex++;
        }

        public void LastPoint()
        {
            if (pointIndex > 0)
                pointIndex--;
        }
    }   
}