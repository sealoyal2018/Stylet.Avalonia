namespace Stylet.Avalonia.StyletIoC.Internal;

internal interface IRegistrationCollection : IReadOnlyRegistrationCollection
{
    IRegistrationCollection AddRegistration(IRegistration registration);
}

internal interface IReadOnlyRegistrationCollection
{
    IRegistration GetSingle();
    List<IRegistration> GetAll();
}
