using BepInEx.Configuration;
using UnityEngine;

namespace PiVrLoader;

public class ModConfig
{
	// General
	private static ConfigEntry<bool> modEnabled;

	// Input
	public static ConfigEntry<bool> laserUiOnly;
	public static ConfigEntry<Color> laserColor;
	public static ConfigEntry<Color> laserClickColor;
	public static ConfigEntry<Color> laserValidColor;
	public static ConfigEntry<Color> laserInvalidColor;
	public static ConfigEntry<float> laserThickness;
	public static ConfigEntry<float> laserClickThicknessMultiplier;

	// Comfort
	public static ConfigEntry<float> teleportRange;
	private static ConfigEntry<bool> vignetteEnabled;
	private static ConfigEntry<bool> vignetteOnTeleport;
	public static ConfigEntry<Color> vignetteColor;
	public static ConfigEntry<float> vignetteIntensity;
	public static ConfigEntry<float> vignetteSmoothness;
	public static ConfigEntry<float> vignetteFadeSpeed;

	// Buttons
	public static ConfigEntry<float> clickTime;

	// UI
	public static ConfigEntry<float> menuScrollSpeed;
	public static ConfigEntry<float> menuScrollDeadzone;

	public static void Init(ConfigFile config)
	{
		// General
		modEnabled = config.Bind("General", "Enabled", true, "Enable mod");

		// Input
		laserUiOnly = config.Bind("Input", "Laser UI Only", true, "Only use laser for UI");
		laserColor = config.Bind("Input", "Laser Color", Color.cyan, "Color of laser");
		laserClickColor = config.Bind("Input", "Laser Click Color", Color.blue, "Color of laser when clicking");
		laserValidColor = config.Bind("Input", "Laser Hover Color", Color.green, "Color of laser when hovering");
		laserInvalidColor = config.Bind("Input", "Laser Invalid Color", Color.red, "Color of laser when hovering over invalid object");
		laserThickness = config.Bind("Input", "Laser Thickness", 0.002f, "Thickness of laser");
		laserClickThicknessMultiplier = config.Bind("Input", "Laser Click Thickness Multiplier", 2f, "Thickness multiplier of laser when clicking");

		// Comfort
		teleportRange = config.Bind("Comfort", "Teleport Range", 12f, "Range of teleporting");
		vignetteEnabled = config.Bind("Comfort", "Vignette Enabled", false, "Enable vignette");
		vignetteOnTeleport = config.Bind("Comfort", "Vignette On Teleport", true, "Enable vignette on teleport");
		vignetteColor = config.Bind("Comfort", "Vignette Color", new Color(0, 0, 0, 1f), "Color of vignette");
		vignetteIntensity = config.Bind("Comfort", "Vignette Intensity", 0.5f, "Intensity of vignette");
		vignetteSmoothness = config.Bind("Comfort", "Vignette Smoothness", 0.15f, "Smoothness of vignette");
		vignetteFadeSpeed = config.Bind("Comfort", "Vignette Fade Speed", 3f, "Fade speed of vignette");

		// Buttons
		clickTime = config.Bind("Buttons", "Click Time", 0.2f, "Speed for clicking. Higher values make it easier to click");

		// UI
		menuScrollSpeed = config.Bind("UI", "Menu Scroll Speed", 0.125f, "Speed of scrolling in menus");
		menuScrollDeadzone = config.Bind("UI", "Menu Scroll Deadzone", 0.35f, "Deadzone of scrolling in menus");
	}

	public static bool ModEnabled()
	{
		return modEnabled.Value;
	}

	// Comfort
	public static bool VignetteEnabled()
	{
		return vignetteEnabled.Value;
	}

	public static bool VignetteOnTeleport()
	{
		return VignetteEnabled() && vignetteOnTeleport.Value;
	}
}
