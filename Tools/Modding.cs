using DS.Game.Luna;
using DS.Game.Updraft;
using System;
using UnityEngine;

namespace Tools
{
    class Modding : MonoBehaviour
    {
        GameplayRewindManager_Server rewindManager = FindObjectOfType<GameplayRewindManager_Server>();
        ZeroDriveEnergy zeroDriveEnergy = FindObjectOfType<ZeroDriveEnergy>();
        PlatformerController2D_Server controller2D = FindObjectOfType<PlatformerController2D_Server>();

        bool hacksActive = false;
        bool infiniteRewind = false;
        bool infiniteDriveEnergy = false;
        bool infiniteJump = false;

        public void OnGUI()
        {
            if (hacksActive)
            {
                if (GUI.Button(new Rect(0f, 0f, 100f, 30f), "Infinite Rewind"))
                {
                    infiniteRewind = !infiniteRewind;
                }
                if (GUI.Button(new Rect(0f, 30f, 100f, 30f), "Infinite Drive Energy"))
                {
                    infiniteDriveEnergy = !infiniteDriveEnergy;
                }
                if (GUI.Button(new Rect(0f, 60f, 100f, 30f), "Infinite Jump"))
                {
                    infiniteJump = !infiniteJump;
                }
            }
        }

        public void Start()
        {

        }
        public void Update()
        {
            if (hacksActive)
            {
                if (infiniteRewind)
                {
                    rewindManager.CurrentCharges = 99f;
                }

                if (infiniteDriveEnergy)
                {
                    zeroDriveEnergy.Value = 75f;
                }

                if (infiniteJump)
                {
                    controller2D.AirJumps = 0;
                }

                if (Input.GetKeyDown(KeyCode.F9))
                {
                    hacksActive = false;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.F8))
                {
                    hacksActive = true;
                }
            }
        }
    }
}
