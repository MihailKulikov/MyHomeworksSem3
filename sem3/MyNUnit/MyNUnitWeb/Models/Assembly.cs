﻿using System;
using System.Collections.Generic;

namespace MyNUnitWeb.Models
{
    public class Assembly
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Test> Tests { get; set; }
    }
}