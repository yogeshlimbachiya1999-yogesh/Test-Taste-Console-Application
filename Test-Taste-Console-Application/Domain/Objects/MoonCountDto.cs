using System;
using System.Collections.Generic;
using System.Text;

namespace Test_Taste_Console_Application.Domain.Objects
{
    public class MoonCountDto
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public double? AverageTemperature { get; set; }

        public double? MassValue { get; set; }

        public int? MassExponent { get; set; }
    }
}
