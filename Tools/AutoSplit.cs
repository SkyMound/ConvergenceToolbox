using DS.Game.Luna;
using DS.Game.Updraft;
using System;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.IO;
using DS.Tech.UI;
using DS.Game.Items;
using DS.Tech.Util;
using DS.Tech.ProjectSettings;
using DS.Tech.Saves;

namespace Tools
{
    class AutoSplit : MonoBehaviour
    {
        readonly string path = @"C:\Windows\Temp\CTB_Debug.txt";
        readonly string startingDoorNode = "P1L1_TUT_Gameplay_RoomDoor_Checkpoint_GameStart";


        private Socket _clientSocket;
        public bool isEnabled; // If the AutoSplit is enabled
        public bool isActive; // If the AutoSplit is running well
        private bool _isPaused; // If the timer is in pause
        private bool _isRunning; // If the run has begun
        private int _splitNumber;
        private float _refreshFrequence; // The frequence at which we check for splitting

        // Retrieval of the object that interest us to split at middle boss
        Hero_Server hero; // TW, PC, PD, TP
        PlatformerController2D_Server controller2D; // Double jump
        Dodger_Server dodger; // Dash

        UITransitions transitions; // Loading screens
        NetworkPlayerSync playerSync; // Pause Menu
        ItemAmountChanged endOfRunListener;

        public void Start()
        {     
            // This text is added only once to the file.
                // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("ConvergenceToolbox Debugger");
                sw.WriteLine(ToolsLoader.GetVersionCTB());
            }

            _refreshFrequence = 0.3f;
            _splitNumber = 0;
            isEnabled = false;
            _isPaused = false;
            _isRunning = false;
            isActive = false;
            endOfRunListener = new ItemAmountChanged(this.CheckFutureEkko3Defeated);
        }

        public int GetGadgetSlots()
        {
            return UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Data.MergedInventoryStacker.GetAmount(GameConfig.Instance.GadgetSlotItemID);
        }

        public UpdraftWorldZone GetWorldZone()
        {
            try
            {
                return Level.Instance.LevelData.WorldZone;
            }catch
            {
                return UpdraftWorldZone.P0_Unknown;
            }
        }

        public void RetrieveServers()
        {
            this.hero = FindObjectOfType<Hero_Server>();
            this.dodger = FindObjectOfType<Dodger_Server>();
            this.controller2D = FindObjectOfType<PlatformerController2D_Server>();
            this.playerSync = FindObjectOfType<NetworkPlayerSync>();
        }

        public void CheckNewGameIsCreated()
        {
            if (UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Data.RespawnDoorNode.name.Equals(startingDoorNode)){
                if(_isRunning)
                    SendCommand(CommandType.Restart);
                else
                    SendCommand(CommandType.Start);
            }
        }

        public void CheckFutureEkko3Defeated(UnityGuid itemID, int oldAmount, int newAmount)
        {
            if (!itemID.IsValid() || newAmount <= oldAmount)
                return;
            if(_isRunning && itemID == Configuration<AchievementConfig>.Instance.Asset.FutureEkko3DefeatedItemID)
            {
                SendCommand(CommandType.Stop);
            }
            
        }

