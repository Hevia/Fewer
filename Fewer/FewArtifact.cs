using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using UnityEngine.AddressableAssets;
using System;

namespace FewArtifact
{
    class FewArtifact : ArtifactBase
    {
        public static string texArtifactCommandDisabled = "RoR2/Base/Command/texArtifactCommandDisabled.png";
        public static string texArtifactCommandEnabled = "RoR2/Base/Command/texArtifactCommandEnabled.png";
        private bool inSpawn;

        public override string ArtifactName => "Artifact of Few";
        public override string ArtifactLangTokenName => "ARTIFACT_OF_FEW";
        public override string ArtifactDescription => "Spawn rates are halved.";
        public override Sprite ArtifactEnabledIcon => Addressables.LoadAssetAsync<Sprite>(texArtifactCommandEnabled).WaitForCompletion();
        public override Sprite ArtifactDisabledIcon => Addressables.LoadAssetAsync<Sprite>(texArtifactCommandDisabled).WaitForCompletion();

        private static int defMonsterCap;
        internal static bool DecreaseSpawnCap = true;

        public Logger RGILogger { get; private set; }

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
                self.creditMultiplier *= 1.25f;
                self.expRewardCoefficient *= 0.8f;
                self.goldRewardCoefficient *= 0.8f;
                //Debug.Log("creditMultiplier = " + self.creditMultiplier);
                //Debug.Log("expRewardCoefficient = " + self.expRewardCoefficient);
                //Debug.Log("goldRewardCoefficient = " + self.goldRewardCoefficient);
            }
            orig(self);
        }

        private void Run_onRunStartGlobal(Run run)
        {
            defMonsterCap = TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit;

            if (NetworkServer.active && ArtifactEnabled)
            {
                foreach (CharacterMaster cm in run.userMasters.Values)
                    cm.inventory.GiveItem(RoR2Content.Items.MonsoonPlayerHelper.itemIndex);

                if (DecreaseSpawnCap)
                {
                    TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit = (int)(TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit/2);
                    TeamCatalog.GetTeamDef(TeamIndex.Void).softCharacterLimit = (int)(TeamCatalog.GetTeamDef(TeamIndex.Void).softCharacterLimit / 2);
                    TeamCatalog.GetTeamDef(TeamIndex.Lunar).softCharacterLimit = (int)(TeamCatalog.GetTeamDef(TeamIndex.Lunar).softCharacterLimit / 2);
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
