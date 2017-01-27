using UnityEngine;

public class AllConditions : ResettableScriptableObject
{
	public Condition[] conditions;

	public static AllConditions instance;

	private const string loadPath = "AllConditions";

	public static AllConditions Instance
	{
		get {
			if (!instance)
				instance = FindObjectOfType<AllConditions> ();
			
			if (!instance)
				instance = Resources.Load<AllConditions> (loadPath);
			
			if (!instance)
				Debug.LogError ("AllConditions has not been created yet. Go to Assets > Create > AllConditions.");

			return instance;
		}

		set { instance = value; }
	}

	public override void Reset()
	{
		if (conditions == null)
			return;

		for (int i = 0; i < conditions.Length; i++)
		{
			conditions [i].satisfied = false;
		}
	}

	public  void CheckCondition(Condition requiredCondition)
	{
		Condition[] allCondition = Instance.conditions;
		Condition globalCondition = null;

		if (allCondition != null && allCondition [0] != null)
		{
			for (int i = 0; i < allCondition.Length; i++)
			{
				if (allCondition [i].hash == requiredCondition.hash)
					globalCondition = allCondition [i];
			}

//			if (!globalCondition)
//				return false;

//			return globalCondition.satisfied == requiredCondition.satisfied;
		}
	}
}