        public void StartAutoSplit()
        {
            isActive = true;
            _isRunning = false;
            
            try
            {
                // Create a TCP/IP socket
                _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect to livesplit server 
                _clientSocket.Connect("localhost", 16834);


                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Connected to the server.");
                }

                SBNetworkManager.Instance.Server_HeroesSpawned += this.RetrieveServers;
                SBNetworkManager.Instance.Server_HeroesSpawned += this.CheckNewGameIsCreated;
                UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Data.MergedInventoryStacker.ItemAmountChanged += endOfRunListener;

                StartCoroutine(CheckSplits());
                StartCoroutine(CheckPauses());
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Error: " + ex.ToString());
                }
            }
        }

        public void StopAutoSplit()
        {
            isActive = false;
            _isRunning = false;
            try
            {
                // Close the socket
                _clientSocket.Shutdown(SocketShutdown.Both);
                _clientSocket.Close();

                SBNetworkManager.Instance.Server_HeroesSpawned -= this.RetrieveServers;
                SBNetworkManager.Instance.Server_HeroesSpawned -= this.CheckNewGameIsCreated;
                UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Data.MergedInventoryStacker.ItemAmountChanged -= endOfRunListener;

                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Connection closed.");
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Error: " + ex.ToString());
                }
            }
        }

        public void SendCommand(CommandType type)
        {
            try
            {
                // Prepare the message to be sent
                string message = "";
                switch (type)
                {
                    case CommandType.Restart:
                        message = "reset\r\n";
                    case CommandType.Start:
                        message += "starttimer\r\n";
                        _splitNumber = 0;
                        _isRunning = true;
                        break;
                    case CommandType.Split:
                        message = "split\r\n";
                        _splitNumber++;
                        break;
                    case CommandType.Stop:
                        int splitsSkipped = 19 - _splitNumber;
                        for(int sk = 0; sk < splitsSkipped; sk++){
                            message += "skipsplit\r\n";
                        }
                        message += "split\r\n";
                        _isRunning = false;
                        break;
                    case CommandType.Resume:
                        message = "unpausegametime\r\n";
                        break;
                    case CommandType.Pause:
                        message = "pausegametime\r\n";
                        break;
                    default:
                        message = "split\r\n";
                        break;

                }
                byte[] buffer = Encoding.ASCII.GetBytes(message);

                // Send the message to the server
                _clientSocket.Send(buffer);


                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Succesfully send " + message);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Couldn't split:" + ex.ToString());
                }
            }
        }

        public IEnumerator CheckPauses()
        {
            while (isEnabled)
            {
                if (isActive && _isRunning)
                {
                    try
                    {
                        if (transitions == null)
                            transitions = FindObjectOfType<UITransitions>();
                        if (transitions.IsVisible && !_isPaused)
                        {
                            _isPaused = !_isPaused;
                            SendCommand(CommandType.Pause);
                        }
                        else if (!transitions.IsVisible)
                        {
                            if (!playerSync.IsPaused && _isPaused)
                            {
                                _isPaused = !_isPaused;
                                SendCommand(CommandType.Resume);
                            }
                            else if (playerSync.IsPaused && !_isPaused)
                            {
                                _isPaused = !_isPaused;
                                SendCommand(CommandType.Pause);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.WriteLine("Error in update: " + ex.ToString());
                        }
                    }
                }
                yield return new WaitForSeconds(_refreshFrequence);
            }
        }

        public IEnumerator CheckSplits()
        {
            while (isEnabled)
            {
                
                if (isActive && _isRunning && HasToSplit())
                {
                    SendCommand(CommandType.Split);
                }
                yield return new WaitForSeconds(_refreshFrequence);
            }
        }

        public bool HasToSplit(){
            try
            {
                switch (_splitNumber)
                {
                    case 0: // Scary Janet

                        return hero != null && hero.HasTimewinderAbility;

                    case 1: // Future Ekko 1

                        return GetGadgetSlots() == 3;

                    case 2: // Factorywood

                        return GetWorldZone() == UpdraftWorldZone.P2_Factorywood;

                    case 3: // Vigilnaut

                        return hero != null && hero.HasParallelConvergenceAbility;
                    case 4: // Zarkon 1 

                        return GetGadgetSlots() == 4;

                    case 5: // Sump sewers

                        return GetWorldZone() == UpdraftWorldZone.P3_Sump;

                    case 6: // Drake

                        return hero != null && hero.HasPhaseDiveAbility;

                    case 7: // Warwick

                        return GetGadgetSlots() == 5;

                    case 8: // Cultivair

                        return GetWorldZone() == UpdraftWorldZone.P4_Cultivair;
                    case 9: // Ferros Captain

                        return controller2D != null && controller2D.AirJumpCount == 1;

                    case 10: // Camille

                        return GetGadgetSlots() == 6;

                    case 11: // Chaincrawler

                        return GetWorldZone() == UpdraftWorldZone.P5_Train;

                    case 12: // Drake and Vale

                        return hero != null && hero.HasTemporalPulseAbility;

                    case 13: // Future Ekko 2

                        return GetGadgetSlots() == 7;

                    case 14: // Fenlow theater

                        return GetWorldZone() == UpdraftWorldZone.P6_Theatre;

                    case 15: // Moshpit Meg

                        return dodger != null && dodger.HasDashAbility;

                    case 16: // Jinx

                        return GetGadgetSlots() == 8;

                    case 17: // Fairgrounds

                        return GetWorldZone() == UpdraftWorldZone.P7_Carnival;

                    case 18: // Zarkon 2

                        return GetGadgetSlots() == 9;

                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Exception when checking:" + ex.ToString());
                }
                return false;
            }
        }

    }

    public enum CommandType
    {
        Start,
        Split,
        Stop,
        Pause,
        Resume,
        Restart
    }
}
