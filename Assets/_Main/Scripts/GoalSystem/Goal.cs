using DG.Tweening;
using GamePlay.Blobs;
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
		public int CurrentAmount { get; set; }
		public int Index { get; private set; }


		private const float MOVE_DURATION = .35F;

		public event UnityAction<Goal> OnComplete;

		public void Setup(CellType cellType, int neededAmount)
		{
			CellType = cellType;
			NeededAmount = neededAmount;
			CurrentAmount = 0;
		}

		private bool CheckIfCompleted()
		{
			return CurrentAmount >= NeededAmount;
		}

		public Tween MoveTo(Vector3 position)
		{
			return transform.DOMove(position, MOVE_DURATION).SetEase(Ease.InOutQuart);
		}

		public void OnCurrentGoal(int index)
		{
			gameObject.SetActive(true);
			
			Index = index;
		}

		public void OnBlobJumping(Blob blob)
		{
		}

		public void OnBlobEntered(Blob blob)
		{
		}

		public Tween Spawn()
		{
			return transform.DOScale(0, .35f).From().SetEase(Ease.OutBack);
		}
	}
}