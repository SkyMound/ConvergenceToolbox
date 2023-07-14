using DS.Game.Luna;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

namespace Tools
{
    public class Gizmos : MonoBehaviour
    {
        // See : https://forum.unity.com/threads/cant-set-color-for-linerenderer-always-comes-out-as-magenta-or-black.968447/
        readonly string path = @"C:\Windows\Temp\CTB_Debug.txt";
        readonly Color cutsceneColor = Color.blue;
        readonly Color checkpointColor = Color.white;
        readonly Color roomDoorColor = Color.green;
        readonly Color battleColor = Color.red;

        Battle_Server[] battleServers;
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
                    
                lineRenderers = new List<LineRenderer>();
                battleServers = FindObjectsOfType<Battle_Server>(); 

                foreach(Battle_Server battle in battleServers)
                {
                    lineRenderers.Add(CreateLineRenderer(battle.gameObject, battle.NetworkBattle.GetTriggerBounds(), battleColor));
                }


                hitboxes = FindObjectsOfType<BoxCollider2D>();
                using (StreamWriter sw = File.AppendText(path))
                {
                    for (int i = 0; i < hitboxes.Length; i++)
                    {
                        BoxCollider2D hitbox = hitboxes[i];
                        if (hitbox.gameObject.name.IndexOf("checkpoint", StringComparison.OrdinalIgnoreCase) >= 0)
                        {

                            lineRenderers.Add(CreateLineRenderer(hitbox.gameObject,hitbox.bounds, checkpointColor));

                        }
                        else if (hitbox.gameObject.name.IndexOf("cutscene", StringComparison.OrdinalIgnoreCase) >= 0)
                        {

                            lineRenderers.Add(CreateLineRenderer(hitbox.gameObject, hitbox.bounds, cutsceneColor));

                        }else if(hitbox.gameObject.name.IndexOf("roomdoor", StringComparison.OrdinalIgnoreCase) >= 0)
                        {

                            lineRenderers.Add(CreateLineRenderer(hitbox.gameObject, hitbox.bounds, roomDoorColor));
                            
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

        bool RetrieveShader(){

            MeshRenderer[] mesh = FindObjectsOfType<MeshRenderer>();
            using (StreamWriter sw = File.AppendText(path))
            {
                for(int i = 0; i < mesh.Length; i++)
                {
                    // if (mesh[i].material.shader.ToString().IndexOf("Halftones_Unlit", StringComparison.OrdinalIgnoreCase) >= 0) // Cant change his color
                    if (mesh[i].material.shader.ToString().IndexOf("Flat", StringComparison.OrdinalIgnoreCase) >= 0 || mesh[i].material.shader.ToString().IndexOf("Color", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        shader = mesh[i].material.shader;
                        sw.WriteLine("Found : "+shader.name);
                        
                    }
                }
            }

            return shader != null;
        }

        void Start()
        {
            try
            {
                RetrieveShader();
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

        LineRenderer CreateLineRenderer(GameObject boundsContainer, Bounds bounds, Color color){
            LineRenderer lr = boundsContainer.AddComponent<LineRenderer>();
            lr.sortingOrder = 32000;
            lr.alignment = LineAlignment.View;
            lr.loop = true;

            lr.startWidth = 0.08f;
            lr.endWidth = 0.08f;
            lr.numCornerVertices = 0;
            Vector3[] positions = new Vector3[4];
            lr.positionCount = positions.Length;
            positions[0] = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, 0);
            positions[1] = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, 0);
            positions[2] = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, 0);
            positions[3] = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, 0);

            lr.SetPositions(positions);
            lr.material.shader = shader;
            lr.material.color = color;

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
