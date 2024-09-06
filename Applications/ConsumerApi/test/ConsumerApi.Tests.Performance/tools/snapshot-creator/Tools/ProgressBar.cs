using System.Text;

namespace Backbone.PerformanceSnapshotCreator.Tools;

public class ProgressBar : IDisposable, IProgress<double>
{
    private const int BLOCK_COUNT = 40;
    private readonly TimeSpan _animationInterval = TimeSpan.FromSeconds(1.0 / 8);
    private const string ANIMATION = @"|/-\";

    private readonly Timer _timer;

    private double _currentProgress;
    private string _currentText = string.Empty;
    private bool _disposed;
    private int _animationIndex;
    private readonly long _upperBound;

    public ProgressBar()
    {
        _timer = new Timer(TimerHandler!);

        // A progress bar is only for temporary display in a console window.
        // If the console output is redirected to a file, draw nothing.
        // Otherwise, we'll end up with a lot of garbage in the target file.
        if (!Console.IsOutputRedirected)
        {
            ResetTimer();
        }
    }

    public ProgressBar(long upperBound = 1) : this()
    {
        if (upperBound < 1)
            throw new ArgumentOutOfRangeException(nameof(upperBound), "must be 1 or greater");
        _upperBound = upperBound == 1 ? 1 : upperBound - 1;
    }

    public void Report(double value)
    {
        // Make sure value is in [0..1] range
        value = Math.Max(0, Math.Min(_upperBound, value));
        Interlocked.Exchange(ref _currentProgress, value);
    }

    private void TimerHandler(object state)
    {
        lock (_timer)
        {
            if (_disposed) return;

            var progressBlockCount = (int)(_currentProgress / _upperBound * BLOCK_COUNT);
            var percent = (int)(_currentProgress / _upperBound * 100);
            var text = string.Format("{4}/{5}[{0}{1}] {2,3}% {3}",
                new string('#', progressBlockCount), new string('-', BLOCK_COUNT - progressBlockCount),
                percent,
                ANIMATION[_animationIndex++ % ANIMATION.Length],
                _currentProgress,
                _upperBound + 1);
            UpdateText(text);

            ResetTimer();
        }
    }

    private void UpdateText(string text)
    {
        // Get length of common portion
        var commonPrefixLength = 0;
        var commonLength = Math.Min(_currentText.Length, text.Length);
        while (commonPrefixLength < commonLength && text[commonPrefixLength] == _currentText[commonPrefixLength])
        {
            commonPrefixLength++;
        }

        // Backtrack to the first differing character
        var outputBuilder = new StringBuilder();
        outputBuilder.Append('\b', _currentText.Length - commonPrefixLength);

        // Output new suffix
        outputBuilder.Append(text[commonPrefixLength..]);

        // If the new text is shorter than the old one: delete overlapping characters
        var overlapCount = _currentText.Length - text.Length;
        if (overlapCount > 0)
        {
            outputBuilder.Append(' ', overlapCount);
            outputBuilder.Append('\b', overlapCount);
        }

        Console.Write(outputBuilder);
        _currentText = text;
    }

    private void ResetTimer()
    {
        _timer.Change(_animationInterval, TimeSpan.FromMilliseconds(-1));
    }

    public void Dispose()
    {
        lock (_timer)
        {
            _disposed = true;
            UpdateText(string.Empty);
        }
    }

    public void Increment()
    {
        Interlocked.Exchange(ref _currentProgress, _currentProgress + 1);
    }
}
