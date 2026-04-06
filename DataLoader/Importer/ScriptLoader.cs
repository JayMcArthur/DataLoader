using Importer.Contracts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Reflection;

public static class ScriptLoader
{
    public static async Task<IImportProfile<TApiModel>?> LoadProfileAsync<TApiModel>(string csxPath)
        where TApiModel : class
    {
        var profiles = await LoadProfilesAsync(csxPath);

        if (profiles.TryGetValue(typeof(TApiModel), out var obj) && obj is IImportProfile<TApiModel> p)
            return p;

        return null;
    }

    /// <summary>
    /// Loads and discovers all IImportProfile&lt;T&gt; implementations from a .csx file.
    /// Returns a map: TRequestType -> profile instance (as object).
    /// </summary>
    public static async Task<IReadOnlyDictionary<Type, object>> LoadProfilesAsync(string csxPath)
    {
        if (!File.Exists(csxPath))
            throw new FileNotFoundException("CSX file not found", csxPath);

        var scriptCode = await File.ReadAllTextAsync(csxPath);

        var options = BuildScriptOptions();

        try
        {
            // Compile the script (we do not need to execute it if profiles are only type definitions).
            var script = CSharpScript.Create(scriptCode, options);

            // Force compilation diagnostics now (clean error reporting)
            var compilation = script.GetCompilation();

            var diags = compilation.GetDiagnostics()
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .ToArray();

            if (diags.Length > 0)
            {
                var errors = string.Join(Environment.NewLine, diags.Select(d => d.ToString()));
                throw new Exception($"Failed to compile script '{csxPath}':{Environment.NewLine}{errors}");
            }

            // Emit to an in-memory assembly so we can reflect over types.
            using var peStream = new MemoryStream();
            using var pdbStream = new MemoryStream();

            var emitResult = compilation.Emit(peStream, pdbStream);

            if (!emitResult.Success)
            {
                var errors = string.Join(Environment.NewLine, emitResult.Diagnostics);
                throw new Exception($"Failed to emit script '{csxPath}':{Environment.NewLine}{errors}");
            }

            peStream.Position = 0;
            pdbStream.Position = 0;

            var assembly = Assembly.Load(peStream.ToArray(), pdbStream.ToArray());

            return DiscoverProfiles(assembly);
        }
        catch (CompilationErrorException ex)
        {
            var errors = string.Join(Environment.NewLine, ex.Diagnostics);
            throw new Exception($"Failed to compile script '{csxPath}':{Environment.NewLine}{errors}");
        }
    }

    private static ScriptOptions BuildScriptOptions()
    {
        // NOTE:
        // Add whatever assemblies your scripts need here (contracts, models, helpers).
        // This is the same pattern you already had, but generalized.
        return ScriptOptions.Default
            .AddReferences(
                typeof(object).Assembly,                 // System.Private.CoreLib
                typeof(Enumerable).Assembly,             // System.Linq
                typeof(IImportProfile<>).Assembly        // Importer.Contracts (or wherever IImportProfile<> lives)
            )
            .AddImports(
                "System",
                "System.Linq",
                "System.Collections.Generic",
                "Importer.Contracts"
            );
    }

    private static IReadOnlyDictionary<Type, object> DiscoverProfiles(Assembly assembly)
    {
        // Maps TRequest -> IImportProfile<TRequest> instance
        var map = new Dictionary<Type, object>();

        foreach (var t in assembly.GetTypes())
        {
            if (t.IsAbstract || t.IsInterface) continue;

            // Find IImportProfile<T>
            var profileIface = t.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IImportProfile<>));

            if (profileIface is null) continue;

            // Enforce parameterless ctor (keeps csx simple)
            if (t.GetConstructor(Type.EmptyTypes) is null)
            {
                throw new InvalidOperationException(
                    $"Import profile '{t.FullName}' must have a parameterless constructor.");
            }

            var requestType = profileIface.GetGenericArguments()[0];

            // Prevent silent overrides
            if (map.TryGetValue(requestType, out var existing))
            {
                throw new InvalidOperationException(
                    $"Duplicate import profile detected for request type '{requestType.FullName}'. " +
                    $"Profiles: '{existing.GetType().FullName}' and '{t.FullName}'.");
            }

            var instance = Activator.CreateInstance(t)!;
            map.Add(requestType, instance);
        }

        return map;
    }
}