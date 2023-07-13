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
        readonly string path = @"C:\Windows\Temp\CTB_Debug.txt";
        readonly Color cutsceneColor = Color.blue;
        readonly Color checkpointColor = Color.white;
        readonly Color combatColor = Color.red;


        BoxCollider2D[] hitboxes;
        List<LineRenderer> lineRenderers;
        Shader shader;

        public bool isEnabled;
        public bool isActive;


        void SetupHitbox()
        {
            try
            {
                if(shader == null){
                    if(!RetrieveShader()){
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.WriteLine("No shader found !");
                        }
                        return;
                    }
                }
                    

                hitboxes = FindObjectsOfType<BoxCollider2D>();
                lineRenderers = new List<LineRenderer>();
                for (int i = 0; i < hitboxes.Length; i++)
                {
                    BoxCollider2D hitbox = hitboxes[i];
                    //sw.WriteLine(hitbox.gameObject.name); // Debug

                    if (hitbox.gameObject.name.IndexOf("checkpoint", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        // sw.WriteLine("-> Saved"); // Debug

                        lineRenderers.Add(CreateLineRenderer(hitbox, checkpointColor));

                    }
                    else if (hitbox.gameObject.name.IndexOf("room_sector", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        // sw.WriteLine("-> Saved"); // Debug

                        lineRenderers.Add(CreateLineRenderer(hitbox, combatColor));

                    }
                    else if (hitbox.gameObject.name.IndexOf("scene", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        // sw.WriteLine("-> Saved"); // Debug

                        lineRenderers.Add(CreateLineRenderer(hitbox, cutsceneColor));

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

        bool RetrieveShader(){
            MeshRenderer[] mesh = FindObjectsOfType<MeshRenderer>();
            using (StreamWriter sw = File.AppendText(path))
            {
                for(int i = 0; i < mesh.Length; i++)
                {
                    sw.WriteLine(mesh[i].material.shader.ToString());
                    if (mesh[i].material.shader.ToString().IndexOf("unlit", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        shader = mesh[i].material.shader;
                    }
                }
            }

            return shader != null;
        }

        void Start()
        {
            try
            {
                SBNetworkManager.Instance.Server_HeroesSpawned += this.SetupHitbox;

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
            }catch(Exception ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Error at start : " + ex.ToString());
                }
            }
            
            
        }

        LineRenderer CreateLineRenderer(BoxCollider2D hitbox, Color color){
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
            lr.material.shader = shader;
            lr.material.color = color;
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(lr.material.shader.ToString()); // Debug
            }

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
                    foreach (LineRenderer line in lineRenderers)
                    {
                        line.enabled = isEnabled;

                        //sw.WriteLine("Showing"+ line.ToString()); // Debug
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


        

    }
}
