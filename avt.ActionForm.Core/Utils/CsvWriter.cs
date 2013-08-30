using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections;

namespace avt.ActionForm
{
    public class CsvWriter
    {
        StringBuilder _sbCsv = new StringBuilder();
        StringBuilder _sbBuf = new StringBuilder();

        bool _isRowStarted = false;
        bool _isFirstRow = true;
        bool _bAlwaysEncloseInQuotes = false;

        public CsvWriter(bool bAlwaysEncloseInQuotes = false)
        {
            _bAlwaysEncloseInQuotes = bAlwaysEncloseInQuotes;
            _isFirstRow = true;
        }

        /// <summary>
        /// write directly to the file!!!
        /// </summary>
        /// <param name="value"></param>
        public CsvWriter __WriteRaw(object value)
        {
            if (_isFirstRow) {
                _isFirstRow = false;
            } else {
                _sbCsv.AppendLine();
            }

            _sbCsv.Append(value.ToString().Trim());
            return this;
        }

        public CsvWriter Write(object value)
        {
            if (!_isRowStarted) {
                if (_isFirstRow) {
                    _isFirstRow = false;
                } else {
                    _sbBuf.AppendLine();
                }
                _isRowStarted = true;
            }

            _sbBuf.Append(Encode(value));
            _sbBuf.Append(",");
            return this;
        }

        public CsvWriter EndRow()
        {
            // write buffer
            if (_sbBuf.Length > 0) {
                _sbBuf.Remove(_sbBuf.Length - 1, 1);
                _sbCsv.Append(_sbBuf.ToString());
            }

            // and reset state
            _isRowStarted = false;
            _sbBuf = new StringBuilder();
            return this;
        }

        protected virtual string Encode(object value)
        {
            if (value == null)
                return _bAlwaysEncloseInQuotes ? "\"\"" : "";

            string output = value.ToString();
            if (_bAlwaysEncloseInQuotes || output.Contains(",") || output.Contains("\"") || output.Contains("\n"))
                output = '"' + output.Replace("\"", "\"\"") + '"';

            return output;
        }

        public override string ToString()
        {
            return _sbCsv.ToString();
        }

    }
}
