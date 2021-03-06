﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bouchon.API.BindingModels
{
    public class UsersInRoleBindingModel
    {
        public string Id { get; set; }
        public List<string> EnrolledUsers { get; set; }
        public List<string> RemovedUsers { get; set; }
    }
}