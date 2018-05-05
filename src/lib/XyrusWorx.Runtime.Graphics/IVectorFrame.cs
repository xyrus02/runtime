using JetBrains.Annotations;

namespace XyrusWorx.Runtime {
	[PublicAPI]
	public interface IVectorFrame 
	{
		void DrawLine(Float4 color, float thickness, Float2 a, Float2 b);
		void DrawRectangle(Float4 fillColor, Float4 borderColor, float borderThickness, Float2 topLeft, Float2 bottomRight);
		void DrawRoundedRectangle(Float4 fillColor, Float4 borderColor, float borderThickness, Float2 topLeft, Float2 bottomRight, float radiusX, float radiusY);
		void DrawEllipse(Float4 fillColor, Float4 borderColor, float borderThickness, Float2 center, float radiusX, float radiusY);
	}
}