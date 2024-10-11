using Newtonsoft.Json;
using r_core.util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace r_core.coresystems.optionalsystems.api
{
    public class R_APIController : R_Singleton<R_APIController>
    {
        private R_PetitionController petition_controller;

        #region Init
        protected override void Initialize(bool dontdestroy = false)
        {
            base.Initialize(dontdestroy);
            
            LoadAPIController();
        }
        
        private void LoadAPIController()
        {
            petition_controller = new R_PetitionController();

            
            if (petition_controller.GetBaseUrl().Length < 1)
            {
                Debug.LogError("No se ha configurado la url base del proyecto");
            }                        
        }
        #endregion

        #region getters ands setters

        public void SetToken(string token)
        {
            petition_controller.SetToken(token);
        }

        public string GetToken()
        {
            return petition_controller.GetToken();
        }

        #endregion

        #region Llamadas

        public IEnumerator GetTileImage<T>(Action<T> lambda)
        {
            //Tengo que hacer un body aqui
            
            var bodyJson = new {
                negative_prompt = "ng_deepnegative_v1_75t, (badhandv4:1.2), (worst quality:2), (low quality:2), (normal quality:2), lowres, bad anatomy, bad hands, ((monochrome)), ((grayscale)) watermark, moles",
                prompt = "dogs",
                width = 1024,
                height = 1024
            };
            

            UnityWebRequest webRequest = petition_controller.CreateApiPostRequest("v3/create-tile", bodyJson);
            yield return webRequest.SendWebRequest();

            //R_Response<T> response = null;
            T response = default(T);

            try{


                if(webRequest.isDone && webRequest.error == null){

                    response = JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
                    lambda(response);
                }
                else if (webRequest.error != null)
                {
                    if(webRequest.error.Contains("Request timeout"))
                    {
                        throw new RequestTimeoutException("El tiempo de petición se ha agotado");
                    }
                    else
                    {
                        Debug.Log(webRequest.error);
                    }
                }

                //ShowIfFailResponse(webRequest);
            }
            catch (RequestTimeoutException timeoutEx)
            {
                Debug.LogError(timeoutEx.Message);
                // Aquí puedo manejar el timeout de forma específica si lo necesito
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }


        public IEnumerator GetTest<T>(Action<T> lambda)
        {
            UnityWebRequest webRequest = petition_controller.CreateApiGetRequest("items/test", null);
            yield return webRequest.SendWebRequest();

            R_Response<T> response = null;

            try
            {
                if (webRequest.isDone && webRequest.error == null)
                {

                    response = JsonConvert.DeserializeObject<R_Response<T>>(webRequest.downloadHandler.text);

                    //Aqui llamamos a la lambda
                    lambda(response.data);
                }

                ShowIfFailResponse(webRequest);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        

        private void ShowIfFailResponse(UnityWebRequest webRequest)
        {
            if (webRequest.isDone && webRequest.error == null)
            {
                Debug.Log("Enseñando respuesta en RAW");
                Debug.Log(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError(webRequest.isDone + " : " + webRequest.error);
            }

        }

        #endregion

    }
}

