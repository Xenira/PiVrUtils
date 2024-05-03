using System.Collections;
using PiUtils.Util;
using UnityEngine;

namespace PiVrLoader.Assets;

class AssetLoader
{
	private static PluginLogger Logger = PluginLogger.GetLogger<AssetLoader>();

	public static PiUtils.Assets.AssetLoader assetLoader;

	// Prefabs
	public static GameObject Vignette;

	// Sounds
	public static AudioClip TeleportGo;
	public static AudioClip TeleportPointerStart;
	public static AudioClip TeleportPointerLoop;
	public static AudioClip SnapTurn;

	// Materials
	public static Material TeleportPointerMat;

	public static IEnumerator Load()
	{
		var steamVrBundle = assetLoader.LoadBundle("steamvr");
		Vignette = assetLoader.LoadAsset<GameObject>(steamVrBundle, "comfort/vignette.prefab");
		TeleportPointerMat = assetLoader.LoadAsset<Material>(steamVrBundle, "SteamVR/InteractionSystem/Teleport/Materials/TeleportPointer.mat");

		TeleportGo = assetLoader.LoadAsset<AudioClip>(steamVrBundle, "SteamVR/InteractionSystem/Teleport/Sounds/TeleportGo.wav");
		TeleportPointerStart = assetLoader.LoadAsset<AudioClip>(steamVrBundle, "SteamVR/InteractionSystem/Teleport/Sounds/TeleportPointerStart.wav");
		TeleportPointerLoop = assetLoader.LoadAsset<AudioClip>(steamVrBundle, "SteamVR/InteractionSystem/Teleport/Sounds/TeleportPointerLoop.wav");
		SnapTurn = assetLoader.LoadAsset<AudioClip>(steamVrBundle, "SteamVR/InteractionSystem/SnapTurn/snapturn_go_01.wav");

		yield break;
	}
}
