using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public static class LocalizationSystem
{
    // 現在の言語
    public static string CurrentLanguage { get; private set; } = "ja"; // デフォルトは日本語
    
    // 翻訳データ
    private static Dictionary<string, string> translationData = new Dictionary<string, string>();
    
    // 利用可能な言語
    private static readonly string[] availableLanguages = { "ja", "en" };
    
    // システムの初期化
    public static void Initialize()
    {
        // デバイスの言語設定を取得
        SystemLanguage deviceLanguage = Application.systemLanguage;
        
        // デバイス言語に基づいて言語を設定
        switch (deviceLanguage)
        {
            case SystemLanguage.Japanese:
                CurrentLanguage = "ja";
                break;
            case SystemLanguage.English:
                CurrentLanguage = "en";
                break;
            default:
                // デフォルトは英語
                CurrentLanguage = "en";
                break;
        }
        
        // 初期言語のロード
        LoadLanguage(CurrentLanguage);
    }
    
    // 言語をロードするメソッド
    public static void LoadLanguage(string languageCode)
    {
        // 指定された言語コードが有効かチェック
        if (System.Array.IndexOf(availableLanguages, languageCode) >= 0)
        {
            CurrentLanguage = languageCode;
            LoadTranslationData(languageCode);
        }
        else
        {
            Debug.LogWarning($"Unsupported language code: {languageCode}. Using default language (ja) instead.");
            CurrentLanguage = "ja";
            LoadTranslationData("ja");
        }
    }
    
    // 翻訳データをロードするメソッド
    private static void LoadTranslationData(string languageCode)
    {
        // 翻訳データをクリア
        translationData.Clear();
        
        // 言語ファイルのパス
        string filePath = $"Data/Localization/{languageCode}";
        
        // リソースファイルからJSONをロード
        TextAsset textAsset = Resources.Load<TextAsset>(filePath);
        
        if (textAsset != null)
        {
            // JSONデータをパース
            Dictionary<string, object> jsonData = MiniJSON.Json.Deserialize(textAsset.text) as Dictionary<string, object>;
            
            if (jsonData != null)
            {
                // 翻訳データをディクショナリーに格納
                foreach (KeyValuePair<string, object> entry in jsonData)
                {
                    translationData[entry.Key] = entry.Value.ToString();
                }
                
                Debug.Log($"Loaded {translationData.Count} translations for language: {languageCode}");
            }
            else
            {
                Debug.LogError($"Failed to parse translation data for language: {languageCode}");
            }
        }
        else
        {
            Debug.LogError($"Translation file not found for language: {languageCode}");
        }
    }
    
    // 翻訳を取得するメソッド
    public static string GetTranslation(string key)
    {
        if (translationData.TryGetValue(key, out string translation))
        {
            return translation;
        }
        else
        {
            // キーが見つからない場合はキーをそのまま返す
            Debug.LogWarning($"Translation key not found: {key}");
            return key;
        }
    }
    
    // 翻訳が存在するかチェックするメソッド
    public static bool HasTranslation(string key)
    {
        return translationData.ContainsKey(key);
    }
    
    // 利用可能な言語を取得するメソッド
    public static string[] GetAvailableLanguages()
    {
        return availableLanguages;
    }
    
    // 翻訳を適用するメソッド（書式指定子対応）
    public static string Format(string key, params object[] args)
    {
        string translation = GetTranslation(key);
        
        if (args != null && args.Length > 0)
        {
            // 書式指定子を適用
            return string.Format(translation, args);
        }
        else
        {
            return translation;
        }
    }
}

// MiniJSON - 軽量JSONパーサー（Unityの公式サンプルから）
// Unity Technologies提供のJSONパーサー
// https://gist.github.com/darktable/1411710
public class MiniJSON
{
    public static class Json
    {
        public static object Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            
            return Parser.Parse(json);
        }
        
        public static string Serialize(object obj)
        {
            return Serializer.Serialize(obj);
        }
        
        private sealed class Parser : IDisposable
        {
            private const string WORD_BREAK = "{}[],:\"";
            
            private StringReader json;
            
            private Parser(string jsonString)
            {
                json = new StringReader(jsonString);
            }
            
            public static object Parse(string jsonString)
            {
                using (var instance = new Parser(jsonString))
                {
                    return instance.ParseValue();
                }
            }
            
            public void Dispose()
            {
                json.Dispose();
                json = null;
            }
            
            private object ParseValue()
            {
                EatWhitespace();
                
