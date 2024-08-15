using System.Collections;
using System.Collections.Generic;
using Fiber.UI;
using Fiber.Managers;
using Fiber.Utilities;
using GamePlay.Blobs;
using GamePlay.Player;
using HolderSystem;
using LevelEditor;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace Managers
{
	public class TutorialManager : Singleton<TutorialManager>
	{
		public CellType TutorialColorPredicate { get; private set; } = CellType.Empty;
		public int TutorialCountPredicate { get; private set; } = 0;

		private TutorialUI tutorialUI => TutorialUI.Instance;

		private void OnEnable()
		{
			LevelManager.OnLevelStart += OnLevelStarted;
			LevelManager.OnLevelUnload += OnLevelUnloaded;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelStart -= OnLevelStarted;
		}

		private void OnDestroy()
		{
			Unsub();
		}

		private void OnLevelUnloaded()
		{
			Unsub();
		}

		private void Unsub()
		{
			StopAllCoroutines();

			LineController.OnLineComplete -= Level1OnLineCompleteBlue;

			if (TutorialUI.Instance)
			{
				tutorialUI.HideFocus();
				tutorialUI.HideHand();
				tutorialUI.HideText();
			}
		}

		private void OnLevelStarted()
		{
			if (LoadingPanelController.Instance && LoadingPanelController.Instance.IsActive)
			{
				StartCoroutine(WaitLoadingScreen());
			}
			else
			{
				LevelStart();
			}
		}

		private IEnumerator WaitLoadingScreen()
		{
			yield return new WaitUntilAction(ref LoadingPanelController.Instance.OnLoadingFinished);
			yield return null;

			LevelStart();
		}

		private void LevelStart()
		{
			if (LevelManager.Instance.LevelNo.Equals(1))
			{
				Level1Tutorial();
			}
		}

		#region Level1 Tutorial

		private void Level1Tutorial()
		{
			TutorialColorPredicate = CellType.Blue;
			TutorialCountPredicate = 16;

			var width = Grid.Instance.GridCells.GetLength(0) - 1;
			var height = Grid.Instance.GridCells.GetLength(1) - 1;
			tutorialUI.ShowText("Make 3 or more links");
			tutorialUI.ShowSwipe(Helper.MainCamera, Grid.Instance.GridCells[0, height].transform.position, Grid.Instance.GridCells[0, 0].transform.position,
				Grid.Instance.GridCells[width, 0].transform.position, Grid.Instance.GridCells[width, height].transform.position);

			LineController.OnLineComplete += Level1OnLineCompleteBlue;
		}

		private void Level1OnLineCompleteBlue(List<Blob> blobs)
		{
			LineController.OnLineComplete -= Level1OnLineCompleteBlue;

			Player.Instance.Inputs.CanInput = false;

			TutorialColorPredicate = CellType.Empty;
			TutorialCountPredicate = 0;

			tutorialUI.HideHand();
			tutorialUI.HideText();

			StartCoroutine(Level1ShowHolder());
		}

		private IEnumerator Level1ShowHolder()
		{
			yield return new WaitForSeconds(2f);

			tutorialUI.ShowFocus(HolderManager.Instance.Holders[0].transform.position, Helper.MainCamera);
			tutorialUI.ShowText("Rest of the blobs go here");

			yield return new WaitForSeconds(2);

			tutorialUI.HideFocus();
			tutorialUI.HideText();

			Player.Instance.Inputs.CanInput = true;
		}

		#endregion
	}
}