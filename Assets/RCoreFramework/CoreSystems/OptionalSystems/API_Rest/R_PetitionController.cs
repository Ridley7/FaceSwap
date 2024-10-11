using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class R_PetitionController
{
    private string baseurl;
    private string token;
    private int tokenRequestTimeout;
    private bool verbose;

    #region constructors
    public R_PetitionController(bool verbose, string baseurl, string token, int tokenRequestTimeOut) 
    {
        this.verbose = verbose;
        this.baseurl = baseurl;
        this.token = token;
        this.tokenRequestTimeout = tokenRequestTimeOut;
    }

    public R_PetitionController()
    {
        //Cargamos datos de fichero
        TextAsset data_configuration = Resources.Load<TextAsset>("API_Configuration/API_Configuration");
        
        if(data_configuration == null)
        {
            Debug.Log("No existe fichero de configuracion");
            return;
        }

        //Decodificamos la informaci�n del JSON de datos
        Hashtable decodedData = (Hashtable)MiniJSON.jsonDecode(data_configuration.text);
        
        verbose = false;
        baseurl = decodedData.GetString("base_url");
        token = decodedData.GetString("token");
        tokenRequestTimeout = decodedData.GetInt("token_request_time_out");
    }
    #endregion

    #region getters and setters
    
    public string GetBaseUrl()
    {
        return baseurl;
    }

    public void SetToken(string token)
    {
        this.token = token;
    }

    public string GetToken()
    {
        return token;
    }
    #endregion

    #region petitions
    public UnityWebRequest CreateApiGetRequest(string actionUrl, object body)
    {
        return CreateApiRequest(actionUrl, UnityWebRequest.kHttpVerbGET, body);
    }

    public  UnityWebRequest CreateApiPostRequest(string actionUrl, object body)
    {
        return CreateApiRequest(actionUrl, UnityWebRequest.kHttpVerbPOST, body);
    }

    private UnityWebRequest CreateApiRequest(string url, string method, object body, bool isImageFile = false)
    {
        url = baseurl + url;
        string bodyString = "";

        var request = new UnityWebRequest
        {
            url = url,
            method = method,
            downloadHandler = new DownloadHandlerBuffer(),
            //certificateHandler = new CertificateWhore() de momento no es necesario usar certificados
            timeout = 5
        };

        if (body != null)
        {
            bodyString = JsonConvert.SerializeObject(body);
            byte[] bodyData = Encoding.UTF8.GetBytes(bodyString);
            request.uploadHandler = new UploadHandlerRaw(bodyData);
            request.SetRequestHeader("Content-Type", "application/json");
            
        }

        if (isImageFile)
        {
            request.SetRequestHeader("Content-Type", "image/jpeg");
        }

        if (token != null)
        {
            request.SetRequestHeader("Authorization", "Bearer " + token);
        }

        request.timeout = tokenRequestTimeout;

        if (verbose)
        {
            Debug.Log("url: " + url + " method: " + method + " header: " +
                request.GetRequestHeader("Content-Type") + " " + request.GetRequestHeader("Authorization") + " bodyString: " + bodyString);
        }

        return request;
    }
    #endregion
}
