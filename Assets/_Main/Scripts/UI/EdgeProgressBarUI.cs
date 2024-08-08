using System.Collections.Generic;
using DG.Tweening;
using Fiber.Managers;
using GamePlay.Blobs;
using GamePlay.Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class EdgeProgressBarUI : MonoBehaviour
	{
		[SerializeField] private int maxCount = 10;
		[Space]
		[SerializeField] private Image[] fillImages;
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
			var color = GameManager.Instance.BlobMaterialsSO.BlobMaterials[blobs[0].CellType].color;
			for (var i = 0; i < fillImages.Length; i++)
			{
				fillImages[i].color = color;
			}

			var count = Mathf.Clamp(blobs.Count, 0f, maxCount);
			count /= maxCount;
			count /= 2f;
			ChangeProgressBar(count);
		}

		private void OnBlobRemovedFromLine(List<Blob> blobs)
		{
			var count = Mathf.Clamp(blobs.Count, 0f, maxCount);
			count /= maxCount;
			count /= 2f;
			ChangeProgressBar(count);
		}

		private void ChangeProgressBar(float percentage)
		{
			for (var i = 0; i < fillImages.Length; i++)
			{
				fillImages[i].DOKill();
				fillImages[i].DOFillAmount(percentage, fillDuration).SetEase(Ease.InOutSine);
			}
		}
	}
}