using System;
using System.Collections.Generic;

namespace r_core.coresystems.optionalsystems.messages
{
    public static class R_MessagesController<U>
    {
        //Diccionario donde se guardan todos los mensajes
        private static Dictionary<int, List<Action<U>>> messageTable = new Dictionary<int, List<Action<U>>>(500);

        /// <summary>
        /// Funcion para añadir una escucha a un tipo de evento indicado
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="handler"></param>
        public static void AddObserver(int messageType, Action<U> handler)
        {
            List<Action<U>> list = null;

            if(!messageTable.TryGetValue(messageType, out list))
            {
                list = new List<Action<U>>();
                messageTable.Add(messageType, list);
            }

            if (!list.Contains(handler))
            {
                messageTable[messageType].Add(handler);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="handler"></param>
        public static void RemoveObserver(int messageType, Action<U> handler)
        {
            List<Action<U>> list = null;

            if(messageTable.TryGetValue(messageType, out list))
            {
                if (list.Contains(handler))
                {
                    list.Remove(handler);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="param"></param>
        public static void Post(int messageType, U param)
        {
            List<Action<U>> list = null;

            if(messageTable.TryGetValue(messageType, out list))
            {
                if (list.Count == 0) return;

                for(var i = list.Count - 1; i > -1; i--)
                {
                    list[i](param);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageType"></param>
        public static void ClearMessageTable(int messageType)
        {
            if (messageTable.ContainsKey(messageType))
            {
                messageTable.Remove(messageType);
            }
        }

        public static void ClearMessageTable()
        {
            messageTable.Clear();
        }

    }

}