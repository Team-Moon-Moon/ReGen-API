﻿using FirebaseAuthDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Services
{
    public interface ISearchService
    {
        Task<IEnumerable<object>> SearchUnfilteredAsync(string recipeName);

        Task<IEnumerable<object>> SearchFilteredAsync(string recipeName, IEnumerable<string> includes, IEnumerable<string> excludes);
    }
}
