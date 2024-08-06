using System.Collections;
using DG.Tweening;
using GamePlay.Blobs;
using LevelEditor;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace GoalSystem
{
	public class Goal : MonoBehaviour
	{
		public bool IsCompleted { get; private set; }
		public CellType CellType { get; private set; }
		public int NeededAmount { get; private set; }
		public int CurrentAmount { get; set; }
		public int Index { get; private set; }

		[Title("UI")]
		[SerializeField] private Canvas ui;
		[SerializeField] private TMP_Text txtCount;

		private const float MOVE_DURATION = .35F;

		public event UnityAction<Goal> OnComplete;

		private void OnDestroy()
		{
			if (waitForCompleteCoroutine is not null)
			{
				StopCoroutine(waitForCompleteCoroutine);
				waitForCompleteCoroutine = null;
			}
		}

		public void Setup(CellType cellType, int neededAmount)
		{
			CellType = cellType;
			NeededAmount = neededAmount;
			CurrentAmount = 0;

			txtCount.SetText(NeededAmount.ToString());
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
			CurrentAmount++;

			IsCompleted = CheckIfCompleted();
		}

		public void OnBlobEntered(Blob blob)
		{
			txtCount.SetText((NeededAmount - CurrentAmount).ToString());

			if (IsCompleted)
			{
				if (waitForCompleteCoroutine is not null)
					StopCoroutine(waitForCompleteCoroutine);

				waitForCompleteCoroutine = StartCoroutine(WaitForComplete(blob));
			}
		}

		private Coroutine waitForCompleteCoroutine;

		private IEnumerator WaitForComplete(Blob blob)
		{
			yield return new WaitUntil(() => !blob.IsMoving);
			yield return Despawn().WaitForCompletion();
			Debug.Log("goal completed");

			OnComplete?.Invoke(this);

			waitForCompleteCoroutine = null;

			Destroy(gameObject);
		}

		private const float SPAWN_DURATION = .3f;

		public Tween Spawn()
		{
			return transform.DOScale(0, SPAWN_DURATION).From().SetEase(Ease.OutBack);
		}

		public Tween Despawn()
		{
			return transform.DOScale(0, SPAWN_DURATION).SetEase(Ease.InBack);
		}
	}
}