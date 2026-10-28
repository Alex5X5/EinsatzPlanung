namespace Einsatzplanung.Types;

public readonly record struct TryResult<T>(bool Success, T? Value);