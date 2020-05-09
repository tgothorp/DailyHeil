using System;
using System.Reflection;
using System.Threading.Tasks;
using DailyHeil.Functions.Functions;
using Microsoft.Azure.WebJobs;
using RazorLight;
using Index = DailyHeil.Functions.Functions.Index;

namespace DailyHeil.Functions.Infrastructure.RazorEngine
{
    public interface IRazorEngine
    {
        Task<string> GetWebPage(ExecutionContext executionContext, Constants.Pages webpage, object model);
    }
    
    public class RazorEngine : IRazorEngine
    {
        RazorLightEngine _engine;

        public async Task<string> GetWebPage(ExecutionContext executionContext, Constants.Pages webpage, object model)
        {
            if (_engine is null)
                BuildEngine(executionContext);

            return await _engine.CompileRenderAsync(Constants.Templates[webpage], model);
        }

        private void BuildEngine(ExecutionContext executionContext)
        {
            _engine = new RazorLightEngineBuilder()
                .SetOperatingAssembly(Assembly.GetExecutingAssembly())
                .UseEmbeddedResourcesProject(typeof(Index))
                .UseFileSystemProject(Constants.TemplateDirectory(executionContext))
                .UseMemoryCachingProvider()
                .Build();
        }
    }
}