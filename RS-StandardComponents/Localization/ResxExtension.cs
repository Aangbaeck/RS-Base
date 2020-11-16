using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "RS_StandardComponents")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2007/xaml/presentation", "RS_StandardComponents")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2008/xaml/presentation", "RS_StandardComponents")]
namespace RS_StandardComponents
{
    /// <summary>
    /// Defines the handling method for the <see cref="ResxExtension.GetResource"/> event
    /// </summary>
    /// <param name="resxName">The name of the resx file</param>
    /// <param name="key">The resource key within the file</param>
    /// <param name="culture">The culture to get the resource for</param>
    /// <returns>The resource</returns>
    public delegate object GetResourceHandler(string resxName, string key, CultureInfo culture);

    [MarkupExtensionReturnType(typeof(object))]
    public class ResxExtension : ManagedMarkupExtension
    {
        /// <summary>
        /// The resource manager to use for this extension.  Holding a strong reference to the
        /// Resource Manager keeps it in the cache while ever there are ResxExtensions that
        /// are using it.
        /// </summary>
        private ResourceManager _resourceManager;

        /// <summary>
        /// Cached resource managers
        /// </summary>
        private static Dictionary<string, WeakReference> _resourceManagers = new Dictionary<string, WeakReference>();
        public static event GetResourceHandler GetResource;

        /// <summary>
        /// Create a new instance of the markup extension
        /// </summary>
        public ResxExtension() : base(MarkupManager)
        {
        }
        public ResxExtension(object key) : base(MarkupManager)
        {
            this.Key = key.ToString();
        }

        public ResxExtension(object key, object resXName) : base(MarkupManager)
        {
            this.Key = key.ToString();
            this.ResxName = resXName.ToString();
        }

        /// <summary>
        /// The fully qualified name of the embedded resx (without .resources) to get
        /// the resource from
        /// </summary>
        public object ResxName { get; set; }

        /// <summary>
        /// The name of the resource key
        /// </summary>
        public object Key { get; set; }

        /// <summary>
        /// The default value to use if the resource can't be found
        /// </summary>
        /// <remarks>
        /// This particularly useful for properties which require non-null
        /// values because it allows the page to be displayed even if
        /// the resource can't be loaded
        /// </remarks>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Return the value for this instance of the Markup Extension
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <returns>The value of the element</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // register the target and property so we can update them
            RegisterTarget(serviceProvider);

            if (string.IsNullOrEmpty(Key.ToString()))
                throw new ArgumentException("You must set the resource Key");

            object result;
            if (TargetProperty == null)
            {
                result = this;
            }
            else
            {
                result = GetValue();
            }
            return result;
        }

        /// <summary>
        /// Return the MarkupManager for this extension
        /// </summary>
        public static MarkupExtensionManager MarkupManager { get; } = new MarkupExtensionManager(40);

