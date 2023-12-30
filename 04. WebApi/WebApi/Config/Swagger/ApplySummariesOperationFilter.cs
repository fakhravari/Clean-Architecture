using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Pluralize.NET;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Config.Swagger
{
    public class ApplySummariesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var controllerActionDescriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor == null) return;

            var pluralizer = new Pluralizer();

            var actionName = controllerActionDescriptor.ActionName;
            var singularizeName = pluralizer.Singularize(controllerActionDescriptor.ControllerName);
            var pluralizeName = pluralizer.Pluralize(singularizeName);

            var parameterCount = operation.Parameters.Where(p => p.Name != "version" && p.Name != "api-version").Count();

            if (IsGetAllAction())
            {
                if (!StringExtensions.HasValue(operation.Summary))
                    operation.Summary = $"Returns all {pluralizeName}";
            }
            else if (IsActionName("Post", "Create"))
            {
                if (!StringExtensions.HasValue(operation.Summary))
                    operation.Summary = $"Creates a {singularizeName}";

                //if (!StringExtensions.HasValue(operation.Parameters[0].Description))
                //    operation.Parameters[0].Description = $"A {singularizeName} representation";
            }
            else if (IsActionName("Read", "Get"))
            {
                if (!StringExtensions.HasValue(operation.Summary))
                    operation.Summary = $"Retrieves a {singularizeName} by unique id";

                if (!StringExtensions.HasValue(operation.Parameters[0].Description))
                    operation.Parameters[0].Description = $"a unique id for the {singularizeName}";
            }
            else if (IsActionName("Put", "Edit", "Update"))
            {
                if (!StringExtensions.HasValue(operation.Summary))
                    operation.Summary = $"Updates a {singularizeName} by unique id";

                //if (!operation.Parameters[0].Description.HasValue())
                //    operation.Parameters[0].Description = $"A unique id for the {singularizeName}";

                //if (!StringExtensions.HasValue(operation.Parameters[0].Description))
                //    operation.Parameters[0].Description = $"A {singularizeName} representation";
            }
            else if (IsActionName("Delete", "Remove"))
            {
                if (!StringExtensions.HasValue(operation.Summary))
                    operation.Summary = $"Deletes a {singularizeName} by unique id";

                if (!StringExtensions.HasValue(operation.Parameters[0].Description))
                    operation.Parameters[0].Description = $"A unique id for the {singularizeName}";
            }

            #region Local Functions
            bool IsGetAllAction()
            {
                foreach (var name in new[] { "Get", "Read", "Select" })
                {
                    if ((actionName.Equals(name, StringComparison.OrdinalIgnoreCase) && parameterCount == 0) ||
                        actionName.Equals($"{name}All", StringComparison.OrdinalIgnoreCase) ||
                        actionName.Equals($"{name}{pluralizeName}", StringComparison.OrdinalIgnoreCase) ||
                        actionName.Equals($"{name}All{singularizeName}", StringComparison.OrdinalIgnoreCase) ||
                        actionName.Equals($"{name}All{pluralizeName}", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }

            bool IsActionName(params string[] names)
            {
                foreach (var name in names)
                {
                    if (actionName.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                        actionName.Equals($"{name}ById", StringComparison.OrdinalIgnoreCase) ||
                        actionName.Equals($"{name}{singularizeName}", StringComparison.OrdinalIgnoreCase) ||
                        actionName.Equals($"{name}{singularizeName}ById", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
            #endregion
        }
    }
}
