using MelonLoader;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle(InstantDeath.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(InstantDeath.BuildInfo.Company)]
[assembly: AssemblyProduct(InstantDeath.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + InstantDeath.BuildInfo.Author)]
[assembly: AssemblyTrademark(InstantDeath.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(InstantDeath.BuildInfo.Version)]
[assembly: AssemblyFileVersion(InstantDeath.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonInfo(typeof(InstantDeath.InstantDeath), InstantDeath.BuildInfo.Name, InstantDeath.BuildInfo.Version, InstantDeath.BuildInfo.Author, InstantDeath.BuildInfo.DownloadLink)]


// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame(null, null)]