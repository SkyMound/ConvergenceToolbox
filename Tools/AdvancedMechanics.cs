using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    public class AdvancedMechanics : MonoBehaviour
    {
        Camera camera;
        GameObject dropdown;
        
        // Use this for initialization
        void Start()
        {
            try
            {
                

                // Will attach a VideoPlayer to the main camera.
                this.camera = FindObjectOfType<Camera>();
                dropdown = new GameObject();
                TMP_Dropdown dp = dropdown.AddComponent<TMP_Dropdown>();
                RectTransform rect = dropdown.AddComponent<RectTransform>();
                

                dropdown.AddComponent<CanvasRenderer>();
                dropdown.AddComponent<Image>();
                dropdown.transform.position = this.camera.transform.position;
                List<string> list = new List<string> { "Roll Cancel", "Whirlwinder", "Nitro Jump" };
                
                dp.AddOptions(list);
                dp.useGUILayout = true;
                dp.Show();




                // VideoPlayer automatically targets the camera backplane when it is added
                // to a camera object, no need to change videoPlayer.targetCamera.
                var videoPlayer = this.camera.gameObject.AddComponent<UnityEngine.Video.VideoPlayer>();

                // Play on awake defaults to true. Set it to false to avoid the url set
                // below to auto-start playback since we're in Start().
                videoPlayer.playOnAwake = false;

                // By default, VideoPlayers added to a camera will use the far plane.
                // Let's target the near plane instead.
                videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;

                // Set the video to play. URL supports local absolute or relative paths.
                // Here, using absolute.
                videoPlayer.url = "D:/Vidéyo/cleeeaaan.mp4";

                // Restart from beginning when done.
                videoPlayer.isLooping = false;

                // Each time we reach the end, we slow down the playback by a factor of 10.
                videoPlayer.loopPointReached += EndReached;

                //videoPlayer.Play();

            }catch(Exception ex)
            {
                Debugger.Log(ex.Message);
            }
        }

        void EndReached(UnityEngine.Video.VideoPlayer vp)
        {
            vp.Stop();
            // vp.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        class MyDropdown : Dropdown
        {
            public MyDropdown()
            {
                
            }
        }
    }
}