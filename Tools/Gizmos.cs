using DS.Game.Luna;
using System;
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
        List<LineRenderer> lineRenderers;
        string path = @"C:\Windows\Temp\CTB_Debug.txt";

        public bool isEnabled;
        public bool isActive;

        void Start()
        {
            lineRenderers = new List<LineRenderer>();
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("ConvergenceToolbox Debugger");
                }
            }
            SBNetworkManager.Instance.Server_HeroesSpawned += this.SetupHitbox;
            
        }

        public void UpdateGizmos()
        {
            try
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(isEnabled ? "Gizmos show" : "Gizmos hide") ;
                }
                isActive = isEnabled;

                if (lineRenderers.Count != 0)
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine("Showing");
                    }
                    foreach (LineRenderer line in lineRenderers)
                    {
                        line.enabled = isEnabled;
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine("No lines");
                    }
                }
            }catch(Exception ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Error when updating gizmos : " + ex.ToString());
                }
            }
        }


        void SetupHitbox()
        {
            try
            {

                hitboxes = FindObjectsOfType<BoxCollider2D>();
                lineRenderers = new List<LineRenderer>();
                for(int i = 0; i < hitboxes.Length; i++)
                {
                    BoxCollider2D hitbox = hitboxes[i];
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(hitbox.gameObject.name);
                    }
                    if(hitbox.gameObject.name.IndexOf("checkpoint", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.WriteLine("-> Saved");
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
                        lineRenderers.Add(lineRenderer);
                    }

                }
                UpdateGizmos();
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Error: " + ex.ToString());
                }
            }

        }

    }
}
