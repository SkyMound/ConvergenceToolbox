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
        // See : https://forum.unity.com/threads/cant-set-color-for-linerenderer-always-comes-out-as-magenta-or-black.968447/
        BoxCollider2D[] hitboxes;
        List<LineRenderer> lineRenderers;
        Material mat;
        Gradient checkpointGradient;
        Gradient combatGradient;
        Gradient cutsceneGradient;
        string path = @"C:\Windows\Temp\CTB_Debug.txt";

        public bool isEnabled;
        public bool isActive;

        void Start()
        {
            lineRenderers = new List<LineRenderer>();

            // Set up line material and colors
            mat = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));

            float alpha = 1.0f;
            Gradient checkpointGradient = new Gradient();
            checkpointGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
            Gradient combatGradient = new Gradient();
            combatGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
            Gradient cutsceneGradient = new Gradient();
            cutsceneGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.green, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );

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

        LineRenderer CreateLineRenderer(BoxCollider2D hitbox, Gradient gradient){
            LineRenderer lr = hitbox.gameObject.AddComponent<LineRenderer>();
            lr.sortingOrder = 32000;
            lr.alignment = LineAlignment.View;
            lr.loop = true;

            lr.startWidth = 0.08f;
            lr.endWidth = 0.08f;
            lr.numCornerVertices = 0;
            Vector3[] positions = new Vector3[4];
            lr.positionCount = positions.Length;
            positions[0] = hitbox.bounds.center + new Vector3(hitbox.bounds.extents.x, hitbox.bounds.extents.y, 0);
            positions[1] = hitbox.bounds.center + new Vector3(-hitbox.bounds.extents.x, hitbox.bounds.extents.y, 0);
            positions[2] = hitbox.bounds.center + new Vector3(-hitbox.bounds.extents.x, -hitbox.bounds.extents.y, 0);
            positions[3] = hitbox.bounds.center + new Vector3(hitbox.bounds.extents.x, -hitbox.bounds.extents.y, 0);

            lr.SetPositions(positions);

            lr.material = mat;
            lr.colorGradient = gradient;

            return lr;
        }

        public void UpdateGizmos()
        {
            try
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(isEnabled ? "Gizmos show" : "Gizmos hide") ; // Debug
                }
                isActive = isEnabled;

                if (lineRenderers.Count != 0)
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine("Showing"); // Debug
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
                        sw.WriteLine("No lines"); // Debug
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
                using (StreamWriter sw = File.AppendText(path))
                {
                    for(int i = 0; i < hitboxes.Length; i++)
                    {
                        BoxCollider2D hitbox = hitboxes[i];
                        sw.WriteLine(hitbox.gameObject.name); // Debug

                        if(hitbox.gameObject.name.IndexOf("checkpoint", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            sw.WriteLine("-> Saved"); // Debug
                            
                            lineRenderers.Add(CreateLineRenderer(hitbox,checkpointGradient));

                        }else if(hitbox.gameObject.name.IndexOf("room_sector", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            sw.WriteLine("-> Saved"); // Debug
                            
                            lineRenderers.Add(CreateLineRenderer(hitbox,combatGradient));

                        }else if(hitbox.gameObject.name.IndexOf("scene", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            sw.WriteLine("-> Saved"); // Debug
                            
                            lineRenderers.Add(CreateLineRenderer(hitbox,cutsceneGradient));

                        }

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
