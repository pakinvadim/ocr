using System;
using System.Drawing;

namespace Appulate.Ocr.Forms {
	public interface IOcrTemplateField {
		Guid Id { get; set; }
		Rectangle Location { get; set; }
	}
}