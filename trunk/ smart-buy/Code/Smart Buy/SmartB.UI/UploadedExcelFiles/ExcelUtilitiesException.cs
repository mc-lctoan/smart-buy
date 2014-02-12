using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartB.UI.UploadedExcelFiles
{
    [Serializable()]
    public class InvalidNumberException : Exception
    {
        public InvalidNumberException()
            : base()
        {
        }
        public InvalidNumberException(string message)
            : base(message) { }

        public InvalidNumberException(int value)
        {
        }
        public InvalidNumberException(string message, Exception e) : base(message, e) { }

    }
}
