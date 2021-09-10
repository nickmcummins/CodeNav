﻿using CodeNav.Mappers;
using CodeNav.Models;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace CodeNav.Tests.MapperTests
{
    [TestFixture]
    public class RegionMapperTests
    {
        [Test]
        public void TestRegions()
        {
            var document = SyntaxMapper.MapDocument(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Files\\TestRegions.cs"), null);

            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);

            // There should be a single class
            var regionClass = (document.First() as IMembers).Members.First() as CodeClassItem;
            Assert.NotNull(regionClass);

            // The class should have a function in it
            Assert.NotNull(regionClass.Members.FirstOrDefault(m => m.Name.Equals("OutsideRegionFunction")));

            // The class should have a region in it
            var regionR1 = regionClass.Members.FirstOrDefault(m => m.Name.Equals("#R1")) as CodeRegionItem;
            Assert.NotNull(regionR1, "Region #R1 not found");

            // Region R1 should have a nested region R15 with a constant in it
            var regionR15 = regionR1.Members.FirstOrDefault(m => m.Name.Equals("#R15")) as CodeRegionItem;
            Assert.NotNull(regionR15);
            Assert.NotNull(regionR15.Members.FirstOrDefault(m => m.Name.Equals("nestedRegionConstant")));

            // Region R1 should have a function Test1 and Test2 in it
            Assert.NotNull(regionR1.Members.FirstOrDefault(m => m.Name.Equals("Test1")));
            Assert.NotNull(regionR1.Members.FirstOrDefault(m => m.Name.Equals("Test2")));
        }

        [Test]
        public void TestRegionsVB()
        {
            var document = SyntaxMapper.MapDocumentVB(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Files\\VisualBasic\\TestRegions.vb"), null);

            Assert.IsTrue(document.Any());

            // There should be a single class
            var regionClass = document.First() as CodeClassItem;
            Assert.NotNull(regionClass);

            // The class should have a property in it
            Assert.NotNull(regionClass.Members.FirstOrDefault(m => m.Name.Equals("outsideRegion$")));

            // The class should have a region in it
            var regionR1 = regionClass.Members.FirstOrDefault(m => m.Name.Equals("#FirstRegion")) as CodeRegionItem;
            Assert.NotNull(regionR1, "Region #FirstRegion not found");

            // Region R1 should have a property in it
            Assert.NotNull(regionR1.Members.FirstOrDefault(m => m.Name.Equals("insideRegion$")), "No property inside region found");
        }

        [Test]
        public void TestRegionsNoName()
        {
            var document = SyntaxMapper.MapDocument(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Files\\TestRegionsNoName.cs"), null);

            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);

            // There should be a single class
            var regionClass = (document.First() as IMembers).Members.First() as CodeClassItem;
            Assert.NotNull(regionClass);

            // The class should have a region in it
            var regionR1 = regionClass.Members.FirstOrDefault(m => m.Name.Equals("#Region")) as CodeRegionItem;
            Assert.NotNull(regionR1, "Region #Region not found");
        }

        [Test]
        public void TestRegionsSpan()
        {
            var document = SyntaxMapper.MapDocument(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Files\\TestRegions.cs"), null);

            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);

            // There should be a single class
            var regionClass = (document.First() as IMembers).Members.First() as CodeClassItem;
            Assert.NotNull(regionClass);

            // The class should have a function in it
            Assert.NotNull(regionClass.Members.FirstOrDefault(m => m.Name.Equals("OutsideRegionFunction")));

            // The class should have a region in it
            var regionR1 = regionClass.Members.FirstOrDefault(m => m.Name.Equals("#R1")) as CodeRegionItem;
            Assert.NotNull(regionR1, "Region #R1 not found");

            // The region should have correct span for outlining usages
            Assert.AreEqual(101, regionR1.Span.Start);
            Assert.AreEqual(111, regionR1.Span.End);
        }
    }
}
