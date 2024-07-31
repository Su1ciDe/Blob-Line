using Fiber.Managers;
using Interfaces;
using Lean.Touch;
using UnityEngine;
using UnityEngine.Events;

namespace GamePlay.Player
{
	public class PlayerInputs : MonoBehaviour, IInputs
	{
		public bool CanInput { get; set; }

		[SerializeField] private LayerMask touchLayers;

		public event UnityAction<Vector3> OnDown;
		public event UnityAction<Vector3> OnMove;
		public event UnityAction<Vector3> OnUp;

		private void Awake()
		{
			Input.multiTouchEnabled = false;
			
			LeanTouch.OnFingerDown += OnFingerDown;
			LeanTouch.OnFingerUpdate += OnFingerUpdate;
			LeanTouch.OnFingerUp += OnFingerUp;

			LevelManager.OnLevelStart += OnLevelStarted;
			LevelManager.OnLevelWin += OnLevelWon;
			LevelManager.OnLevelLose += OnLevelLost;
		}

		private void OnDestroy()
		{
			LeanTouch.OnFingerDown -= OnFingerDown;
			LeanTouch.OnFingerUpdate -= OnFingerUpdate;
			LeanTouch.OnFingerUp -= OnFingerUp;

			LevelManager.OnLevelStart -= OnLevelStarted;
			LevelManager.OnLevelStart -= OnLevelWon;
			LevelManager.OnLevelStart -= OnLevelLost;
		}

		private void OnFingerDown(LeanFinger finger)
		{
		}

		private void OnFingerUpdate(LeanFinger finger)
		{
		}

		private void OnFingerUp(LeanFinger finger)
		{
		}

		private void OnLevelStarted()
		{
		}

		private void OnLevelWon()
		{
		}

		private void OnLevelLost()
		{
		}
	}
}