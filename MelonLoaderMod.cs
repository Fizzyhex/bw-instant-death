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
        public const string Version = "1.0.3"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class InstantDeath : MelonMod
    {
        private static Player_Health playerHealth;

        private static bool instantDeathEnabled = false;
        private static float deathTimeAmount = 2;

        private static bool healthOverridesEnabled = false;
        private static bool healthOverridesWarningUndelivered = false;
        private static float maxHealthOverride = 10;
        private static float regenDurationOverride = 10;

        private static MenuCategory boneMenuCategory;
        private static MenuCategory healthCategory;

        public static MelonPreferences_Category preferences;

        public static MelonPreferences_Entry<float> maxHealthPreference;
        public static MelonPreferences_Entry<float> healthRegenPreference;

        public void DisplayHealthOverrideWarning()
        {
            if (healthOverridesEnabled != true && healthOverridesWarningUndelivered)
            {
                healthOverridesWarningUndelivered = false;
                ModThatIsNotMod.Notifications.SendNotification("Enable the override to use these settings!", 4, Color.red);
            }
        }

        public override void OnApplicationStart()
        {
            boneMenuCategory = MenuManager.CreateCategory("Instant Death", Color.red);

            preferences = MelonPreferences.CreateCategory("InstantDeath", "Instant Death");
            preferences.SetFilePath("UserData/InstantDeathPreferences.cfg");

            maxHealthPreference = preferences.CreateEntry<float>("MaxHealth", maxHealthOverride, "Max Health");
            healthRegenPreference = preferences.CreateEntry<float>("HealthRegenDuration", regenDurationOverride, "Health Regeneration Duration");

            maxHealthOverride = maxHealthPreference.Value;
            regenDurationOverride = healthRegenPreference.Value;

            boneMenuCategory.CreateBoolElement("Instant Death Enabled", Color.white, false, delegate (bool enabled)
            {
                instantDeathEnabled = enabled;
                UpdateSettings();
            });

            healthCategory = boneMenuCategory.CreateSubCategory("Health Settings", Color.green);

            healthCategory.CreateBoolElement("Enable health settings", Color.green, healthOverridesEnabled, delegate (bool enabled)
            {
                healthOverridesEnabled = enabled;

                UpdateSettings();
            });

            healthCategory.CreateFloatElement("Max Health (default: 10)", Color.white, maxHealthOverride, delegate (float maxHealth)
            {
                DisplayHealthOverrideWarning();

                maxHealthOverride = maxHealth;
                UpdateSettings();
            }, 1, 1, 1000, true);

            healthCategory.CreateFloatElement("Regen Duration (default: 10)", Color.white, maxHealthOverride, delegate (float regenDuration)
            {
                DisplayHealthOverrideWarning();

                regenDurationOverride = regenDuration;
                UpdateSettings();
            }, 1, 0, 1000, true);

            boneMenuCategory.CreateFunctionElement("Suicide", Color.red, delegate ()
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
            UpdateSettings();
        }

        public void UpdateSettings()
        {
            MelonLogger.Msg($"Max health was {playerHealth.max_Health}");
            MelonLogger.Msg($"Regen speed was {playerHealth.totalRegenDuration}");

            if (healthOverridesEnabled)
            {
                playerHealth.max_Health = maxHealthOverride;
                playerHealth.totalRegenDuration = regenDurationOverride;
            }

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

            maxHealthPreference.Value = maxHealthOverride;
            healthRegenPreference.Value = regenDurationOverride;
            preferences.SaveToFile();

            MelonLogger.Msg($"instantDeathEnabled is set to {instantDeathEnabled}");
            MelonLogger.Msg($"maxHealthOverride is set to {maxHealthOverride}");
            MelonLogger.Msg($"totalRegenDuration is set to {playerHealth.totalRegenDuration}");
        }
    }
}