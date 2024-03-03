/* This file is both for handling setting, loading etc just general management for settings 
 * AND allowing a hook for modders or future devs to easily add their own settings
 * 
 * There are two seperate settings types: 
 * - User-based i.e controls, audio etc
 * - Save-based i.e tortie chance, moonskips etc
 */


using System.Collections;
using System.IO;

namespace ClangenNET.Core;

public static class Settings // FIX -> this works but everything is explicit and kinda icky
{
    public static bool DarkMode { get; set; } = false;
    public static bool CustomCursor { get; set; } = false;
    public static bool Keybinds { get; set; } = false;
    public static bool Shaders { get; set; } = false;
    public static bool Gore {  get; set; } = false;
    public static bool Discord { get; set; } = false;
    public static bool CheckForUpdate { get; set; } = true;
    public static bool ShowChangelog { get; set; } = true;
    public static bool SpecialDates {  get; set; } = true;
    public static bool Fullscreen { get; set; } = false;

    public static string Language { get; set; } = "LEnglish";
}



public class WorldSettings
{

}

