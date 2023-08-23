using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;

namespace Tools
{
    public class RouteManager : MonoBehaviour
    {

        Route currentRoute;
        public bool isEnabled;
        public bool isActive;
        
        // Use this for initialization
        void Start()
        {
            try
            {

                Route r = new Route("Standard route", Color.cyan, "me");
                SaveRouteToJson(r);
            }catch(Exception ex)
            {
                Debugger.Log(ex.Message);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

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
    }

    public class Route
    {
        public string name;
        public Color color;
        public string author;
        List<Segment> segments;

        public Route(string name, Color color, string author)
        {
            this.name = name;
            this.color = color;
            this.author = author;
        }
    }

    public class Segment : List<Vector2>
    {
        public string name;
    }   
}