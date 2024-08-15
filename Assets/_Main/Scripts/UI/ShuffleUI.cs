using System.Collections;
using Fiber.Managers;
using Fiber.CurrencySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Grid = GridSystem.Grid;

namespace UI
{
	public class ShuffleUI : MonoBehaviour
	{
		[SerializeField] private int cost = 30;
		[Space]
		[SerializeField] private Button btnShuffle;
		[SerializeField] private TMP_Text txtCost;

		private void Awake()
		{
			btnShuffle.onClick.AddListener(ShuffleButtonClicked);
			txtCost.SetText(cost.ToString());
			IsEnough();

			LevelManager.OnLevelStart += OnLevelStarted;

			CurrencyManager.Money.OnAmountAdded += OnMoneyAdded;
			CurrencyManager.Money.OnAmountSpent += OnMoneySpent;
		}

		private void OnDestroy()
		{
			LevelManager.OnLevelStart -= OnLevelStarted;

			CurrencyManager.Money.OnAmountAdded -= OnMoneyAdded;
			CurrencyManager.Money.OnAmountSpent -= OnMoneySpent;
		}

		private void OnLevelStarted()
		{
			IsEnough();
		}

		private void ShuffleButtonClicked()
		{
			btnShuffle.interactable = false;

			Grid.Instance.Shuffle();
			CurrencyManager.Money.SpendCurrency(cost);

			StartCoroutine(WaitShuffle());
		}

		private IEnumerator WaitShuffle()
		{
			yield return new WaitUntil(() => !Grid.Instance.IsShuffling);

			IsEnough();
		}

		private void OnMoneySpent(long money)
		{
			IsEnough();
		}

		private void OnMoneyAdded(long money, Vector3? pos, bool isWorldPosition)
		{
			IsEnough();
		}

		private void IsEnough()
		{
			btnShuffle.interactable = CurrencyManager.Money.IsEnough(cost);
		}
	}
}