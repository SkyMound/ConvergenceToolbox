using DS.Game.Luna;
using DS.Tech.Util;
using DS.Game.Updraft;
using System.IO;
using System.Collections;
using UnityEngine;
using System;
using System.Linq;

namespace Tools
{
    public class PersistentCheckpoint : MonoBehaviour
    {
        public UpdraftRoomDoor persistentCheckpoint;
        public string pathToFolder;
        public bool autoloadEnabled;
        //NetworkPlayerSync playerSync; // Pause Menu


        bool IsPersistentSet(){
            return persistentCheckpoint.DoorNode != null;
        }

        void SetCurrentToPersistent()
        {
            persistentCheckpoint.DoorNode = ServiceLocator.Instance.GetService<ServerLevelFlowScope>().SpawningDoorNode; // It only save DoorNode, won't really work.
        }

        bool LoadPersistent()
        {
            if(!IsPersistentSet())
                return false;
            return true;
        }

        void SetSaveToPersistent(string name){

        }

        bool SavePersistent(string name){
            if(!IsPersistentSet())
                return false;

            if(GetSaves().Contains(name))
                return false;
            return true;
        }

        string[] GetSaves(){
            return Directory.GetFiles(pathToFolder);
        }

        // Use this for initialization
        void Start()
        {
            autoloadEnabled = false;

            persistentCheckpoint = new UpdraftRoomDoor();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debugger.Log("Hit p");
                try
                {
                    //this.playerSync = FindObjectOfType<NetworkPlayerSync>();
                    //this.playerSync.PartySignal = PartySignal.RestartFromLastCheckpoint;
                    ServiceLocator.Instance.GetService<ServerLevelFlowScope>().OnLevelFailed(LevelFailureType.KnockedOut);

                }
                catch(Exception ex)
                {
                    Debugger.Log(ex.Message);
                }
            }
        }
    }
}