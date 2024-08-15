using System.Collections;
using DG.Tweening;
using Fiber.Managers;
using GamePlay.Blobs;
using LevelEditor;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GoalSystem
{
	public class Goal : MonoBehaviour
	{
		public bool IsCompleted { get; private set; }
		public CellType CellType { get; private set; }
		public int NeededAmount { get; private set; }
		public int CurrentAmount { get; set; }
		public int Index { get; private set; }

		[SerializeField] private Renderer mr;
		[Title("UI")]
		[SerializeField] private Canvas ui;
		[SerializeField] private TMP_Text txtCount;
		[SerializeField] private Image imgFill;

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

			var color = GameManager.Instance.BlobMaterialsSO.BlobColors[cellType];
			// mr.material.color = mat.color;
			txtCount.SetText(NeededAmount.ToString());
			imgFill.color = color;
			// imgFill.fillAmount = 0;
		}

		private bool CheckIfCompleted()
		{
			return CurrentAmount >= NeededAmount;
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
			transform.DOComplete();
			transform.DOPunchScale(.2f * Vector3.one, .15f, 1, 1);

			txtCount.SetText((NeededAmount - CurrentAmount).ToString());
			// imgFill.fillAmount = (float)CurrentAmount / NeededAmount;

			if (!IsCompleted) return;

			if (waitForCompleteCoroutine is not null)
				StopCoroutine(waitForCompleteCoroutine);

			waitForCompleteCoroutine = StartCoroutine(WaitForComplete(blob));
		}

		private Coroutine waitForCompleteCoroutine;

		private IEnumerator WaitForComplete(Blob blob)
		{
			yield return new WaitUntil(() => !blob.IsMoving);
			yield return Despawn().WaitForCompletion();

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