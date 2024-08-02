using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Fiber.Managers;
using Fiber.Utilities;
using GamePlay.Blobs;
using GamePlay.Player;
using HolderSystem;
using LevelEditor;
using UnityEngine;
using UnityEngine.Events;

namespace GoalSystem
{
	public class GoalManager : Singleton<GoalManager>
	{
		public List<Goal> CurrentGoals { get; private set; } = new List<Goal>();

		[SerializeField] private Goal goalPrefab;
		[SerializeField] private float goalWidth = 2;

		private readonly Queue<Goal> goalQueue = new Queue<Goal>();

		private int goalCount;
		private float offset;

		public static event UnityAction OnGoal;
		public static event UnityAction<Goal> OnNewGoal;

		private void OnEnable()
		{
			LevelManager.OnLevelLoad += OnLevelLoaded;
			LevelManager.OnLevelStart += OnLevelStarted;

			LineController.OnLineToGoal += OnBlobsToGoal;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelLoad -= OnLevelLoaded;
			LevelManager.OnLevelStart -= OnLevelStarted;

			LineController.OnLineToGoal -= OnBlobsToGoal;
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
		}

		private void OnLevelLoaded()
		{
			goalCount = LevelManager.Instance.CurrentLevelData.ActiveGoalCount;
			var goals = LevelManager.Instance.CurrentLevelData.Goals;
			offset = goalCount * goalWidth / 2f - goalWidth / 2f;

			for (int i = 0; i < goals.Length; i++)
			{
				var goal = Instantiate(goalPrefab, transform);
				goal.Setup(goals[i].GaolColor, goals[i].Count);
				goal.gameObject.SetActive(false);
				goalQueue.Enqueue(goal);

				goal.OnComplete += OnGoalCompleted;
			}
		}

		private void OnLevelStarted()
		{
			CurrentGoals.Clear();

			for (int i = 0; i < goalCount; i++)
			{
				if (!goalQueue.TryDequeue(out var goal)) continue;

				goal.transform.localPosition = new Vector3(i * goalWidth - offset, 0, 0);

				goal.OnCurrentGoal(i);
				CurrentGoals.Add(goal);
			}
		}

		private void OnGoalCompleted(Goal goal)
		{
			goal.OnComplete -= OnGoalCompleted;
			var index = goal.Index;

			if (!goalQueue.TryDequeue(out var nextGoal))
			{
				CurrentGoals[index] = null;
				return;
			}

			//TODO: destroy
			goal.gameObject.SetActive(false);

			CurrentGoals[index] = nextGoal;
			nextGoal.transform.position = goal.transform.position;
			nextGoal.OnCurrentGoal(index);
			nextGoal.Spawn().OnComplete(() => { OnNewGoal?.Invoke(nextGoal); });
		}

		private void OnBlobsToGoal(List<Blob> blobsInLine, Goal goal)
		{
			StartCoroutine(OnBlobsToGoalCoroutine(blobsInLine, goal));
		}

		private const float GOAL_DELAY = .1F;
		private readonly WaitForSeconds goalDelay = new WaitForSeconds(GOAL_DELAY);

		private IEnumerator OnBlobsToGoalCoroutine(List<Blob> blobsInLine, Goal goal)
		{
			var tempList = new List<Blob>(blobsInLine);
			for (var i = 0; i < blobsInLine.Count; i++)
			{
				var blob = blobsInLine[i];
				tempList.RemoveAt(i);
				blob.OnJumpToGoal();
				goal.OnBlobJumping(blob);
				if (!goal.IsCompleted)
				{
					blob.JumpTo(goal.transform.position).OnComplete(() =>
					{
						blob.OnEnterToGoal();
						goal.OnBlobEntered(blob);
						blob.transform.DOMoveY(-5, 0.25f).SetRelative(true).SetEase(Ease.InQuad);
					});
				}
				else
				{
					HolderManager.Instance.OnBlobsToHolder(tempList);
				}

				yield return goalDelay;
			}
		}

		public Goal GetCurrentGoalByType(CellType cellType)
		{
			for (var i = 0; i < CurrentGoals.Count; i++)
			{
				var currentGoalHolder = CurrentGoals[i];
				if (!currentGoalHolder) continue;
				if (currentGoalHolder.IsCompleted) continue;
				if (currentGoalHolder.CellType == cellType)
					return currentGoalHolder;
			}

			return null;
		}
	}
}