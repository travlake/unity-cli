using System;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.Compilation;

namespace UnityCliConnector.Tools
{
    [UnityCliTool(Description = "Refresh Unity assets and optionally request script compilation.")]
    public static class RefreshUnity
    {
        public class Parameters
        {
            [ToolParameter("Refresh mode: if_dirty (default) or force")]
            public string Mode { get; set; }

            [ToolParameter("Scope: all (default) or specific path")]
            public string Scope { get; set; }

            [ToolParameter("Compile mode: none (default) or request")]
            public string Compile { get; set; }
        }

        public static object HandleCommand(JObject @params)
        {
            string mode = @params?["mode"]?.ToString() ?? "if_dirty";
            string scope = @params?["scope"]?.ToString() ?? "all";
            string compile = @params?["compile"]?.ToString() ?? "none";

            bool refreshTriggered = false;
            bool compileRequested = false;

            bool shouldRefresh = string.Equals(mode, "force", StringComparison.OrdinalIgnoreCase)
                                 || string.Equals(mode, "if_dirty", StringComparison.OrdinalIgnoreCase);

            if (shouldRefresh && !string.Equals(scope, "scripts", StringComparison.OrdinalIgnoreCase))
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                refreshTriggered = true;
            }

            if (string.Equals(compile, "request", StringComparison.OrdinalIgnoreCase))
            {
                CompilationPipeline.RequestScriptCompilation();
                compileRequested = true;
            }

            if (string.Equals(scope, "all", StringComparison.OrdinalIgnoreCase) && !refreshTriggered)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                refreshTriggered = true;
            }

            return new SuccessResponse("Refresh requested.", new
            {
                refresh_triggered = refreshTriggered,
                compile_requested = compileRequested,
            });
        }
    }
}
