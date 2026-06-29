using System.Reflection;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class ActionFeedbackBehaviorTests
{
    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    public void ButtonFormatsAriaDisabledAsLiteralBoolean(bool value, string expected)
    {
        var formatted = InvokeStaticString(typeof(Button), "FormatBoolean", value);

        Assert.Equal(expected, formatted);
    }

    [Fact]
    public void BadgeExposesDisabledStateForButtonBadges()
    {
        var badge = new Badge();
        SetProperty(badge, nameof(Badge.Button), true);
        SetProperty(badge, nameof(Badge.Disabled), true);

        Assert.True(badge.Button);
        Assert.True(badge.Disabled);
    }

    [Fact]
    public void WarningNotificationsExposeCopyAffordance()
    {
        var message = new NotificationMessage
        {
            Severity = NotificationSeverity.Warning,
            Summary = "Review needed",
            Detail = "The mobile proof still has a spacing concern to resolve."
        };

        var canCopy = InvokeStaticBoolean(typeof(Notification), "CanCopy", message);
        var copyValue = InvokeStaticString(typeof(Notification), "BuildCopyValue", message);

        Assert.True(canCopy);
        Assert.Equal(
            $"Review needed{Environment.NewLine}The mobile proof still has a spacing concern to resolve.",
            copyValue);
    }

    [Fact]
    public void InformationalNotificationsDoNotExposeCopyAffordance()
    {
        var message = new NotificationMessage
        {
            Severity = NotificationSeverity.Info,
            Summary = "Sandbox refreshed",
            Detail = "The current group examples have been reloaded for review."
        };

        var canCopy = InvokeStaticBoolean(typeof(Notification), "CanCopy", message);

        Assert.False(canCopy);
    }

    private static string InvokeStaticString(Type declaringType, string methodName, params object[] parameters)
        => (string)declaringType
            .GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic)!
            .Invoke(null, parameters)!;

    private static bool InvokeStaticBoolean(Type declaringType, string methodName, params object[] parameters)
        => (bool)declaringType
            .GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic)!
            .Invoke(null, parameters)!;

    private static void SetProperty(object target, string propertyName, object value)
        => target.GetType()
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)!
            .SetValue(target, value);
}
