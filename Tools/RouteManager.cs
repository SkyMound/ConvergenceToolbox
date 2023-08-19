using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Route r = new Route("Standard route", Color.cyan, "me");
        }

        // Update is called once per frame
        void Update()
        {

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