        public static bool IsInDesignMode
        {
            get
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
                //MessageBox.Show(process.ProcessName);  //Uncomment this for figuring out what's wrong
                bool res = process.ProcessName == "WpfSurface";
                process.Dispose();
                return res;
            }
        }
        private static string LastCultureDesigner;
        private static bool RunOnce = false;

        /// <summary>
        /// Class constructor
        /// </summary>
        static ResxExtension()
        {
            if (IsInDesignMode && LastCultureDesigner != KeyboardLayout.GetCurrentKeyboardLayout().Name)
            {
                if (!RunOnce)  //We only need one instance of updater for the GUI. The designer sometimes spam this constructor
                {
                    KeyboardLayout.Instance.KeyboardLayoutChanged += UpdateDesignerGUI;
                    RunOnce = true;
                }
                LastCultureDesigner = KeyboardLayout.GetCurrentKeyboardLayout().Name;
            }
        }

        private static void UpdateDesignerGUI(CultureInfo oldCultureInfo, CultureInfo newCultureInfo)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    CultureManager.UICulture = KeyboardLayout.GetCurrentKeyboardLayout();
                }));
        }

        /// <summary>
        /// Return the localized resource given a resource Key
        /// </summary>
        /// <param name="resourceKey">The resourceKey</param>
        /// <returns>The value for the current UICulture</returns>
        /// <remarks>Calls GetResource event first then if not handled uses the resource manager</remarks>
        protected virtual object GetLocalizedResource(string resourceKey)
        {
            if (string.IsNullOrEmpty(resourceKey)) return null;
            object result = null;
            if (!string.IsNullOrEmpty(ResxName.ToString()))
            {
                try
                {
                    if (GetResource != null)
                    {
                        result = GetResource(ResxName.ToString(), resourceKey, CultureManager.UICulture);
                    }
                    if (result == null)
                    {
                        if (_resourceManager == null)
                        {
                            _resourceManager = GetResourceManager(ResxName.ToString());
                        }
                        if (_resourceManager != null)
                        {
                            result = _resourceManager.GetObject(resourceKey, CultureManager.UICulture);
                        }
                    }
                }
                catch
                {
                }
            }
            return result;
        }
        /// <summary>
        /// Return the value for the markup extension
        /// </summary>
        /// <returns>The value from the resources if possible otherwise the default value</returns>
        /// <summary>
        /// Return the localized value with the key from the resource.
        /// </summary>
        /// <param name="key">The key too look for in the resource file</param>
        /// <param name="resxName">The path to the resource file (including the file name). Example: [namespace].Folder.SubFolder.ResourceFileName (dont include language code or file extension)</param>
        /// <returns>Returns the object in the resouce file. If string is empty or not found it will return Key.</returns>
        public static string GetValueManual(string key, string resxName)
        {
            return GetValueManual<string>(key, resxName, out string _);
        }
        /// <summary>
        /// Return the value for the markup extension
        /// </summary>
        /// <returns>The value from the resources if possible otherwise the default value</returns>
        /// <summary>
        /// Return the localized value with the key from the resource.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key too look for in the resource file</param>
        /// <param name="resxName">The path to the resource file (including the file name). Example: [namespace].Folder.SubFolder.ResourceFileName (dont include language code or file extension)</param>
        /// <returns>Returns the object in the resouce file. If string is empty or not found it will return Key.</returns>
        public static T GetValueManual<T>(string key, string resxName)
        {
            return GetValueManual<T>(key, resxName, out string _);
        }
        /// <summary>
        /// Return the value for the markup extension
        /// </summary>
        /// <returns>The value from the resources if possible otherwise the default value</returns>
        /// <summary>
        /// Return the localized value with the key from the resource.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key too look for in the resource file</param>
        /// <param name="resxName">The path to the resource file (including the file name). Example: [namespace].Folder.SubFolder.ResourceFileName (dont include language code or file extension)</param>
        /// <param name="culture">if not set (i.e. null) it will be set to the applications current culture.</param>
        /// <param name="errorMsg">Can be used to understand why the method is not returning the right result</param>
        /// <returns>Returns the object in the resouce file. If string is empty or not found it will return Key.</returns>
        public static T GetValueManual<T>(string key, string resxName, out string errorMsg, CultureInfo culture = null)
        {
            errorMsg = "no error";
            if (culture == null) culture = CultureManager.UICulture;
            if (string.IsNullOrEmpty(key))
            {
                errorMsg = "key IsNullOrEmpty";
                return default;
            }

            if (!string.IsNullOrEmpty(resxName))
            {
                try
                {
                    Assembly assembly = FindResourceAssembly(resxName);
                    if (assembly == null)
                    {
                        errorMsg = $"could not FindResourceAssembly for {resxName}";
                        return default;
                    }

                    var resourceManager = new ResourceManager(resxName, assembly);
                    if (resourceManager == null)
                    {
                        errorMsg = $"could not find ResourceManager for {resxName}";
                        return default;
                    }

                    object result = resourceManager.GetObject(key, culture);
                    if (result == null)
                    {
                        if (typeof(T) == typeof(String))
                        {
                            errorMsg = $"result == null but string so returning key";
                            result = "#" + key;
                            return (T)result;
                        }
                        errorMsg = $"result == null";
                        return default;
                    }
                    if (typeof(T) == typeof(String) && (String)result == "")
                    {
                        errorMsg = $"result == String.Empty so returning key instead";
                        result = "#" + key;
                        return (T)result;
                    }
                    return (T)result;
                }
                catch (Exception ex)
                {
                    errorMsg = $"Exception: {ex.Message}";
                }
            }
            return default;
        }

        /// <summary>
        /// Return the value for the markup extension
        /// </summary>
        /// <returns>The value from the resources if possible otherwise the default value</returns>
        protected override object GetValue()
        {
            object result = GetLocalizedResource(Key.ToString());
            var sResult = result as String;
            if (result == null || sResult == "")
            {
                result = GetDefaultValue(Key.ToString());
            }
            return result;
        }

        private static bool HasEmbeddedResx(Assembly assembly, string resxName)
        {
            if (assembly.IsDynamic) return false;

            string[] resources = assembly.GetManifestResourceNames();
            string searchName = resxName.ToLower() + ".resources";
            foreach (string resource in resources)
            {
                if (resource.ToLower() == searchName) return true;
            }
            return false;
        }

        /// <summary>
        /// Find the assembly that contains the type
        /// </summary>
        /// <returns>The assembly if loaded (otherwise null)</returns>
        private static Assembly FindResourceAssembly(string resxName)
        {
            Assembly assembly = Assembly.GetEntryAssembly();

            // check the entry assembly first - this will short circuit a lot of searching
            //
            if (assembly != null && HasEmbeddedResx(assembly, resxName)) return assembly;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly searchAssembly in assemblies)
            {
                // skip system assemblies
                //
                string name = searchAssembly.FullName;
                if (!name.StartsWith("Microsoft.") &&
                    !name.StartsWith("System.") &&
                    !name.StartsWith("System,") &&
                    !name.StartsWith("mscorlib,") &&
                    !name.StartsWith("PresentationCore,") &&
                    !name.StartsWith("PresentationFramework,") &&
                    !name.StartsWith("WindowsBase,"))
                {
                    if (HasEmbeddedResx(searchAssembly, resxName)) return searchAssembly;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the resource manager for this type
        /// </summary>
        /// <param name="resxName">The name of the embedded resx</param>
        /// <returns>The resource manager</returns>
        /// <remarks>Caches resource managers to improve performance</remarks>
        private ResourceManager GetResourceManager(string resxName)
        {
            ResourceManager result = null;
            if (resxName == null) return null;
            WeakReference reference;
            if (_resourceManagers.TryGetValue(resxName, out reference))
            {
                result = reference.Target as ResourceManager;
                if (result == null)
                {
                    _resourceManagers.Remove(resxName);
                }
            }

            if (result == null)
            {
                Assembly assembly = FindResourceAssembly(resxName);
                if (assembly != null)
                {
                    result = new ResourceManager(resxName, assembly);
                }
                _resourceManagers.Add(resxName, new WeakReference(result));
            }
            return result;
        }

        private object GetDefaultValue(string key)
        {
            object result = DefaultValue;
            Type targetType = TargetPropertyType;
            if (DefaultValue == null)
            {
                if (targetType == typeof(String) || targetType == typeof(object))
                {
                    result = "#" + key;
                }
            }
            else if (targetType != null)
            {
                if (targetType != typeof(String) && targetType != typeof(object))
                {
                    try
                    {
                        TypeConverter tc = TypeDescriptor.GetConverter(targetType);
                        result = tc.ConvertFromInvariantString(DefaultValue.ToString());
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }
    }
}