﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace onlineKredit.web.Models
{
    public class PersönlicheDatenModel
    {
        [HiddenInput(DisplayValue = false)]
        [Required]
        public int ID_Kunde { get; set; }
    }
}