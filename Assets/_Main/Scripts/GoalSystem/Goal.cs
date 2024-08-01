using DG.Tweening;
using LevelEditor;
using UnityEngine;
using UnityEngine.Events;

namespace GoalSystem
{
	public class Goal : MonoBehaviour
	{
		public bool IsCompleted { get; set; }
		public CellType CellType { get; private set; }
		public int NeededAmount { get; private set; }
		public int LineIndex { get; private set; }

		private int currentAmount;

		private const float MOVE_DURATION = .35F;

		public event UnityAction<Goal> OnComplete;

		public void Setup(CellType cellType, int neededAmount, int lineIndex)
		{
			CellType = cellType;
			NeededAmount = neededAmount;
			LineIndex = lineIndex;
			currentAmount = 0;
		}

		private bool CheckIfCompleted()
		{
			return currentAmount >= NeededAmount;
		}

		public Tween MoveTo(Vector3 position)
		{
			return transform.DOMove(position, MOVE_DURATION).SetEase(Ease.InOutQuart);
		}

		public void OnCurrentGoal()
		{
		}
	}
}