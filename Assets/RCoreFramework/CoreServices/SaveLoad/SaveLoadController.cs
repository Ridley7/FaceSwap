using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace r_core.save_load
{
    public class SaveLoadController
    {
        /// <summary>
        /// Metodo que guarda información en disco.
        /// </summary>
        /// <param name="informationToSave">Esta es la información a guardar. Normalmente debe ser un objeto en formato JSON</param>
        /// <param name="filename">Nombre del fichero donde se guarda toda la información</param>
        public void Save(string informationToSave, string filename)
        {
            //Aplicamos encriptación
            string cryptedString = CryptoString.Encrypt(informationToSave);

            //Guardamos en un fichero
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(Application.dataPath + "/" + filename, FileMode.Create);
            binaryFormatter.Serialize(file, cryptedString);

            file.Close();
        }
        
        /// <summary>
        /// Metodo para cargar información de disco
        /// </summary>
        /// <typeparam name="T">Esta es la clase que se pretende recuperar. Normalmente concidira con el valor devuelto</typeparam>
        /// <param name="filename">Ruta donde se encuentra la información</param>
        /// <returns></returns>
        public T Load<T>(string filename)
        {
            if(File.Exists(Application.dataPath + "/" + filename))  
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream file = File.Open(Application.dataPath + "/" + filename, FileMode.Open);
                string data = (string)binaryFormatter.Deserialize(file);
                file.Close();

                //Devolvemos un objeto de la clase que pasamos por parametro <T>
                //no sin antes desencriptarla
                return JsonUtility.FromJson<T>(CryptoString.Decrypt(data));
               
            }

            return default(T);
        }
        
    }
}