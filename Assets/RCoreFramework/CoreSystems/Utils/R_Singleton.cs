using UnityEngine;
using UnityEditor.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace r_core.util
{
	public class R_Singleton<T> : MonoBehaviour where T : R_Singleton<T>
	{
		private static T instance;
		private static bool isAppClosing = false;
		private static bool isCreated = false;
        private bool initialized { get; set; }

        private static object syncRoot = new Object();

		public static T GetInstance()
		{
			if (isAppClosing)
			{
				return null;
			}

			if (!Application.isPlaying) return null;

			if (isCreated) return instance;
			
			lock (syncRoot)
			{
                //null, try to get the object if it's on the scene
                instance = GameObject.FindObjectOfType<T>();

				//still null?
				if (!instance)
				{
                    //create new instance
                    GameObject go = new GameObject(typeof(T).Name);
                    instance = go.AddComponent<T>();
                }

				isCreated = true;

				//initialize instance if necessary
				if (!instance.initialized)
				{
					instance.Initialize();
					instance.initialized = true;
				}
				
			}

			return instance;

		}


		private void Awake()
		{
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				return;
			}
#endif
			if(instance == null)
			{
				GetInstance();
			}
		}

        private void OnApplicationQuit()
        {
			isAppClosing = true;
			isCreated = false;
			instance = null;
        }

        public virtual void OnDestroy()
		{
			isCreated = false;
			initialized = false;
			instance = null;
		}

		protected virtual void Initialize(bool dontdestroy = false)
		{
			if (dontdestroy)
			{
				DontDestroyOnLoad(instance.gameObject);
			}
		}

	}

}