using JetBrains.Annotations;

#pragma warning disable IDE0130
namespace Microsoft.CodeAnalysis;

[AttributeUsage(AttributeTargets.All)]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class EmbeddedAttribute : Attribute;
