using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Appulate.Ocr.VintaSoft.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Appulate.Ocr.VintaSoft.IntegrationTests {
	[TestFixture]
	public class OcrVintaSoftEngineTests {
		[Test]
		public void RecognizesAcord130UsingAnOcrTemplate() {
			var effectiveDataFieldId = "aa05ca35-57dd-4eba-9c65-3db6f0b9b1aa";
			var expirationDataFieldId = "51ca522b-45c7-47d2-8e86-23711297a5d8";

			string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\Acord130_3698_LB_d8968da4-a7cf-4343-9403-39ed4e526c3b\page1.bmp");
			string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Templates\ACORD_130_D8968DA4-A7CF-4343-9403-39ED4E526C3B\form-ocr-one-page.fdt");

			var s1 = new Stopwatch();
			s1.Start();
			var engine = new VintaSoftEngine(templatePath);
			s1.Stop();
			Console.WriteLine($"first initialization: {s1.Elapsed.Seconds} seconds");

			var s2 = new Stopwatch();
			s2.Start();
			List<(int, string, string)> result = engine.Run(filePath);
			s2.Stop();
			Console.WriteLine($"identification and recognition: {s2.Elapsed.Seconds} seconds");

			result.Should().NotBeNull();
			Dictionary<string, string> items = result.ToDictionary(i => i.Item2, i => i.Item3);
			items[effectiveDataFieldId].Trim().Should().Contain("07/17/2017");
			items[expirationDataFieldId].Trim().Should().Contain("07/17/2018");
		}
	}
}