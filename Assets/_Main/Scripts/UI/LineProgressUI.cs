using System.Collections.Generic;
using DG.Tweening;
using Fiber.Utilities.Extensions;
using GamePlay.Blobs;
using GamePlay.Player;
using GoalSystem;
using TMPro;
using TriInspector;
using UnityEngine;

namespace UI
{
	public class LineProgressUI : MonoBehaviour
	{
		[SerializeField] private TMP_Text txtCount;
		[SerializeField] private float scaleMultiplier = .1f;
		[SerializeField] private float scaleDuration = .1f;
		[Space]
		[SerializeField] private TMP_Text txtProgress;
		[TableList]
		[SerializeField] private Progress[] progresses;

		private void OnEnable()
		{
			LineController.OnBlobAddedToLine += OnBlobAddedToLine;
			LineController.OnBlobRemovedFromLine += OnBlobRemovedFromLine;
			LineController.OnLineToGoal += OnLineToGoal;
		}

		private void OnDisable()
		{
			LineController.OnBlobAddedToLine -= OnBlobAddedToLine;
			LineController.OnBlobRemovedFromLine -= OnBlobRemovedFromLine;
			LineController.OnLineToGoal -= OnLineToGoal;
		}

		private void OnBlobAddedToLine(List<Blob> blobs)
		{
			var count = blobs.Count;
			if (!txtCount.gameObject.activeSelf)
				txtCount.gameObject.SetActive(true);

			ChangeCountText(count);
			ChangeScale(count);
		}

		private void OnBlobRemovedFromLine(List<Blob> blobs)
		{
			var count = blobs.Count;

			if (count.Equals(0))
				txtCount.gameObject.SetActive(false);

			ChangeCountText(count);
			ChangeScale(count);
		}

		private void ChangeCountText(int count)
		{
			txtCount.SetText("X" + count.ToString());
		}

		private void ChangeScale(int count)
		{
			var scale = 1 + scaleMultiplier * count;
			txtCount.DOComplete();
			txtCount.DOScale(scale, scaleDuration).SetEase(Ease.OutBack, 10);
		}

		private void OnLineToGoal(List<Blob> blobs, Goal goal)
		{
			for (var i = progresses.Length - 1; i >= 0; i--)
			{
				if (blobs.Count > progresses[i].Count)
				{
					ShowProgressMessage(progresses[i].Text.RandomItem());
					break;
				}
			}
		}

		private void ShowProgressMessage(string message)
		{
			txtProgress.DOComplete();
			
			txtProgress.SetText(message);
			txtProgress.gameObject.SetActive(true);

			txtProgress.DOScale(1.25f, .25f).SetEase(Ease.InOutSine).SetLoops(4, LoopType.Yoyo).OnComplete(() => txtProgress.gameObject.SetActive(false));
		}

		[System.Serializable]
		private class Progress
		{
			public int Count;
			public string[] Text;
		}
	}
}