namespace Infrastructure.Repository.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NavigationPropertyAttribute : Attribute
    {
    }
}