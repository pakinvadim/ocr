using System;
using System.Drawing;
using Appulate.Ocr.Forms;

namespace Appulate.Ocr.Accusoft.Recognition {
	public abstract class FieldResult {
		public abstract string Text { get; }
		public abstract Rectangle Location { get; }
		public abstract Rectangle Area { get; }

		public Guid Id { get; }

		protected FieldResult(OcrTemplateField field) {
			Id = field.Id;
			FieldLocation = field.Location;
		}

		public Rectangle FieldLocation { get; }
	}
}