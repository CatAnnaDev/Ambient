#import <AppKit/AppKit.h>
#import <CoreGraphics/CoreGraphics.h>

extern "C" {

int ambient_ping()
{
    return 42;
}

void ambient_nudge_cursor(double dx, double dy)
{
    CGEventRef ev = CGEventCreate(NULL);
    CGPoint p = CGEventGetLocation(ev);
    if (ev) CFRelease(ev);
    CGPoint np = CGPointMake(p.x + dx, p.y + dy);
    CGWarpMouseCursorPosition(np);
    CGAssociateMouseAndMouseCursorPosition(true);
}

void ambient_shake_window(double px, double py)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        NSApplication *app = [NSApplication sharedApplication];
        NSWindow *win = [app mainWindow];
        if (win == nil) win = [app keyWindow];
        if (win == nil) {
            NSArray<NSWindow *> *wins = [app windows];
            if ([wins count] > 0) win = [wins objectAtIndex:0];
        }
        if (win != nil) {
            NSRect f = [win frame];
            NSPoint o = f.origin;
            o.x += px;
            o.y += py;
            [win setFrameOrigin:o];
        }
    });
}

}
