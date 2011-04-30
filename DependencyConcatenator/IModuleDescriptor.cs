using System.Collections.Generic;

namespace DependencyConcatenator {
    internal interface IModuleDescriptor {
        IEnumerable<FileModuleDescriptor> Dependencies { get; }
    }
}