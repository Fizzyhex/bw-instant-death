using MelonLoader;
using UnityEngine;
using ModThatIsNotMod.BoneMenu;
using System.Threading;
using HarmonyLib;
using System.Reflection;
using System.Threading.Tasks;

namespace InstantDeath
{
    public static class BuildInfo
    {
        public const string Name = "InstantDeath"; // Name of the Mod.  (MUST BE SET)
        public const string Author = "Virshal"; // Author of the Mod.  (Set as null if none)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.4"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class InstantDeath : MelonMod
    {
        private static Player_Health playerHealth;

        private static bool instantDeathEnabled = false;
        private static float deathTimeAmount = 2;

        private static bool healthOverridesEnabled = false;
        private static bool healthOverridesWarningUndelivered = false;

        private static MenuCategory boneMenuCategory;
        private static MenuCategory healthCategory;

        private static MelonPreferences_Category preferences;

        private static MelonPreferences_Entry<float> maxHealthPreference;
        private static MelonPreferences_Entry<float> healthRegenPreference;

        private void DisplayHealthOverrideWarning()
        {
            if (healthOverridesEnabled != true && healthOverridesWarningUndelivered)
            {
                healthOverridesWarningUndelivered = false;
                ModThatIsNotMod.Notifications.SendNotification("Enable the override to use these settings!", 4, Color.red);
            }
        }

        public void UpdatePreferences()
        {
            preferences.SaveToFile();
            MelonLogger.Msg($"instantDeathEnabled is set to {instantDeathEnabled}");
            MelonLogger.Msg($"maxHealthOverride is set to {maxHealthPreference.Value}");
            MelonLogger.Msg($"totalRegenDuration is set to {playerHealth.totalRegenDuration}");
        }

        private void UpdateSettings()
        {
            if (healthOverridesEnabled)
            {
                playerHealth.max_Health = maxHealthPreference.Value;
                playerHealth.totalRegenDuration = healthRegenPreference.Value;
            }

            if (instantDeathEnabled)
            {
                if (playerHealth.deathTimeAmount != 0)
                {
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
        }

        public static void MaxHealthPostfix(ref float __result)
        {
            if (healthOverridesEnabled)
            {
                __result = maxHealthPreference.Value;
            }
        }

        public override void OnApplicationStart()
        {
            boneMenuCategory = MenuManager.CreateCategory("Instant Death", Color.red);

            preferences = MelonPreferences.CreateCategory("InstantDeath", "Instant Death");
            preferences.SetFilePath("UserData/InstantDeathPreferences.cfg");

            maxHealthPreference = preferences.CreateEntry<float>("MaxHealth", 10, "Max Health");
            healthRegenPreference = preferences.CreateEntry<float>("HealthRegenDuration", 10, "Health Regeneration Duration");

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

            healthCategory.CreateFloatElement("Max Health (default: 10)", Color.white, maxHealthPreference.Value, delegate (float maxHealth)
            {
                DisplayHealthOverrideWarning();

                maxHealthPreference.Value = maxHealth;

                UpdateSettings();
                UpdatePreferences();
            }, 1, 1, 1000, true);

            healthCategory.CreateFloatElement("Regen Duration (default: 10)", Color.white, healthRegenPreference.Value, delegate (float regenDuration)
            {
                DisplayHealthOverrideWarning();

                healthRegenPreference.Value = regenDuration;

                UpdateSettings();
                UpdatePreferences();
            }, 1, 0, 1000, true);

            boneMenuCategory.CreateFunctionElement("Suicide", Color.red, delegate ()
            {
                UpdateSettings();

                var oldHealthMode = playerHealth.healthMode;
                // Health mode needs to be set to normal, otherwise the player may take damage and live.
                playerHealth.healthMode = Player_Health.HealthMode.Mortal;
                playerHealth.Death();
                // Revert health mode back to the old one
                playerHealth.healthMode = oldHealthMode;
            });
        }

        async void DelayedSettingsUpdate()
        {
            await Task.Delay(3000);
            UpdateSettings();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            playerHealth = GameObject.FindObjectOfType<Player_Health>();
            UpdateSettings();

            // ew... couldn't find a better solution for the game overwriting max health unfortunately, feel free to yell at me though
            DelayedSettingsUpdate();
        }
    }
}