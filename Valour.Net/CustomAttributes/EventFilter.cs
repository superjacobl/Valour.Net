using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling;

namespace Valour.Net.CustomAttributes;

/// <summary>
/// Provides an interface for implementing a filter targetting a route handler.
/// </summary>
public interface IEventFilter
{
    /// <summary>
    /// Implements the core logic associated with the filter given a <see cref="EndpointFilterInvocationContext"/>
    /// and the next filter to call in the pipeline.
    /// </summary>
    /// <param name="context">The <see cref="EventFilterInvocationContext"/> associated with the current command/interaction.</param>
    /// <returns>An awaitable result of calling the handler and apply
    /// any modifications made by filters in the pipeline.</returns>
    ValueTask<object?> InvokeAsync(EventFilterInvocationContext context);
}

/// <summary>
/// Provides an abstraction for wrapping the <see cref="HttpContext"/> and arguments
/// provided to a route handler.
/// </summary>
public class EventFilterInvocationContext
{
    /// <summary>
    /// The <see cref="IContext"/> associated with the current event being processed by the filter.
    /// </summary>
    public IContext Context { get; set; }

    /// <summary>
    /// A list of arguments provided in the current event to the filter.
    /// <remarks>
    /// This list is not read-only to permit modifying of existing arguments by filters.
    /// </remarks>
    /// </summary>
    public object[] Arguments { get; set; }

    /// <summary>
    /// Retrieve the argument given its position in the argument list.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the resolved argument.</typeparam>
    /// <param name="index">An integer representing the position of the argument in the argument list.</param>
    /// <returns>The argument at a given <paramref name="index"/>.</returns>
    public T GetArgument<T>(int index) => (T)Arguments[index];
}

public interface IEventFilterAttribute
{
    public IEventFilter eventFilter { get; }
}