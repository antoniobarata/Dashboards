using System;
using System.Collections.Generic;
using System.Text;

namespace Updater.Models
{
    public class FileManifestModel
    {
        public string FileFullName { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public Commit LastCommit { get; set; }
        public dynamic Default { get; internal set; }
    }

    public class Commit
    {
        public string ModifiedOn { get; set; }
        public Author ModifiedBy { get; set; }
        public Author Commits { get; set; }
        public Author Changes { get; set; }
    }

    public class Author
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }

    }
}