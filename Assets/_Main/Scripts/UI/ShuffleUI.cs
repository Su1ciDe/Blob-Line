using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Grid = GridSystem.Grid;

namespace UI
{
	public class ShuffleUI : MonoBehaviour
	{
		[SerializeField] private Button btnShuffle;

		private void Awake()
		{
			btnShuffle.onClick.AddListener(ShuffleButtonClicked);
		}

		private void ShuffleButtonClicked()
		{
			btnShuffle.interactable = false;
			
			Grid.Instance.Shuffle();

			StartCoroutine(WaitShuffle());
		}

		private IEnumerator WaitShuffle()
		{
			yield return new WaitUntil(() => !Grid.Instance.IsShuffling);

			btnShuffle.interactable = true;
		}
	}
}