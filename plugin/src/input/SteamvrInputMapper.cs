using PiUtils.Input;
using PiUtils.Util;
using UnityEngine;
using Valve.VR;

namespace PiVrLoader.Input;

public static class SteamVRInputMapper
{
	private static PluginLogger Logger = new PluginLogger(typeof(SteamVRInputMapper));

	public static BitState buttonState = new BitState();
	public static long DefaultState = buttonState.GetNextFlag();
	public static long UiState = buttonState.GetNextFlag();
	public static long PlayerInputBlocked = buttonState.GetNextFlag();

	public static Vector2 MoveAxes { get; private set; }
	public static float TurnAxis { get; private set; }

	public static GameObject leftHandObject;
	public static GameObject rightHandObject;

	public static void MapActions()
	{
		Logger.LogInfo("Mapping SteamVR actions...");
		buttonState.setState(DefaultState);
		SteamVR_Actions._default.Move.AddOnUpdateListener(HandleSteamVRMove, SteamVR_Input_Sources.Any);
		SteamVR_Actions._default.SmoothTurn.AddOnUpdateListener(HandleSteamVRSmoothTurn, SteamVR_Input_Sources.Any);

		SteamVR_Actions._default.PoseLeft.AddOnUpdateListener(SteamVR_Input_Sources.Any, LeftHandUpdate);
		SteamVR_Actions._default.PoseRight.AddOnUpdateListener(SteamVR_Input_Sources.Any, RightHandUpdate);
	}

	public static void UnmapActions()
	{
		Logger.LogInfo("Unmapping SteamVR actions...");
		SteamVR_Actions._default.Move.RemoveOnUpdateListener(HandleSteamVRMove, SteamVR_Input_Sources.Any);
		SteamVR_Actions._default.SmoothTurn.RemoveOnUpdateListener(HandleSteamVRSmoothTurn, SteamVR_Input_Sources.Any);

		SteamVR_Actions._default.PoseLeft.RemoveOnUpdateListener(SteamVR_Input_Sources.Any, LeftHandUpdate);
		SteamVR_Actions._default.PoseRight.RemoveOnUpdateListener(SteamVR_Input_Sources.Any, RightHandUpdate);
	}

	private static void HandleSteamVRMove(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
	{
		MoveAxes = axis;
	}

	private static void HandleSteamVRSmoothTurn(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
	{
		TurnAxis = axis.x;
	}

	private static void LeftHandUpdate(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource)
	{
		if (leftHandObject == null)
		{
			return;
		}

		leftHandObject.transform.localPosition = fromAction.localPosition;
		leftHandObject.transform.localRotation = fromAction.localRotation;
	}

	private static void RightHandUpdate(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource)
	{
		if (rightHandObject == null)
		{
			return;
		}

		rightHandObject.transform.localPosition = fromAction.localPosition;
		rightHandObject.transform.localRotation = fromAction.localRotation;
	}

	public static void PlayVibration(SteamVR_Input_Sources input_sources, float amplitude, float? duration = null, float? frequency = null)
	{
		SteamVR_Actions._default.Haptic.Execute(0, duration ?? Time.deltaTime, frequency ?? 1f / 60f, amplitude, input_sources);
	}
}
