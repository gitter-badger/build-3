using Build;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    namespace TestSet8
    {
        [TestClass]
        public class UnitTest8
        {
            IContainer commonPersonContainer;

            [TestInitialize]
            public void Initialize()
            {
                commonPersonContainer = new Container();
                commonPersonContainer.RegisterType<SqlDataRepository>();
                commonPersonContainer.RegisterType<ServiceDataRepository>();
            }
            [TestMethod]
            public void TestSet8_Method1()
            {
                //TestSet8
                var srv1 = commonPersonContainer.CreateInstance<ServiceDataRepository>();
                Assert.IsNotNull(srv1);
            }
            [TestMethod]
            public void TestSet8_Method2()
            {
                //TestSet8
                var srv2 = commonPersonContainer.CreateInstance<ServiceDataRepository>();
                Assert.IsNotNull(srv2);
            }
            [TestMethod]
            public void TestSet8_Method3()
            {
                //TestSet8
                var srv1 = commonPersonContainer.CreateInstance<ServiceDataRepository>();
                Assert.IsNotNull(srv1.Repository);
            }
            [TestMethod]
            public void TestSet8_Method4()
            {
                //TestSet8
                var srv2 = commonPersonContainer.CreateInstance<ServiceDataRepository>();
                Assert.IsNotNull(srv2.Repository);
            }
            [TestMethod]
            public void TestSet8_Method5()
            {
                //TestSet8
                var srv1 = commonPersonContainer.CreateInstance<ServiceDataRepository>();
                var srv2 = commonPersonContainer.CreateInstance<ServiceDataRepository>();
                Assert.AreNotEqual(srv1, srv2);
            }
            [TestMethod]
            public void TestSet8_Method6()
            {
                //TestSet8
                var srv1 = commonPersonContainer.CreateInstance<ServiceDataRepository>();
                var srv2 = commonPersonContainer.CreateInstance<ServiceDataRepository>();
                Assert.AreNotEqual(srv1.Repository, srv2.Repository);
            }
        }
    }
}
