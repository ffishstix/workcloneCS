using System.Text.RegularExpressions;
using FluentValidation;
namespace WorkCloneCS;

public class ConnectionSettings
{
    public string IP { get; set; }
    public string Port { get; set; }
    public string Database { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    private string ipPattern = @"^(?:[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?(?:\.[A-Za-z]{2,63})+|(?:25[0-5]|2[0-4]\d|1?\d{1,2})(?:\.(?:25[0-5]|2[0-4]\d|1?\d{1,2})){3})$";
    //could be either ip or domain so makes it more complicated
    private string portPattern = @"";
}





public class ConnectionSettingsValidator : AbstractValidator<ConnectionSettings>
{
    public ConnectionSettingsValidator()
    {
        RuleFor(x => x.IP)
            .NotEmpty().WithMessage("IP/Host is required")
            .Must(BeValidSqlServerIdentifier).WithMessage("Invalid SQL Server identifier");

        RuleFor(x => x.Port)
            .NotEmpty().WithMessage("Port is required")
            .Must(BeValidPort).WithMessage("Port must be between 1 and 65535");

        RuleFor(x => x.Database)
            .NotEmpty().WithMessage("Database name is required")
            .MaximumLength(128).WithMessage("Database name too long")
            .Matches("^[a-zA-Z0-9_-]*$").WithMessage("Database name can only contain letters, numbers, underscores, and hyphens")
            .Must(x => !x.StartsWith("_")).WithMessage("Database name cannot start with an underscore");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MaximumLength(128).WithMessage("Username too long")
            .Matches("^[a-zA-Z0-9_-]*$").WithMessage("Username can only contain letters, numbers, underscores, and hyphens")
            .Must(x => !x.StartsWith("_")).WithMessage("Username cannot start with an underscore");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .MaximumLength(128).WithMessage("Password too long");
    }

    private bool BeValidSqlServerIdentifier(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
            return false;

        var sqlServerRegex = @"^(?:\(localDB\)\\[^\\/:*?""<>|]+|\.|\(local\)|localhost|(?:(?!-)[A-Za-z0-9-]{1,63}(?<!-)(?:\.(?!-)[A-Za-z0-9-]{1,63}(?<!-))*|(?:\d{1,3}\.){3}\d{1,3})(?:\\[A-Za-z0-9_]{1,128})?)$";

        return Regex.IsMatch(host, sqlServerRegex, RegexOptions.IgnoreCase);
    }

    private bool BeValidPort(string port)
    {
        return int.TryParse(port, out int portNum) && portNum > 0 && portNum <= 65535;
    }
}


