using MelonLoader;
using UnityEngine;
using StressLevelZero;
using ModThatIsNotMod.BoneMenu;

namespace InstantDeath
{
    public static class BuildInfo
    {
        public const string Name = "InstantDeath"; // Name of the Mod.  (MUST BE SET)
        public const string Author = "Virshal"; // Author of the Mod.  (Set as null if none)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.2"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class InstantDeath : MelonMod
    {
        public static bool instantDeathEnabled = false;
        private static Player_Health playerHealth = null;
        private static float deathTimeAmount = 2;

        public override void OnApplicationStart()
        {
            MenuCategory i = MenuManager.CreateCategory("Instant Death", UnityEngine.Color.red);

            i.CreateBoolElement("Enabled", UnityEngine.Color.white, false, delegate (bool enabled)
            {
                SetInstantDeathEnabled(enabled);
            });

            i.CreateFunctionElement("Suicide", UnityEngine.Color.red, delegate ()
            {
                var oldHealthMode = playerHealth.healthMode;
                // Health mode needs to be set to normal, otherwise the player may take damage and live.
                playerHealth.healthMode = Player_Health.HealthMode.Mortal;
                playerHealth.Death();
                // Revert health mode back to the old one
                playerHealth.healthMode = oldHealthMode;
            });
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            playerHealth = GameObject.FindObjectOfType<Player_Health>();
            SetInstantDeathEnabled(instantDeathEnabled);
        }

        public void SetInstantDeathEnabled(bool enabled)
        {
            instantDeathEnabled = enabled;

            if (instantDeathEnabled)
            {
                if (playerHealth.deathTimeAmount != 0) {
                    deathTimeAmount = playerHealth.deathTimeAmount;
                };

                playerHealth.healthMode = Player_Health.HealthMode.Mortal;
                playerHealth.deathTimeAmount = 0;
            }
            else
            {
                // Revert to default value
                playerHealth.deathTimeAmount = deathTimeAmount;
            }

            MelonLogger.Msg($"instantDeathEnabled was set to {instantDeathEnabled}!");
        }
    }
}