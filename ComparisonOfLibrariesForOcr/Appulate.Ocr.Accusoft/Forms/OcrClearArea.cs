using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace Appulate.Ocr.Forms {
	[DataContract(Name = "ClearArea")]
	public class OcrClearArea : IOcrTemplateField {
		[DataMember(Name = "Id")]
		public Guid Id { get; set; }

		[DataMember(Name = "Location")]
		public Rectangle Location { get; set; }

		public OcrClearArea(Guid id) {
			Id = id;
		}
	}
}