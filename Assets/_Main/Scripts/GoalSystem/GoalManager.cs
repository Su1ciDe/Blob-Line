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
		public bool IsGoalSequence { get; private set; }

		public List<Goal> CurrentGoals { get; private set; } = new List<Goal>();

		[SerializeField] private Goal goalPrefab;
		[SerializeField] private float goalWidth = 2;

		private readonly Queue<Goal> goalQueue = new Queue<Goal>();

		private int goalCount;
		private float offset;

		public static event UnityAction OnGoal;
		public static event UnityAction<Goal> OnNewGoal;
		public static event UnityAction OnGoalSequenceComplete;

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
			var goalOptions = LevelManager.Instance.CurrentLevelData.Goals;
			offset = goalCount * goalWidth / 2f - goalWidth / 2f;

			for (int i = 0; i < goalOptions.Length; i++)
			{
				var goal = Instantiate(goalPrefab, transform);
				goal.Setup(goalOptions[i].GaolColor, goalOptions[i].Count);
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

			CurrentGoals[index] = nextGoal;
			nextGoal.transform.position = goal.transform.position;
			nextGoal.OnCurrentGoal(index);
			nextGoal.Spawn().SetDelay(0.35f).OnComplete(() => { OnNewGoal?.Invoke(nextGoal); });
		}

		public void OnBlobsToGoal(List<Blob> blobsInLine, Goal goal)
		{
			StartCoroutine(OnBlobsToGoalCoroutine(blobsInLine, goal));
		}

		private readonly WaitForSeconds goalDelay = new WaitForSeconds(.1f);

		private IEnumerator OnBlobsToGoalCoroutine(List<Blob> blobsInLine, Goal goal)
		{
			var count = blobsInLine.Count;
			var tempList = new List<Blob>(blobsInLine);
			for (var i = 0; i < count; i++)
			{
				var blob = tempList[0];
				if (!goal.IsCompleted)
				{
					tempList.RemoveAt(0);
					goal.OnBlobJumping(blob);
					blob.OnJumpToGoal();
					blob.JumpTo(goal.transform.position).OnComplete(() =>
					{
						goal.OnBlobEntered(blob);
						blob.OnEnterToGoal();
						blob.transform.DOMoveY(-5, 0.35f).SetRelative(true).SetEase(Ease.InQuad).OnComplete(() => Destroy(blob.gameObject));
					});
				}
				else if (GetCurrentGoalByType(blob.CellType) is { } nextGoal)
				{
					StartCoroutine(OnBlobsToGoalCoroutine(tempList, nextGoal));
					yield break;
				}
				else
				{
					HolderManager.Instance.OnBlobsToHolder(tempList);
					yield break;
				}

				yield return goalDelay;
			}

			if (tempList.Count.Equals(0))
			{
				OnGoalSequenceComplete?.Invoke();
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

		public Goal GetCurrentGoalByTypeException(CellType cellType, Goal goal)
		{
			for (var i = 0; i < CurrentGoals.Count; i++)
			{
				var currentGoalHolder = CurrentGoals[i];
				if (!currentGoalHolder) continue;
				if (currentGoalHolder.Equals(goal)) continue;
				if (currentGoalHolder.IsCompleted) continue;
				if (currentGoalHolder.CellType == cellType)
					return currentGoalHolder;
			}

			return null;
		}
	}
}