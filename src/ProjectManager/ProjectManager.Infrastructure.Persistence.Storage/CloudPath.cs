using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.OrchestrationEngine.Infrastructure.Persistence.Storage
{
    public class CloudPath
    {
        /// <summary>
        /// Creates and instance providing the full path of the resource
        /// </summary>
        /// <param name="targetPath">The full resource path</param>
        public CloudPath(string targetPath)
        {
            if (string.IsNullOrWhiteSpace(targetPath))
                throw new ArgumentException(nameof(targetPath));

            Parts = SplitNormalize(targetPath);
        }

        /// <summary>
        /// Creates and instance providing the containing path and the resource name
        /// </summary>
        /// <param name="path">The containing path of the resource</param>
        /// <param name="target">The resource name</param>
        public CloudPath(string path, string target) : this(System.IO.Path.Combine(path, target))
        {
        }

        public IEnumerable<string> Parts { get; }

        /// <summary>
        /// The containing path of the resource
        /// </summary>summary>
        public string Path
        {
            get
            {
                if (Parts.Any())
                {
                    return System.IO.Path.Combine(Parts.Take(Parts.Count() - 1).ToArray());
                }

                return null;
            }
        }

        /// <summary>
        /// The resource name
        /// </summary>
        public string Target => Parts.LastOrDefault();

        /// <summary>
        /// The full path of the resource
        /// </summary>
        public string TargetPath
        {
            get
            {
                if (Parts.Any())
                {
                    return System.IO.Path.Combine(Parts.ToArray());
                }

                return "/";
            }
        }

        private IEnumerable<string> SplitNormalize(string text) =>
            text.Split(new[] { "/", "\\" }, StringSplitOptions.RemoveEmptyEntries);
    }
}