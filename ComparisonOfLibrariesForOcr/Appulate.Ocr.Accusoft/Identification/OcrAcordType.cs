using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Appulate.Ocr.Accusoft.Identification {
	[DataContract(Name = "OcrAcordType")]
	public class OcrAcordType {
		[DataMember(Name = "AcordName")]
		public string AcordName { get; set; }

		[DataMember(Name = "AcordVersion")]
		public string AcordVersion { get; set; }

		public string GetVersionYear() {
			int index = AcordVersion?.IndexOf("/", StringComparison.InvariantCulture) ?? 0;
			return index > 0 ? AcordVersion?.Remove(index) : null;
		}

		public static string GetVersionFormat(DateTime date) {
			return $"{date.ToString("yyyy/MM", CultureInfo.InvariantCulture)}";
		}
	}
}