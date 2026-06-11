using System.Linq.Expressions;

namespace Dukaan.Infrastructure.Data.Repositories;

/// <summary>
/// Helper class to convert LINQ expressions into string paths for Entity Framework Include operations.
/// </summary>
public static class ExpressionPathHelper
{
    public static string? GetIncludePath(Expression expression)
    {
        if (expression is LambdaExpression lambda)
        {
            return GetIncludePath(lambda.Body);
        }

        if (expression is MethodCallExpression call)
        {
            // Handle .Select(x => x.Property) for collection navigations
            if (call.Method.Name == "Select" && call.Arguments.Count == 2)
            {
                var parentPath = GetIncludePath(call.Arguments[0]);
                var childPath = GetIncludePath(call.Arguments[1]);
                if (parentPath != null && childPath != null)
                    return $"{parentPath}.{childPath}";
                return parentPath ?? childPath;
            }
            
            // Handle other common methods if necessary
            return GetIncludePath(call.Arguments[0]);
        }

        if (expression is MemberExpression member)
        {
            var parentPath = GetIncludePath(member.Expression!);
            if (parentPath == null) return member.Member.Name;
            return $"{parentPath}.{member.Member.Name}";
        }

        return null;
    }
}
