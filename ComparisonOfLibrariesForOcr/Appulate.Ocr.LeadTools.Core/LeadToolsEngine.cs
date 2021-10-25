using System;
using System.Collections.Generic;
using Leadtools;
using Leadtools.Forms.Auto;
using Leadtools.Forms.Processing;

namespace Appulate.Ocr.LeadTools.Core {
	public class LeadToolsEngine {
		private const string LicenseFile = @"C:\LEADTOOLS22\Support\Common\License\LEADTOOLS.LIC";
		private const string DeveloperKey = "n8x3XulTh5aRbS3DYdFJk5KRVsARLiMI2N393rBp7DM=";
		private readonly LeadToolsIdentificator _leadToolsIdentificator;

		public LeadToolsEngine(string templatePath) {
			RasterSupport.SetLicense(LicenseFile, DeveloperKey);
			if (RasterSupport.IsLocked(RasterSupportType.Document)) {
				throw new Exception("IsLocked");
			}

			_leadToolsIdentificator = new LeadToolsIdentificator(templatePath);
		}

		public List<(int Page, string Field, string Text)> Run(string filePath) {
			AutoFormsRunResult result = _leadToolsIdentificator.Run(filePath);
			return CreateResults(result);
		}

		private List<(int Page, string Field, string Text)> CreateResults(AutoFormsRunResult result) {
			if (result == null) {
				return null;
			}
			var data = new List<(int Page, string Field, string Text)>();
			foreach (FormPage resultFormPage in result.RecognitionResult.FormPages) {
				foreach (FormField field in resultFormPage) {
					if (field is TextFormField textFormField) {
						data.Add((resultFormPage.PageNumber,
						          field.Name,
						          ((TextFormFieldResult)textFormField.Result).Text));
					} else {
						data.Add((resultFormPage.PageNumber,
						          field.Name,
						          "Unknown"));
					}
				}
			}
			return data;
		}
	}
}