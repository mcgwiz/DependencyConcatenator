using System.Collections.Generic;

namespace DependencyConcatenator {
    class RootModuleDescriptor : IModuleDescriptor  {
        public RootModuleDescriptor(IEnumerable<FileModuleDescriptor> deps) {
            Dependencies = deps;
        }

        public IEnumerable<FileModuleDescriptor> Dependencies { get; private set; }
    }
}