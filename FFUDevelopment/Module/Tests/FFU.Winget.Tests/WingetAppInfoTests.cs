using System.Reflection;
using System.Runtime.CompilerServices;
using FFU.Core.Attributes;
using Newtonsoft.Json;

namespace FFU.Winget.Tests;

[TestFixture]
[TestOf(typeof(WingetAppInfo))]
[Category("Winget")]
public class WingetAppInfoTests
{
  [Test]
  public void Equality_Is_ByValue()
  {
    WingetAppInfo a = new("Name", "Id");
    WingetAppInfo b = new("Name", "Id");

    Assume.That(a, Is.Not.SameAs(b));
    Assert.That(a, Is.EqualTo(b));
  }

  [Test]
  public void NonDestructiveCopy_CreatesNewIdenticalInstance()
  {
    WingetAppInfo a = new("Name", "Id");
    WingetAppInfo b = a with { };

    Assume.That(a, Is.Not.SameAs(b));
    Assert.That(a, Is.EqualTo(b));
  }

  [SkipLocalsInit]
  private static bool TryGetPropertyChangedTestValuesForProperty(
    PropertyInfo property,
    out object? preChangeValue,
    out object? postChangeValue)
  {
    Type propType = property.PropertyType;
    Type? underlyingType = Nullable.GetUnderlyingType(propType);
    switch (propType, underlyingType)
    {
      case ({ IsValueType: true }, { IsEnum: true })
        when Enum.GetValues(underlyingType) is { Length: > 0 } enumValues:
      {
        preChangeValue = null!;
        postChangeValue = enumValues.GetValue(0)!;
        return true;
      }
      case ({ IsValueType: true }, not null)
        when propType == typeof(int):
      {
        preChangeValue = null!;
        postChangeValue = 1;
        return true;
      }
      case ({ IsValueType: true, IsEnum: true }, null)
        when Enum.GetValues(propType) is { Length: > 1 } enumValues:
      {
        preChangeValue = enumValues.GetValue(0)!;
        postChangeValue = enumValues.GetValue(1)!;
        return true;
      }
      case ({ IsValueType: true }, null)
        when propType == typeof(bool):
      {
        preChangeValue = false;
        postChangeValue = true;
        return true;
      }
      case ({ IsValueType: true }, null)
        when propType == typeof(int):
      {
        preChangeValue = 1;
        postChangeValue = 2;
        return true;
      }
      case ({ IsClass: true }, null)
        when propType == typeof(string):
      {
        preChangeValue = $"{property.Name}_Initial";
        postChangeValue = $"{property.Name}_Changed";
        return true;
      }
      default:
      {
        preChangeValue = null!;
        postChangeValue = null!;
        return false;
      }
    }
  }

  private static IEnumerable<TestCaseData> GetPropertyChangeTestCases()
  {
    PropertyInfo[] properties = WingetAppInfoType
      .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
      .Where(static prop =>
        !prop.IsDefined(typeof(SkipPropertyChangedTestAttribute))
        && prop.CanWrite).ToArray();

    foreach (PropertyInfo property in properties)
    {
      if (!TryGetPropertyChangedTestValuesForProperty(property, out object? initial, out object? changed))
      {
        continue;
      }

      yield return new(property, initial, changed)
      {
        TypeArgs = Nullable.GetUnderlyingType(property.PropertyType) is not { IsValueType: true } nullableValueType
          ? [property.PropertyType]
          : [property.PropertyType.GetGenericTypeDefinition().MakeGenericType(nullableValueType)]
      };
    }
  }

