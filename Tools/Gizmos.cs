using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools
{
    public class Gizmos : MonoBehaviour
    {

        //BoxCollider2D boxCollider2D;
        BoxCollider2D[] hitboxes;
        MeshCollider[] meshes;
        string path = @"D:\enregistrements\convergence\ConvergenceToolbox\CTB_Debug.txt";
        void Start()
        {
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("ConvergenceToolbox Debugger");
                }
            }

            hitboxes = FindObjectsOfType<BoxCollider2D>();
            foreach (BoxCollider2D hitbox in hitboxes)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(hitbox.gameObject.name);
                }
                LineRenderer lineRenderer = hitbox.gameObject.AddComponent<LineRenderer>();
                lineRenderer.sortingOrder = 32000;
                lineRenderer.alignment = LineAlignment.View;
                lineRenderer.loop = true;
                Material material = lineRenderer.material;

                // Set the new color
                material.color = new Color(0, 0, 1);
                lineRenderer.startWidth = 0.08f;
                lineRenderer.endWidth = 0.08f;
                lineRenderer.numCornerVertices = 0;
                Vector3[] positions = new Vector3[4];
                lineRenderer.positionCount = positions.Length;
                positions[0] = hitbox.bounds.center + new Vector3(hitbox.bounds.extents.x, hitbox.bounds.extents.y, 0);
                positions[1] = hitbox.bounds.center + new Vector3(-hitbox.bounds.extents.x, hitbox.bounds.extents.y, 0);
                positions[2] = hitbox.bounds.center + new Vector3(-hitbox.bounds.extents.x, -hitbox.bounds.extents.y, 0);
                positions[3] = hitbox.bounds.center + new Vector3(hitbox.bounds.extents.x, -hitbox.bounds.extents.y, 0);


                lineRenderer.SetPositions(positions);


            }
        }

    }
}
