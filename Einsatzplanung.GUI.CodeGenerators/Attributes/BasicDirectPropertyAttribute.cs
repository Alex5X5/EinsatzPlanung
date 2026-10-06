namespace Einsatzplanung.GUI.CodeGenerators.Attributes;

using System;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class BasicDirectPropertyAttribute<TOwner> : Attribute {
}

