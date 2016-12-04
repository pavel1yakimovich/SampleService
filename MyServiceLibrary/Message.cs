using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServiceLibrary
{
    [Serializable]
    public enum Operation
    {
        Add,
        AddRange,
        Remove
    }

    [Serializable]
    public class Message
    {
        public Operation Operation { get; set; }

        public object Parameter { get; set; }

        public Message(Operation op, object param)
        {
            this.Operation = op;
            this.Parameter = param;
        }
    }
}
