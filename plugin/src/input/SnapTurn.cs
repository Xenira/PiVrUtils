using PiVrLoader.Assets;
using PiVrLoader.VRCamera;
using UnityEngine;

namespace PiVrLoader.Input;

public static class SnapTurn
{
	public static void Turn(GameObject player, float horizontalRotation)
	{
		VRCameraManager.mainCamera.gameObject.GetComponent<AudioSource>().PlayOneShot(AssetLoader.SnapTurn, 0.25f);
		player.transform.RotateAround(VRCameraManager.mainCamera.transform.position, Vector3.up, horizontalRotation);
	}
}
