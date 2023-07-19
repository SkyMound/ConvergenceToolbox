using DS.Game.Luna;
using DS.Tech.Util;
using DS.Game.Updraft;
using System.IO;
using System.Collections;
using UnityEngine;

namespace Tools
{
    public class PersistentCheckpoint : MonoBehaviour
    {
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

        void LoadPersistent()
        {
            if(IsPersistentSet()){
                ServiceLocator.Instance.GetService<ServerLevelFlowScope>().OnCheckpointReached(persistentCheckpoint, true);
            }

        }

        void SetSaveToPersistent(string name){

        }

        void SavePersistent(string name){

        }

        string[] GetSaves(){
            return Directory.GetFiles(pathToFolder);
        }

        // Use this for initialization
        void Start()
        {
            autoloadEnabled = false;
            pathToFolder = Directory.GetCurrentDirectory();
            persistentCheckpoint = new UpdraftRoomDoor();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}