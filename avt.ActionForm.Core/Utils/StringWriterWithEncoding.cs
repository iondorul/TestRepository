using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace avt.ActionForm.Utils
{
    public class StringWriterWithEncoding : StringWriter
    {
        Encoding encoding;

        public StringWriterWithEncoding(StringBuilder builder, Encoding encoding)
            : base(builder)
        {
            this.encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return encoding; }
        }
    }
}
