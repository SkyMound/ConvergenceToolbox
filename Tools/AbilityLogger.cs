using DS.Game.Luna;
using DS.Tech.App;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Tools
{
    public class AbilityLogger : MonoBehaviour
    {

        int _currentTick = 0;
        public bool isEnabled;
        DisplayedAbilities abilitiesDisplayed = new DisplayedAbilities();
        NetworkPlayerSync playerSync;
        ICommandFrameSource CommandFrameSource;

        
        
        void OnGUI()
        {
            int currentShift = 40;
            for (int i = 0; i < abilitiesDisplayed.Count; i++)
            {
                currentShift += abilitiesDisplayed[i].margin;
                GUI.Label(new Rect((float)Screen.width-currentShift, (float)Screen.height-40 , 40, 40), abilitiesDisplayed[i].name, abilitiesDisplayed[i].textStyle);
            }
        }

        public void RetrieveServers()
        {
            this.playerSync = FindObjectOfType<NetworkPlayerSync>();
        }

        void Start()
        {
            isEnabled = true;
            SBNetworkManager.Instance.Server_HeroesSpawned += this.RetrieveServers;
            this.playerSync = FindObjectOfType<NetworkPlayerSync>();
            this.CommandFrameSource = new ControllerInputHandler();
            StartCoroutine(UpdateCommandFrame());

        }

        void UpdateAbilitiesDisplayed(List<Ability> abilitiesFrame) {
            abilitiesFrame.ForEach(ability =>
            {
                if (ability.holding)
                {
                    int index = abilitiesDisplayed.IndexOf(ability);
                    if(index != -1)
                    {
                        if (!abilitiesDisplayed[index].holding)
                        {
                            abilitiesDisplayed.InsertAbility(ability);
                        }
                    }
                    else
                    {
                        abilitiesDisplayed.InsertAbility(ability);
                    }
                }else{
                    int index = abilitiesDisplayed.IndexOf(ability);
                    if (index != -1 && abilitiesDisplayed[index].holding)
                    {
                        abilitiesDisplayed[index].Released();
                    }
                }
            });
        }

        IEnumerator UpdateCommandFrame()
        {
            while (isEnabled)
            {
                try
                {

                    this._currentTick = NetworkTime.SimulationNetworkClock.Tick;
                    CommandFrame currentFrame = this.CommandFrameSource.GetCommandFrame(this.playerSync,this._currentTick);

                    List<Ability> abilitiesFrame = new List<Ability>();
                    abilitiesFrame.Add(new Ability("BA", currentFrame.BasicAttackHeld, _currentTick));
                    abilitiesFrame.Add(new Ability("A", currentFrame.AcrobaticHeld, _currentTick));
                    abilitiesFrame.Add(new Ability("TW", currentFrame.TimewinderHeld, _currentTick));
                    abilitiesFrame.Add(new Ability("PD", currentFrame.PhaseDiveHeld, _currentTick));
                    abilitiesFrame.Add(new Ability("TP", currentFrame.TemporalPulseHeld, _currentTick));
                    abilitiesFrame.Add(new Ability("R", currentFrame.RewindHeld, _currentTick));
                    abilitiesFrame.Add(new Ability("J", currentFrame.JumpHeld, _currentTick));
                    abilitiesFrame.Add(new Ability("PC", currentFrame.ParallelConvergenceHeld, _currentTick));
                    abilitiesFrame.Add(new Ability("CB", currentFrame.ChronobreakPressed, _currentTick));
                    abilitiesFrame.Add(new Ability("D", currentFrame.DodgeHeld, _currentTick));


                    UpdateAbilitiesDisplayed(abilitiesFrame);
                }catch(Exception ex)
                {
                    //Debugger.Log(ex.Message);
                }
                yield return null;
            }
        }
        void LogAbilites(List<Ability> abilities)
        {
            Debugger.Log("Abilities :" + abilities.Count.ToString());
            for (int i = 0; i < abilities.Count; i++)
            {
                Debugger.Log(abilities[i].ToString());
            }
        }

        class Ability : IEquatable<Ability>
        {
            readonly Font font = AssetBundleMap.Instance.LoadAsset<Font>("Assets/DS.Game.Updraft/UI/Fonts_RestOfWorld/FontAssets/FSLola-Medium.otf");

            public string name;
            public GUIStyle textStyle;
            public int margin;
            int tick;
            public bool holding;

            public Ability(string name, bool pressed ,int tickWhenPressed)
            {
                this.textStyle = new GUIStyle();
                this.margin = name.Length == 1 ? 35 : 60;
                this.textStyle.font = font;
                this.textStyle.fontSize = 40;
                this.textStyle.normal.textColor = pressed ? Color.white : Color.gray;
                this.name = name;
                this.holding = pressed;
                this.tick = tickWhenPressed;
            }

            public bool Equals(Ability other)
            {
                return this.name == other.name;
            }

            public void Released()
            {
                this.holding = false;
                this.textStyle.normal.textColor = Color.gray;
            }

            public override string ToString()
            {
                return this.name + ':'+ this.holding.ToString();
            }
        }


        class DisplayedAbilities : List<Ability>
        {
            readonly int maxAbility = 7;

            public void InsertAbility(Ability ability)
            {
                try
                { 
                    this.Insert(0, ability);
                    int idx = this.Count-1;
                    while(this.Count > maxAbility && idx >= 0)
                    {
                        if (!this[idx].holding)
                        {
                            this.RemoveAt(idx);
                        }
                        idx--;
                    }
                }catch(Exception ex)
                {
                    Debugger.Log(ex.Message);
                }
            }
        }
    }
}