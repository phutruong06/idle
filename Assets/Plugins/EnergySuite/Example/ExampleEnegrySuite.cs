using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace EnergySuite
{
    public class ExampleEnegrySuite : MonoBehaviour
    {
		#region Public Vars

		public Text CurrentKeyAmountText;
		public Text CurrentLifeAmountText;
		public Text KeyTimeLeftText;
		public Text LifeTimeLeftText;
		public Slider KeyTimeLeftSlider;
		public Slider LifeTimeLeftSlider;
		public Button AddKeyButton;
		public Button UseKeyButton;
		public Button AddLifeButton;
		public Button UseLifeButton;

        #endregion

        #region Private Vars

        #endregion

        void OnEnable()
        {
			//Example
            Time.timeScale = 0;

			EnergySuiteManager.OnAmountChanged += OnAmountChanged;
			EnergySuiteManager.OnTimeLeftChanged += OnTimeLeftChanged;
			AddKeyButton.onClick.AddListener(AddKeyButtonClicked);
			UseKeyButton.onClick.AddListener(UseKeyButtonClicked);
        }

        void OnDisable()
        {
			EnergySuiteManager.OnAmountChanged -= OnAmountChanged;
			EnergySuiteManager.OnTimeLeftChanged -= OnTimeLeftChanged;
            AddKeyButton.onClick.RemoveListener(AddKeyButtonClicked);
            UseKeyButton.onClick.RemoveListener(UseKeyButtonClicked);
        }

        void Start()
        {
			CurrentKeyAmountText.text = EnergySuiteManager.GetAmount(TimeValue.Energy) + "/" + EnergySuiteManager.GetMaxAmount(TimeValue.Energy);
        }

        #region Event Handlers

        void AddKeyButtonClicked()
        {
			EnergySuiteManager.Add(TimeValue.Energy, 1);
        }

        void UseKeyButtonClicked()
        {
			EnergySuiteManager.Use(TimeValue.Energy, 1);
        }

		void OnAmountChanged(int amount, TimeBasedValue timeBasedValue)
        {
			string text = amount + "/" + timeBasedValue.MaxAmount;

			switch (timeBasedValue.Type)
			{
				case TimeValue.Energy:
					CurrentKeyAmountText.text = text;
					break;
				default:
					break;
			}
        }

        void OnTimeLeftChanged(TimeSpan timeLeft, TimeBasedValue timeBasedValue)
        {
			string formatString = string.Format("{0:00}:{1:00}", timeLeft.Minutes, timeLeft.Seconds);
			float sliderValue = EnergySuiteManager.ConvertToSliderValue(timeLeft, timeBasedValue);

			switch (timeBasedValue.Type)
			{
				case TimeValue.Energy:
					KeyTimeLeftText.text = formatString;
					KeyTimeLeftSlider.value = sliderValue;
					break;
				default:
					break;
			}
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}
