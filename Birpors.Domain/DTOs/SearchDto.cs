﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Birpors.Domain.DTOs
{
    public class SearchDto
    {
        public int Id { get; set; }

        public int SearchTypeId { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }
    }
}