  [Test]
  [Category("WPF")]
  [TestCaseSource(nameof(GetPropertyChangeTestCases))]
  public void Property_Raises_PropertyChanged_Only_When_Value_Changes<TProperty>(
    PropertyInfo property,
    TProperty? preChangeValue,
    TProperty? postChangeValue)
  {
    Assume.That(property.CanWrite);
    Assume.That(property.IsDefined(typeof(SkipPropertyChangedTestAttribute)), Is.False);
    Assume.That(preChangeValue, Is.Not.SameAs(postChangeValue));
    Assume.That(preChangeValue, Is.Not.EqualTo(postChangeValue));

    WingetAppInfo testApp = new("App", "App");


    property.SetValue(testApp, preChangeValue);

    Assume.That(property.GetValue(testApp), Is.EqualTo(preChangeValue));

    PropertyChangedEventTracker eventTracker = new();
    Assume.That(eventTracker, Is.Not.Null);
    Assume.That(eventTracker.ChangedPropertyNames, Is.Not.Null.And.Empty);
    testApp.PropertyChanged += eventTracker.ReceivedPropertyChangedNotification;

    // Test that event is NOT raised when we set the same value.
    {
      property.SetValue(testApp, preChangeValue);
      using IDisposable noEventRaisedScope = Assert.EnterMultipleScope();
      Assert.That(property.GetValue(testApp), Is.EqualTo(preChangeValue));
      Assert.That(eventTracker.ChangedPropertyNames, Is.Empty);
    }

    // Test that event IS raised when we set a different value.
    {
      property.SetValue(testApp, postChangeValue);
      using IDisposable eventRaisedScope = Assert.EnterMultipleScope();
      Assert.That(property.GetValue(testApp), Is.EqualTo(postChangeValue));
      Assert.That(eventTracker.ChangedPropertyNames, Has.Exactly(1).InstanceOf<string>());
      Assert.That(eventTracker.ChangedPropertyNames, Contains.Item(property.Name));
    }
  }

  [Test]
  public void CompareTo_SameObject_ReturnsZero()
  {
    WingetAppInfo app = new("App", "App");

    Assert.That(app.CompareTo(app), Is.Zero);
  }

  [Test]
  public void CompareTo_SameValue_ReturnsZero()
  {
    WingetAppInfo app1 = new("App", "App");
    WingetAppInfo app2 = new("App", "App");

    Assume.That(app1, Is.Not.SameAs(app2));
    Assume.That(app1, Is.EqualTo(app2));

    using IDisposable forwardAndBackwardScope = Assert.EnterMultipleScope();
    Assert.That(app1.CompareTo(app2), Is.Zero);
    Assert.That(app2.CompareTo(app1), Is.Zero);
  }

  [Test]
  public void GetHashCode_IsOrdinalStringComparison_Over_Id_And_Name()
  {
    WingetAppInfo app = new("AppName", "AppId");

    int expectedHashCode = HashCode.Combine(
      app.Id.GetHashCode(StringComparison.Ordinal),
      app.Name.GetHashCode(StringComparison.Ordinal)
    );

    Assert.That(app.GetHashCode(), Is.EqualTo(expectedHashCode));
  }

  private static readonly Type WingetAppInfoType = typeof(WingetAppInfo);

  private static IEnumerable<PropertyInfo> NonSerializedProperties_Actual
  {
    get
    {
      return WingetAppInfoType
        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        .Where(static prop => prop.IsDefined(typeof(JsonIgnoreAttribute)));
    }
  }

  private static IEnumerable<string> NonSerializedProperties_Expected =>
  [
    "DownloadProgress",
    "DownloadState",
    "DownloadStatus"
  ];

  [Test]
  [Category("Serialization")]
  public void PropertiesWithJsonIgnoreAttribute_AreExpected(
    [ValueSource(nameof(NonSerializedProperties_Actual))]
    PropertyInfo prop)
  {
    using IDisposable scope = Assert.EnterMultipleScope();
    Assume.That(WingetAppInfoType, Has.Property(prop.Name));
    Assert.That(prop, Has.Attribute(typeof(JsonIgnoreAttribute)));
    Assert.That(NonSerializedProperties_Expected, Has.Member(prop.Name));
  }

  [Test]
  [Category("Serialization")]
  public void PropertiesExpectedToBeJsonIgnored_HaveJsonIgnoreAttribute(
    [ValueSource(nameof(NonSerializedProperties_Expected))]
    string propName)
  {
    PropertyInfo? prop = WingetAppInfoType
      .GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    using IDisposable scope = Assert.EnterMultipleScope();
    Assert.That(prop, Is.Not.Null);
    Assert.That(WingetAppInfoType, Has.Property(propName));
    Assert.That(prop, Has.Attribute<JsonIgnoreAttribute>());
  }
}