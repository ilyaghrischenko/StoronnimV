namespace StoronnimV.Application.Exceptions;

public sealed class MediaValidationException(string message) : PhotoResizingException(message);
