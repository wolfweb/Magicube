using System.Drawing;

namespace Magicube.Media.Images {
    public class ColorService {
		private readonly Color _color;
		public ColorService(Color color) {
			_color = color;
		}

		public Color GetCloseColor() {
			Color fillColor = Color.White;
			if (IsBlack) fillColor = Color.White;
			else if (IsWhite) fillColor = Color.Black;
			else {
				fillColor = IsLight ? CalcCloseColor(0.9f, Color.Black) : CalcCloseColor(0.9f, Color.White);
			}

			return fillColor;
		}

		private Color CalcCloseColor(float threshold, Color to) {
			return Color.FromArgb(_color.A,
			   (int)(_color.R * (1 - threshold) + to.R * threshold),
			   (int)(_color.G * (1 - threshold) + to.G * threshold),
			   (int)(_color.B * (1 - threshold) + to.B * threshold)
			);
		}

		private bool IsWhite => _color.GetBrightness() >= 0.99f;

		private bool IsBlack => _color.GetBrightness() <= 0.01f;

		private bool IsLight => _color.GetBrightness() >= 0.85f;
	}
}
