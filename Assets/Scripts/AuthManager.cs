using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class AuthManager : MonoBehaviour
{
    public string authURL = "http://localhost:3000/auth"; // Backend URL

    public TMP_InputField signUpUsername;
    public TMP_InputField SignUpPassword;
    public TMP_InputField loginUsername;
    public TMP_InputField loginPassword;
    public TMP_InputField email;
    public TMP_InputField phoneNum;

    public void Signup()
    {
        StartCoroutine(AuthCoroutine("signup", signUpUsername.text, SignUpPassword.text, email.text, phoneNum.text));
    }

    public void Login()
    {
        StartCoroutine(AuthCoroutine("login", loginUsername.text, loginPassword.text, email: null, phoneNum: null));
    }

    private IEnumerator AuthCoroutine(string action, string username, string password, string email, string phoneNum)
    {
        // Create a JSON object
        string jsonBody = JsonUtility.ToJson(new AuthRequest(action, username, password, email, phoneNum));

        // Create UnityWebRequest with JSON body
        UnityWebRequest request = new UnityWebRequest(authURL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request
        yield return request.SendWebRequest();

        // Handle response
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(action + " success: " + request.downloadHandler.text);

            if (action == "login")
            {
                // Handle token (e.g., store it locally)
                string token = request.downloadHandler.text.Split('"')[3]; // Example for extracting token
                PlayerPrefs.SetString("AuthToken", token);
                PlayerPrefs.Save();
            }
        }
        else
        {
            Debug.LogError(action + " failed: " + request.downloadHandler.text);
        }
    }

    [System.Serializable]
    public class AuthRequest
    {
        public string action;
        public string username;
        public string password;
        public string email;
        public string phoneNum;

        public AuthRequest(string action, string username, string password, string email, string phoneNum)
        {
            this.action = action;
            this.username = username;
            this.password = password;
            this.email = email;
            this.phoneNum = phoneNum;
        }
    }
}
