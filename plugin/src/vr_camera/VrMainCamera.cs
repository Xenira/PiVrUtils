using System;
using PiUtils.Util;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace PiVrLoader.VRCamera;

public class VrMainCamera : MonoBehaviour
{
	private static PluginLogger Logger = PluginLogger.GetLogger<VrMainCamera>();

	public static event Action<VrMainCamera> OnMainCameraCreated;
	public static event Action<VrMainCamera> OnMainCameraDestroyed;

	public Transform camRoot;

	private Vector3 offset = Vector3.zero;
	private void Start()
	{
		// Get the PostProcessLayer component attached to the camera
		var postProcessLayer = GetComponent<PostProcessLayer>();

		// Disable the PostProcessLayer
		if (postProcessLayer != null)
		{
			postProcessLayer.enabled = false;
		}

		OnMainCameraCreated?.Invoke(this);
	}

	private void OnDestroy()
	{
		Logger.LogInfo("Destroying vr main camera...");
		OnMainCameraDestroyed?.Invoke(this);
	}
}
