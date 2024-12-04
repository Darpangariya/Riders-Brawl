using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MetaAuthManager : MonoBehaviour
{
    public string metaLoginURL = "https://www.facebook.com/v12.0/dialog/oauth";
    public string appId = "your_meta_app_id"; // Replace with your Meta App ID
    public string redirectUri = "https://your-backend-url.com/auth/meta/callback";
    public string backendAuthURL = "http://localhost:3000/auth/meta";

    public void SignInWithMeta()
    {
        string authUrl = $"{metaLoginURL}?client_id={appId}&redirect_uri={redirectUri}&response_type=token&scope=email";
        Application.OpenURL(authUrl);
    }

    public void ProcessMetaToken(string accessToken)
    {
        StartCoroutine(BackendAuth(accessToken));
    }

    private IEnumerator BackendAuth(string accessToken)
    {
        var jsonBody = $"{{\"accessToken\":\"{accessToken}\"}}";
        UnityWebRequest request = new UnityWebRequest(backendAuthURL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Meta login success: " + request.downloadHandler.text);
            // Store the token for future use
        }
        else
        {
            Debug.LogError("Meta login failed: " + request.downloadHandler.text);
        }
    }
}
