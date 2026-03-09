using System.Reflection;
using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;

namespace JojaAutoTasks.Tests.Domain;

/// <summary>
/// Phase 2 boundary tests to keep domain models isolated from UI, store, and persistence namespaces.
/// </summary>
public class Phase2BoundaryTests
{
    private static readonly string[] ForbiddenNamespaceFragments =
    {
        ".UI",
        ".ViewModel",
        ".Views",
        ".StarML",
        ".Store",
        ".StateStore",
        ".Persistence"
    };

    [Fact]
    public void DomainIdentifierTypes_ShouldNotReferenceForbiddenNamespaces()
    {
        Type[] domainTypes =
        {
            typeof(TaskId),
            typeof(DayKey),
            typeof(RuleId),
            typeof(SubjectId),
            typeof(TaskIdFactory),
            typeof(TaskIdFormat),
            typeof(DayKeyFactory)
        };

        AssertNoForbiddenTypeDependencies(domainTypes);
    }

    [Fact]
    public void DomainTaskTypes_ShouldNotReferenceForbiddenNamespaces()
    {
        Type[] domainTypes =
        {
            typeof(TaskObject)
        };

        AssertNoForbiddenTypeDependencies(domainTypes);
    }

    private static void AssertNoForbiddenTypeDependencies(IEnumerable<Type> domainTypes)
    {
        HashSet<string> violations = new HashSet<string>(StringComparer.Ordinal);

        foreach (Type domainType in domainTypes)
        {
            foreach (Type dependency in GetTypeDependencies(domainType))
            {
                string? namespaceName = dependency.Namespace;
                if (string.IsNullOrWhiteSpace(namespaceName))
                {
                    continue;
                }

                bool isForbidden = ForbiddenNamespaceFragments.Any(fragment =>
                    namespaceName.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) >= 0);

                if (isForbidden)
                {
                    violations.Add($"{domainType.FullName} -> {dependency.FullName}");
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            "Forbidden domain dependency detected: " + string.Join("; ", violations.OrderBy(x => x, StringComparer.Ordinal)));
    }

    private static IEnumerable<Type> GetTypeDependencies(Type type)
    {
        HashSet<Type> dependencies = new HashSet<Type>();

        AddType(dependencies, type.BaseType);

        foreach (Type interfaceType in type.GetInterfaces())
        {
            AddType(dependencies, interfaceType);
        }

        const BindingFlags AllMembers = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        foreach (FieldInfo field in type.GetFields(AllMembers))
        {
            AddType(dependencies, field.FieldType);
        }

        foreach (PropertyInfo property in type.GetProperties(AllMembers))
        {
            AddType(dependencies, property.PropertyType);
        }

        foreach (MethodInfo method in type.GetMethods(AllMembers))
        {
            AddType(dependencies, method.ReturnType);

            foreach (ParameterInfo parameter in method.GetParameters())
            {
                AddType(dependencies, parameter.ParameterType);
            }
        }

        foreach (ConstructorInfo constructor in type.GetConstructors(AllMembers))
        {
            foreach (ParameterInfo parameter in constructor.GetParameters())
            {
                AddType(dependencies, parameter.ParameterType);
            }
        }

        return dependencies;
    }

    private static void AddType(ISet<Type> dependencies, Type? type)
    {
        if (type == null)
        {
            return;
        }

        if (type.IsByRef || type.IsPointer)
        {
            AddType(dependencies, type.GetElementType());
            return;
        }

        if (type.IsArray)
        {
            AddType(dependencies, type.GetElementType());
            return;
        }

        if (!dependencies.Add(type))
        {
            return;
        }

        if (type.IsGenericType)
        {
            Type genericTypeDefinition = type.GetGenericTypeDefinition();
            if (genericTypeDefinition != type)
            {
                AddType(dependencies, genericTypeDefinition);
            }

            foreach (Type argument in type.GetGenericArguments())
            {
                AddType(dependencies, argument);
            }

            return;
        }
    }
}
