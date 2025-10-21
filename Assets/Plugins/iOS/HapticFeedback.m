#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

// Ensure this block is within proper Objective-C syntax
#ifdef __cplusplus
extern "C" {
#endif

void PlayiOSHapticFeedback(int feedbackType)
{
    if (@available(iOS 10.0, *)) {
        UIImpactFeedbackGenerator *generator;

        switch (feedbackType) {
            case 0: // Light
                generator = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleLight];
                break;
            case 1: // Medium
                generator = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleMedium];
                break;
            case 2: // Heavy
                generator = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleHeavy];
                break;
            default:
                generator = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleMedium];
                break;
        }

        [generator prepare];
        [generator impactOccurred];
    }
}

#ifdef __cplusplus
}
#endif
