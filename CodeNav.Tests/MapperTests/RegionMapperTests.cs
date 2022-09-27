using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;

using System;
using System.IO;
using System.Linq;

namespace CodeNav.Tests.MapperTests
{
    [TestClass]
    public class RegionMapperTests
    {
        [TestMethod]
        public async Task TestRegionsAsync()
        {
            var document = await SyntaxMapper.MapDocumentAsync($@"Files\TestRegions.cs");

            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);

            // There should be a single class
            var regionClass = (document.First() as IMembers).Members.First() as CodeClassItem;
            Assert.IsNotNull(regionClass);

            // The class should have a function in it
            Assert.IsNotNull(regionClass.Members.FirstOrDefault(m => m.Name.Equals("OutsideRegionFunction")));

            // The class should have a region in it
            var regionR1 = regionClass.Members.FirstOrDefault(m => m.Name.Equals("#R1")) as CodeRegionItem;
            Assert.IsNotNull(regionR1, "Region #R1 not found");

            // Region R1 should have a nested region R15 with a constant in it
            var regionR15 = regionR1.Members.FirstOrDefault(m => m.Name.Equals("#R15")) as CodeRegionItem;
            Assert.IsNotNull(regionR15);
            Assert.IsNotNull(regionR15.Members.FirstOrDefault(m => m.Name.Equals("nestedRegionConstant")));

            // Region R1 should have a function Test1 and Test2 in it
            Assert.IsNotNull(regionR1.Members.FirstOrDefault(m => m.Name.Equals("Test1")));
            Assert.IsNotNull(regionR1.Members.FirstOrDefault(m => m.Name.Equals("Test2")));
        }

        [TestMethod]
        public async Task TestRegionsVBAsync()
        {
            var document = await SyntaxMapper.MapDocumentAsync($@"Files\\VisualBasic\\TestRegions.vb");

            Assert.IsTrue(document.Any());

            // There should be a single class
            var regionClass = document.First() as CodeClassItem;
            Assert.IsNotNull(regionClass);

            // The class should have a property in it
            Assert.IsNotNull(regionClass.Members.FirstOrDefault(m => m.Name.Equals("outsideRegion$")));

            // The class should have a region in it
            var regionR1 = regionClass.Members.FirstOrDefault(m => m.Name.Equals("#FirstRegion")) as CodeRegionItem;
            Assert.IsNotNull(regionR1, "Region #FirstRegion not found");

            // Region R1 should have a property in it
            Assert.IsNotNull(regionR1.Members.FirstOrDefault(m => m.Name.Equals("insideRegion$")), "No property inside region found");
        }

        [TestMethod]
        public async Task TestRegionsNoNameAsync()
        {
            var document = await SyntaxMapper.MapDocumentAsync($@"Files\TestRegionsNoName.cs");

            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);

            // There should be a single class
            var regionClass = (document.First() as IMembers).Members.First() as CodeClassItem;
            Assert.IsNotNull(regionClass);

            // The class should have a region in it
            var regionR1 = regionClass.Members.FirstOrDefault(m => m.Name.Equals("#Region")) as CodeRegionItem;
            Assert.IsNotNull(regionR1, "Region #Region not found");
        }

        [TestMethod]
        public async Task TestRegionsSpanAsync()
        {
            var document = await SyntaxMapper.MapDocumentAsync($@"Files\TestRegions.cs");

            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);

            // There should be a single class
            var regionClass = (document.First() as IMembers).Members.First() as CodeClassItem;
            Assert.IsNotNull(regionClass);

            // The class should have a function in it
            Assert.IsNotNull(regionClass.Members.FirstOrDefault(m => m.Name.Equals("OutsideRegionFunction")));

            // The class should have a region in it
            var regionR1 = regionClass.Members.FirstOrDefault(m => m.Name.Equals("#R1")) as CodeRegionItem;
            Assert.IsNotNull(regionR1, "Region #R1 not found");

            // The region should have correct span for outlining usages
            Assert.AreEqual(101, regionR1.Span.Start);
            Assert.AreEqual(111, regionR1.Span.End);
        }
    }
}
