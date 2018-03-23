using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

namespace CGImageTest
{
    public partial class ViewController : UIViewController
    {
        private long baseMemory = 0;
        private int count = 0;
        partial void UIButton394_TouchUpInside(UIButton sender)
        {
            System.Diagnostics.Debug.WriteLine($"Pressed : {GC.GetTotalMemory(false)} are used");
            using (var cgImage = CreateImageWithAutoReleasePool(count++))
            using (var uiImage = new UIImage(cgImage))
            {
                ImageView.Image = uiImage;
            }
            System.Diagnostics.Debug.WriteLine($" Before GC : {GC.GetTotalMemory(false) - baseMemory}");
            GC.Collect();
            System.Diagnostics.Debug.WriteLine($" After GC : {GC.GetTotalMemory(false) - baseMemory}");
            System.Diagnostics.Debug.WriteLine($"Total : {GC.GetTotalMemory(false)} are used");
            System.Diagnostics.Debug.WriteLine("");
        }

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            baseMemory = GC.GetTotalMemory(false);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public CGImage CreateImageWithAutoReleasePool( int c = 1)
        {
            using (var pool = new NSAutoreleasePool())
            {
                var colorSpace = CGColorSpace.CreateDeviceRGB();
                var width = 1000;
                var height = 1000;
                var bitsPerSample = 8;
                var components = 3;//RGB
                var bitsPerPixel = bitsPerSample * components;
                var bytesPerRow = bitsPerPixel / 8 * width;
                var bufferToDisplay = new byte[width * height * components];
                var bufferDontUse = new byte[width * height * components * 2];

                System.Diagnostics.Debug.WriteLine($"   Allocated buffer size {bufferToDisplay.Length} + {bufferDontUse.Length}");
                System.Diagnostics.Debug.WriteLine($"  before local GC {GC.GetTotalMemory(false) - baseMemory}");
                bufferDontUse = null;
                GC.Collect();
                System.Diagnostics.Debug.WriteLine($"  after local GC {GC.GetTotalMemory(false) - baseMemory}");

                int a = 0;
                for (int cnt = 0; cnt < width * height; cnt++)
                {
                    bufferToDisplay[a++] = c % 3 == 0 ? (byte)0xFF : (byte)0x00;
                    bufferToDisplay[a++] = c % 3 == 1 ? (byte)0xFF : (byte)0x00;
                    bufferToDisplay[a++] = c % 3 == 2 ? (byte)0xFF : (byte)0x00;
                }

                using (var data = new CGDataProvider(bufferToDisplay))
                {
                    var cgImage = new CGImage(
                        width,
                        height,
                        bitsPerSample,
                        bitsPerPixel,
                        bytesPerRow,
                        colorSpace,
                        CGBitmapFlags.ByteOrderDefault,
                        data,
                        decode: null,
                        shouldInterpolate: false,
                        intent: CGColorRenderingIntent.Default);
                    bufferToDisplay = null;
                    return cgImage;
                }

            }
        }
    }
}
