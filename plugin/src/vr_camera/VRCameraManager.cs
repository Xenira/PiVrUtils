using UnityEngine;
using Valve.VR;
using PiUtils.Util;

namespace PiVrLoader.VRCamera;
public class VRCameraManager : MonoBehaviour
{
	private static PluginLogger Logger = PluginLogger.GetLogger<VRCameraManager>();

	public static Camera mainCamera;
	public static VRCameraManager instance;

	public static VRCameraManager Create()
	{
		instance = new GameObject(nameof(VRCameraManager)).AddComponent<VRCameraManager>();

		return instance;
	}

	private void Start()
	{
		DontDestroyOnLoad(gameObject);
	}

	private void OnDestroy()
	{
		Logger.LogInfo("Destroying vr camera manager...");
	}

	private void OnEnable()
	{
		Logger.LogInfo("Enabling vr camera manager...");
	}

	private void OnDisable()
	{
		Logger.LogInfo("Disabling vr camera manager...");
	}

	private void Update()
	{
		if (Camera.main == null)
		{
			return;
		}

		if (Camera.main != mainCamera)
		{
			mainCamera = Camera.main;
			SetupCamera();
		}
	}

	private void SetupCamera()
	{
		Logger.LogInfo("Setting up camera...");

		mainCamera.gameObject.AddComponent<SteamVR_Camera>();
		mainCamera.gameObject.AddComponent<SteamVR_TrackedObject>();
		mainCamera.gameObject.AddComponent<VrMainCamera>();
	}
}