                var c = PeekChar();
                
                if (c == '"')
                {
                    return ParseString();
                }
                else if (c == '{')
                {
                    return ParseObject();
                }
                else if (c == '[')
                {
                    return ParseArray();
                }
                else if (c == '-' || char.IsDigit(c))
                {
                    return ParseNumber();
                }
                else if (c == 't' || c == 'f')
                {
                    return ParseBoolean();
                }
                else if (c == 'n')
                {
                    return ParseNull();
                }
                
                return null;
            }
            
            private Dictionary<string, object> ParseObject()
            {
                Dictionary<string, object> obj = new Dictionary<string, object>();
                
                // 開始波括弧
                json.Read();
                
                while (true)
                {
                    EatWhitespace();
                    
                    if (PeekChar() == '}')
                    {
                        json.Read();
                        return obj;
                    }
                    
                    // キー
                    string key = ParseString();
                    
                    // コロン
                    EatWhitespace();
                    if (json.Read() != ':')
                    {
                        throw new System.Exception("Expected ':' in object");
                    }
                    
                    // 値
                    obj[key] = ParseValue();
                    
                    // カンマまたは閉じ波括弧
                    EatWhitespace();
                    char c = (char)json.Read();
                    if (c == '}')
                    {
                        return obj;
                    }
                    else if (c != ',')
                    {
                        throw new System.Exception("Expected ',' or '}' in object");
                    }
                }
            }
            
            private List<object> ParseArray()
            {
                List<object> array = new List<object>();
                
                // 開始角括弧
                json.Read();
                
                while (true)
                {
                    EatWhitespace();
                    
                    if (PeekChar() == ']')
                    {
                        json.Read();
                        return array;
                    }
                    
                    // 値
                    array.Add(ParseValue());
                    
                    // カンマまたは閉じ角括弧
                    EatWhitespace();
                    char c = (char)json.Read();
                    if (c == ']')
                    {
                        return array;
                    }
                    else if (c != ',')
                    {
                        throw new System.Exception("Expected ',' or ']' in array");
                    }
                }
            }
            
            private string ParseString()
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                
                // 開始ダブルクォート
                json.Read();
                
                while (true)
                {
                    int c = json.Read();
                    if (c < 0)
                    {
                        throw new System.Exception("Unterminated string");
                    }
                    else if (c == '"')
                    {
                        return sb.ToString();
                    }
                    else if (c == '\\')
                    {
                        c = json.Read();
                        switch (c)
                        {
                            case '"':
                                sb.Append('"');
                                break;
                            case '\\':
                                sb.Append('\\');
                                break;
                            case '/':
                                sb.Append('/');
                                break;
                            case 'b':
                                sb.Append('\b');
                                break;
                            case 'f':
                                sb.Append('\f');
                                break;
                            case 'n':
                                sb.Append('\n');
                                break;
                            case 'r':
                                sb.Append('\r');
                                break;
                            case 't':
                                sb.Append('\t');
                                break;
                            case 'u':
                                int u = 0;
                                for (int i = 0; i < 4; i++)
                                {
                                    c = json.Read();
                                    switch (c)
                                    {
                                        case '0': case '1': case '2': case '3': case '4':
                                        case '5': case '6': case '7': case '8': case '9':
                                            u = (u << 4) | (c - '0');
                                            break;
                                        case 'a': case 'b': case 'c': case 'd': case 'e': case 'f':
                                            u = (u << 4) | (c - 'a' + 10);
                                            break;
                                        case 'A': case 'B': case 'C': case 'D': case 'E': case 'F':
                                            u = (u << 4) | (c - 'A' + 10);
                                            break;
                                        default:
                                            throw new System.Exception("Illegal unicode escape sequence");
                                    }
                                }
                                sb.Append((char)u);
                                break;
                            default:
                                throw new System.Exception("Illegal escape sequence");
                        }
                    }
                    else
                    {
                        sb.Append((char)c);
                    }
                }
            }
            
            private object ParseNumber()
            {
                string number = "";
                bool isFloat = false;
                
                while (true)
                {
                    char c = PeekChar();
                    if (c == 'e' || c == 'E' || c == '.' || c == '-' || c == '+')
                    {
                        isFloat = true;
                    }
                    
                    if ((c < '0' || c > '9') &&
                        c != 'e' && c != 'E' &&
                        c != '.' && c != '-' && c != '+')
                    {
                        break;
                    }
                    
                    number += (char)json.Read();
                }
                
