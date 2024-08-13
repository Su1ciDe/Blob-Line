using Fiber.AudioSystem;

namespace Fiber.CurrencySystem
{
	public class MoneyUI : CurrencyUI
	{
		protected override void OnEnable()
		{
			Init(CurrencyManager.Money);

			base.OnEnable();
		}

		protected override void AfterCurrencyAdded()
		{
			base.AfterCurrencyAdded();

			AudioManager.Instance.PlayAudio(AudioName.Money).SetVolume(0.9f);
		}
	}
}