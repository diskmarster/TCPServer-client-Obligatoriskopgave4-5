﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    public class JsonRequest
    {
        public string RequestMethod { get; set; }

        public int Number1 { get; set; }
        public int Number2 { get; set; }

    }
}