                if (isFloat)
                {
                    double parsedDouble;
                    if (double.TryParse(number, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out parsedDouble))
                    {
                        return parsedDouble;
                    }
                }
                else
                {
                    int parsedInt;
                    if (int.TryParse(number, out parsedInt))
                    {
                        return parsedInt;
                    }
                    
                    long parsedLong;
                    if (long.TryParse(number, out parsedLong))
                    {
                        return parsedLong;
                    }
                }
                
                // 数値パースに失敗した場合は文字列として返す
                return number;
            }
            
            private bool ParseBoolean()
            {
                if (PeekChar() == 't')
                {
                    json.Read(); // t
                    json.Read(); // r
                    json.Read(); // u
                    json.Read(); // e
                    return true;
                }
                else
                {
                    json.Read(); // f
                    json.Read(); // a
                    json.Read(); // l
                    json.Read(); // s
                    json.Read(); // e
                    return false;
                }
            }
            
            private object ParseNull()
            {
                json.Read(); // n
                json.Read(); // u
                json.Read(); // l
                json.Read(); // l
                return null;
            }
            
            private void EatWhitespace()
            {
                while (char.IsWhiteSpace(PeekChar()))
                {
                    json.Read();
                }
            }
            
            private char PeekChar()
            {
                int c = json.Peek();
                return c >= 0 ? (char)c : '\0';
            }
        }
        
        private sealed class Serializer
        {
            private System.Text.StringBuilder builder;
            
            private Serializer()
            {
                builder = new System.Text.StringBuilder();
            }
            
            public static string Serialize(object obj)
            {
                var instance = new Serializer();
                instance.SerializeValue(obj);
                return instance.builder.ToString();
            }
            
            private void SerializeValue(object value)
            {
                if (value == null)
                {
                    builder.Append("null");
                }
                else if (value is string)
                {
                    SerializeString((string)value);
                }
                else if (value is bool)
                {
                    builder.Append((bool)value ? "true" : "false");
                }
                else if (value is float ||
                         value is double ||
                         value is int ||
                         value is uint ||
                         value is long ||
                         value is ulong ||
                         value is short ||
                         value is ushort ||
                         value is sbyte ||
                         value is byte)
                {
                    SerializeNumber(value);
                }
                else if (value is Dictionary<string, object> ||
                         value is System.Collections.IDictionary)
                {
                    SerializeObject(value as System.Collections.IDictionary);
                }
                else if (value is List<object> ||
                         value is System.Collections.IList)
                {
                    SerializeArray(value as System.Collections.IList);
                }
                else
                {
                    SerializeString(value.ToString());
                }
            }
            
            private void SerializeObject(System.Collections.IDictionary obj)
            {
                builder.Append('{');
                
                bool first = true;
                foreach (System.Collections.DictionaryEntry entry in obj)
                {
                    if (!first)
                    {
                        builder.Append(',');
                    }
                    
                    SerializeString(entry.Key.ToString());
                    builder.Append(':');
                    SerializeValue(entry.Value);
                    
                    first = false;
                }
                
                builder.Append('}');
            }
            
            private void SerializeArray(System.Collections.IList array)
            {
                builder.Append('[');
                
                bool first = true;
                foreach (object value in array)
                {
                    if (!first)
                    {
                        builder.Append(',');
                    }
                    
                    SerializeValue(value);
                    
                    first = false;
                }
                
                builder.Append(']');
            }
            
            private void SerializeString(string str)
            {
                builder.Append('"');
                
                foreach (char c in str)
                {
                    switch (c)
                    {
                        case '"':
                            builder.Append("\\\"");
                            break;
                        case '\\':
                            builder.Append("\\\\");
                            break;
                        case '\b':
                            builder.Append("\\b");
                            break;
                        case '\f':
                            builder.Append("\\f");
                            break;
                        case '\n':
                            builder.Append("\\n");
                            break;
                        case '\r':
                            builder.Append("\\r");
                            break;
                        case '\t':
                            builder.Append("\\t");
                            break;
                        default:
                            if (c < ' ')
                            {
                                builder.Append("\\u");
                                builder.Append(((int)c).ToString("X4"));
                            }
                            else
                            {
                                builder.Append(c);
                            }
                            break;
                    }
                }
                
                builder.Append('"');
            }
            
            private void SerializeNumber(object number)
            {
                builder.Append(Convert.ToString(number, System.Globalization.CultureInfo.InvariantCulture));
            }
        }
    }
}
