using UnityEditor;
using UnityEngine;

//[CreateAssetMenu(fileName = "Counter Scriptable Objects", menuName = "EZLoc/GeneratedScriptsCount", order = 0)]
public class CountObject : ScriptableObject
{
    public int count = 0;
    private void OnValidate()
    {
        if(count > 16777215)
        {
            Debug.LogError("You have created the manimum number of unique IDs. Impressive!\nSorry I didn't account for this eventually, please contact me for a refund I guess :(");
        }
        //keeps the value positive and within the 
        //bounds that can be represented with 6 hex characters
        count = Mathf.Clamp(count, 0, 16777215);

#if UNITY_EDITOR
	    if (!EditorApplication.isPlayingOrWillChangePlaymode)
		{
			EditorUtility.SetDirty(this);
			//AssetDatabase.SaveAssets();
			//AssetDatabase.Refresh();
		}
#endif
    }
}
