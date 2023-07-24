using DS.Game.Luna;
using DS.Tech.Util;
using DS.Game.Updraft;
using System.IO;
using System.Collections;
using UnityEngine;
using System;
using System.Linq;
using DS.Tech.Saves;
using System.Text;

namespace Tools
{
    public class PersistentCheckpoint : MonoBehaviour
    {
        public SaveRootDataView<UpdraftSaveProfile> persistent;
        public UpdraftRoomDoor persistentCheckpoint;
        public Hero_Server persistentHero;
        public string pathToFolder;
        public bool autoloadEnabled;
        //NetworkPlayerSync playerSync; // Pause Menu


        bool IsPersistentSet(){
            return persistentCheckpoint.DoorNode != null && persistentHero != null;
        }

        void SetCurrentToPersistent()
        {
            //persistentHero = NetworkHeroManager.Instance.NetworkHero.ServerHero;
            //persistentCheckpoint.DoorNode = UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Data.RespawnDoorNode;
            //persistent = UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.
        }

        bool LoadPersistent()
        {
            //if (!IsPersistentSet())
              //  return false;
            string saveFile = @"D:\Steam\userdata\317573976\1276800\remote\Profile_1";

            using (var stream = File.Open(saveFile, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Data.ReadFromSave(reader);
                }
            }
            //UpdraftGame.Instance.SaveProfileManager.Save(persistentHero, persistentCheckpoint.DoorNode);
            ServiceLocator.Instance.GetService<ServerLevelFlowScope>().OnLevelFailed(LevelFailureType.KnockedOut);
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
            return Directory.GetFiles(ToolsManager.Instance.SavesFolder);
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
            try
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    Debugger.Log("Hit p");
                    LoadPersistent();

                }
                else if (Input.GetKeyDown(KeyCode.O))
                {
                    Debugger.Log("Hit o");
                    SetCurrentToPersistent();
                }
            }
            catch (Exception ex)
            {
                Debugger.Log(ex.Message);
            }
            
        }
    }
}