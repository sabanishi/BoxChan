using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

public static class MiniJson
{
    public static object Deserialize(string json)
    {
        if (json == null) return null;
        return Parser.Parse(json);
    }

    public static string Serialize(object obj)
        => Serializer.Serialize(obj);

    private sealed class Parser : IDisposable
    {
        private readonly string _json;
        private int _index;

        private Parser(string json) { _json = json; }

        public static object Parse(string json)
        {
            using var p = new Parser(json);
            return p.ParseValue();
        }

        public void Dispose() { }

        private char Peek => _index < _json.Length ? _json[_index] : '\0';
        private char Next => _index < _json.Length ? _json[_index++] : '\0';

        private void SkipWhitespace()
        {
            while (_index < _json.Length && char.IsWhiteSpace(_json[_index])) _index++;
        }

        private object ParseValue()
        {
            SkipWhitespace();
            var c = Peek;
            if (c == '{') return ParseObject();
            if (c == '[') return ParseArray();
            if (c == '"') return ParseString();
            if (c == '-' || char.IsDigit(c)) return ParseNumber();
            if (Match("true")) return true;
            if (Match("false")) return false;
            if (Match("null")) return null;
            throw new Exception("Invalid JSON");
        }

        private bool Match(string s)
        {
            SkipWhitespace();
            if (_index + s.Length > _json.Length) return false;
            for (int i = 0; i < s.Length; i++)
            {
                if (_json[_index + i] != s[i]) return false;
            }
            _index += s.Length;
            return true;
        }

        private IDictionary<string, object> ParseObject()
        {
            // {
            var obj = new Dictionary<string, object>();
            var _ = Next;
            while (true)
            {
                SkipWhitespace();
                if (Peek == '}')
                {
                    var _2 = Next; 
                    break;
                }

                var key = ParseString();
                SkipWhitespace();
                if (Next != ':') throw new Exception("Expected ':'");

                var val = ParseValue();
                obj[key] = val;

                SkipWhitespace();
                var ch = Next;
                if (ch == '}') break;
                if (ch != ',') throw new Exception("Expected ',' or '}'");
            }
            return obj;
        }

        private IList<object> ParseArray()
        {
            var arr = new List<object>();
            var _ = Next; // [
            while (true)
            {
                SkipWhitespace();
                if (Peek == ']')
                {
                    var _2 = Next;
                    break;
                }

                arr.Add(ParseValue());

                SkipWhitespace();
                var ch = Next;
                if (ch == ']') break;
                if (ch != ',') throw new Exception("Expected ',' or ']'");
            }
            return arr;
        }

        private string ParseString()
        {
            if (Next != '"') throw new Exception("Expected '\"'");
            var sb = new StringBuilder();
            while (true)
            {
                if (_index >= _json.Length) throw new Exception("Unterminated string");
                var c = Next;
                if (c == '"') break;
                if (c == '\\')
                {
                    if (_index >= _json.Length) throw new Exception("Bad escape");
                    var e = Next;
                    switch (e)
                    {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'u':
                            var hex = _json.Substring(_index, 4);
                            sb.Append((char)Convert.ToInt32(hex, 16));
                            _index += 4;
                            break;
                        default: throw new Exception("Bad escape");
                    }
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }

        private object ParseNumber()
        {
            SkipWhitespace();
            int start = _index;
            if (Peek == '-') _index++;
            while (char.IsDigit(Peek)) _index++;
            if (Peek == '.')
            {
                _index++;
                while (char.IsDigit(Peek)) _index++;
            }
            if (Peek == 'e' || Peek == 'E')
            {
                _index++;
                if (Peek == '+' || Peek == '-') _index++;
                while (char.IsDigit(Peek)) _index++;
            }
            var s = _json.Substring(start, _index - start);

            // JSON numberはdoubleで受ける
            return double.Parse(s, CultureInfo.InvariantCulture);
        }
    }

    private sealed class Serializer
    {
        private readonly StringBuilder _sb = new StringBuilder();

        public static string Serialize(object obj)
        {
            var s = new Serializer();
            s.SerializeValue(obj);
            return s._sb.ToString();
        }

        private void SerializeValue(object v)
        {
            switch (v)
            {
                case null: _sb.Append("null"); break;
                case string str: SerializeString(str); break;
                case bool b: _sb.Append(b ? "true" : "false"); break;
                case IDictionary dict: SerializeObject(dict); break;
                case IList list: SerializeArray(list); break;
                case double d: _sb.Append(d.ToString(CultureInfo.InvariantCulture)); break;
                case float f: _sb.Append(f.ToString(CultureInfo.InvariantCulture)); break;
                case int i: _sb.Append(i.ToString(CultureInfo.InvariantCulture)); break;
                case long l: _sb.Append(l.ToString(CultureInfo.InvariantCulture)); break;
                default:
                    // それ以外は ToString で逃げる（必要なら拡張）
                    SerializeString(v.ToString());
                    break;
            }
        }

        private void SerializeObject(IDictionary obj)
        {
            bool first = true;
            _sb.Append('{');
            foreach (DictionaryEntry e in obj)
            {
                if (!first) _sb.Append(',');
                first = false;
                SerializeString((string)e.Key);
                _sb.Append(':');
                SerializeValue(e.Value);
            }
            _sb.Append('}');
        }

        private void SerializeArray(IList arr)
        {
            bool first = true;
            _sb.Append('[');
            foreach (var v in arr)
            {
                if (!first) _sb.Append(',');
                first = false;
                SerializeValue(v);
            }
            _sb.Append(']');
        }

        private void SerializeString(string str)
        {
            _sb.Append('"');
            foreach (var c in str)
            {
                switch (c)
                {
                    case '"': _sb.Append("\\\""); break;
                    case '\\': _sb.Append("\\\\"); break;
                    case '\b': _sb.Append("\\b"); break;
                    case '\f': _sb.Append("\\f"); break;
                    case '\n': _sb.Append("\\n"); break;
                    case '\r': _sb.Append("\\r"); break;
                    case '\t': _sb.Append("\\t"); break;
                    default:
                        if (c < 32) _sb.Append("\\u" + ((int)c).ToString("x4"));
                        else _sb.Append(c);
                        break;
                }
            }
            _sb.Append('"');
        }
    }
}
