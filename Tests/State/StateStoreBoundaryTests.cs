using System.Reflection;
using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using JojaAutoTasks.State.Commands;
using Container = JojaAutoTasks.State.StateContainer;
using Store = JojaAutoTasks.State.StateStore;

namespace JojaAutoTasks.Tests.StateStoreBoundaries;

public sealed class StateStoreBoundaryTests
{
    [Fact]
    public void StateStore_ApiSurface_ExposesCommandMutationThroughDispatchMethodsOnly()
    {
        MethodInfo[] visibleMethods = GetVisibleInstanceMethods(typeof(Store));

        MethodInfo[] commandMethods = visibleMethods
            .Where(static method => method.GetParameters().Any(static parameter => parameter.ParameterType == typeof(IStateCommand)))
            .ToArray();

        Assert.Single(commandMethods);
        Assert.Equal(nameof(Store.Dispatch), commandMethods[0].Name);

        MethodInfo dispatchCreateManualTaskMethod = Assert.Single(
            visibleMethods,
            static method => method.Name == nameof(Store.DispatchCreateManualTaskCommand));

        ParameterInfo[] createMethodParameters = dispatchCreateManualTaskMethod.GetParameters();
        Assert.Collection(
            createMethodParameters,
            static parameter => Assert.Equal(typeof(TaskCategory), parameter.ParameterType),
            static parameter => Assert.Equal(typeof(string), parameter.ParameterType),
            static parameter => Assert.Equal(typeof(string), parameter.ParameterType),
            static parameter => Assert.Equal(typeof(DayKey), parameter.ParameterType));

        MethodInfo? handleMethod = typeof(Store).GetMethod(
            "Handle",
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(handleMethod);
        Assert.True(handleMethod.IsPrivate);
    }

    [Fact]
    public void StateStore_ApiSurface_DoesNotExposeMutableDictionaryOrStateContainerPath()
    {
        const BindingFlags VisibleMembers = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        PropertyInfo[] visibleProperties = typeof(Store)
            .GetProperties(VisibleMembers)
            .Where(static property => IsVisibleToConsumers(property.GetMethod, property.SetMethod))
            .ToArray();

        FieldInfo[] visibleFields = typeof(Store)
            .GetFields(VisibleMembers)
            .Where(static field => field.IsPublic || field.IsAssembly)
            .ToArray();

        MethodInfo[] visibleMethods = GetVisibleInstanceMethods(typeof(Store));

        Assert.Empty(visibleProperties);
        Assert.Empty(visibleFields);
        Assert.DoesNotContain(visibleMethods, static method => ReturnsMutableStateType(method.ReturnType));
        Assert.DoesNotContain(visibleMethods, static method => method.GetParameters().Any(static parameter => IsMutableStateType(parameter.ParameterType)));
    }

    [Fact]
    public void Dispatch_WhenCommandTypeHasNoRegisteredHandler_ThrowsInvalidOperationException()
    {
        Store sut = new();
        IStateCommand unknownCommand = new UnknownBoundaryCommand(new TaskId("BuiltIn_UnknownBoundary"));

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => sut.Dispatch(unknownCommand));

        Assert.Contains(nameof(UnknownBoundaryCommand), exception.Message, StringComparison.Ordinal);
    }

    private static MethodInfo[] GetVisibleInstanceMethods(Type type)
    {
        return type
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
            .Where(static method => method.IsPublic || method.IsAssembly)
            .ToArray();
    }

    private static bool IsVisibleToConsumers(MethodInfo? getter, MethodInfo? setter)
    {
        return IsVisible(getter) || IsVisible(setter);
    }

    private static bool IsVisible(MethodInfo? method)
    {
        return method is { IsPublic: true } or { IsAssembly: true };
    }

    private static bool ReturnsMutableStateType(Type returnType)
    {
        return IsMutableStateType(returnType);
    }

    private static bool IsMutableStateType(Type type)
    {
        if (type == typeof(void))
        {
            return false;
        }

        if (type == typeof(Container))
        {
            return true;
        }

        if (!type.IsGenericType)
        {
            return false;
        }

        Type genericTypeDefinition = type.GetGenericTypeDefinition();
        return genericTypeDefinition == typeof(Dictionary<,>)
            || genericTypeDefinition == typeof(IDictionary<,>)
            || genericTypeDefinition == typeof(IReadOnlyDictionary<,>);
    }

    private sealed record UnknownBoundaryCommand(TaskId TaskId) : IStateCommand;
}