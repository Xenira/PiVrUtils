using System;
using PiVrLoader.Assets;
using PiVrLoader.VRCamera;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace PiVrLoader.Input;

public class Teleport : MonoBehaviour
{
	public Button teleportButton;
	public int layerMask;
	public event Action<Vector3> OnTeleport;
	public TeleportArc teleportArc;

	private bool teleporting = false;
	private RaycastHit hitPoint;
	private float teleportRange = 12f;

	// Audio is currently not working
	private AudioSource audioSource;
	private AudioSource loopAudioSource;
	private AudioClip teleportGo;
	private AudioClip teleportPointerStart;

	public static Teleport Create(Button teleportButton, int layerMask)
	{
		var instance = new GameObject(nameof(Teleport)).AddComponent<Teleport>();
		instance.teleportButton = teleportButton;
		instance.layerMask = layerMask;

		return instance;
	}

	private void Start()
	{
		teleportRange = ModConfig.teleportRange.Value;

		teleportArc = gameObject.AddComponent<TeleportArc>();
		teleportArc.material = Instantiate(AssetLoader.TeleportPointerMat);
		teleportArc.traceLayerMask = layerMask;

		VRCameraManager.mainCamera.gameObject.AddComponent<AudioListener>();

		audioSource = new GameObject("Teleport Audio Src").AddComponent<AudioSource>();
		audioSource.transform.parent = VRCameraManager.mainCamera.transform;
		audioSource.transform.localPosition = Vector3.zero;
		audioSource.playOnAwake = false;

		loopAudioSource = new GameObject("Teleport Loop Audio Src").AddComponent<AudioSource>();
		loopAudioSource.transform.parent = VRCameraManager.mainCamera.transform;
		loopAudioSource.transform.localPosition = Vector3.zero;
		loopAudioSource.clip = AssetLoader.TeleportPointerLoop;
		loopAudioSource.loop = true;
		loopAudioSource.playOnAwake = false;

		teleportGo = AssetLoader.TeleportGo;
		teleportPointerStart = AssetLoader.TeleportPointerStart;
	}

	private void Update()
	{
		if (SteamVRInputMapper.buttonState.hasState(SteamVRInputMapper.PlayerInputBlocked))
		{
			if (teleporting)
			{
				teleporting = false;
				teleportArc.Hide();
				loopAudioSource.Stop();
			}
			return;
		}

		if (teleportButton.IsPressed())
		{
			teleporting = true;
			teleportArc.Show();
			audioSource.PlayOneShot(teleportPointerStart);
			loopAudioSource.Play();
		}

		if (teleporting)
		{
			UpdateTeleport();
		}

		if (teleportButton.IsReleased())
		{
			loopAudioSource.Stop();

			teleporting = false;
			teleportArc.Hide();

			TeleportToHitPoint();
		}
	}

	private void UpdateTeleport()
	{
		teleportArc.SetArcData(SteamVRInputMapper.rightHandObject.transform.position, -SteamVRInputMapper.rightHandObject.transform.up * teleportRange, true, false);
		teleportArc.DrawArc(out hitPoint);

		if (hitPoint.collider != null)
		{
			teleportArc.SetColor(Color.green);
		}
		else
		{
			teleportArc.SetColor(Color.red);
		}
	}

	private void TeleportToHitPoint()
	{
		if (hitPoint.collider == null)
		{
			return;
		}

		if (ModConfig.VignetteOnTeleport())
		{
			Vignette.instance.OneShot(ExecuteTeleport, 1f);
		}
		else
		{
			ExecuteTeleport();
		}
	}

	private void ExecuteTeleport()
	{
		audioSource.PlayOneShot(teleportGo);
		OnTeleport?.Invoke(hitPoint.point);
	}

	private void OnDisable()
	{
		teleportArc.Hide();
		teleporting = false;
	}

}
