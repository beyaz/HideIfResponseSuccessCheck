using IntraTextAdornmentSample;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class VariableAssignmentLineTest
    {
        #region Public Methods


        [TestMethod]
        public void Parse0()
        {
            var info = VariableAssignmentLine.Parse("  a = b;  ");

            Assert.AreEqual("a", info.VariableName);
            Assert.AreEqual(null, info.VariableTypeName);
            Assert.AreEqual("b", info.AssignedValue);
        }

        [TestMethod]
        public void Parse0_await()
        {
            var info = VariableAssignmentLine.Parse("  a = await b;  ");

            Assert.AreEqual("a", info.VariableName);
            Assert.AreEqual(null, info.VariableTypeName);
            Assert.AreEqual("await b", info.AssignedValue);
        }


        [TestMethod]
        public void Parse1()
        {
            var info = VariableAssignmentLine.Parse(" var  a = b; ");

            Assert.AreEqual("a", info.VariableName);
            Assert.AreEqual("var", info.VariableTypeName);
            Assert.AreEqual("b", info.AssignedValue);
        }
        [TestMethod]
        public void Parse2()
        {
            var info = VariableAssignmentLine.Parse(" List<object>  a = b.call(77,665, 8); ");

            Assert.AreEqual("a", info.VariableName);
            Assert.AreEqual("List<object>", info.VariableTypeName);
            Assert.AreEqual("b.call(77,665, 8)", info.AssignedValue);
        }

        [TestMethod]
        public void Parse3()
        {
            var info = VariableAssignmentLine.Parse(" ff.g = b; ");

            Assert.IsNull(info);
        }
        #endregion
    }
}