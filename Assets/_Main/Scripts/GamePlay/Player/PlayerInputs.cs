using Fiber.Managers;
using Fiber.Utilities;
using GoalSystem;
using HolderSystem;
using Interfaces;
using Lean.Touch;
using UnityEngine;
using UnityEngine.Events;
using Grid = GridSystem.Grid;

namespace GamePlay.Player
{
	public class PlayerInputs : MonoBehaviour, IInputs
	{
		public bool CanInput { get; set; }

		[SerializeField] private LayerMask touchLayers;

		[Space]
		[SerializeField] private LineController lineController;

		public static event UnityAction<Vector3> OnDown;
		public static event UnityAction<Vector3> OnMove;
		public static event UnityAction<Vector3> OnUp;

		private void Awake()
		{
			Input.multiTouchEnabled = false;

			LeanTouch.OnFingerDown += OnFingerDown;
			LeanTouch.OnFingerUpdate += OnFingerUpdate;
			LeanTouch.OnFingerUp += OnFingerUp;

			LevelManager.OnLevelStart += OnLevelStarted;
			LevelManager.OnLevelWin += OnLevelWon;
			LevelManager.OnLevelLose += OnLevelLost;

			Grid.OnFallFillFinish += OnFallFillFinished;
		}

		private void OnDestroy()
		{
			LeanTouch.OnFingerDown -= OnFingerDown;
			LeanTouch.OnFingerUpdate -= OnFingerUpdate;
			LeanTouch.OnFingerUp -= OnFingerUp;

			LevelManager.OnLevelStart -= OnLevelStarted;
			LevelManager.OnLevelWin -= OnLevelWon;
			LevelManager.OnLevelLose -= OnLevelLost;

			Grid.OnFallFillFinish -= OnFallFillFinished;
		}

		private void OnFingerDown(LeanFinger finger)
		{
			if (!CanInput) return;
			if (finger.IsOverGui) return;

			var ray = finger.GetRay(Helper.MainCamera);
			if (Physics.Raycast(ray, out var hit, 200, touchLayers))
			{
				OnDown?.Invoke(hit.point);
			}
		}

		private void OnFingerUpdate(LeanFinger finger)
		{
			if (!CanInput) return;
			if (finger.IsOverGui) return;

			var ray = finger.GetRay(Helper.MainCamera);
			if (Physics.Raycast(ray, out var hit, 200, touchLayers))
			{
				OnMove?.Invoke(hit.point);
			}
		}

		private void OnFingerUp(LeanFinger finger)
		{
			if (!CanInput) return;
			if (finger.IsOverGui) return;

			var ray = finger.GetRay(Helper.MainCamera);
			if (Physics.Raycast(ray, out var hit, 200, touchLayers))
			{
				OnUp?.Invoke(hit.point);
			}
		}

		private void OnFallFillFinished()
		{
			CanInput = true;
		}

		private void OnLevelStarted()
		{
			CanInput = true;
		}

		private void OnLevelWon()
		{
			CanInput = false;
		}

		private void OnLevelLost()
		{
			CanInput = false;
		}
	}
}