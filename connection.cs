using FluentValidation;
namespace WorkCloneCS;
public class ConnectionSettings
{
    public string IP { get; set; }
    public string Port { get; set; }
    public string Database { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

public class ConnectionSettingsValidator : AbstractValidator<ConnectionSettings>
{
    public ConnectionSettingsValidator()
    {
        RuleFor(x => x.IP)
            .NotEmpty().WithMessage("IP/Host is required")
            .Must(BeValidIpOrDomain).WithMessage("Invalid IP address or domain name format");


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
            .MinimumLength(2).WithMessage("Password must be at least 8 characters")
            .MaximumLength(128).WithMessage("Password too long");

    }

    private bool BeValidIpOrDomain(string host)
    {
        // Check if it's a valid IP address
        if (System.Net.IPAddress.TryParse(host, out _))
            return true;

        // Check if it's a valid domain name
        try
        {
            // Domain name validation regex
            // Allows: letters, numbers, dots, and hyphens
            // Cannot start or end with hyphen
            // Cannot have consecutive dots
            var domainRegex = @"^(?!-)[A-Za-z0-9-]{1,63}(?<!-)(\.[A-Za-z0-9-]{1,63})*(\.[A-Za-z]{2,})$";
            return System.Text.RegularExpressions.Regex.IsMatch(host, domainRegex);
        }
        catch
        {
            return false;
        }
    }

    private bool BeValidPort(string port)
    {
        return int.TryParse(port, out int portNum) && portNum > 0 && portNum <= 65535;
    }
}


