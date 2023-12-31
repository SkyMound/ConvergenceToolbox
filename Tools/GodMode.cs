﻿using DS.Game.Items;
using DS.Game.Luna;
using DS.Game.Updraft;
using DS.Tech.App;
using DS.Tech.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    class GodMode
    {
        readonly DSApplicationBuildSettings defaultSettings;
        readonly DSApplicationBuildSettings godModeSettings;
        public bool isEnabled;
        public bool isActive;

        public GodMode()
        {
            defaultSettings = DSApplicationBuildConfig.Instance.DSApplicationBuildSettings;
            godModeSettings = new DSApplicationBuildSettings();
            isEnabled = false;
        }

        public void ToggleGodMode()
        {
            try{
                isActive = isEnabled;
                if (isEnabled)
                {
                    DSApplicationBuildConfig.Instance.WriteBuildSettings(godModeSettings);
                }
                else
                {
                    DSApplicationBuildConfig.Instance.WriteBuildSettings(defaultSettings);
                }
            }catch(Exception ex){
                Debugger.Log(ex.Message);
            }
            
        }


        public void SetSpeedrunPreset()
        {
            InventoryStacker stacker = UpdraftGame.Instance.SaveProfileManager.CurrentSaveProfile.Data.PermanentPlayerInventory.Stacker;
            List<ItemData> list;
            CollectionPool.Request<ItemData>(out list);
            list.AddRange(ItemManager.Instance.SkillItemDatas);
            list.AddRange(ItemManager.Instance.GadgetItemDatas);

            foreach (ItemData itemData in list) 
            {
                stacker.SetAmount(itemData, 0);
                if (itemData.GameObject.name.Equals("Gadget_RewindBuff"))
                    stacker.SetAmount(itemData, 1);
                
            }
            CollectionPool.Return<ItemData>(ref list);
        }
    }
}
