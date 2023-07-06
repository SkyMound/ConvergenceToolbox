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

namespace TTestCheat
{
    class AutoSplit : MonoBehaviour
    {
        string path = @"D:\enregistrements\convergence\ConvergenceToolbox\CTB_Debug.txt";

        private Socket _clientSocket;
        private bool _isEnabled;
        private bool _isRunning;
        private bool _isPaused;
        private bool _hasStarted;
        private int _splitNumber;
        private float _refreshFrequence; // The frequence at which we check for splitting

        // Retrieval of the object that interest us to split at middle boss
        Hero_Server hero; // TW, PC, PD, TP
        PlatformerController2D_Server controller2D; // Double jump
        Dodger_Server dodger; // Dash
        UITransitions transitions;

        public void Start()
        {
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("ConvergenceToolbox Debugger");
                }
            }



            _refreshFrequence = 0.3f;
            _splitNumber = 0;
            _isEnabled = true;
            _isPaused = false;
            _isRunning = false;
            _hasStarted = false;
            StartAutoSplit();
        }

        public int GetGadgetSlots()
        {
            return UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Data.MergedInventoryStacker.GetAmount(GameConfig.Instance.GadgetSlotItemID);
        }

        public UpdraftWorldZone GetWorldZone()
        {
            return Level.Instance.LevelData.WorldZone;
        }

        public bool HasLoadGame()
        {
            return UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile != null;
        }

        public float GetSecondsPlayed()
        {
            return UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Data.SecondsPlayed;
        }

        public void ListenForEndOfRun()
        {
            UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Data.MergedInventoryStacker.ItemAmountChanged += new ItemAmountChanged(this.CheckFutureEkko3Defeated);
        }

        public void CheckFutureEkko3Defeated(UnityGuid itemID, int oldAmount, int newAmount)
        {
            if (!itemID.IsValid() || newAmount <= oldAmount)
                return;
            if(itemID == Configuration<AchievementConfig>.Instance.Asset.FutureEkko3DefeatedItemID)
            {
                SendCommand(CommandType.Stop);
            }
            
        }

        public void StartAutoSplit()
        {
            try
            {
                // Create a TCP/IP socket
                _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect to livesplit server 
                _clientSocket.Connect("localhost", 16834);

                _isRunning = true;

                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine("Connected to the server.");
                }
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
            try
            {
                // Close the socket
                _clientSocket.Shutdown(SocketShutdown.Both);
                _clientSocket.Close();

                Console.WriteLine("Connection closed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
        }

        public void SendCommand(CommandType type)
        {
            try
            {
                // Prepare the message to be sent
                string message;
                switch (type)
                {
                    case CommandType.Start:
                        message = "start\r\n";
                        _splitNumber++;
                        _hasStarted = true;

                        break;
                    case CommandType.Split:
                        message = "split\r\n";
                        _splitNumber++;
                        break;
                    case CommandType.Stop:
                        message = "stop\r\n";
                        _isRunning = false;
                        break;
                    case CommandType.Pause:
                        message = "pause\r\n";
                        break;
                    case CommandType.Resume:
                        message = "resume\r\n";
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
            while (_isEnabled)
            {
                if (_isRunning && _hasStarted)
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
                            if (!PauseStack.GetInstance(NetworkCollisionLayer.None).IsPaused && _isPaused)
                            {
                                _isPaused = !_isPaused;
                                SendCommand(CommandType.Resume);
                            }
                            else if (PauseStack.GetInstance(NetworkCollisionLayer.None).IsPaused && !_isPaused)
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
            while (_isEnabled)
            {
                if(_hasStarted && _isRunning)
                {
                    try
                    {
                        switch (_splitNumber)
                        {
                            case 0: // Checks if a game has just been created

                                if (HasLoadGame() && GetSecondsPlayed() < 1f)
                                    SendCommand(CommandType.Start);
                                break;

                            case 1: // Scary Janet

                                if (hero == null)
                                    hero = FindObjectOfType<Hero_Server>();
                                else if (hero.HasTimewinderAbility)
                                    SendCommand(CommandType.Split);
                                break;

                            case 2: // Future Ekko 1

                                if (GetGadgetSlots() == 3) SendCommand(CommandType.Split);
                                break;

                            case 3: // Factorywood

                                if (GetWorldZone() == UpdraftWorldZone.P2_Factorywood) SendCommand(CommandType.Split);
                                break;


                            case 4: // Vigilnaut

                                if (hero.HasParallelConvergenceAbility) SendCommand(CommandType.Split);
                                break;

                            case 5: // Zarkon 1 

                                if (GetGadgetSlots() == 4) SendCommand(CommandType.Split);
                                break;

                            case 6: // Sump sewers

                                if (GetWorldZone() == UpdraftWorldZone.P3_Sump) SendCommand(CommandType.Split);
                                break;

                            case 7: // Drake

                                if (hero.HasPhaseDiveAbility) SendCommand(CommandType.Split);
                                break;

                            case 8: // Warwick

                                if (GetGadgetSlots() == 5) SendCommand(CommandType.Split);
                                break;

                            case 9: // Cultivair

                                if (GetWorldZone() == UpdraftWorldZone.P4_Cultivair) SendCommand(CommandType.Split);
                                break;

                            case 10: // Ferros Captain

                                if (controller2D == null)
                                    controller2D = FindObjectOfType<PlatformerController2D_Server>();
                                else if (controller2D.AirJumpCount == 1)
                                    SendCommand(CommandType.Split);
                                break;

                            case 11: // Camille

                                if (GetGadgetSlots() == 6) SendCommand(CommandType.Split);
                                break;

                            case 12: // Chaincrawler

                                if (GetWorldZone() == UpdraftWorldZone.P5_Train) SendCommand(CommandType.Split);
                                break;

                            case 13: // Drake and Vale

                                if (hero.HasTemporalPulseAbility) SendCommand(CommandType.Split);
                                break;

                            case 14: // Future Ekko 2

                                if (GetGadgetSlots() == 7) SendCommand(CommandType.Split);
                                break;

                            case 15: // Fenlow theater

                                if (GetWorldZone() == UpdraftWorldZone.P6_Theatre) SendCommand(CommandType.Split);
                                break;

                            case 16: // Moshpit Meg

                                if (dodger == null)
                                    dodger = FindObjectOfType<Dodger_Server>();
                                else if (dodger.HasDashAbility)
                                    SendCommand(CommandType.Split);
                                break;

                            case 17: // Jinx

                                if (GetGadgetSlots() == 8) SendCommand(CommandType.Split);
                                break;

                            case 18: // Fairgrounds

                                if (GetWorldZone() == UpdraftWorldZone.P7_Carnival) SendCommand(CommandType.Split);
                                break;

                            case 19: // Zarkon 2

                                if (GetGadgetSlots() == 9) SendCommand(CommandType.Split);
                                break;

                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.WriteLine("Exception when checking:" + ex.ToString());
                        }
                    }
                    yield return new WaitForSeconds(_refreshFrequence);
                }
            }
        }

    }

    public enum CommandType
    {
        Start,
        Split,
        Stop,
        Pause,
        Resume
    }
}
