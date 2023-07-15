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
        public bool hasCreatedLine;

        void SetupHitbox()
        {
            hasCreatedLine = false;
            try
            {
                if (shader == null)
                    if(!RetrieveShader())
                        return;

                if (!isEnabled)
                    return;

                lineRenderers = new List<LineRenderer>();
                battleServers = FindObjectsOfType<Battle_Server>();
                hitboxes = FindObjectsOfType<BoxCollider2D>();

                foreach (Battle_Server battle in battleServers)
                {
                    lineRenderers.Add(CreateLineRenderer(battle.gameObject, battle.NetworkBattle.GetTriggerBounds(), battleColor));
                }

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
                hasCreatedLine = true;
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
                    sw.WriteLine(mesh[i].material.shader.name);

                    if (mesh[i].material.shader.ToString().IndexOf("Flat", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        mesh[i].material.shader.ToString().IndexOf("Color", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        mesh[i].material.shader.ToString().IndexOf("Sprites/Default", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        shader = mesh[i].material.shader;
                        sw.WriteLine("Found : "+shader.name);
                        return true;
                    }
                }
            }
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine("No shader found !");
            }
            return false;
        }

        void Start()
        {
            lineRenderers = new List<LineRenderer>();
            hasCreatedLine = false;
            try
            {
                RetrieveShader();
                SBNetworkManager.Instance.Server_HeroesSpawned += this.SetupHitbox;


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
                    sw.WriteLine("Error at Gizmos start : " + ex.ToString());
                }
            }
            
            
        }

        LineRenderer CreateLineRenderer(GameObject boundsContainer, Bounds bounds, Color color){
            LineRenderer lr = boundsContainer.AddComponent<LineRenderer>();
            lr.sortingOrder = 32000;
            lr.alignment = LineAlignment.View;
            lr.loop = true;

            lr.startWidth = 0.07f;
            lr.endWidth = 0.07f;
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
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(isEnabled ? "Show Gizmos" : "Hide Gizmos"); // Debug
            }

            isActive = isEnabled;

            if (isEnabled && !hasCreatedLine)
                SetupHitbox();

            if (lineRenderers.Count != 0)
            {
                foreach (LineRenderer line in lineRenderers)
                {
                    line.enabled = isEnabled;
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Can't display !"); // Debug
                }
            }
        }


        

    }
}
