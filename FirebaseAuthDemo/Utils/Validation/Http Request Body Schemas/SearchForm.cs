using FirebaseAuthDemo.Utils.Validation.NewFolder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Utils.Validation.Http_Request_Body_Schemas
{
    public class SearchForm
    {
        [StringEnumerableConstraints(StringMinLength = 1, StringMaxLength = 30, MinSize = 0, MaxSize = 10)]
        public IEnumerable<string> IncludeTags
        { get; set; }

        [StringEnumerableConstraints(StringMinLength = 1, StringMaxLength = 30, MinSize = 0, MaxSize = 10)]
        public IEnumerable<string> ExcludeTags
        { get; set; }
    }
}
