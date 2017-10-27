using UnityEngine;
using UnityEngine.UI;

namespace Lean.Localization
{
	public class LeanSliderToArg : MonoBehaviour
	{
		[Tooltip("The slider we're getting values from")]
		public Slider Slider;

		[Tooltip("The LeanLocalizedTextArgs we will send arguments to")]
		public LeanLocalizedTextArgs LocalizedTextArgs;
		
		// Called from the Slider.OnValueChanged event
		public void OnValueChanged(float value)
		{
			if (LocalizedTextArgs != null)
			{
				LocalizedTextArgs.SetArg(value, 0);
			}
		}

		protected virtual void Awake()
		{
			if (Slider != null)
			{
				if (LocalizedTextArgs != null)
				{
					LocalizedTextArgs.SetArg(Slider.value, 0);
				}
			}
		}
	}
}