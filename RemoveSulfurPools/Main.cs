using System;
using BepInEx;
using RoR2;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using R2API;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Collections;
using R2API.Utils;
using R2API.ContentManagement;
using On.RoR2;
using System.Collections.Generic;
using MonoMod.RuntimeDetour;
using System.Reflection;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace SylmarDev.RemoveSulfurPools
{
    [BepInDependency(R2APIContentManager.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod)]

    public class Main : BaseUnityPlugin
    {
        public const string PluginAuthor = "SylmarDev";
        public const string PluginName = "RemoveSulfurPools";
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginVersion = "1.0.0";

        public List<string> stagesToRemove = new List<string>() { "sulfurpools" };

        private ILHook _hook;

        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            //Init our logging class so that we can properly log for debugging
            Log.Init(Logger);

            Log.LogInfo($"{PluginGUID} // ver {PluginVersion}");

            Log.LogInfo("Assigning hooks. . .");

            On.RoR2.Run.CanPickStage += Run_CanPickStage;
            //On.RoR2.BazaarController.Awake += BazaarController_Awake;
            

            // This line of log will appear in the bepinex console when the Awake method is done.
            Log.LogInfo(nameof(Awake) + " done.");
        }

        private void BazaarController_Awake(On.RoR2.BazaarController.orig_Awake orig, RoR2.BazaarController self)
        {
            MethodInfo baazarSetUpSeerStations = typeof(RoR2.BazaarController).GetMethod("SetUpSeerStations", BindingFlags.Instance | BindingFlags.Public);

            // init ILHook
            _hook = new ILHook(baazarSetUpSeerStations, seerStationsHook);

            orig(self);
            //throw new NotImplementedException();
        }

        private bool Run_CanPickStage(On.RoR2.Run.orig_CanPickStage orig, RoR2.Run self, RoR2.SceneDef sceneDef)
        {
            var isInvalidStage = stagesToRemove.Contains(sceneDef.cachedName);
            if (isInvalidStage)
            {
                Log.LogMessage($"Stopped invalid stage {sceneDef.cachedName}");
                return false;
            }
            return orig(self, sceneDef);
        }

        private void seerStationsHook(ILContext il)
        {
            var cursor = new ILCursor(il);

            cursor.EmitDelegate<Action>(() => Log.LogInfo("SeerStationsHook has been called"));
            cursor.Goto(50);
            cursor.Emit(OpCodes.Ldloc_0);
            cursor.EmitDelegate<Action<List<RoR2.SceneDef>>>(list =>
            {
                Log.LogInfo("SeerStationsHook is removing bad stages...");
                list = list.Where(x => !stagesToRemove.Contains(x.cachedName)).ToList();
            });
        }


        //private IEnumerator SetSceneDefsHook(On.RoR2.SceneCatalog.orig_SetSceneDefs orig, RoR2.SceneDef[] newSceneDefs)
        //{
        //    // debug
        //    Log.LogInfo($"BEFORE scene def names: {String.Join(",", newSceneDefs.ToList().Select(x => x.cachedName))}");
        //    newSceneDefs = newSceneDefs.ToList().Where(x => x.cachedName != "sulfurpools").ToArray();
        //    Log.LogInfo($"AFTER scene def names: {String.Join(",", newSceneDefs.ToList().Select(x => x.cachedName))}");

        //    var returnValue = orig(newSceneDefs ?? new RoR2.SceneDef[] { new RoR2.SceneDef() });

        //    Log.LogInfo($"allBaseSceneNames: {String.Join(",", RoR2.SceneCatalog.allBaseSceneNames.ToList())}");
        //    Log.LogInfo($"allSceneDefs: {String.Join(",", RoR2.SceneCatalog.allSceneDefs.ToList().Select(x => x.cachedName))}");
        //    Log.LogInfo($"allStageSceneDefs: {String.Join(",", RoR2.SceneCatalog.allStageSceneDefs.ToList().Select(x => x.cachedName))}");

        //    return returnValue;
        //}

        private void Update()
        {

        }
    }
}
