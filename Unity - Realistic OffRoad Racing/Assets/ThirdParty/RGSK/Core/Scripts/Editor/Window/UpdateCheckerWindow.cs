using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace RGSK.Editor
{
    [InitializeOnLoad]
    public class UpdateCheckerWindow : EditorWindow
    {
        public static int assetID = 22615;
        int successState;
        Version currentVersion;
        Version latestVersion;

        static bool UpdateCheckPerformed
        {
            get
            {
                return SessionState.GetBool(EditorHelper.UpdateCheckedKey, false);
            }
            set
            {
                SessionState.SetBool(EditorHelper.UpdateCheckedKey, value);
            }
        }

        static UpdateCheckerWindow()
        {
            EditorApplication.update += CheckForUpdatesAfterEditorUpdate;
        }

        public static void ShowWindow()
        {
            var window = GetWindowWithRect<UpdateCheckerWindow>(new Rect(Screen.width / 2, Screen.height / 2, 400, 150), true);
            window.titleContent = new GUIContent("RGSK Update Checker", CustomEditorStyles.MenuIconContent.image);
            window.Show();
        }

        void OnEnable()
        {
            successState = 0;
            _ = CheckForUpdates(OnVersionsRetrieved);
        }

        void OnGUI()
        {
            switch (successState)
            {
                case 0:
                    {
                        EditorGUILayout.LabelField("Checking for updates...", CustomEditorStyles.MenuLabelCenter);
                        break;
                    }

                case 1:
                    {
                        if (currentVersion != null && latestVersion != null)
                        {
                            var updateAvailable = currentVersion < latestVersion;
                            var text = updateAvailable ? "An update is available!\nPlease back up your project before updating." : "You have the latest version!";
                            
                            EditorGUILayout.LabelField(text, CustomEditorStyles.MenuLabelCenter);
                            EditorGUILayout.LabelField($"Current: {currentVersion.ToString()}\nLatest: {latestVersion.ToString()}", CustomEditorStyles.MenuLabelCenter);

                            GUILayout.BeginArea(new Rect(0, position.height - 55, position.width, 100));
                            {
                                using (new GUILayout.HorizontalScope())
                                {
                                    if (updateAvailable && GUILayout.Button("Update", GUILayout.Height(30)))
                                    {
                                        Application.OpenURL(EditorHelper.AssetStoreLink);
                                        Close();
                                    }

                                    if (GUILayout.Button(updateAvailable ? "Later" : "Ok", GUILayout.Height(30)))
                                    {
                                        Close();
                                    }
                                }

                                RGSKCore.Instance.GeneralSettings.autoCheckForUpdates = EditorGUILayout.Toggle("Auto Check For Updates", RGSKCore.Instance.GeneralSettings.autoCheckForUpdates);
                                GUILayout.EndArea();
                            }
                        }
                        break;
                    }

                case -1:
                    {
                        EditorGUILayout.LabelField("Could not retrieve information about the latest version.", CustomEditorStyles.MenuLabelCenter);
                        break;
                    }
            }
        }

        static async void CheckForUpdatesAfterEditorUpdate()
        {
            EditorApplication.update -= CheckForUpdatesAfterEditorUpdate;

            if (!ShouldCheckForUpdates())
                return;

            await CheckForUpdates((success, currentVersion, latestVersion) =>
            {
                if (success && currentVersion < latestVersion)
                {
                    ShowWindow();
                }
            });
        }

        static bool ShouldCheckForUpdates()
        {
            if (!RGSKCore.Instance.GeneralSettings.autoCheckForUpdates)
                return false;

            return UpdateCheckPerformed == false;
        }

        static async Task CheckForUpdates(Action<bool, Version, Version> OnUpdatesChecked)
        {
            UpdateCheckPerformed = true;

            var latestVersionResult = await GetLatestAssetVersion(assetID);

            if (!latestVersionResult.Success)
            {
                OnUpdatesChecked?.Invoke(false, null, null);
                return;
            }

            Version currentVersion = null;
            Version latestVersion = null;

            try
            {
                var latestVersionStr = latestVersionResult.Response["version"].AsString();
                var currentVersionStr = RGSKCore.Instance.versionNumber;

                currentVersion = new Version(currentVersionStr);
                latestVersion = new Version(latestVersionStr);
            }
            catch
            {
                OnUpdatesChecked?.Invoke(false, null, null);
            }

            OnUpdatesChecked?.Invoke(true, currentVersion, latestVersion);
        }

        public static async Task<APIResult> GetLatestAssetVersion(int ID)
        {
            HttpClient httpClient = new HttpClient();

            try
            {
                var url = $"https://api.assetstore.unity3d.com/package/latest-version/{ID}";
                var result = await httpClient.GetAsync(url);

                result.EnsureSuccessStatusCode();

                var resultStr = await result.Content.ReadAsStringAsync();

                var json = JSONParser.SimpleParse(resultStr);

                return new APIResult() { Success = true, Response = json };
            }
            catch (Exception e)
            {
                return new APIResult() { Success = false, Error = ASError.GetGenericError(e) };
            }
        }

        void OnVersionsRetrieved(bool success, Version currentVersion, Version latestVersion)
        {
            if (success)
            {
                this.currentVersion = currentVersion;
                this.latestVersion = latestVersion;
                successState = 1;
            }
            else
            {
                successState = -1;
            }

            Focus();
        }
    }

    #region UNITY ASSET STORE API
    public class APIResult
    {
        public JsonValue Response;
        public bool Success;
        public bool SilentFail;
        public ASError Error;

        public static implicit operator bool(APIResult value)
        {
            return value != null && value.Success != false;
        }
    }

    public class ASError
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }

        public ASError() { }

        public static ASError GetGenericError(Exception ex)
        {
            ASError error = new ASError()
            {
                Message = ex.Message,
                Exception = ex
            };

            return error;
        }

        public static ASError GetLoginError(HttpResponseMessage response) => GetLoginError(response, null);

        public static ASError GetLoginError(HttpResponseMessage response, HttpRequestException ex)
        {
            ASError error = new ASError() { Exception = ex };

            switch (response.StatusCode)
            {
                // Add common error codes here
                case HttpStatusCode.Unauthorized:
                    error.Message = "Incorrect email and/or password. Please try again.";
                    break;
                case HttpStatusCode.InternalServerError:
                    error.Message = "Authentication request failed\nIf you were logging in with your Unity Cloud account, please make sure you are still logged in.\n" +
                        "This might also be caused by too many invalid login attempts - if that is the case, please try again later.";
                    break;
                default:
                    ParseHtmlMessage(response, out string message);
                    error.Message = message;
                    break;
            }

            return error;
        }

        public static ASError GetPublisherNullError(string publisherName)
        {
            ASError error = new ASError
            {
                Message = $"Your Unity ID {publisherName} is not currently connected to a publisher account. " +
                          $"Please create a publisher profile."
            };

            return error;
        }

        static bool ParseHtmlMessage(HttpResponseMessage response, out string message)
        {
            message = "An undefined error has been encountered";
            string html = response.Content.ReadAsStringAsync().Result;

            if (!html.Contains("<!DOCTYPE HTML"))
                return false;

            message += " with the following message:\n\n";
            var startIndex = html.IndexOf("<p>", StringComparison.Ordinal) + "<p>".Length;
            var endIndex = html.IndexOf("</p>", StringComparison.Ordinal);

            if (startIndex == -1 || endIndex == -1)
                return false;

            string htmlBodyMessage = html.Substring(startIndex, (endIndex - startIndex));
            htmlBodyMessage = htmlBodyMessage.Replace("\n", " ");

            message += htmlBodyMessage;
            message += "\n\nIf this error message is not very informative, please report this to Unity";

            return true;
        }

        public override string ToString()
        {
            return Message;
        }
    }

    public struct JsonValue
    {
        public JsonValue(object o)
        {
            data = o;
        }
        public static implicit operator JsonValue(string s)
        {
            return new JsonValue(s);
        }

        public static implicit operator string(JsonValue s)
        {
            return s.AsString();
        }

        public static implicit operator JsonValue(float s)
        {
            return new JsonValue(s);
        }

        public static implicit operator float(JsonValue s)
        {
            return s.AsFloat();
        }

        public static implicit operator JsonValue(bool s)
        {
            return new JsonValue(s);
        }

        public static implicit operator bool(JsonValue s)
        {
            return s.AsBool();
        }

        public static implicit operator JsonValue(int s)
        {
            return new JsonValue((float)s);
        }

        public static implicit operator int(JsonValue s)
        {
            return (int)s.AsFloat();
        }

        public static implicit operator JsonValue(List<JsonValue> s)
        {
            return new JsonValue(s);
        }

        public static implicit operator List<JsonValue>(JsonValue s)
        {
            return s.AsList();
        }

        public static implicit operator Dictionary<string, JsonValue>(JsonValue s)
        {
            return s.AsDict();
        }

        public bool IsString() { return data is string; }
        public bool IsFloat() { return data is float; }
        public bool IsList() { return data is List<JsonValue>; }
        public bool IsDict() { return data is Dictionary<string, JsonValue>; }
        public bool IsBool() { return data is bool; }
        public bool IsNull() { return data == null; }

        public string AsString(bool nothrow = false)
        {
            if (data is string)
                return (string)data;
            if (!nothrow)
                throw new JSONTypeException("Tried to read non-string json value as string");
            return "";
        }
        public float AsFloat(bool nothrow = false)
        {
            if (data is float)
                return (float)data;
            if (!nothrow)
                throw new JSONTypeException("Tried to read non-float json value as float");
            return 0.0f;
        }
        public bool AsBool(bool nothrow = false)
        {
            if (data is bool)
                return (bool)data;
            if (!nothrow)
                throw new JSONTypeException("Tried to read non-bool json value as bool");
            return false;
        }
        public List<JsonValue> AsList(bool nothrow = false)
        {
            if (data is List<JsonValue>)
                return (List<JsonValue>)data;
            if (!nothrow)
                throw new JSONTypeException("Tried to read " + data.GetType().Name + " json value as list");
            return null;
        }
        public Dictionary<string, JsonValue> AsDict(bool nothrow = false)
        {
            if (data is Dictionary<string, JsonValue>)
                return (Dictionary<string, JsonValue>)data;
            if (!nothrow)
                throw new JSONTypeException("Tried to read non-dictionary json value as dictionary");
            return null;
        }

        public static JsonValue NewString(string val)
        {
            return new JsonValue(val);
        }

        public static JsonValue NewFloat(float val)
        {
            return new JsonValue(val);
        }

        public static JsonValue NewDict()
        {
            return new JsonValue(new Dictionary<string, JsonValue>());
        }

        public static JsonValue NewList()
        {
            return new JsonValue(new List<JsonValue>());
        }

        public static JsonValue NewBool(bool val)
        {
            return new JsonValue(val);
        }

        public static JsonValue NewNull()
        {
            return new JsonValue(null);
        }

        public JsonValue InitList()
        {
            data = new List<JsonValue>();
            return this;
        }

        public JsonValue InitDict()
        {
            data = new Dictionary<string, JsonValue>();
            return this;
        }

        public JsonValue this[string index]
        {
            get
            {
                Dictionary<string, JsonValue> dict = AsDict();
                return dict[index];
            }
            set
            {
                if (data == null)
                    data = new Dictionary<string, JsonValue>();
                Dictionary<string, JsonValue> dict = AsDict();
                dict[index] = value;
            }
        }

        public bool ContainsKey(string index)
        {
            if (!IsDict())
                return false;
            return AsDict().ContainsKey(index);
        }

        // Get the specified field in a dict or null json value if
        // no such field exists. The key can point to a nested structure
        // e.g. key1.key2 in  { key1 : { key2 : 32 } }
        public JsonValue Get(string key, out bool found)
        {
            found = false;
            if (!IsDict())
                return new JsonValue(null);
            JsonValue value = this;
            foreach (string part in key.Split('.'))
            {

                if (!value.ContainsKey(part))
                    return new JsonValue(null);
                value = value[part];
            }
            found = true;
            return value;
        }

        public JsonValue Get(string key)
        {
            bool found;
            return Get(key, out found);
        }

        public bool Copy(string key, ref string dest)
        {
            return Copy(key, ref dest, true);
        }

        public bool Copy(string key, ref string dest, bool allowCopyNull)
        {
            bool found;
            JsonValue jv = Get(key, out found);
            if (found && (!jv.IsNull() || allowCopyNull))
                dest = jv.IsNull() ? null : jv.AsString();
            return found;
        }

        public bool Copy(string key, ref bool dest)
        {
            bool found;
            JsonValue jv = Get(key, out found);
            if (found && !jv.IsNull())
                dest = jv.AsBool();
            return found;
        }

        public bool Copy(string key, ref int dest)
        {
            bool found;
            JsonValue jv = Get(key, out found);
            if (found && !jv.IsNull())
                dest = (int)jv.AsFloat();
            return found;
        }

        // Convenience dict value setting
        public void Set(string key, string value)
        {
            Set(key, value, true);
        }
        public void Set(string key, string value, bool allowNull)
        {
            if (value == null)
            {
                if (!allowNull)
                    return;
                this[key] = NewNull();
                return;
            }
            this[key] = NewString(value);
        }

        // Convenience dict value setting
        public void Set(string key, float value)
        {
            this[key] = NewFloat(value);
        }

        // Convenience dict value setting
        public void Set(string key, bool value)
        {
            this[key] = NewBool(value);
        }

        // Convenience list value add
        public void Add(string value)
        {
            List<JsonValue> list = AsList();
            if (value == null)
            {
                list.Add(NewNull());
                return;
            }
            list.Add(NewString(value));
        }

        // Convenience list value add
        public void Add(float value)
        {
            List<JsonValue> list = AsList();
            list.Add(NewFloat(value));
        }

        // Convenience list value add
        public void Add(bool value)
        {
            List<JsonValue> list = AsList();
            list.Add(NewBool(value));
        }

        public override string ToString()
        {
            return ToString(null, "");
        }
        /* 
		 * Serialize a JSON value to string. 
		 * This will recurse down through dicts and list type JSONValues.
		 */
        public string ToString(string curIndent, string indent)
        {
            bool indenting = curIndent != null;

            if (IsString())
            {
                return "\"" + EncodeString(AsString()) + "\"";
            }
            else if (IsFloat())
            {
                return AsFloat().ToString();
            }
            else if (IsList())
            {
                string res = "[";
                string delim = "";
                foreach (JsonValue i in AsList())
                {
                    res += delim + i.ToString();
                    delim = ", ";
                }
                return res + "]";
            }
            else if (IsDict())
            {
                string res = "{" + (indenting ? "\n" : "");
                string delim = "";
                foreach (KeyValuePair<string, JsonValue> kv in AsDict())
                {
                    res += delim + curIndent + indent + '"' + EncodeString(kv.Key) + "\" : " + kv.Value.ToString(curIndent + indent, indent);
                    delim = ", " + (indenting ? "\n" : "");
                }
                return res + (indenting ? "\n" + curIndent : "") + "}";
            }
            else if (IsBool())
            {
                return AsBool() ? "true" : "false";
            }
            else if (IsNull())
            {
                return "null";
            }
            else
            {
                throw new JSONTypeException("Cannot serialize json value of unknown type");
            }
        }



        // Encode a string into a json string
        static string EncodeString(string str)
        {
            str = str.Replace("\\", "\\\\");
            str = str.Replace("\"", "\\\"");
            str = str.Replace("/", "\\/");
            str = str.Replace("\b", "\\b");
            str = str.Replace("\f", "\\f");
            str = str.Replace("\n", "\\n");
            str = str.Replace("\r", "\\r");
            str = str.Replace("\t", "\\t");
            // We do not use \uXXXX specifier but direct unicode in the string.
            return str;
        }

        object data;
    }

    internal class JSONParseException : Exception
    {
        public JSONParseException(string msg) : base(msg)
        {
        }
    }

    internal class JSONTypeException : Exception
    {
        public JSONTypeException(string msg) : base(msg)
        {
        }
    }

    public class JSONParser
    {
        string json;
        int line;
        int linechar;
        int len;
        int idx;
        int pctParsed;
        char cur;

        public static JsonValue SimpleParse(string jsondata)
        {
            var parser = new JSONParser(jsondata);
            try
            {
                return parser.Parse();
            }
            catch (JSONParseException ex)
            {
                Console.WriteLine(ex.Message);
                //DebugUtils.LogError(ex.Message);
            }
            return new JsonValue(null);
        }

        public static bool AssetStoreResponseParse(string responseJson, out ASError error, out JsonValue jval)
        {
            jval = new JsonValue();
            error = null;

            try
            {
                JSONParser parser = new JSONParser(responseJson);
                jval = parser.Parse();
            }
            catch (JSONParseException)
            {
                error = ASError.GetGenericError(new Exception("Error parsing reply from AssetStore"));
                return false;
            }

            // Some json responses return an error field on error
            if (jval.ContainsKey("error"))
            {
                // Server side error message
                // Do not write to console since this is an error that 
                // is "expected" ie. can be handled by the gui.
                error = ASError.GetGenericError(new Exception(jval["error"].AsString(true)));
            }
            // Some json responses return status+message fields instead of an error field. Go figure.
            else if (jval.ContainsKey("status") && jval["status"].AsString(true) != "ok")
            {
                error = ASError.GetGenericError(new Exception(jval["message"].AsString(true)));
            }
            return error == null;
        }

        /*
		 * Setup a parse to be ready for parsing the given string
		 */
        public JSONParser(string jsondata)
        {
            // TODO: fix that parser needs trailing spaces;
            json = jsondata + "    ";
            line = 1;
            linechar = 1;
            len = json.Length;
            idx = 0;
            pctParsed = 0;
        }

        /*
		 * Parse the entire json data string into a JSONValue structure hierarchy
		 */
        public JsonValue Parse()
        {
            cur = json[idx];
            return ParseValue();
        }

        char Next()
        {
            if (cur == '\n')
            {
                line++;
                linechar = 0;
            }
            idx++;
            if (idx >= len)
                throw new JSONParseException("End of json while parsing at " + PosMsg());

            linechar++;

            int newPct = (int)((float)idx * 100f / (float)len);
            if (newPct != pctParsed)
            {
                pctParsed = newPct;
            }
            cur = json[idx];
            return cur;
        }

        void SkipWs()
        {
            const string ws = " \n\t\r";
            while (ws.IndexOf(cur) != -1) Next();
        }

        string PosMsg()
        {
            return "line " + line.ToString() + ", column " + linechar.ToString();
        }

        JsonValue ParseValue()
        {
            // Skip spaces
            SkipWs();

            switch (cur)
            {
                case '[':
                    return ParseArray();
                case '{':
                    return ParseDict();
                case '"':
                    return ParseString();
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return ParseNumber();
                case 't':
                case 'f':
                case 'n':
                    return ParseConstant();
                default:
                    throw new JSONParseException("Cannot parse json value starting with '" + json.Substring(idx, 5) + "' at " + PosMsg());
            }
        }

        JsonValue ParseArray()
        {
            Next();
            SkipWs();
            List<JsonValue> arr = new List<JsonValue>();
            while (cur != ']')
            {
                arr.Add(ParseValue());
                SkipWs();
                if (cur == ',')
                {
                    Next();
                    SkipWs();
                }
            }
            Next();
            return new JsonValue(arr);
        }

        JsonValue ParseDict()
        {
            Next();
            SkipWs();
            Dictionary<string, JsonValue> dict = new Dictionary<string, JsonValue>();
            while (cur != '}')
            {
                JsonValue key = ParseValue();
                if (!key.IsString())
                    throw new JSONParseException("Key not string type at " + PosMsg());
                SkipWs();
                if (cur != ':')
                    throw new JSONParseException("Missing dict entry delimiter ':' at " + PosMsg());
                Next();
                dict.Add(key.AsString(), ParseValue());
                SkipWs();
                if (cur == ',')
                {
                    Next();
                    SkipWs();
                }
            }
            Next();
            return new JsonValue(dict);
        }

        static char[] endcodes = { '\\', '"' };

        JsonValue ParseString()
        {
            string res = "";

            Next();

            while (idx < len)
            {
                int endidx = json.IndexOfAny(endcodes, idx);
                if (endidx < 0)
                    throw new JSONParseException("missing '\"' to end string at " + PosMsg());

                res += json.Substring(idx, endidx - idx);

                if (json[endidx] == '"')
                {
                    cur = json[endidx];
                    idx = endidx;
                    break;
                }

                endidx++; // get escape code
                if (endidx >= len)
                    throw new JSONParseException("End of json while parsing while parsing string at " + PosMsg());

                // char at endidx is \			
                char ncur = json[endidx];
                switch (ncur)
                {
                    case '"':
                        goto case '/';
                    case '\\':
                        goto case '/';
                    case '/':
                        res += ncur;
                        break;
                    case 'b':
                        res += '\b';
                        break;
                    case 'f':
                        res += '\f';
                        break;
                    case 'n':
                        res += '\n';
                        break;
                    case 'r':
                        res += '\r';
                        break;
                    case 't':
                        res += '\t';
                        break;
                    case 'u':
                        // Unicode char specified by 4 hex digits 
                        string digit = "";
                        if (endidx + 4 >= len)
                            throw new JSONParseException("End of json while parsing while parsing unicode char near " + PosMsg());
                        digit += json[endidx + 1];
                        digit += json[endidx + 2];
                        digit += json[endidx + 3];
                        digit += json[endidx + 4];
                        try
                        {
                            int d = Int32.Parse(digit, System.Globalization.NumberStyles.AllowHexSpecifier);
                            res += (char)d;
                        }
                        catch (FormatException)
                        {
                            throw new JSONParseException("Invalid unicode escape char near " + PosMsg());
                        }
                        endidx += 4;
                        break;
                    default:
                        throw new JSONParseException("Invalid escape char '" + ncur + "' near " + PosMsg());
                }
                idx = endidx + 1;
            }
            if (idx >= len)
                throw new JSONParseException("End of json while parsing while parsing string near " + PosMsg());

            cur = json[idx];

            Next();
            return new JsonValue(res);
        }

        JsonValue ParseNumber()
        {
            string resstr = "";

            if (cur == '-')
            {
                resstr = "-";
                Next();
            }

            while (cur >= '0' && cur <= '9')
            {
                resstr += cur;
                Next();
            }
            if (cur == '.')
            {
                Next();
                resstr += '.';
                while (cur >= '0' && cur <= '9')
                {
                    resstr += cur;
                    Next();
                }
            }

            if (cur == 'e' || cur == 'E')
            {
                resstr += "e";
                Next();
                if (cur != '-' && cur != '+')
                {
                    // throw new JSONParseException("Missing - or + in 'e' potent specifier at " + PosMsg());				
                    resstr += cur;
                    Next();
                }
                while (cur >= '0' && cur <= '9')
                {
                    resstr += cur;
                    Next();
                }
            }

            try
            {
                float f = Convert.ToSingle(resstr);
                return new JsonValue(f);
            }
            catch (Exception)
            {
                throw new JSONParseException("Cannot convert string to float : '" + resstr + "' at " + PosMsg());
            }
        }

        JsonValue ParseConstant()
        {
            string c = "" + cur + Next() + Next() + Next();
            Next();
            if (c == "true")
            {
                return new JsonValue(true);
            }
            else if (c == "fals")
            {
                if (cur == 'e')
                {
                    Next();
                    return new JsonValue(false);
                }
            }
            else if (c == "null")
            {
                return new JsonValue(null);
            }
            throw new JSONParseException("Invalid token at " + PosMsg());
        }
    };
    #endregion
}