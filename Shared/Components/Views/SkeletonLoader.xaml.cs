namespace SmartBusinessManager.Shared.Components.Views;

public partial class SkeletonLoader : ContentView
{
    private bool _isAnimating;

    public SkeletonLoader()
    {
        InitializeComponent();
    }

    private void OnLoaded(object sender, EventArgs e)
    {
        StartPulseAnimation();
    }

    private void OnUnloaded(object sender, EventArgs e)
    {
        _isAnimating = false;
        
        try
        {
            this.CancelAnimations(); // Halt native rendering cycles immediately to free resources
        }
        catch
        {
            // Fail-safe protection against early object disposal
        }
    }

    private async void StartPulseAnimation()
    {
        if (_isAnimating) return;
        _isAnimating = true;

        // Performant infinite visual pulse looping using native hardware transitions
        while (_isAnimating && this.Window != null)
        {
            // Fade down to 35% opacity
            await this.FadeTo(0.35, 750, Easing.SinInOut);
            
            if (!_isAnimating) break;

            // Fade back to 100% opacity
            await this.FadeTo(1.0, 750, Easing.SinInOut);
        }
    }
}
