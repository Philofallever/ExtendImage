﻿﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtendUI.SymbolText
{
    class LineNode : NodeBase
	{
        public Font font;
        public FontStyle fs;
        public int fontSize;

        float height = 0f;

        public override float getHeight() { return height; }

        public override float getWidth() { return 0f; }

        public override void fill(ref Vector2 currentpos, List<Line> lines, float maxWidth, float pixelsPerUnit)
        {
            height = FontCache.GetLineHeight(font, (int)(fontSize * pixelsPerUnit), fs) / pixelsPerUnit;
            lines.Add(new Line(new Vector2(NextLineX, height)));
        }

        public override void render(float maxWidth, RenderCache cache, ref float x, ref uint yline, List<Line> lines, float offsetX, float offsetY)
		{
            yline++;
            x = offsetX;
        }

        protected override void ReleaseSelf()
        {
            height = 0f;
        }
	};
}
