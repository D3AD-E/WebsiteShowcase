using System.Collections.Generic;
using System.Reflection;

namespace Website.Models.ViewModels
{
    public class ProjectViewModel : ProjectModel
    {
        public Dictionary<int, string> OtherProjects { get; set; }

        public ProjectViewModel(ProjectModel other)
        {
            InitInhertedProperties(other);
            OtherProjects = new Dictionary<int, string>();
        }

        private void InitInhertedProperties(object baseClassInstance)
        {
            foreach (PropertyInfo propertyInfo in baseClassInstance.GetType().GetProperties())
            {
                object value = propertyInfo.GetValue(baseClassInstance, null);
                if (null != value) propertyInfo.SetValue(this, value, null);
            }
        }
    }
}
