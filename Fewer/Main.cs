using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FewArtifact
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(LanguageAPI), nameof(ItemAPI), nameof(PrefabAPI), nameof(RecalculateStatsAPI))]
    public class Main : BaseUnityPlugin
    {
        public const string PluginAuthor = "Buns";
        public const string PluginName = "FewArtifact";
        public const string PluginVersion = "1.0.0";

        public const string PluginGUID = PluginAuthor + "." + PluginName;

        public static ConfigFile FewArtifactConfig;
        public static ManualLogSource FewArtifactLogger;

        public List<ArtifactBase> Artifacts = new List<ArtifactBase>();

        public void Awake()
        {
            var ArtifactTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ArtifactBase)));

            foreach (var artifactType in ArtifactTypes)
            {
                ArtifactBase artifact = (ArtifactBase)Activator.CreateInstance(artifactType);
                if (ValidateArtifact(artifact, Artifacts))
                {
                    artifact.Init(Config);
                }
            }

            Log.Init(Logger);
            FewArtifactLogger = Logger;
            FewArtifactConfig = Config; 
        }

        public bool ValidateArtifact(ArtifactBase artifact, List<ArtifactBase> artifactList)
        {
            var enabled = Config.Bind<bool>("Artifact: " + artifact.ArtifactName, "Enable Artifact?", true, "Should this artifact appear for selection?").Value;
            if (enabled)
            {
                artifactList.Add(artifact);
            }
            return enabled;
        }
    }
}