#if UNITY_2021_2_OR_NEWER
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
 
namespace Editor 
{
  public class BuildProcessor : IPreprocessBuildWithReport 
  {
    [serializeField]
    private bool hardcodeDebugPort = false;

    [serializeField]
    private int debugPort = 50000;

    public int callbackOrder
    {
      get { return 0; }
    }
 
    public void OnPreprocessBuild(BuildReport report)
    {
        if (hardcodeDebugPort)
        {
            EditorUserBuildSettings.managedDebuggerFixedPort = debugPort;
        }
    }
  }
}
#endif