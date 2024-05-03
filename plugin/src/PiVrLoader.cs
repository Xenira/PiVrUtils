using System.Collections.Generic;
using BepInEx;
using Unity.XR.OpenVR;
using Valve.VR;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR;
using PiUtils.Util;
using System.IO;
using PiVrLoader.VRCamera;
using PiVrLoader.Input;
using PiUtils.Assets;
using System;

namespace PiVrLoader;

[BepInPlugin("de.xenira.pi_vr_loader", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("de.xenira.pi_utils", "0.4.0")]
public class PiVrLoader : BaseUnityPlugin
{
	private static PluginLogger Logger;
	public static string HMDModel = "";

	public static XRManagerSettings managerSettings = null;

	public static List<XRDisplaySubsystem> displays = new List<XRDisplaySubsystem>();
	public static XRDisplaySubsystem MyDisplay = null;

	public static event Action OnVrLoaded;

	//Create a class that actually inherits from MonoBehaviour
	public class VRLoader : MonoBehaviour
	{
	}

	//Variable reference for the class
	public static VRLoader staticVrLoader = null;

	private void Awake()
	{
		// Plugin startup logic
		Logger = PluginLogger.GetLogger<PiVrLoader>();
		Logger.LogInfo($"Loading plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION}...");
		License.LogLicense(Logger, "xenira", MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION);

		ModConfig.Init(Config);

		if (!ModConfig.ModEnabled())
		{
			Logger.LogInfo("Mod is disabled, skipping...");
			return;
		}

		var dllPath = Path.GetDirectoryName(Info.Location);
		Assets.AssetLoader.assetLoader = new PiUtils.Assets.AssetLoader(Path.Combine(dllPath, "assets"));

		DependencyLoader.LoadDirectory(Path.Combine(dllPath, "bin", "Managed"));
		CopyPlugins(Path.Combine(dllPath, "bin", "Plugins", "x86_64"));

		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

		if (staticVrLoader == null)
		{
			GameObject vrLoader = new GameObject("VRLoader");
			staticVrLoader = vrLoader.AddComponent<VRLoader>();
			DontDestroyOnLoad(staticVrLoader);
		}

		staticVrLoader.StartCoroutine(InitVRLoader());

		VRCameraManager.Create();

		StartCoroutine(Assets.AssetLoader.Load());

		Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");
	}

	private void CopyPlugins(string pluginSource)
	{
		var data_path = Path.Combine(Paths.ManagedPath, "..");
		var plugins_path = Path.Combine(data_path, "Plugins", "x86_64");
		if (!Directory.Exists(plugins_path))
		{
			Directory.CreateDirectory(plugins_path);
		}

		foreach (var file in Directory.GetFiles(pluginSource))
		{
			var dest = Path.Combine(plugins_path, Path.GetFileName(file));
			if (File.Exists(dest))
			{
				continue;
			}

			File.Copy(file, dest);
		}
	}

	public static void CopyVrConfig(string source, string dest, bool prependManaged = false)
	{
		if (prependManaged)
		{
			dest = Path.Combine(Paths.ManagedPath, "..", dest);
		}

		if (!Directory.Exists(dest))
		{
			Directory.CreateDirectory(dest);
		}

		foreach (var dir in Directory.GetDirectories(source))
		{
			CopyVrConfig(dir, Path.Combine(dest, Path.GetFileName(dir)));
		}

		foreach (var file in Directory.GetFiles(source))
		{
			var dest_file = Path.Combine(dest, Path.GetFileName(file));
			if (File.Exists(dest_file))
			{
				continue;
			}

			File.Copy(file, dest_file);
		}
	}

	public static System.Collections.IEnumerator InitVRLoader()
	{
		Logger.LogInfo("Initiating VRLoader...");

		SteamVR_Actions.PreInitialize();

		Logger.LogDebug("Creating XRGeneralSettings");
		var general = ScriptableObject.CreateInstance<XRGeneralSettings>();
		Logger.LogDebug("Creating XRManagerSettings");
		managerSettings = ScriptableObject.CreateInstance<XRManagerSettings>();
		Logger.LogDebug("Creating OpenVRLoader");
		var xrLoader = ScriptableObject.CreateInstance<OpenVRLoader>();

		Logger.LogDebug("Setting OpenVR settings");
		var settings = OpenVRSettings.GetSettings();
		// settings.MirrorView = OpenVRSettings.MirrorViewModes.Right;
		settings.StereoRenderingMode = OpenVRSettings.StereoRenderingModes.MultiPass;

		Logger.LogDebug("Adding XRLoader to XRManagerSettings");
		general.Manager = managerSettings;
		managerSettings.loaders.Clear();
		managerSettings.loaders.Add(xrLoader);
		managerSettings.InitializeLoaderSync();

		XRGeneralSettings.AttemptInitializeXRSDKOnLoad();
		XRGeneralSettings.AttemptStartXRSDKOnBeforeSplashScreen();

		Logger.LogDebug("Initializing SteamVR");
		SteamVR.Initialize(true);

		Logger.LogDebug("Getting XRDisplaySubsystemDescriptors");
		SubsystemManager.GetInstances(displays);
		Logger.LogDebug("Got " + displays.Count + " XRDisplaySubsystems");
		foreach (var display in displays)
		{
			Logger.LogDebug("Display running status: " + display.running);
			Logger.LogDebug("Display name: " + display.SubsystemDescriptor.id);
		}
		MyDisplay = displays[0];
		Logger.LogDebug("Starting XRDisplaySubsystem");
		MyDisplay.Start();
		Logger.LogDebug("After starting, display running status: " + MyDisplay.running);

		Logger.LogDebug("Getting HMD Model");
		HMDModel = SteamVR.instance.hmd_ModelNumber;
		Logger.LogInfo("SteamVR hmd modelnumber: " + HMDModel);

		SteamVR_Settings.instance.pauseGameWhenDashboardVisible = true;
		SteamVR_Settings.instance.autoEnableVR = true;

		SteamVRInputMapper.MapActions();

		Logger.LogInfo("Reached end of InitVRLoader");

		PrintSteamVRSettings();
		PrintOpenVRSettings();
		PrintUnityXRSettings();

		OnVrLoaded?.Invoke();

		yield return null;

	}

	private static void PrintSteamVRSettings()
	{
		SteamVR_Settings settings = SteamVR_Settings.instance;
		if (settings == null)
		{
			Logger.LogWarning("SteamVR Settings are null.");
			return;
		}
		Logger.LogDebug("SteamVR Settings:");
		Logger.LogDebug("  actionsFilePath: " + settings.actionsFilePath);
		Logger.LogDebug("  editorAppKey: " + settings.editorAppKey);
		Logger.LogDebug("  activateFirstActionSetOnStart: " + settings.activateFirstActionSetOnStart);
		Logger.LogDebug("  autoEnableVR: " + settings.autoEnableVR);
		Logger.LogDebug("  inputUpdateMode: " + settings.inputUpdateMode);
		Logger.LogDebug("  legacyMixedRealityCamera: " + settings.legacyMixedRealityCamera);
		Logger.LogDebug("  mixedRealityCameraPose: " + settings.mixedRealityCameraPose);
		Logger.LogDebug("  lockPhysicsUpdateRateToRenderFrequency: " + settings.lockPhysicsUpdateRateToRenderFrequency);
		Logger.LogDebug("  mixedRealityActionSetAutoEnable: " + settings.mixedRealityActionSetAutoEnable);
		Logger.LogDebug("  mixedRealityCameraInputSource: " + settings.mixedRealityCameraInputSource);
		Logger.LogDebug("  mixedRealityCameraPose: " + settings.mixedRealityCameraPose);
		Logger.LogDebug("  pauseGameWhenDashboardVisible: " + settings.pauseGameWhenDashboardVisible);
		Logger.LogDebug("  poseUpdateMode: " + settings.poseUpdateMode);
		Logger.LogDebug("  previewHandLeft: " + settings.previewHandLeft);
		Logger.LogDebug("  previewHandRight: " + settings.previewHandRight);
		Logger.LogDebug("  steamVRInputPath: " + settings.steamVRInputPath);
	}

	private static void PrintOpenVRSettings()
	{
		OpenVRSettings settings = OpenVRSettings.GetSettings(false);
		if (settings == null)
		{
			Logger.LogWarning("OpenVRSettings are null.");
			return;
		}
		Logger.LogDebug("OpenVR Settings:");
		Logger.LogDebug("  StereoRenderingMode: " + settings.StereoRenderingMode);
		Logger.LogDebug("  InitializationType: " + settings.InitializationType);
		Logger.LogDebug("  ActionManifestFileRelativeFilePath: " + settings.ActionManifestFileRelativeFilePath);
		Logger.LogDebug("  MirrorView: " + settings.MirrorView);

	}

	private static void PrintUnityXRSettings()
	{
		Logger.LogDebug("Unity.XR.XRSettings: ");
		Logger.LogDebug("  enabled: " + XRSettings.enabled);
		Logger.LogDebug("  deviceEyeTextureDimension: " + XRSettings.deviceEyeTextureDimension);
		Logger.LogDebug("  eyeTextureDesc: " + XRSettings.eyeTextureDesc);
		Logger.LogDebug("  eyeTextureHeight: " + XRSettings.eyeTextureHeight);
		Logger.LogDebug("  eyeTextureResolutionScale: " + XRSettings.eyeTextureResolutionScale);
		Logger.LogDebug("  eyeTextureWidth: " + XRSettings.eyeTextureWidth);
		Logger.LogDebug("  gameViewRenderMode: " + XRSettings.gameViewRenderMode);
		Logger.LogDebug("  isDeviceActive: " + XRSettings.isDeviceActive);
		Logger.LogDebug("  loadedDeviceName: " + XRSettings.loadedDeviceName);
		Logger.LogDebug("  occlusionMaskScale: " + XRSettings.occlusionMaskScale);
		Logger.LogDebug("  renderViewportScale: " + XRSettings.renderViewportScale);
		Logger.LogDebug("  showDeviceView: " + XRSettings.showDeviceView);
		Logger.LogDebug("  stereoRenderingMode: " + XRSettings.stereoRenderingMode);
		Logger.LogDebug("  supportedDevices: " + XRSettings.supportedDevices);
		Logger.LogDebug("  useOcclusionMesh: " + XRSettings.useOcclusionMesh);
	}


}
