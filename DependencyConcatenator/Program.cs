using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DependencyConcatenator {
    class Program {
        static void Main(string[] args) {
            string outFile = null;
            var infiles = new string[0];

            for (var i = 0; i < args.Length; i++) {
                switch (args[i]) {
                    case "-o":
                        i++;
                        outFile = args[i];
                        break;
                    default:
                        infiles = new string[args.Length - i];
                        for (var j = 0; i < args.Length; j++) {
                            infiles[j] = args[i];
                            i++;
                        }
                        break;
                }
            }

            if (infiles.Length == 0) return;

            var rootNode = new RootModuleDescriptor(infiles.Select(i => FileModuleDescriptor.Get(i, null)));
            var resolved = new Collection<FileModuleDescriptor>();
            ResolveDependencies(rootNode, resolved, new Collection<FileModuleDescriptor>());

            using (var outFileWriter = new StreamWriter(outFile == null ? Console.OpenStandardOutput() : new FileStream(outFile, FileMode.OpenOrCreate))) {
                foreach (var mod in resolved) {
                    outFileWriter.Write(File.ReadAllText(mod.FileInfo.FullName));
                }
            }
        }

        /*
         * from http://www.electricmonk.nl/docs/dependency_resolving_algorithm/dependency_resolving_algorithm.html
   def dep_resolve(node, resolved, unresolved):
   unresolved.append(node)
   for edge in node.edges:
          if edge not in resolved:
                 if edge in unresolved:
                        raise Exception('Circular reference detected: %s -> %s' % (node.name, edge.name))
                 dep_resolve(edge, resolved, unresolved)
   resolved.append(node)
   unresolved.remove(node)
        */
        static void ResolveDependencies(IModuleDescriptor node, Collection<FileModuleDescriptor> resolved, Collection<FileModuleDescriptor> unresolved) {
            if (node is FileModuleDescriptor) {
                unresolved.Add((FileModuleDescriptor)node);
            }
            foreach (var edge in node.Dependencies) {
                if (!resolved.Any(Compare(edge))) {
                    if (unresolved.Any(Compare(edge))){throw new ArgumentException("circulat dependency: " + edge.FileInfo.FullName);}
                    ResolveDependencies(edge, resolved, unresolved);
                }
            }
            if (node is FileModuleDescriptor) {
                resolved.Add((FileModuleDescriptor) node);
                unresolved.Remove((FileModuleDescriptor)node);
            }
        }

        private static Func<FileModuleDescriptor, bool> Compare(FileModuleDescriptor edge) {
            return m => m.FileInfo.FullName == edge.FileInfo.FullName;
        }
    }
}
