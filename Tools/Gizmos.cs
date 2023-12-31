﻿using DS.Game.Luna;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Tools
{
    public class Gizmos : MonoBehaviour
    {
        readonly Color cutsceneColor = Color.blue;
        readonly Color checkpointColor = Color.white;
        readonly Color roomDoorColor = Color.green;
        readonly Color battleColor = Color.red;

        Battle_Server[] battleServers;
        BoxCollider2D[] hitboxes;
        List<LineRenderer> lineRenderers;        

        public bool isEnabled;
        public bool isActive;
        public bool hasCreatedLine;


        void Start()
        {
            lineRenderers = new List<LineRenderer>();
            hasCreatedLine = false;
            try
            {
                SBNetworkManager.Instance.Server_HeroesSpawned += this.SetupHitbox;

            }catch(Exception ex)
            {
                Debugger.Log("Error at Gizmos constructor : " + ex.ToString());
            } 
        }


        void SetupHitbox()
        {
            hasCreatedLine = false;
            try
            {
                if (!isEnabled || ToolsManager.Instance.shader == null)
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
                Debugger.Log("Error: " + ex.ToString());
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
            lr.material.shader = ToolsManager.Instance.shader;
            lr.material.color = color;

            return lr;
        }

        public void UpdateGizmos()
        {
            Debugger.Log(isEnabled ? "Show Gizmos" : "Hide Gizmos");

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
                Debugger.Log("Can't display !");
            }
        }
    }
}
