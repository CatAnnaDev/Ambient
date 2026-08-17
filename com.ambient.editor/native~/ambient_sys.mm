#import <Foundation/Foundation.h>
#import <IOKit/ps/IOPowerSources.h>
#import <IOKit/ps/IOPSKeys.h>
#include <sys/sysctl.h>
#include <time.h>
#include <string.h>

extern "C" {

int ambient_sys_ping()
{
    return 7;
}

int ambient_hour()
{
    time_t tt = time(NULL);
    struct tm lt;
    localtime_r(&tt, &lt);
    return lt.tm_hour;
}

double ambient_uptime_hours()
{
    struct timeval boot;
    size_t sz = sizeof(boot);
    int mib[2] = { CTL_KERN, KERN_BOOTTIME };
    if (sysctl(mib, 2, &boot, &sz, NULL, 0) != 0) return -1.0;
    time_t now = time(NULL);
    double secs = difftime(now, boot.tv_sec);
    return secs / 3600.0;
}

int ambient_battery()
{
    CFTypeRef info = IOPSCopyPowerSourcesInfo();
    CFArrayRef list = info ? IOPSCopyPowerSourcesList(info) : NULL;
    int pct = -1;
    if (list && CFArrayGetCount(list) > 0) {
        CFDictionaryRef ps = IOPSGetPowerSourceDescription(info, CFArrayGetValueAtIndex(list, 0));
        if (ps) {
            CFNumberRef cap = (CFNumberRef)CFDictionaryGetValue(ps, CFSTR(kIOPSCurrentCapacityKey));
            CFNumberRef max = (CFNumberRef)CFDictionaryGetValue(ps, CFSTR(kIOPSMaxCapacityKey));
            int c = 0, m = 0;
            if (cap) CFNumberGetValue(cap, kCFNumberIntType, &c);
            if (max) CFNumberGetValue(max, kCFNumberIntType, &m);
            if (m > 0) pct = (int)((c * 100) / m);
        }
    }
    if (list) CFRelease(list);
    if (info) CFRelease(info);
    return pct;
}

const char* ambient_username()
{
    static char buf[256];
    NSString *u = NSUserName();
    const char *c = [u UTF8String];
    if (!c) { buf[0] = 0; return buf; }
    strncpy(buf, c, sizeof(buf) - 1);
    buf[sizeof(buf) - 1] = 0;
    return buf;
}

}
