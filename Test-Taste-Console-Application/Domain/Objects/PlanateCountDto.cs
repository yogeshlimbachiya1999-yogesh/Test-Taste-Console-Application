using System;
using System.Collections.Generic;
using System.Text;

namespace Test_Taste_Console_Application.Domain.Objects
{
    public class PlanateCountDto
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string AverageTemprature { get; set; }

        public List<MoonCountDto> Moons { get; set; } = new();
    }
}
