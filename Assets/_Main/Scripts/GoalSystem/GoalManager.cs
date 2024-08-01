using System.Collections.Generic;
using DG.Tweening;
using Fiber.Managers;
using Fiber.Utilities;
using Fiber.Utilities.Extensions;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace GoalSystem
{
	public class GoalManager : Singleton<GoalManager>
	{
		public List<Goal> CurrentGoals { get; private set; } = new List<Goal>();

		[SerializeField] private Goal goalPrefab;
		[SerializeField] private float goalLength = 2;

		[Title("Lines")]
		[SerializeField] private Transform[] lines = new Transform[LINE_COUNT];

		private List<Queue<Goal>> lineQueues = new List<Queue<Goal>>();

		private const int LINE_COUNT = 3;

		public static event UnityAction OnGoal;
		public static event UnityAction<Goal> OnNewGoal;

		private void OnEnable()
		{
			LevelManager.OnLevelLoad += OnLevelLoaded;
			LevelManager.OnLevelStart += OnLevelStarted;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelLoad -= OnLevelLoaded;
			LevelManager.OnLevelStart -= OnLevelStarted;
		}

		private void OnLevelLoaded()
		{
			for (int i = 0; i < LINE_COUNT; i++)
				lineQueues.Add(new Queue<Goal>());

			var goalStages = LevelManager.Instance.CurrentLevelData.GoalStages;
			for (var i = 0; i < goalStages.Length; i++)
			{
				var goalStage = goalStages[i];
				for (int j = 0; j < LINE_COUNT; j++)
				{
					if (goalStage.Goals.Length <= j) continue;

					var line = lines[j];
					var goalOptions = goalStage.Goals[j];

					var goal = Instantiate(goalPrefab, line.transform);
					goal.transform.SetLocalPositionZ(-goalLength * i);
					goal.Setup(goalOptions.GaolColor, goalOptions.Count, j);
					goal.OnComplete += OnGoalCompleted;

					lineQueues[j].Enqueue(goal);
				}
			}
		}

		private void OnLevelStarted()
		{
			CurrentGoals.Clear();

			for (int i = 0; i < LINE_COUNT; i++)
			{
				if (!lineQueues[i].TryDequeue(out var goalHolder)) continue;

				CurrentGoals.Add(goalHolder);
				goalHolder.OnCurrentGoal();
			}
		}

		private void OnGoalCompleted(Goal goal)
		{
			goal.OnComplete -= OnGoalCompleted;

			var index = goal.LineIndex;

			//TODO: destroy

			if (!lineQueues[index].TryDequeue(out var nextGoal)) return;

			CurrentGoals[index] = nextGoal;
			nextGoal.MoveTo(lines[index].position).OnComplete(() =>
			{
				nextGoal.OnCurrentGoal();
				OnNewGoal?.Invoke(nextGoal);
			});

			var i = 1;
			foreach (var goalInLine in lineQueues[index])
			{
				goalInLine.MoveTo(lines[index].position + i * goalLength * Vector3.forward);
				i++;
			}
		}
	}
}