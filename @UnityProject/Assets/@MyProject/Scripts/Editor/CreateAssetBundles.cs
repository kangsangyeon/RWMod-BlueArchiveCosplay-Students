using UnityEditor;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles(
            "../Contents/Assets", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}