using DS.Game.Luna;
using DS.Tech.Util;
using DS.Game.Updraft;
using System.IO;
using System.Collections;
using UnityEngine;
using System;
using System.Linq

namespace Tools
{
    public class PersistentCheckpoint : MonoBehaviour
    {
        readonly string path = @"C:\Windows\Temp\CTB_Debug.txt";

        public UpdraftRoomDoor persistentCheckpoint;
        public string pathToFolder;
        public bool autoloadEnabled;

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
        }

        void SetSaveToPersistent(string name){

        }

        bool SavePersistent(string name){
            if(!IsPersistentSet())
                return false;

            if(GetSaves().Contains(name))
                return false;
        }

        string[] GetSaves(){
            return Directory.GetFiles(pathToFolder);
        }

        // Use this for initialization
        void Start()
        {
            autoloadEnabled = false;
            pathToFolder = Directory.GetCurrentDirectory();
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(pathToFolder);
                GetSaves().ToString();
            }

            persistentCheckpoint = new UpdraftRoomDoor();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Hit p");
                }
                try
                {
                    StartCoroutine(SBNetworkManager.Instance.Server_StartLevel(false));
                    
                }catch(Exception ex)
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}