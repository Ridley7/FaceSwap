using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace r_core.language
{
    public enum LanguageType
    {
        ENGLISH = 0,
        SPANISH = 1,
        FRENCH = 2,
        CATALAN = 3,
        ITALIAN = 4,
        DEUTSCH = 5,
        JAPANESE = 6,
        //CHINESE = 7,
        NONE = 7,
        MAX = 8
    };

    public class LanguageController
    {
        #region data classes for json
        //this is the serialized json class to read from.
        //we are really interested only in the contained object
        [System.Serializable]
        //Esta es la lista que almacena toda la información de los idiomas
        public class AllLanguageFromJson
        {
            public List<DataLanguages> dataList = new List<DataLanguages>();
        }

        [System.Serializable]
        //Esta son las strings por idioma, normalmente sera key el idioma,
        //y la lista seran todas las cadenas traducidas
        public class DataLanguages
        {
            public string key;
            public List<ObjectLanguage> stringsToRead = new List<ObjectLanguage>();
        }

        [System.Serializable]
        //Esta es una traducción atomica.
        public class ObjectLanguage
        {
            public string key;
            public string text;
        }
        #endregion

        #region json translation files
        //store our translations
        List<DataLanguages> _localeStrings = new List<DataLanguages>();
        #endregion

        #region csv localization vars
        //Parte del CSV que no se va a usar de momento
        //Index of the selected language within the multi-language dictionary
        //this will be set on load the csv file according to the language selected
        /*
        int _languageIndex = -1;

        //Loaded languages, if any
        string[] _languagesListStr = null;
        byte[] _buffer;
        List<string> _tempList = new List<string>();
        int _readingOffset = 0; //store position when reading the buffer
        Dictionary<string, string[]> _localeCSVDict = new Dictionary<string, string[]>();

        bool canRead { get { return (_buffer != null && _readingOffset < _buffer.Length); } }
        */
        #endregion

        //our local language selected enum
        LanguageType _languageSelected;
        string _languageSelectedName;

        //an event to suscribers
        public System.Action OnLanguageChanged;
        public System.Action OnLanguageChangedError;

        #region init
        //our public init function
        public void LoadLanguageFileJson(TextAsset translationFile, LanguageType _myLanguage, System.Action<bool> callback)
        {
            _languageSelected = _myLanguage;
            _languageSelectedName = _languageSelected.ToString();

            LoadTextFileJSON(ref translationFile, (success) =>
            {
                if (success)
                {
                    callback(true);
                }
                else
                {
                    callback(false);
                }
            });
        }

        /*
        Carga con CSV
        public void LoadLanguageFileCSV(TextAsset translationsFile, LanguageType myLanguage, System.Action<bool> callback)
        {
            _languageSelected = myLanguage;
            _languageSelectedName = _languageSelected.ToString();

            _buffer = translationsFile.bytes;

            if(_buffer == null)
            {
                callback(false);
                return;
            }

            if (LoadCSVFile(_buffer, translationsFile))
                callback(true);
            else
                callback(false);
        }
        */
        #endregion

        #region parse json file
        //here's the version with the list, we read the file and store it directly in a list
        //with that list we have the object with a key and within that object the list with translations
        void LoadTextFileJSON(ref TextAsset translationsIndex, System.Action<bool> callback)
        {
            if(translationsIndex == null)
            {
                Debug.LogError("Could not find file in Resources/Translations/TranslationFiles/: " + translationsIndex.name);
                callback(false);
                Resources.UnloadUnusedAssets();
                return;
            }

            //JsonUtility te hace toda la magia de la carga de los JSON a estructura, y al parecer se la pela
            //el nombre del json y las variables locales. Falta comprobar mas la ultima parte.
            AllLanguageFromJson dataInternal = JsonUtility.FromJson<AllLanguageFromJson>(translationsIndex.text);

            for(int i = 0; i < dataInternal.dataList.Count; i++)
            {
                _localeStrings.Add(dataInternal.dataList[i]);
            }

            //clean up a bit after loading all our text
            Resources.UnloadUnusedAssets();

            callback(true);
        }
        #endregion

        #region public functions
        public void Destroy()
        {
            ClearAllStrings();
        }

        public void ClearAllStrings()
        {
            if(_localeStrings != null)
            {
                _localeStrings.Clear();
            }
        }

        //set the nwe language and send the delegate for any listener
        public void SetNewLanguage(LanguageType _newLanguage)
        {
            _languageSelected = _newLanguage;
            _languageSelectedName = _languageSelected.ToString();

            //there's csv content...parte de CSV
            /*
            if(_languagesListStr != null)
            {
                for(int i = 0; i < _languagesListStr.Length; ++i)
                {
                    if(_languagesListStr[i] == _languageSelectedName)
                    {
                        _languageIndex = i;
                        break;
                    }
                }
            }
            */

            if (OnLanguageChanged != null)
                OnLanguageChanged();
        }

        public LanguageType GetLanguage()
        {
            return _languageSelected;
        }

        //Esta es la función de la magia
        public string GetString(string key)
        {
            for(int i = 0; i < _localeStrings.Count; i++)
            {
                DataLanguages data = _localeStrings[i];

                if(data.key == key)
                {
                    for(int cnt = 0; cnt < data.stringsToRead.Count; cnt++)
                    {
                        ObjectLanguage obj = data.stringsToRead[cnt];
                        if(obj.key == _languageSelectedName)
                        {
                            return obj.text;
                        }
                    }
                }
            }

            /*
            string[] values;
            if(_localeCSVDict.TryGetValue(key, out values))
            {
                if(_languageIndex < values.Length)
                {
                    return values[_languageIndex];
                }
            }*/

            return "Not found: " + key;
        }

        //Esta funcion no le encuentro el sentido
        /*
        public string GetString(string key, string lookupExpression, string toReplace)
        {
            string output = System.Text.RegularExpressions.Regex.Replace(GetString("BattleWinner"), lookupExpression, toReplace);
            return output;
        }
        */
        #endregion

        
        //Funcion de CSV que no van a ser usadas
        #region read from file functions and parsing the csv file
        /*
        bool LoadCSVFile(byte [] bytes, TextAsset asset)
        {
            //The first line should contain "KEY", followed by languages.
            List<string> header = ReadCSV();

            // There must be at least two collumns in a valid CSV file
            //first column is the Kye, he second is at least 1 language
            if (header.Count < 2) return false;
            header.RemoveAt(0);

            string[] languagesToAdd = null;

            //Clear the dictionary
            if(_languagesListStr == null || _languagesListStr.Length == 0)
            {
                _localeCSVDict.Clear();

                _languagesListStr = new string[header.Count];

                for(int i = 0; i < header.Count; ++i)
                {
                    _languagesListStr[i] = header[i];

                    if (_languagesListStr[i] == _languageSelected.ToString())
                        _languageIndex = i;
                }
            }
            else
            {
                languagesToAdd = new string[header.Count];

                for (int i = 0; i < header.Count; ++i)
                    languagesToAdd[i] = header[i];

                //Automatically resize the existing languages and add the new language to the mix
                for(int i = 0; i < header.Count; ++i)
                {
                    if (!HasLanguage(header[i]))
                    {
                        int newSize = _languagesListStr.Length + 1;

                        System.Array.Resize(ref _languagesListStr, newSize);

                        _languagesListStr[newSize - 1] = header[i];

                        Dictionary<string, string[]> newDict = new Dictionary<string, string[]>();

                        foreach (KeyValuePair<string, string[]> pair in _localeCSVDict)
                        {
                            string[] arr = pair.Value;

                            System.Array.Resize(ref arr, newSize);

                            arr[newSize - 1] = arr[0];
                            newDict.Add(pair.Key, arr);
                        }
                        _localeCSVDict = newDict;
                    }
                }
            }

            Dictionary<string, int> languageIndices = new Dictionary<string, int>();
            for (int i = 0; i < _languagesListStr.Length; ++i)
                languageIndices.Add(_languagesListStr[i], i);

            // Read the entire CSV file into memory
            for (; ; )
            {
                List<string> temp = ReadCSV();
                if (temp == null || temp.Count == 0) break;
                if (string.IsNullOrEmpty(temp[0])) continue;

                AddCSVToDictionary(temp, languagesToAdd, languageIndices);
            }

            return true;
        }

        string ReadLine(bool skipEmptyLines)
        {
            int max = _buffer.Length;

            // Skip empty characters
            if (skipEmptyLines)
            {
                while (_readingOffset < max && _buffer[_readingOffset] < 32) ++_readingOffset;
            }

            int end = _readingOffset;

            if (end < max)
            {
                for (; ; )
                {
                    if (end < max)
                    {
                        int ch = _buffer[end++];
                        if (ch != '\n' && ch != '\r') continue;
                    }
                    else ++end;

                    string line = ReadLine(_buffer, _readingOffset, end - _readingOffset - 1);
                    _readingOffset = end;
                    return line;
                }
            }
            _readingOffset = max;
            return null;
        }

        string ReadLine(byte[] buffer, int start, int count)
        {
            return Encoding.UTF8.GetString(buffer, start, count);
        }

        List<string> ReadCSV()
        {
            _tempList.Clear();
            string line = "";
            bool insideQuotes = false;
            int wordStart = 0;

            while (canRead)
            {
                if (insideQuotes)
                {
                    string s = ReadLine(false);
                    if (s == null) return null;
                    s = s.Replace("\\n", "\n");
                    line += "\n" + s;
                }
                else
                {
                    line = ReadLine(true);
                    if (line == null) return null;
                    line = line.Replace("\\n", "\n");
                    wordStart = 0;
                }

                for (int i = wordStart, imax = line.Length; i < imax; ++i)
                {
                    char ch = line[i];

                    if (ch == ',')
                    {
                        if (!insideQuotes)
                        {
                            _tempList.Add(line.Substring(wordStart, i - wordStart));
                            wordStart = i + 1;
                        }
                    }
                    else if (ch == '"')
                    {
                        if (insideQuotes)
                        {
                            if (i + 1 >= imax)
                            {
                                _tempList.Add(line.Substring(wordStart, i - wordStart).Replace("\"\"", "\""));
                                return _tempList;
                            }

                            if (line[i + 1] != '"')
                            {
                                _tempList.Add(line.Substring(wordStart, i - wordStart).Replace("\"\"", "\""));
                                insideQuotes = false;

                                if (line[i + 1] == ',')
                                {
                                    ++i;
                                    wordStart = i + 1;
                                }
                            }
                            else ++i;
                        }
                        else
                        {
                            wordStart = i + 1;
                            insideQuotes = true;
                        }
                    }
                }

                if (wordStart < line.Length)
                {
                    if (insideQuotes) continue;
                    _tempList.Add(line.Substring(wordStart, line.Length - wordStart));
                }
                return _tempList;
            }
            return null;
        }

        bool HasLanguage(string languageName)
        {
            for (int i = 0, imax = _languagesListStr.Length; i < imax; ++i)
                if (_languagesListStr[i] == languageName) return true;
            return false;
        }

        void AddCSVToDictionary(List<string> newValues, string[] newLanguages, Dictionary<string, int> languageIndices)
        {
            if (newValues.Count < 2) return;
            string key = newValues[0];

            if (string.IsNullOrEmpty(key)) return;


            string[] copy = ExtractStrings(newValues, newLanguages, languageIndices);

            if (_localeCSVDict.ContainsKey(key))
            {
                _localeCSVDict[key] = copy;
                if (newLanguages == null) Debug.LogWarning("Localization key '" + key + "' is already present");
            }
            else
            {
                try
                {
                    _localeCSVDict.Add(key, copy);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Unable to add '" + key + "' to the Localization dictionary.\n" + ex.Message);
                }
            }
        }

        string[] ExtractStrings(List<string> added, string[] newLanguages, Dictionary<string, int> languageIndices)
        {
            if (newLanguages == null)
            {
                string[] values = new string[_languagesListStr.Length];
                for (int i = 1, max = Mathf.Min(added.Count, values.Length + 1); i < max; ++i)
                    values[i - 1] = added[i];
                return values;
            }
            else
            {
                string[] values;
                string s = added[0];

                if (!_localeCSVDict.TryGetValue(s, out values))
                    values = new string[_languagesListStr.Length];

                for (int i = 0, imax = newLanguages.Length; i < imax; ++i)
                {
                    string language = newLanguages[i];
                    int index = languageIndices[language];
                    values[index] = added[i + 1];
                }
                return values;
            }
        }
        */
        #endregion

    }
}