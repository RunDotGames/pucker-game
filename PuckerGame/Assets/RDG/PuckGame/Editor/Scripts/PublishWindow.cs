using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class PublishWindow : EditorWindow {

    private const string API_KEY_PREF = "RDG.butlerKey";
    private const string SLACK_WEBHOOK_PREF = "RDG.slackWebhook";
    private const string POST_TO_SLACK_PREF = "RDG.postToSlack";
    private const string ITCH_IO_GAME_PREF = "Rdg.itchGame";
    
    private string apiKey;
    private string slackWebhook;
    private bool postToSlack;
    private string itchIoGame;
    
    [MenuItem("File/RDG/Publish")]
    public static void Init() {
        // Get existing open window or if none, make a new one:
        var window = (PublishWindow)GetWindow(typeof(PublishWindow), false, "Publish");
        window.Show();
    }

    public void Awake() {
        apiKey = EditorPrefs.GetString(API_KEY_PREF + PlayerSettings.productName);
        slackWebhook = EditorPrefs.GetString(SLACK_WEBHOOK_PREF + PlayerSettings.productName);
        postToSlack = EditorPrefs.GetBool(POST_TO_SLACK_PREF + PlayerSettings.productName);
        itchIoGame = EditorPrefs.GetString(ITCH_IO_GAME_PREF + PlayerSettings.productName);
    }

    void OnGUI() {
        GUILayout.Space(12);
        EditorGUILayout.LabelField("Settings");
        PrefTextField("itch.io api-key", ref apiKey, API_KEY_PREF);
        PrefTextField("itch.io game path", ref itchIoGame, ITCH_IO_GAME_PREF);
        PrefTextField("slack webhook", ref slackWebhook, SLACK_WEBHOOK_PREF);
        PrefBoolField("post to slack", ref postToSlack, POST_TO_SLACK_PREF);
        GUILayout.Space(12);
        if (GUILayout.Button("Publish",  GUILayout.ExpandWidth(false))) {
            Publish();
        }
    }

    private static void PrefTextField(string label, ref string state, string pref) {
        var updated = EditorGUILayout.TextField(label, state);
        if (updated == state) {
            return;
        }
        
        EditorPrefs.SetString(pref + PlayerSettings.productName, updated);
        state = updated;
    }
    
    private static void PrefBoolField(string label, ref bool state, string pref) {
        var updated = EditorGUILayout.Toggle(label, state);
        if (updated == state) {
            return;
        }
        
        EditorPrefs.SetBool(pref + PlayerSettings.productName, updated);
        state = updated;
    }

    private void Publish() {
        if (string.IsNullOrEmpty(apiKey)) {
            throw new Exception("Butler API Key Required");
        }
        
        if (string.IsNullOrEmpty(slackWebhook) && postToSlack) {
            throw new Exception("Webhook required when posting to slack");
        }
        
        var hasButler = RunCommand("butler help", false);
        if (!hasButler) {
            throw new Exception("Butler Not Installed. Download Butler from Itch.io and add it to PATH");
        }
        
        Debug.Log("Building WebGL...");
        var webGlPath = MakeTempBuildPath();
        var report = BuildPipeline.BuildPlayer(GetEnabledScenes(), webGlPath, BuildTarget.WebGL, BuildOptions.None);
        if (report.summary.result != BuildResult.Succeeded) {
            throw new Exception($"failed to build for webgl: {report.summary.result}");
        }

        Debug.Log("Uploading to Itch.io");
        var uploadResult = RunCommand($"butler push \"{webGlPath}\" {itchIoGame}", true);
        Directory.Delete(webGlPath, true);
        if (!uploadResult) {
            throw new Exception("failed to upload to itch.io");
        }

        if (!postToSlack) {
            Debug.Log("Done.");
            return;
        }
        
        Debug.Log("Posting to slack...");
        PostToSlack(":tada: New Browser Playable Build Published").ContinueWith((success) => {
            if (!success.Result) {
                Debug.LogError("Failed Posting To Slack");
                return;
            }
            
            Debug.Log("Done.");
        });
    }


    private bool RunCommand(string command, bool show) {
        var procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", $"/c {command}"){
            RedirectStandardOutput = true,
            CreateNoWindow = !show,
            UseShellExecute = false,
            EnvironmentVariables = {{ "BUTLER_API_KEY", apiKey}}
        };
        var proc = new System.Diagnostics.Process{ StartInfo = procStartInfo };
        proc.Start();
        proc.StandardOutput.ReadToEnd();
        return proc.ExitCode == 0;
    }
    
    private static string[] GetEnabledScenes() {
        return EditorBuildSettings.scenes
            .Where((scene) => scene.enabled && !string.IsNullOrEmpty(scene.path))
            .Select((scene) => scene.path).ToArray();
    }

    private static string MakeTempBuildPath() {
        return Path.Combine(new[]{
            Path.GetTempPath(), Path.GetRandomFileName()
        });
    }

    private async Task<bool> PostToSlack(string message) {
        var myJson = "{\"text\":\"" + message + "\"}";
        var client = new HttpClient();
        var response = await client.PostAsync(slackWebhook, new StringContent(myJson, Encoding.UTF8, "application/json"));
        return response.StatusCode == HttpStatusCode.OK;
    }
}
