namespace SolidMReader.Services.Interfaces;

public interface IValidation<M>
{
    bool IsValid<T>(T validate) where T : M;
}