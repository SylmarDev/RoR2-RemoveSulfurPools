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

        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            //Init our logging class so that we can properly log for debugging
            Log.Init(Logger);

            Log.LogInfo($"{PluginGUID} // ver {PluginVersion}");

            Log.LogInfo("Assigning hooks. . .");

            On.RoR2.SceneCatalog.SetSceneDefs += SetSceneDefsHook;

            // This line of log will appear in the bepinex console when the Awake method is done.
            Log.LogInfo(nameof(Awake) + " done.");
        }

        private IEnumerator SetSceneDefsHook(On.RoR2.SceneCatalog.orig_SetSceneDefs orig, SceneDef[] newSceneDefs)
        {
            // debug
            Log.LogInfo($"BEFORE scene def names: {String.Join(",", newSceneDefs.ToList().Select(x => x.cachedName))}");
            newSceneDefs = newSceneDefs.ToList().Where(x => x.cachedName != "sulfurpools").ToArray();
            Log.LogInfo($"AFTER scene def names: {String.Join(",", newSceneDefs.ToList().Select(x => x.cachedName))}");
            return orig(newSceneDefs);
        }

        private void Update()
        {

        }
    }
}
