using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DependencyConcatenator {
    class FileModuleDescriptor : IModuleDescriptor {
        private FileModuleDescriptor() { }

        public DirectoryInfo DirectoryInfo { get; private set; }
        public FileInfo FileInfo { get; private set; }
        public IEnumerable<FileModuleDescriptor> Dependencies {
            get {
                if (_dependencies == null) {
                    var deps = new HashSet<FileModuleDescriptor>();

                    using(var file = new StreamReader(FileInfo.FullName) ) {
                        while(true){
                            string line;
                            do {
                                line = file.ReadLine().Trim();
                            } while (string.IsNullOrEmpty(line));
                            
                            var dependency = Regex.Match(line, @"/\*requires (.*?)\s*\*/");
                            if (dependency.Groups.Count < 2 && dependency.Groups[0].Captures.Count == 0) break;
                            var dep = dependency.Groups[1].Captures[0].Value;
                            if (string.IsNullOrEmpty(dep)) break;
                            deps.Add(Get(dep, DirectoryInfo.FullName));
                        }
                    }
                    _dependencies = deps;
                }
                return _dependencies;
            }
        }

        static Dictionary<string, FileModuleDescriptor> _cache = new Dictionary<string, FileModuleDescriptor>();
        private HashSet<FileModuleDescriptor> _dependencies;

        public static FileModuleDescriptor Get(string filename, string root) {
            FileInfo fileInfo;
            if (root == null) fileInfo = new FileInfo(Path.GetFullPath(filename));
            else fileInfo = new FileInfo(Path.Combine(root,filename));

            if (_cache.ContainsKey(fileInfo.FullName))
                return _cache[fileInfo.FullName];

            var mod = new FileModuleDescriptor { FileInfo = fileInfo, DirectoryInfo = fileInfo.Directory};
            _cache[mod.FileInfo.FullName] = mod;
            return mod;
        }

    }
}