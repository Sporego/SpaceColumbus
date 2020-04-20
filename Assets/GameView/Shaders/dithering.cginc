#define DITHER_CONSTANTS float _ApplyDither; float _DitherStrength;
#define APPLY_DITHER(color) if (_ApplyDither) color += DitherArray8x8(IN.uv.xy, _SizeX, _SizeY) * _DitherStrength;
#define APPLY_DITHER_WEIGHTED(color, weight) if (_ApplyDither) color += DitherArray8x8(IN.uv.xy, _SizeX, _SizeY) * _DitherStrength;

// array/table version from http://www.anisopteragames.com/how-to-fix-color-banding-with-dithering/
static const uint ArrayDitherArray8x8[] =
{
    0, 32,  8, 40,  2, 34, 10, 42,   /* 8x8 Bayer ordered dithering  */
    48, 16, 56, 24, 50, 18, 58, 26,  /* pattern.  Each input pixel   */
    12, 44,  4, 36, 14, 46,  6, 38,  /* is scaled to the 0..63 range */
    60, 28, 52, 20, 62, 30, 54, 22,  /* before looking in this table */
    3, 35, 11, 43,  1, 33,  9, 41,   /* to determine the action.     */
    51, 19, 59, 27, 49, 17, 57, 25,
    15, 47,  7, 39, 13, 45,  5, 37,
    63, 31, 55, 23, 61, 29, 53, 21
};

float DitherArray8x8(float2 uv, float width, float height)
{
    uv.x *= width;
    uv.y *= height;
    uint stippleOffset = ((uint)uv.y % 8) * 8 + ((uint)uv.x % 8) % 64;
    float dither = ArrayDitherArray8x8[stippleOffset] / 64.0;
    return dither - 1 / 32.0;
}
