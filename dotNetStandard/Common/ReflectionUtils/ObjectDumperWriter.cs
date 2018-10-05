using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace XJK.ReflectionUtils
{
    internal class ObjectDumperWriter
    {
        public string Text => ToString();
        public int ColumnWidth { get; set; } = 40;
        public int IndentSize { get; set; } = 4;
        public int Indent
        {
            get => _indent; set
            {
                Flush();
                _indent = value;
            }
        }

        private readonly bool debug = false;
        private StringBuilder sb = new StringBuilder();
        private int _column;
        private string[] Col;
        private int _indent = 0;

        public ObjectDumperWriter(int column)
        {
            if (column < 2) throw new Exception("DumperWriter column num must >= 2");
            _column = column;
            Col = new string[_column];
        }

        public void Add(int column, string content)
        {
            if (!string.IsNullOrEmpty(Col[column]))
            {
                Flush();
            }
            Col[column] = content;
        }

        private void Flush()
        {
            if (!IsFlushed())
            {
                string s = new string(' ', Indent * IndentSize);
                int c = 0;
                while (c < _column)
                {
                    s += ReplaceLF(Col[c]);
                    int space = ColumnWidth * (c + 1) - new StringInfo(s).LengthInTextElements;
                    while (space < 0)
                    {
                        space += ColumnWidth;
                    }
                    s += new string(' ', space);
                    if (debug) s += "/";
                    Col[c] = "";
                    c++;
                }
                sb.AppendLine(s);
            }
        }

        private bool IsFlushed()
        {
            foreach (var col in Col)
            {
                if (!string.IsNullOrEmpty(col)) return false;
            }
            return true;
        }

        public static string ReplaceLF(string text)
        {
            if (text == null) return "";
            return text.Replace("\r", "\\r").Replace("\n", "\\n");
        }

        public override string ToString()
        {
            Flush();
            return sb.ToString();
        }
    }
}
