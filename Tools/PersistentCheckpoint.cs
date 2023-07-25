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
        public string persistentName;
        public string pathToFolder;
        public bool autoloadEnabled;
        //NetworkPlayerSync playerSync; // Pause Menu


        bool IsPersistentSet(){
            return GetSaves().Contains("Current");
        }

        void SetCurrentToPersistent()
        {
            File.Copy(
                Path.Combine(ToolsManager.Instance.SteamSavesFolder,UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Root.Key),
                Path.Combine(ToolsManager.Instance.SavesFolder,"Current")
            );
            // persistentHero = NetworkHeroManager.Instance.NetworkHero.ServerHero;
            // persistentCheckpoint.DoorNode = UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Data.RespawnDoorNode;
            // persistent = UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile;
        }

        IEnumerator LoadPersistent()
        {
            if(!IsPersistentSet())
                return false;

            File.Copy(
                Path.Combine(ToolsManager.Instance.SavesFolder,"Current"),
                Path.Combine(ToolsManager.Instance.SteamSavesFolder,UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Root.Key)
            );
            
            Debugger.Log(UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Data.SecondsPlayed);
            UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Load();
            yield return UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.WaitUntilLoaded;
            Debugger.Log(UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Data.SecondsPlayed);

            ServiceLocator.Instance.GetService<ServerLevelFlowScope>().OnLevelFailed(LevelFailureType.KnockedOut);

            // Copy saved file to steam folder
            // UpdraftGame.Instance.SaveProfileManager.LoadSaveData() ou UpdraftGame.Instance.SaveProfileManager.InitSaveDataViews()
            // assign currentsaveprofile to GetlatestSaveProfile()
            // restart game

            // Copy persistent file to steam folder en utilisant UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Root.Key
            // UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Load();
            // restart game


            // SaveSystem
            // Deep copy into currentsaveprofile or UpdraftGame.Instance.SaveProfileManager.Save(...)
            // https://stackoverflow.com/questions/129389/how-do-you-do-a-deep-copy-of-an-object-in-net
            // restart game

            //yield return UpdraftGame.Instance.SaveProfileManager.LoadSaveData();
            // using (var stream = File.Open(saveFile, FileMode.Open))
            // {
            //     using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
            //     {
            //         UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Data.ReadFromSave(reader);
            //     }
            // }
            //UpdraftGame.Instance.SaveProfileManager.Save(persistentHero, persistentCheckpoint.DoorNode);
            
        }

        void SetSaveToPersistent(string name){
            if(!GetSaves().Contains(name))
                return;

            File.Copy(
                Path.Combine(ToolsManager.Instance.SavesFolder,name),
                Path.Combine(ToolsManager.Instance.SavesFolder,"Current")
            );
        }

        bool SavePersistent(string name){
            if(!IsPersistentSet())
                return false;
            File.Copy(
                Path.Combine(ToolsManager.Instance.SavesFolder,"Current"),
                Path.Combine(ToolsManager.Instance.SavesFolder,name)
            );
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