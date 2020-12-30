using MediatR;

namespace ProjectManager.Queries
{
    public interface IQuery<out T> : IRequest<T>
    {
    }
}