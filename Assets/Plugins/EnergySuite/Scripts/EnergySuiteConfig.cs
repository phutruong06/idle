using System.Collections.Generic;

namespace EnergySuite
{
	public static class EnergySuiteConfig
	{
		public static Dictionary<TimeValue, TimeBasedValue> StoredInfo = new Dictionary<TimeValue, TimeBasedValue>()
		{
			{ TimeValue.Energy, new TimeBasedValue(TimeValue.Energy, 4, 30, 10)},
		};

		//Change this values once and never again
		public const string Password = "IdleGunner";
		public const string PasswordSalt = "IdleGunnerSalt";

		//Dont touch this
		public const string AmountPrefixKey = "Amount_";
		public const string LastTimeAddedPrefixKey = "LastTimeAdded_";
	}
}
