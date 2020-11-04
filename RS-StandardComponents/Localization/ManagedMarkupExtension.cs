using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "RS_StandardComponents")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2007/xaml/presentation", "RS_StandardComponents")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2008/xaml/presentation", "RS_StandardComponents")]
namespace RS_StandardComponents
{
    public abstract class ManagedMarkupExtension : MarkupExtension
    {

        public ManagedMarkupExtension(MarkupExtensionManager manager)
        {
            manager.RegisterExtension(this);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            RegisterTarget(serviceProvider);
            object result = this;
            if (TargetProperty != null)
            {
                result = GetValue();
            }
            return result;
        }
        
        protected virtual void RegisterTarget(IServiceProvider serviceProvider)
        {
            var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            object target = provideValueTarget.TargetObject;
            if (target != null)
            {
                TargetProperty = provideValueTarget.TargetProperty;
                TargetObjects.Add(new WeakReference(target));
            }
        }
        protected virtual void UpdateTarget(object target)
        {
            if (TargetProperty is DependencyProperty)
            {
                DependencyObject dependencyObject = target as DependencyObject;
                if (dependencyObject != null)
                {
                    dependencyObject.SetValue(TargetProperty as DependencyProperty, GetValue());
                }
            }
            else if (TargetProperty is PropertyInfo)
            {
                (TargetProperty as PropertyInfo).SetValue(target, GetValue(), null);
            }
        }
        public void UpdateTargets()
        {
            foreach (WeakReference reference in TargetObjects)
            {
                if (reference.IsAlive)
                {
                    UpdateTarget(reference.Target);
                }
            }
        }
        public bool IsTargetsAlive
        {
            get
            {
                foreach (WeakReference reference in TargetObjects)
                {
                    if (reference.IsAlive) return true;
                }
                return false;
            }
        }
        protected List<WeakReference> TargetObjects { get; } = new List<WeakReference>();
        protected object TargetProperty { get; private set; }
        protected Type TargetPropertyType
        {
            get
            {
                Type result = null;
                if (TargetProperty is DependencyProperty)
                    result = (TargetProperty as DependencyProperty).PropertyType;
                else if (TargetProperty is PropertyInfo)
                    result = (TargetProperty as PropertyInfo).PropertyType;
                else if (TargetProperty != null)
                    result = TargetProperty.GetType();
                return result;
            }
        }
        protected abstract object GetValue();
    }
}