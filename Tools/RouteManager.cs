using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public class RouteManager : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public class Route
    {
        List<Segment> segments;
    }

    public class Segment
    {
        List<Vector2> points;
    }
}