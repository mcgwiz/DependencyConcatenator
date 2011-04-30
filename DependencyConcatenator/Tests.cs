using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DependencyConcatenator {
    // note: for now, tests must be run in debug config.
    [TestClass]
    public class Tests {
        [TestMethod]
        public void Get_ShouldCreate_WhenNewFilePassed() {
            var mod = FileModuleDescriptor.Get(@"..\..\..\file1.css", null);
            Assert.IsNotNull(mod);
        }

        [TestMethod]
        public void Get_ShouldNotCreate_WhenExistingFilePassed() {
            var created = FileModuleDescriptor.Get(@"..\..\..\file1.css", null);
            var existing = FileModuleDescriptor.Get(@"..\..\..\file1.css", null);
            Assert.AreSame(created,existing);
        }

        [TestMethod]
        public void Dependencies_ShouldReturnOne_WhenPassedFile1DotCss() {
            var mod = FileModuleDescriptor.Get(@"..\..\..\file1.css", null);
            Assert.AreEqual("file2.css",mod.Dependencies.First().FileInfo.Name);
        }
    }
    
}
