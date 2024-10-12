namespace Jobber_Server.Exceptions
{
    public class NotFoundException(string msg = "Requested resource not found"): Exception(msg) {}
    public class BadRequestException(string msg = "Bad request"): Exception(msg) {}
    public class UnauthorizedException(string msg = "Unauthorized"): Exception(msg) {}
}