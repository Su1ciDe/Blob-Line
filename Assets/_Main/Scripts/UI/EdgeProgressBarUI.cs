using System.Collections.Generic;
using DG.Tweening;
using Fiber.Managers;
using Fiber.Utilities.Extensions;
using Fiber.Utilities.UI;
using GamePlay.Blobs;
using GamePlay.Player;
using UnityEngine;

namespace UI
{
	public class EdgeProgressBarUI : MonoBehaviour
	{
		[SerializeField] private int maxCount = 10;
		[Space]
		[SerializeField] private SlicedFilledImage imgFill;
		[SerializeField] private float fillDuration = .1f;

		private void OnEnable()
		{
			LineController.OnBlobAddedToLine += OnBlobAddedToLine;
			LineController.OnBlobRemovedFromLine += OnBlobRemovedFromLine;
		}

		private void OnDisable()
		{
			LineController.OnBlobAddedToLine -= OnBlobAddedToLine;
			LineController.OnBlobRemovedFromLine -= OnBlobRemovedFromLine;
		}

		private void OnBlobAddedToLine(List<Blob> blobs)
		{
			imgFill.color = GameManager.Instance.BlobMaterialsSO.BlobMaterials[blobs[0].CellType].color;
			var count = Mathf.Clamp(blobs.Count, 0f, maxCount);
			count /= maxCount;
			ChangeProgressBar(count);
		}

		private void OnBlobRemovedFromLine(List<Blob> blobs)
		{
			var count = Mathf.Clamp(blobs.Count, 0f, maxCount);
			count /= maxCount;
			ChangeProgressBar(count);
		}

		private void ChangeProgressBar(float percentage)
		{
			imgFill.DOKill();
			imgFill.DOFillAmount(percentage, fillDuration).SetEase(Ease.InOutSine);
		}
	}
}