using System.IO;
using UnityEditor.Callbacks;
using UnityEngine;



public class workaround
{
#pragma warning disable IDE0051 // Remove unused private members
    [DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        Debug.Log("Fixing all csproj files...");
        foreach (var filename in Directory.GetFiles(".", "*.csproj"))
        {
            var csprojContent = File.ReadAllText(filename);
            var fixedCsprojContent = csprojContent.Replace("<ReferenceOutputAssembly>false</ReferenceOutputAssembly>", "<ReferenceOutputAssembly>true</ReferenceOutputAssembly>");
            if (csprojContent != fixedCsprojContent)
            {
                File.WriteAllText(filename, fixedCsprojContent);
                Debug.Log("    " + filename);
            }
            else
            {
                Debug.Log("    " + filename + " (skipped)");
            }
        }
        Debug.Log("...Done");
    }
#pragma warning restore IDE0051 // Remove unused private members
}