using DS.Game.Luna;
using DS.Game.Updraft;
using System;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Collections;

namespace Tools
{
    class AutoSplit : MonoBehaviour
    {

        private Socket _clientSocket;
        private bool _isEnabled;
        private int _splitNumber;
        private float _refreshFrequence; // The frequence at which we check for splitting

        // Retrieval of the object that interest us to split at middle boss
        readonly Hero_Server hero = FindObjectOfType<Hero_Server>(); // TW, PC, PD, TP
        readonly PlatformerController2D_Server controller2D = FindObjectOfType<PlatformerController2D_Server>(); // Double jump
        readonly Dodger_Server dodger = FindObjectOfType<Dodger_Server>(); // Dash


        public void Start()
        {
            _refreshFrequence = 0.3f;
            _splitNumber = 0;
            _isEnabled = true;
            StartCoroutine(CheckSplits());
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

        public void StartAutoSplit()
        {
            try
            {
                // Create a TCP/IP socket
                _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect to livesplit server 
                _clientSocket.Connect("localhost", 16834);

                Console.WriteLine("Connected to the server.");

                StartCoroutine(CheckSplits());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
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

        public IEnumerator CheckSplits()
        {
            while (_isEnabled)
            {
                if (HasToSplit())
                {
                    Split();
                }
                yield return new WaitForSeconds(_refreshFrequence);
            }
        }

        public void Split()
        {
            try
            {
                // Prepare the message to be sent
                string message = "startorsplit\r\n";
                byte[] buffer = Encoding.ASCII.GetBytes(message);

                // Send the message to the server
                _clientSocket.Send(buffer);
                _splitNumber++;

                Console.WriteLine("Message sent: " + message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
        }


        public bool HasToSplit()
        {
            switch (_splitNumber)
            {
                case 0: // Checks if a game has just been created

                    return HasLoadGame() && GetSecondsPlayed() < 1f;

                case 1: // Scary Janet

                    return hero.HasTimewinderAbility;

                case 2: // Future Ekko 1

                    return GetGadgetSlots() == 3;

                case 3: // Factorywood

                    return GetWorldZone() == UpdraftWorldZone.P2_Factorywood;

                case 4: // Vigilnaut

                    return hero.HasParallelConvergenceAbility;

                case 5: // Zarkon 1 

                    return GetGadgetSlots() == 4;

                case 6: // Sump sewers

                    return GetWorldZone() == UpdraftWorldZone.P3_Sump;

                case 7: // Drake

                    return hero.HasPhaseDiveAbility;

                case 8: // Warwick

                    return GetGadgetSlots() == 5;

                case 9: // Cultivair

                    return GetWorldZone() == UpdraftWorldZone.P4_Cultivair;

                case 10: // Ferros Captain

                    return controller2D.AirJumpCount == 1;

                case 11: // Camille

                    return GetGadgetSlots() == 6;

                case 12: // Chaincrawler

                    return GetWorldZone() == UpdraftWorldZone.P5_Train;

                case 13: // Drake and Vale

                    return hero.HasTemporalPulseAbility;

                case 14: // Future Ekko 2

                    return GetGadgetSlots() == 7;

                case 15: // Fenlow theater

                    return GetWorldZone() == UpdraftWorldZone.P6_Theatre;

                case 16: // Moshpit Meg

                    return dodger.HasDashAbility;

                case 17: // Jinx

                    return GetGadgetSlots() == 8;

                case 18: // Fairgrounds

                    return GetWorldZone() == UpdraftWorldZone.P7_Carnival;

                case 19: // Zarkon 2

                    return GetGadgetSlots() == 9;

                case 20: // Future Ekko 3

                    return false;

                default:

                    return false;

            }
        }
    }
}
