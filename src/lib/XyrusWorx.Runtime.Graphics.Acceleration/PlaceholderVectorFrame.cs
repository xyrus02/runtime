using System;

namespace XyrusWorx.Runtime
{
	class PlaceholderVectorFrame : IVectorFrame
	{
		public void DrawLine(Float4 color, float thickness, Float2 a, Float2 b) => throw new NotSupportedException("Vector operations are not supported by this implementation.");
		public void DrawRectangle(Float4 fillColor, Float4 borderColor, float borderThickness, Float2 topLeft, Float2 bottomRight) => throw new NotSupportedException("Vector operations are not supported by this implementation.");
		public void DrawRoundedRectangle(Float4 fillColor, Float4 borderColor, float borderThickness, Float2 topLeft, Float2 bottomRight, float radiusX, float radiusY) => throw new NotSupportedException("Vector operations are not supported by this implementation.");
		public void DrawEllipse(Float4 fillColor, Float4 borderColor, float borderThickness, Float2 center, float radiusX, float radiusY) => throw new NotSupportedException("Vector operations are not supported by this implementation.");
	}
}