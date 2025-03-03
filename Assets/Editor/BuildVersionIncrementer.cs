using UnityEditor;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BuildVersionIncrementer : IPreprocessBuildWithReport
{
    private const string BuildInfoPath = "Assets/BuildInfo.asset";
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        var buildInfo = AssetDatabase.LoadAssetAtPath<BuildInfo>(BuildInfoPath);
        if(buildInfo == null)
        {
            buildInfo = ScriptableObject.CreateInstance<BuildInfo>();
            AssetDatabase.CreateAsset(buildInfo, BuildInfoPath);
        }

        buildInfo.buildNumber++;
        EditorUtility.SetDirty(buildInfo);
        AssetDatabase.SaveAssets();
        Debug.Log($"New Build Number: {buildInfo.buildNumber}");
    }
}
