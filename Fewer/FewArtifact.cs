﻿using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using UnityEngine.AddressableAssets;

namespace FewArtifact
{
    class FewArtifact : ArtifactBase
    {
        public static string texArtifactCommandDisabled = "RoR2/Base/Command/texArtifactCommandDisabled.png";
        public static string texArtifactCommandEnabled = "RoR2/Base/Command/texArtifactCommandEnabled.png";

        public override string ArtifactName => "Artifact of Few";
        public override string ArtifactLangTokenName => "ARTIFACT_OF_FEW";
        public override string ArtifactDescription => "Monster team sizes are halved.";
        public override Sprite ArtifactEnabledIcon => Addressables.LoadAssetAsync<Sprite>(texArtifactCommandEnabled).WaitForCompletion();
        public override Sprite ArtifactDisabledIcon => Addressables.LoadAssetAsync<Sprite>(texArtifactCommandDisabled).WaitForCompletion();

        private static int defMonsterCap;
        internal static bool DecreaseSpawnCap = true;

        public override void Init(ConfigFile config)
        {
            CreateLang();
            CreateArtifact();
            Hooks();
        }

        public override void Hooks()
        {
            Run.onRunStartGlobal += Run_onRunStartGlobal;
            Run.onRunDestroyGlobal += Run_onRunDestroyGlobal;
        }

        public static void CombatDirector_Awake(On.RoR2.CombatDirector.orig_Awake orig, CombatDirector self)
        {
            if (DecreaseSpawnCap)
            {
                //self.creditMultiplier *= 1.25f;
                self.expRewardCoefficient *= 2f;
                self.goldRewardCoefficient *= 2f;
            }
            orig(self);
        }

        private void Run_onRunStartGlobal(Run run)
        {
            defMonsterCap = TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit;

            if (NetworkServer.active && ArtifactEnabled)
            {
                if (DecreaseSpawnCap)
                {
                    //Log.Debug("Caps before decrease...");
                    //Log.Debug("TeamIndex.Monster cap: " + TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit);
                    //Log.Debug("TeamIndex.Void cap: " + TeamCatalog.GetTeamDef(TeamIndex.Void).softCharacterLimit);
                    //Log.Debug("TeamIndex.Lunar cap: " + TeamCatalog.GetTeamDef(TeamIndex.Lunar).softCharacterLimit);
                    
                    TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit = (int)(TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit/2);
                    TeamCatalog.GetTeamDef(TeamIndex.Void).softCharacterLimit = (int)(TeamCatalog.GetTeamDef(TeamIndex.Void).softCharacterLimit / 2);
                    TeamCatalog.GetTeamDef(TeamIndex.Lunar).softCharacterLimit = (int)(TeamCatalog.GetTeamDef(TeamIndex.Lunar).softCharacterLimit / 2);

                    //Log.Debug("Caps after decrease...");
                    //Log.Debug("TeamIndex.Monster cap: " + TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit);
                    //Log.Debug("TeamIndex.Void cap: " + TeamCatalog.GetTeamDef(TeamIndex.Void).softCharacterLimit);
                    //Log.Debug("TeamIndex.Lunar cap: " + TeamCatalog.GetTeamDef(TeamIndex.Lunar).softCharacterLimit);

                    On.RoR2.CombatDirector.Awake += CombatDirector_Awake;
                }
            }
        }

        private void Run_onRunDestroyGlobal(Run run)
        {
            TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit = defMonsterCap;
            TeamCatalog.GetTeamDef(TeamIndex.Void).softCharacterLimit = defMonsterCap;
            TeamCatalog.GetTeamDef(TeamIndex.Lunar).softCharacterLimit = defMonsterCap;

            if (DecreaseSpawnCap)
                On.RoR2.CombatDirector.Awake -= CombatDirector_Awake;
        }
    }
}
