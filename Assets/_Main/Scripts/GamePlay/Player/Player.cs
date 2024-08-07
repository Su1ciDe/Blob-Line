using Fiber.Managers;
using Fiber.Utilities;
using Interfaces;
using UnityEngine;

namespace GamePlay.Player
{
	/// <summary>
	/// /// Handle Player Specific Information Assigning,  ...
	/// </summary>
	public class Player : Singleton<Player>
	{
		public IInputs Inputs { get; private set; }
		
		[SerializeField] private Transform inputEyeTarget;
		public Transform InputEyeTarget => inputEyeTarget;

		private void Awake()
		{
			Inputs = GetComponent<IInputs>();
		}

		private void OnEnable()
		{
			LevelManager.OnLevelLoad += OnLevelLoaded;
			LevelManager.OnLevelStart += OnStart;
			LevelManager.OnLevelWin += OnWin;
			LevelManager.OnLevelWinWithMoveCount += OnLevelWinWithMoveCount;
			LevelManager.OnLevelLose += OnLose;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelLoad -= OnLevelLoaded;
			LevelManager.OnLevelStart -= OnStart;
			LevelManager.OnLevelWin -= OnWin;
			LevelManager.OnLevelWinWithMoveCount -= OnLevelWinWithMoveCount;
			LevelManager.OnLevelLose -= OnLose;
		}

		private void OnLevelLoaded()
		{
		}

		// OnStart is called when click "tap to play button"
		private void OnStart()
		{
			// TODO  
		}

		// OnWin is called when game is completed as succeed
		private void OnWin()
		{
			// TODO
		}

		private void OnLevelWinWithMoveCount(int moveCount)
		{
			OnWin();
		}

		// OnLose is called when game is completed as failed
		private void OnLose()
		{
			// TODO
		}
	}
}