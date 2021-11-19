using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Optimization;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Provider;

namespace cms4seo.Service.Content
{
    public static class DynamicBundles
    {
        private static Dictionary<string, Bundle> _bundleDictionary = new Dictionary<string, Bundle>();

        private static bool _BundlesFinalized;
        private static bool _SupportBootswatch;
        private static bool _updatedContents;

        public static bool BundlesFinalized
        {
            get
            {
                return _BundlesFinalized;
            }
        }

        public static bool SupportBootswatch
        {
            get
            {
                return _SupportBootswatch;
            }
        }

        public static bool UpdateContents
        {
            get
            {
                return _updatedContents;
            }
        }

        public static void EnableBootswatch()
        {
            _SupportBootswatch = true;
        }

        /// <summary>
        /// Add a bundle to the bundle dictionary
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public static bool RegisterBundle(Bundle bundle)
        {
            if (bundle == null)
                throw new ArgumentNullException("bundle");
            if (_BundlesFinalized)
                throw new InvalidOperationException("The bundles have been finalized and frozen, you can only finalize the bundles once as an app pool recycle is needed to change the bundles afterwards!");
            if (_bundleDictionary.ContainsKey(bundle.Path))
                return false;
            _bundleDictionary.Add(bundle.Path, bundle);
            return true;
        }
        /// <summary>
        /// Finalize the bundles, which commits them to the BundleTable.Bundles collection, respects the web.config's debug setting for optimizations
        /// </summary>
        public static void FinalizeBundles()
        {
            FinalizeBundles(null);
        }
        /// <summary>
        /// Finalize the bundles, which commits them to the BundleTable.Bundles collection
        /// </summary>
        /// <param name="forceMinimize">Null = Respect web.config debug setting, True force minification regardless of web.config, False force no minification regardless of web.config</param>
        public static void FinalizeBundles(bool? forceMinimize)
        {
            var bundles = BundleTable.Bundles;
            foreach (var bundle in _bundleDictionary.Values)
            {
                bundles.Add(bundle);
            }
            if (forceMinimize != null)
                BundleTable.EnableOptimizations = forceMinimize.Value;

            _BundlesFinalized = true;

            bool result = Setting.Contents.InitializationTheme();

            _updatedContents = result;
        }


        public static void Clear()
        {
            _BundlesFinalized = false;
            _SupportBootswatch = false;
            _updatedContents = false;
            _bundleDictionary = new Dictionary<string, Bundle>();
            Setting.Clear(); // reload language  & theme
        }
    }
}
